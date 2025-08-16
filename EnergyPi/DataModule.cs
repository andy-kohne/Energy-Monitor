using Brultech;
using Brultech.ECM1240;
using EnergyMonitor.Data;
using EnergyMonitor.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EnergyPi;

public class StorageOptions
{
    /// <summary>
    /// Enables/Disables data compaction
    /// </summary>
    public bool Compaction { get; set; } = false;

    /// <summary>
    /// Configures the age a record must be before being eligible for compaction
    /// </summary>
    public TimeSpan CompactionEndThreshold { get; set; } = TimeSpan.FromDays(62);

    /// <summary>
    /// Configures how far back to begin looking for records to compact
    /// </summary>
    public TimeSpan CompactionBeginThreshold { get; set; } = TimeSpan.FromDays(365);

    /// <summary>
    /// The period duration for which to remove excess records
    /// </summary>
    public TimeSpan CompactToPeriod { get; set; } = TimeSpan.FromMinutes(2.5);

}

public class DataModule
{
    private readonly IDbContextFactory<EnergyMonitorContext> _dbFactory;
    private readonly StorageOptions _options;

    private Dictionary<ushort, Packet>? _lastPackets = new();
    private readonly Dictionary<ushort, DateTime> _compactionProgress = new();
    private CancellationTokenSource _cts = new();
    private Task? _compactionTask;

    public DataModule(IDbContextFactory<EnergyMonitorContext> dbFactory, StorageOptions options)
    {
        _dbFactory = dbFactory;
        _options = options;

        Console.WriteLine(
            $"{nameof(DataModule)} beginning with compaction {(_options.Compaction ? "enabled" : "disabled")}");
        Console.WriteLine(
            $"{nameof(DataModule)} beginning with CompactionBeginThreshold {_options.CompactionBeginThreshold}");
        Console.WriteLine(
            $"{nameof(DataModule)} beginning with CompactionEndThreshold {_options.CompactionEndThreshold}");
        Console.WriteLine(
            $"{nameof(DataModule)} beginning with CompactToPeriod {_options.CompactToPeriod}");
    }


    public async Task Init()
    {
        _lastPackets = await GetLastPackets().ConfigureAwait(false);

        if (_options.Compaction)
        {
            foreach (var packet in _lastPackets)
            {
                _compactionProgress.Add(packet.Key, DateTime.UtcNow.Subtract(_options.CompactionBeginThreshold));
            }

            _compactionTask = CompactionTask(_cts.Token);
        }
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    private async Task<Dictionary<ushort, Packet>> GetLastPackets()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var devices = await
            (from ur in db.UsageReadings
             select ur.device).Distinct().ToListAsync();

        var ret = new Dictionary<ushort, Packet>();

        foreach (var deviceId in devices)
        {
            var stored = await
                (from ur in db.UsageReadings
                 where ur.device == deviceId
                 orderby ur.dateTimeUTC descending
                 select ur).FirstAsync().ConfigureAwait(false);
            var packet = stored.ToPacket();
            ret.Add(packet.DeviceAddress, packet);
        }

        return ret;
    }


    public void PacketReceived(object? sender, PacketReceivedArgs e)
    {
        try
        {
            var currentPacket = e.Packet;
            Debug.Assert(_lastPackets != null, nameof(_lastPackets) + " != null");
            if (_lastPackets.TryGetValue(currentPacket.DeviceAddress, out var previousPacket))
            {
                _lastPackets[currentPacket.DeviceAddress] = currentPacket;
            }
            else
            {
                _lastPackets.TryAdd(currentPacket.DeviceAddress, currentPacket);
            }

            var newReading = new UsageReading
            {
                dateTimeUTC = DateTime.UtcNow,
                device = currentPacket.DeviceAddress,
                resetCount = currentPacket.ResetCounter,
                volts = currentPacket.Volt,
                seconds = currentPacket.SecondsCounter,
                ch1Current = currentPacket.Ch1Current,
                ch2Current = currentPacket.Ch2Current,
                ch1Polarity = currentPacket.Ch1PolarityBit,
                ch2Polarity = currentPacket.Ch2PolarityBit,
                ch1AbsWS = currentPacket.Ch1AbsoluteWsCounter,
                ch2AbsWS = currentPacket.Ch2AbsoluteWsCounter,
                ch1PolWS = currentPacket.Ch1PolarizedWsCounter,
                ch2PolWS = currentPacket.Ch2PolarizedWsCounter,
                aux1WS = currentPacket.Aux1WsCounter,
                aux2WS = currentPacket.Aux2WsCounter,
                aux3WS = currentPacket.Aux3WsCounter,
                aux4WS = currentPacket.Aux4WsCounter,
                aux5WS = currentPacket.Aux5WsCounter,
            };

            if (previousPacket != null)
            {
                // if new counter is lower, could be rollover, or could be out of order
                if ((currentPacket.SecondsCounter < previousPacket.SecondsCounter) &&
                    (0xffffffu - previousPacket.SecondsCounter) + currentPacket.SecondsCounter > TimeSpan.FromDays(60).TotalSeconds)
                {
                    Console.WriteLine($"Skipping out of order packet for {currentPacket.DeviceAddress} with seconds {currentPacket.SecondsCounter} less than previous {previousPacket.SecondsCounter}");
                }

            }


            using (var db = _dbFactory.CreateDbContext())
            {
                db.UsageReadings.Add(newReading);
                db.SaveChanges();
            }

            Console.WriteLine($"    record packet for {newReading.device}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

    }


    private async Task CompactionTask(CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var tasks = _lastPackets.Select(p => CompactForUnit(p.Key, cancellationToken)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            var delay = tasks.Any(t => t.Result == false)
                ? TimeSpan.FromSeconds(1)
                : TimeSpan.FromMinutes(10);

            Console.WriteLine($"compaction delayed for {delay} before next cycle");

            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<bool> CompactForUnit(ushort deviceAddress, CancellationToken cancellationToken)
    {
        var startThreshold = _compactionProgress[deviceAddress];
        var endThreshold = DateTime.UtcNow.Subtract(_options.CompactionEndThreshold);

        var counter = 0;
        
        Console.WriteLine($"{deviceAddress} - beginning compaction from {startThreshold}");
        var sw = Stopwatch.StartNew();

        await using var dbContext = await _dbFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var query =
            from ur in dbContext.UsageReadings
            where ur.device == deviceAddress
                  && ur.dateTimeUTC >= startThreshold
                  && ur.dateTimeUTC < endThreshold
            orderby ur.dateTimeUTC
            select ur;

        var readings = await query.Take(10000).ToListAsync(cancellationToken).ConfigureAwait(false);

        Console.WriteLine($"{deviceAddress} - fetched {readings.Count} readings");

        if (readings.Any())
        {
            var last = readings.First();
            var removed = false;
            foreach (var reading in readings.Skip(1))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (reading.dateTimeUTC.Subtract(last.dateTimeUTC) < _options.CompactToPeriod)
                {
                    counter++;
                    removed = true;
                    dbContext.UsageReadings.Remove(reading);
                    continue;
                }

                if (removed)
                {
                    await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    removed = false;
                }

                last = reading;
            }

            _compactionProgress[deviceAddress] = last.dateTimeUTC;

            if (counter > 0)
                Console.WriteLine($"{deviceAddress} - removed {counter} records in {sw.Elapsed.TotalSeconds} seconds  between {readings.First().dateTimeUTC} and {last.dateTimeUTC}");
        }
        else
        {
            _compactionProgress[deviceAddress] = endThreshold;
        }

        return readings.Count < 10000;
    }

}