using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.Loader;
using System.Threading;

namespace EnergyMonitor
{
    class Program
    {
        private static EnergyMonitorService receiver;

        static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += Default_Unloading;

            var serviceProvider = DI.Regsiter().BuildServiceProvider();

            receiver = serviceProvider.GetService<EnergyMonitorService>();

            receiver.MonitorAsync(CancellationToken.None).Wait();
        }
        private static void Default_Unloading(AssemblyLoadContext obj)
        {
            Console.WriteLine("shutting down");
            receiver.StopMonitoring();
            Console.WriteLine("shutdown complete");
        }

    }
}
