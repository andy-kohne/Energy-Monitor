using Microsoft.EntityFrameworkCore;

namespace EnergyMonitor.Data
{
    public class EnergyMonitorContext : DbContext
    {
        public DbSet<UsageReading> UsageReadings { get; set; }
        public DbSet<WeatherReading> WeatherReadings { get; set; }

        public EnergyMonitorContext()
        { }

        public EnergyMonitorContext(DbContextOptions opts) : base(opts)
        { }
    }
}
