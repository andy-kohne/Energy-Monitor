using Brultech;
using Brultech.ECM1240;
using EnergyMonitor.Data;
using EnergyMonitor.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EnergyPi;

public class StorageOptions
{
    public bool Compaction { get; set; } = false;
    public TimeSpan CompactionThreshold { get; set; } = TimeSpan.FromDays(62);
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
    }


    public async Task Init()
    {
        _lastPackets = await GetLastPackets().ConfigureAwait(false);

        if (_options.Compaction)
        {
            foreach (var packet in _lastPackets)
            {
                _compactionProgress.Add(packet.Key, DateTime.UtcNow.AddMonths(-12));
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

                var rates = currentPacket.CompareTo(previousPacket);
                newReading.ch1Watts = rates.Ch1AbsoluteRate;
                newReading.ch2Watts = rates.Ch2AbsoluteRate;
                newReading.aux1Watts = rates.Aux1Rate;
                newReading.aux2Watts = rates.Aux2Rate;
                newReading.aux3Watts = rates.Aux3Rate;
                newReading.aux4Watts = rates.Aux4Rate;
                newReading.aux5Watts = (int)rates.Aux5Rate;
                newReading.otherLoads = rates.OtherRate;
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

            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<bool> CompactForUnit(ushort deviceAddress, CancellationToken cancellationToken)
    {
        var startThreshold = _compactionProgress[deviceAddress];
        var endThreshold = DateTime.UtcNow.AddMonths(-2);
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

        var last = readings.First();
        var removed = false;
        foreach (var reading in readings.Skip(1))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            if (reading.dateTimeUTC.Subtract(last.dateTimeUTC) < TimeSpan.FromMinutes(5))
            {
                counter++;
                removed = true;
                dbContext.UsageReadings.Remove(reading);
                continue;
            }

            if (removed)
            {
                var newRates = reading.ToPacket().CompareTo(last.ToPacket());
                reading.ch1Watts = newRates.Ch1AbsoluteRate;
                reading.ch2Watts = newRates.Ch2AbsoluteRate;
                reading.aux1Watts = newRates.Aux1Rate;
                reading.aux2Watts = newRates.Aux2Rate;
                reading.aux3Watts = newRates.Aux3Rate;
                reading.aux4Watts = newRates.Aux4Rate;
                reading.aux5Watts = (int)newRates.Aux5Rate;
                reading.otherLoads = newRates.OtherRate;
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            last = reading;
        }

        _compactionProgress[deviceAddress] = last.dateTimeUTC;

        if (counter > 0)
            Console.WriteLine($"{deviceAddress} - removed {counter} records in {sw.Elapsed.TotalSeconds} seconds");

        return readings.Count < 10000;
    }

}