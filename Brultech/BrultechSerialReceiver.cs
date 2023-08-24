namespace Brultech;

public class BrultechSerialReceiver
{
    private readonly BrultechSettings _brultechSettings;
    private readonly List<BrultechSerialPort> _ports;
    
    public EventHandler<PacketReceivedArgs>? PacketReceived;

    public BrultechSerialReceiver(BrultechSettings brultechSettings)
    {
        _brultechSettings = brultechSettings;
        _ports = _brultechSettings.Ports.Select(p =>
        {
            var ret = new BrultechSerialPort(p);
            ret.PacketReceived += SerialPacketReceived;
            return ret;
        }).ToList();
    }

    public void Start()
    {
        _ports.ForEach(p => p.Start());
    }

    public void Stop()
    {
        _ports.ForEach(p => p.Stop());
    }

    private void SerialPacketReceived(object? sender, PacketReceivedArgs e)
    {
        Task.Run(() => PacketReceived?.Invoke(this, e));
    }
}