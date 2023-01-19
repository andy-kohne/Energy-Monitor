using Brultech;
using EnergyMonitor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EnergyPi;

class Program
{
    static async Task Main(string[] args)
    {
        var hostBuilder = new HostBuilder()
            .UseSystemd()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureHostConfiguration(config =>
            {
                config.AddEnvironmentVariables(prefix: "DOTNET_");
                config.AddCommandLine(args);
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                var env = context.HostingEnvironment;
                builder.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true).AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);

                services.AddSingleton( typeof(SshTunnel));
                services.AddSingleton(typeof(IHostedService), typeof(EnergyMonitorService));
                services.AddSingleton(typeof(DataModule));
                services.AddSingleton(typeof(BrultechSerialReceiver));

                services.AddSingleton(context.Configuration.GetSection("BrultechSettings").Get<BrultechSettings>() ?? new BrultechSettings());

                var connectionString = context.Configuration.GetConnectionString("EnergyMonitoring");

                services.AddDbContextFactory<EnergyMonitorContext>(optionsBuilder =>
                {
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }).AddEntityFrameworkMySql();
            });


        var host = hostBuilder.Build();

        // make sure the tunnel is created first, since it is needed by the DbContextOptionsBuilder
        var tunnel = host.Services.GetRequiredService<SshTunnel>();

        await host.RunAsync();
    }


}