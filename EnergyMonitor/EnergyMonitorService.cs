using Brultech.ECM1240;
using EnergyMonitor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EnergyMonitor
{
    public class EnergyMonitorService
    {
        private readonly Settings _settings;
        private readonly TcpReceiver _tcpReceiver;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<ushort, Packet> _lastPackets;
        private readonly Dictionary<ushort, DateTime> _compactionProgress;
        private readonly List<DateTime> _skipDates = new List<DateTime>();

        public EnergyMonitorService(Settings settings, TcpReceiver tcpReceiver, IServiceProvider serviceProvider)
        {
            _settings = settings;
            _tcpReceiver = tcpReceiver;
            _serviceProvider = serviceProvider;

            _lastPackets = new Dictionary<ushort, Packet>();
            _compactionProgress = new Dictionary<ushort, DateTime>();

            _tcpReceiver.PacketReceived += HandleEvent;
        }

        public async Task MonitorAsync(CancellationToken cancellationToken)
        {
            foreach (var packet in await GetLastPackets())
            {
                if (_lastPackets.TryAdd(packet.DeviceAddress, packet))
                    _compactionProgress.Add(packet.DeviceAddress, DateTime.UtcNow.AddMonths(-4));
            }

            var tcpReceiverTask = _tcpReceiver.ListenAsync(_settings.AllowedRemotes, _settings.Port, cancellationToken);
            var compactionTask = CompactionTask(cancellationToken);
            var weatherTask = WeatherTask(cancellationToken);

            await tcpReceiverTask;
        }

        public void StopMonitoring()
        {
            _tcpReceiver.StopListening();
        }

        private void HandleEvent(object sender, PacketReceivedArgs args)
        {
            try
            {
                var currentPacket = args.Packet;
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
                    var rates = currentPacket.CompareTo(previousPacket);
                    newReading.ch1Watts = rates.Ch1AbsoluteRate;
                    newReading.ch2Watts = rates.Ch2AbsoluteRate;
                    newReading.aux1Watts = rates.Aux1Rate;
                    newReading.aux2Watts = rates.Aux2Rate;
                    newReading.aux3Watts = rates.Aux3Rate;
                    newReading.aux4Watts = rates.Aux4Rate;
                    newReading.aux5Watts = (int) rates.Aux5Rate;
                    newReading.otherLoads = rates.OtherRate;
                }

                using (var scope = _serviceProvider.CreateScope())
                using (var dbContext = scope.ServiceProvider.GetService<EnergyMonitorContext>())
                {
                    dbContext.Add(newReading);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task<List<Packet>> GetLastPackets()
        {
            using (var scope = _serviceProvider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetService<EnergyMonitorContext>())
            {
                var devices = await
                    (from ur in dbContext.UsageReadings
                     select ur.device).Distinct().ToListAsync();

                var ret = new List<Packet>();

                foreach (var deviceId in devices)
                {
                    var stored = await
                        (from ur in dbContext.UsageReadings
                         where ur.device == deviceId
                         orderby ur.dateTimeUTC descending
                         select ur).FirstOrDefaultAsync();

                    ret.Add(stored.ToPacket());
                }

                return ret;
            }
        }

        private async Task CompactionTask(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var tasks = _lastPackets.Select(p => CompactForUnit(p.Key)).ToArray();
                await Task.WhenAll(tasks);

                var delay = tasks.Any(t => t.Result == false)
                    ? TimeSpan.FromSeconds(1)
                    : TimeSpan.FromMinutes(10);

                await Task.Delay(delay, cancellationToken);
            }
        }

        private async Task<bool> CompactForUnit(ushort deviceAddress)
        {
            var startThreshold = _compactionProgress[deviceAddress];
            var endThreshold = DateTime.UtcNow.AddMonths(-2);
            var counter = 0;

            using (var scope = _serviceProvider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetService<EnergyMonitorContext>())
            {
                var query =
                    from ur in dbContext.UsageReadings
                    where ur.device == deviceAddress
                          && ur.dateTimeUTC >= startThreshold
                          && ur.dateTimeUTC < endThreshold
                    orderby ur.dateTimeUTC
                    select ur;

                var readings = await query.Take(10000).ToListAsync();

                var last = readings.First();
                var removed = false;
                foreach (var reading in readings.Skip(1))
                {
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
                        await dbContext.SaveChangesAsync();
                    }

                    last = reading;
                }

                _compactionProgress[deviceAddress] = last.dateTimeUTC;

                if (counter > 0)
                    Console.WriteLine($"{deviceAddress} - removed {counter} records");

                return readings.Count < 10000;
            }
        }

        private async Task WeatherTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);

                try
                {
                    await RefreshWeather();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION: {ex}");
                }
            }
        }

        private async Task RefreshWeather()
        {
            if (string.IsNullOrEmpty(_settings.WeatherStation))
                return;

            Console.WriteLine($"refresh weather from {_settings.WeatherStation}");

            using (var scope = _serviceProvider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetService<EnergyMonitorContext>())
            {
                var latest = await dbContext.WeatherReadings
                    .Where(o => o.station == _settings.WeatherStation)
                    .OrderByDescending(o => o.datetimeUtc)
                    .Select(o => o.datetimeUtc).FirstOrDefaultAsync();

                if (latest < DateTime.UtcNow.AddMonths(-2))
                    latest = DateTime.UtcNow.AddMonths(-2);

                var current = latest.ToLocalTime().Date;
                while (current <= DateTime.Now.Date)
                {
                    if (_skipDates.Any(d => d == current || d == current.AddDays(1)))
                        continue;

                    var data =
                        WUnderground.DataSet.Query(_settings.WuApiKey,
                            new[] { WUnderground.Data.History.FeatureRequestString(current) }, $"pws:{_settings.WeatherStation}");

                    if (data.DataFeaturesContent.Count == 0)
                    {
                        Console.WriteLine($"no history data found for {_settings.WeatherStation} from {current}");
                    }
                    else
                    {
                        var observations = ((WUnderground.Data.History)data.DataFeaturesContent.First()).Observations;
                        if (observations.Any())
                        {
                            foreach (var h in observations.Where(o => o.Utcdate.Standardized > latest))
                            {
                                var r = new WeatherReading
                                {
                                    station = _settings.WeatherStation.ToUpper(),
                                    datetimeUtc = h.Utcdate.Standardized,
                                    temperature = h.Tempi,
                                    humidity = h.Hum,
                                    windspeed = h.Wspdi,
                                    winddir = h.Wdird,
                                    windgust = h.Wgusti,
                                };
                                dbContext.WeatherReadings.Add(r);
                            }
                            await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            if (current != DateTime.Now.Date)
                                _skipDates.Add(current);

                            Console.WriteLine(
                                $"got history data from {_settings.WeatherStation} for {current} containing 0 observations");
                        }
                    }

                    current = current.AddDays(1);

                    //rate limit per terms of service
                    if (current <= DateTime.Now.Date)
                        await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}
