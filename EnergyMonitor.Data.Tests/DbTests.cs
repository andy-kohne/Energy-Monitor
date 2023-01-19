using EnergyMonitor.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace EnergyMonitor.Data.Tests
{
    public class DbTests : DbTestBase
    {
        public DbTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) 
        {
        }

        [Fact]
        public async Task Test1()
        {
            await using var db = await DbFactory.CreateDbContextAsync();

            (await db.BrultechLoadChannelAssignments.ToListAsync()).ForEach(o => db.BrultechLoadChannelAssignments.Remove(o));
            (await db.BrultechSourceChannelAssignment.ToListAsync()).ForEach(o => db.BrultechSourceChannelAssignment.Remove(o));
            (await db.BrultechSources.ToListAsync()).ForEach(o => db.BrultechSources.Remove(o));
            (await db.BrultechLoads.ToListAsync()).ForEach(o => db.BrultechLoads.Remove(o));
            (await db.BrultechDevices.ToListAsync()).ForEach(o => db.BrultechDevices.Remove(o));
            await db.SaveChangesAsync();


            var dev = db.BrultechDevices.Add(new BrultechDevice { Description = "position 1", DeviceId = 4673, CommandByte = 0xfc });

            var src = db.BrultechSources.Add(new BrultechSource { Name = "BGE Circuit A" });
            var load = db.BrultechLoads.Add(new BrultechLoad { Name = "Fridge" });

            db.BrultechSourceChannelAssignment.Add(new BrultechSourceChannelAssignment { Source = src.Entity, ChannelId = 1, Device = dev.Entity });
            db.BrultechLoadChannelAssignments.Add(new BrultechLoadChannelAssignment { Device = dev.Entity, ChannelId = 4, Load = load.Entity, Source = src.Entity });
            await db.SaveChangesAsync();

            var loaded = await db.BrultechSources.Include(o => o.ChannelAssignments).ToListAsync();
        }

    }
}