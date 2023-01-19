using Brultech;
using Microsoft.Extensions.Hosting;

namespace EnergyPi;

public class EnergyMonitorService : IHostedService
{
    private readonly BrultechSerialReceiver _brultech;
    private readonly DataModule _dataModule;

    public EnergyMonitorService(DataModule dataModule, BrultechSerialReceiver brultech)
    {
        _dataModule = dataModule;
        _brultech = brultech;

        _brultech.PacketReceived += _dataModule.PacketReceived;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dataModule.Init();
        _brultech.Start();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _brultech.Stop();
        _dataModule.Stop();

        return Task.CompletedTask;
    }
}