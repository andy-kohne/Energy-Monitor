using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit.Abstractions;

namespace EnergyMonitor.Data.Tests;

public class DbTestBase : IDisposable
{
    protected readonly ITestOutputHelper TestOutputHelper;
    protected IDbContextFactory<EnergyMonitorContext> DbFactory;
    protected ServiceProvider ServiceProvider;

    public DbTestBase(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;

        var serviceCollection = new ServiceCollection()
            .AddLogging() 
            .AddDbContextFactory<EnergyMonitorContext>(optionsBuilder =>
            {
                optionsBuilder.UseInMemoryDatabase("InMemory");
                optionsBuilder.LogTo(TestOutputHelper.WriteLine);
            }).AddEntityFrameworkMySql();

        ServiceProvider = serviceCollection.BuildServiceProvider();

        DbFactory = ServiceProvider.GetRequiredService<IDbContextFactory<EnergyMonitorContext>>();
    }

    public void Dispose()
    {
        ServiceProvider.Dispose();
    }
}