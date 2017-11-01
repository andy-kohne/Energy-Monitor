using Brultech.ECM1240;
using EnergyMonitor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace EnergyMonitor
{
    public static class ConfigurationExtensions
    {
        public static void RegisterConfigurationItem<TConfiguration>(this IServiceCollection services,
            IConfiguration configuration) where TConfiguration : class, new()
        {
            var item = new TConfiguration();
            configuration.Bind(item);
            services.AddSingleton(item);
        }

    }

    public static class DI
    {
        public static ServiceCollection Regsiter()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(typeof(TcpReceiver));
            serviceCollection.AddSingleton(typeof(EnergyMonitorService));
#if DEBUG
            var path = AppDomain.CurrentDomain.BaseDirectory;
#else
            var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Path.Combine(Environment.GetEnvironmentVariable("ProgramData"), "EnergyMonitor")
                : Path.Combine("/etc", "energy");
#endif

            var builder = new ConfigurationBuilder()
                    .SetBasePath(path)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                //.AddEnvironmentVariables()
                ;

            var configuration = builder.Build();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.RegisterConfigurationItem<Settings>(configuration.GetSection("Settings"));

            var connectionString = configuration.GetConnectionString("EnergyMonitoring");

            serviceCollection
                .AddDbContext<EnergyMonitorContext>(optionsBuilder =>
                {
                    optionsBuilder.UseMySql(connectionString);
                })
                .AddEntityFrameworkMySql();

            return serviceCollection;
        }
    }
}
