using EnergyMonitor.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace EnergyMonitor.Data;

public class EnergyMonitorContext : DbContext
{
    public DbSet<UsageReading> UsageReadings { get; set; }
    public DbSet<WeatherReading> WeatherReadings { get; set; }


    public DbSet<BrultechDevice> BrultechDevices { get; set; }
    public DbSet<BrultechChannel> BrultechChannels { get; set; }
    public DbSet<BrultechSource> BrultechSources { get; set; }
    public DbSet<BrultechLoad> BrultechLoads { get; set; }
    public DbSet<BrultechSourceChannelAssignment> BrultechSourceChannelAssignment { get; set; }
    public DbSet<BrultechLoadChannelAssignment> BrultechLoadChannelAssignments { get; set; }


    public EnergyMonitorContext()
    { }

    public EnergyMonitorContext(DbContextOptions opts) : base(opts)
    { }
}