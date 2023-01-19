using EnergyMonitor.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnergyMonitor.Data.Query
{
    public static class Helpers
    {
        public static IQueryable<T> ForDates<T>(this IQueryable<T> source, DateTime start, DateTime end) where T : IHaveNullableStartAndEndDates
        {
            return source
                .Where(o =>
                    (o.StartDate == null && o.EndDate == null) ||
                    (o.StartDate == null && o.EndDate >= start) ||
                    (o.EndDate == null && o.StartDate <= end) ||
                    (o.StartDate != null && o.EndDate != null &&
                     (
                         (o.StartDate >= start && o.StartDate <= end) ||
                         (o.EndDate >= start && o.EndDate <= end) ||
                         (start >= o.StartDate && start <= o.EndDate) ||
                         (end >= o.StartDate && end <= o.EndDate)
                     )
                    )
                );
        }

        public static decimal? FindSplit(this DateTime referenceDate, DateTime a, DateTime b)
        {
            if (a == b) return 0;

            var start = a < b ? a : b;
            var end = a < b ? b : a;

            if (referenceDate < start || referenceDate > end) return null;

            var totalTicks = end.Ticks - start.Ticks;
            var referenceTicks = referenceDate.Ticks - start.Ticks;

            return (decimal)referenceTicks / totalTicks;
        }

        public static uint FindMidPoint(this decimal split, uint a, uint b)
        {
            return a <= b
                ? a + (uint)((b - a) * split)
                : a - (uint)((a - b) * split);
        }
        public static ulong FindMidPoint(this decimal split, ulong a, ulong b)
        {
            return a <= b
                ? a + (ulong)((b - a) * split)
                : a - (ulong)((a - b) * split);
        }

        public static DateTime? LaterOf(DateTime? a, DateTime? b, bool nullIsInfinity)
        {
            if (a == null && b == null) return null;

            if (a == null)
                return nullIsInfinity ? a : b;

            if (b == null)
                return nullIsInfinity ? b : a;

            return a >= b ? a : b;
        }
        public static DateTime? EarlierOf(DateTime? a, DateTime? b, bool nullIsInfinity)
        {
            if (a == null && b == null) return null;

            if (a == null)
                return nullIsInfinity ? a : b;

            if (b == null)
                return nullIsInfinity ? b : a;

            return a <= b ? a : b;
        }

        internal static uint CheckForRolloverOf3ByteCounter(this uint currentValue, uint previousValue)
        {
            //if the current value is less than the previous value, assume it has rolled over to zero
            if (currentValue < previousValue)
                return (0xffffffu - previousValue) + currentValue;
            else
                return currentValue - previousValue;
        }

        public static uint CheckForRolloverOf4ByteCounter(this uint currentValue, uint previousValue)
        {
            //if the current value is less than the previous value, assume it has rolled over to zero
            if (currentValue < previousValue)
                return (0xffffffffu - previousValue) + currentValue;
            else
                return currentValue - previousValue;
        }

        public static ulong CheckForRolloverOf5ByteCounter(this ulong currentValue, ulong previousValue)
        {
            //if the current value is less than the previous value, assume it has rolled over to zero
            if (currentValue < previousValue)
                return (0xffffffffffuL - previousValue) + currentValue;
            else
                return currentValue - previousValue;
        }


    }

    public class UsageDataPoint
    {
        public DateTime DateTime { get; set; }
        public decimal Watts { get; set; }
    }
    public static class UsageQuery
    {
        public static async
            Task<(Dictionary<BrultechSource, List<UsageDataPoint>>, Dictionary<BrultechLoad, List<UsageDataPoint>>)>
            GetUsageAsync(this IDbContextFactory<EnergyMonitorContext> dbFactory, DateTime start, DateTime end)
        {
            var usageReadingsTask = dbFactory.GetUsageReadingsAsync(start, end);
            var sourceAssignmentsTask = dbFactory.GetSourceAssignments(start, end);
            var loadAssignmentsTask = dbFactory.GetLoadAssignmentsAsync(start, end);

            var usageReadings = await usageReadingsTask.ConfigureAwait(false);
            await using (var dbContext = await dbFactory.CreateDbContextAsync())
            {

                var devices = usageReadings.Select(o => o.device).Distinct().ToList();
                foreach (var dev in devices)
                {
                    var frontEnd = await dbContext.UsageReadings.AsNoTracking().Where(u => u.device == dev && u.dateTimeUTC <= start)
                        .OrderByDescending(u => u.dateTimeUTC).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (frontEnd != null)
                        usageReadings.Add(frontEnd);

                    var tailEnd = await dbContext.UsageReadings.AsNoTracking().Where(u => u.device == dev && u.dateTimeUTC >= end)
                        .OrderBy(u => u.dateTimeUTC).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (tailEnd != null)
                        usageReadings.Add(tailEnd);
                }
            }

            await Task.WhenAll(sourceAssignmentsTask, loadAssignmentsTask).ConfigureAwait(false);


            return default;
        }

        private static async Task<List<BrultechSourceChannelAssignment>> GetSourceAssignments(this IDbContextFactory<EnergyMonitorContext> dbFactory, DateTime start, DateTime end)
        {
            await using var dbContext = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            return await dbContext.BrultechSourceChannelAssignment
                .AsNoTracking()
                .Include(a => a.Source)
                .Include(a => a.Device)
                .ForDates(start, end)
                .ToListAsync().ConfigureAwait(false);
        }

        public static async Task<List<BrultechLoadChannelAssignment>> GetLoadAssignmentsAsync(
            this IDbContextFactory<EnergyMonitorContext> dbFactory, DateTime start, DateTime end)
        {
            await using var dbContext = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            return await dbContext.BrultechLoadChannelAssignments
                .AsNoTracking()
                .Include(a => a.Source)
                .Include(a => a.Device)
                .Include(a => a.Load)
                .ForDates(start, end)
                .ToListAsync().ConfigureAwait(false);
        }

        public static async Task<List<UsageReading>> GetUsageReadingsAsync(
            this IDbContextFactory<EnergyMonitorContext> dbFactory, DateTime start, DateTime end)
        {
            await using var dbContext = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
            return await dbContext.UsageReadings
                .AsNoTracking()
                .Where(o => o.dateTimeUTC >= start && o.dateTimeUTC <= end)
                .ToListAsync().ConfigureAwait(false);
        }

        public static UsageReading FindMidpointValues(this DateTime referenceDate, UsageReading a, UsageReading b)
        {
            if (a == null && b == null) return null;
            if (a == null) return b;
            if (b == null) return a;

            var start = a.dateTimeUTC < b.dateTimeUTC ? a : b;
            var end = a.dateTimeUTC < b.dateTimeUTC ? b : a;

            var split = referenceDate.FindSplit(start.dateTimeUTC, end.dateTimeUTC);
            if (split == null) return null;

            var ret = new UsageReading
            {
                dateTimeUTC = referenceDate,
                device = a.device,
                seconds = split.Value.FindMidPoint(a.seconds, b.seconds),

                ch1AbsWS = split.Value.FindMidPoint(start.ch1AbsWS, end.ch1AbsWS),
                ch2AbsWS = split.Value.FindMidPoint(start.ch2AbsWS, end.ch2AbsWS),
                aux1WS =   split.Value.FindMidPoint(start.aux1WS,   end.aux1WS),
                aux2WS =   split.Value.FindMidPoint(start.aux2WS,   end.aux2WS),
                aux3WS =   split.Value.FindMidPoint(start.aux3WS,   end.aux3WS),
                aux4WS =   split.Value.FindMidPoint(start.aux4WS,   end.aux4WS),
                aux5WS =   split.Value.FindMidPoint(start.aux5WS,   end.aux5WS),
            };
            return ret;
        }

        public static object GetChannel(this UsageReading reading, int channel)
        {
            switch (channel)
            {
                case 1: return reading?.ch1AbsWS;
                case 2: return reading?.ch2AbsWS;
                case 3: return reading?.aux1WS;
                case 4: return reading?.aux2WS;
                case 5: return reading?.aux3WS;
                case 6: return reading?.aux4WS;
                case 7: return reading?.aux5WS;
            }

            throw new ArgumentOutOfRangeException(nameof(channel));
        }

        public static async Task<(Dictionary<BrultechSource, long> Sources, Dictionary<BrultechLoad, long> Loads)> GetUsageBetweenDates(this IDbContextFactory<EnergyMonitorContext> dbFactory, DateTime start, DateTime end)
        {
            List<BrultechSourceChannelAssignment> sourceAssignments;
            List<BrultechLoadChannelAssignment> loadAssignments;
            await using (var dbContext = await dbFactory.CreateDbContextAsync())
            {
                sourceAssignments = await dbContext.BrultechSourceChannelAssignment
                    .AsNoTracking()
                    .Include(a => a.Source)
                    .Include(a => a.Device)
                    .ForDates(start, end)
                    .ToListAsync().ConfigureAwait(false);

                loadAssignments = await dbContext.BrultechLoadChannelAssignments
                    .AsNoTracking()
                    .Include(a => a.Source)
                    .Include(a => a.Device)
                    .Include(a => a.Load)
                    .ForDates(start, end)
                    .ToListAsync().ConfigureAwait(false);
            };

            var sources = new Dictionary<BrultechSource, long>();
            foreach (var source in sourceAssignments.GroupBy(sa => sa.Source))
            {
                long ws = 0;
                foreach (var assignment in source)
                {
                    var sTask = dbFactory.FindStartRecordAsync(assignment.DeviceId, assignment.StartDate, start);
                    var eTask = dbFactory.FindEndRecordAsync(assignment.DeviceId, assignment.EndDate, end);

                    await Task.WhenAll(sTask, eTask).ConfigureAwait(false);

                   var delta = (ulong)eTask.Result.GetChannel(assignment.ChannelId) - (ulong)sTask.Result.GetChannel(assignment.ChannelId);
                    ws += (long)delta;
                }
                sources.Add(source.Key, ws);
            }

            var otherLoads = sources.ToDictionary(s => s.Key.SourceId, s => s.Value);
            var loads = new Dictionary<BrultechLoad, long>();
            foreach (var load in loadAssignments.GroupBy(la => la.LoadId))
            {
                long ws = 0;
                foreach (var assignment in load)
                {
                    var sTask = dbFactory.FindStartRecordAsync(assignment.DeviceId, assignment.StartDate, start);
                    var eTask = dbFactory.FindEndRecordAsync(assignment.DeviceId, assignment.EndDate, end);

                    await Task.WhenAll(sTask, eTask).ConfigureAwait(false);

                    var ctr1 = sTask.Result.GetChannel(assignment.ChannelId);
                    var ctr2 = eTask.Result.GetChannel(assignment.ChannelId);

                    long delta=0;
                    if (ctr1 is uint u1 && ctr2 is uint u2)
                    {
                       delta = Helpers.CheckForRolloverOf4ByteCounter(u2, u1);

                    }
                    else if (ctr1 is ulong ul1 && ctr2 is ulong ul2)
                    {
                        delta = Convert.ToInt64( Helpers.CheckForRolloverOf5ByteCounter(ul2, ul1));
                    }

                    ws += delta;
                    otherLoads[assignment.SourceId] -= ws;
                }
                loads.Add(load.First().Load, ws);
            }

            foreach (var otherLoad in otherLoads)
            {
                loads.Add(new BrultechLoad { LoadId = -1, Name = $"{sources.Where(s => s.Key.SourceId == otherLoad.Key).Select(o => o.Key.Name).FirstOrDefault()} Other Loads" }, otherLoad.Value);
            };

            return (sources, loads);
        }

        public static async Task<UsageReading> FindStartRecordAsync(this IDbContextFactory<EnergyMonitorContext> dbFactory,
            int deviceId, DateTime? limit, DateTime start)
        {
            await using var dbContext = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);

            if (limit >= start)
            {
                return await dbContext.UsageReadings.Where(u => u.device == deviceId)
                    .Where(u => u.dateTimeUTC >= limit).OrderByDescending(u => u.dateTimeUTC)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
            }
            var p1 = await dbContext.UsageReadings.Where(u => u.device == deviceId)
                .Where(u => u.dateTimeUTC <= start).OrderByDescending(u => u.dateTimeUTC)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            var p2 = await dbContext.UsageReadings.Where(u => u.device == deviceId)
                .Where(u => u.dateTimeUTC >= start).OrderBy(u => u.dateTimeUTC)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return start.FindMidpointValues(p1, p2);
        }

        public static async Task<UsageReading> FindEndRecordAsync(this IDbContextFactory<EnergyMonitorContext> dbFactory,
            int deviceId, DateTime? limit, DateTime end)
        {
            await using var dbContext = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);

            if (limit <= end)
            {
                return await dbContext.UsageReadings.Where(u => u.device == deviceId)
                    .Where(u => u.dateTimeUTC <= limit).OrderByDescending(u => u.dateTimeUTC)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
            }
            var p1 = await dbContext.UsageReadings.Where(u => u.device == deviceId)
                .Where(u => u.dateTimeUTC <= end).OrderByDescending(u => u.dateTimeUTC)
                .FirstOrDefaultAsync().ConfigureAwait(false);
            var p2 = await dbContext.UsageReadings.Where(u => u.device == deviceId)
                .Where(u => u.dateTimeUTC >= end).OrderBy(u => u.dateTimeUTC)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return end.FindMidpointValues(p1, p2);
        }

    }
}
