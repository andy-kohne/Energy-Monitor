using Brultech.ECM1240;
using System.IO.Ports;

namespace Brultech;

public class BrultechSettings
{
    public BrultechSerialPortOptions[] Ports { get; set; }
}


public class BrultechSerialPortOptions
{
    public string PortName { get; set; }
    public ushort[] Devices { get; set; } = Array.Empty<ushort>();
}

public class BrultechSerialPort
{
    private readonly BrultechSerialPortOptions _brultechSerialPortOptions;
    public EventHandler<PacketReceivedArgs>? PacketReceived;

    private const int PacketLength = 65;
    private readonly byte[] _receiveBuffer = new byte[1024];
    private readonly List<DateTime> _badPackets = new();
    private int _count = 0;
    private int _processing;
    private readonly SerialPort _serialPort;

    public BrultechSerialPort(BrultechSerialPortOptions brultechSerialPortOptions)
    {
        _brultechSerialPortOptions = brultechSerialPortOptions;
        _serialPort = new SerialPort(_brultechSerialPortOptions.PortName, 19200, Parity.None, 8, StopBits.One);
        _serialPort.DataReceived += SerialDataReceived;
    }

    public void Start()
    {
        _serialPort.Open();
    }

    public void Stop()
    {
        _serialPort.Close();
    }

    private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        if (Interlocked.CompareExchange(ref _processing, 1, 0) != 0)
            return;

        var bytesRead = ((SerialPort)sender).Read(_receiveBuffer, _count, 1024 - _count);
        _count += bytesRead;

        ProcessBuffer();

        _processing = 0;
    }

    internal void ProcessBuffer()
    {
        if (_count < PacketLength) return;

        var pos = 0;
        var end = 0;
        var packets = 0;
        while (_count - pos >= PacketLength)
        {
            if (!IsPacket(_receiveBuffer, pos))
            {
                pos++;
                continue;
            }

            var packetBytes = _receiveBuffer.AsSpan()[pos..(pos + PacketLength)];

            pos += PacketLength;
            end = pos;

            try
            {
                packets++;
                var packet = new Packet(packetBytes);
                if (_brultechSerialPortOptions.Devices.Any() && !_brultechSerialPortOptions.Devices.Contains(packet.DeviceAddress))
                {
                    Console.WriteLine($"UNEXPECTED DEVICE: {packet.DeviceAddress}");
                    continue;
                }

                Task.Run(() => PacketReceived?.Invoke(this, new PacketReceivedArgs(packet)));
            }
            catch (PacketException)
            {
                Console.WriteLine($"BAD CHECKSUM - from head {pos}");
                for (var i = _badPackets.Count - 1; i >= 0; i--)
                {
                    if (_badPackets[i] < DateTime.UtcNow.AddMinutes(-2))
                        _badPackets.RemoveAt(i);
                }
                _badPackets.Add(DateTime.UtcNow);
                if (_badPackets.Count > 5)
                    throw;
            }
        }

        if (_count - end > PacketLength)
            end = _count - PacketLength;

        var bytesToKeep = _count - end;

        var discarding = end - (packets * PacketLength);
        if (discarding > 0)
            Console.WriteLine($" discarding {discarding} out of {_count} bytes, processed {packets} packets");

        Array.Copy(_receiveBuffer, _count - bytesToKeep, _receiveBuffer, 0, bytesToKeep);
        _count = bytesToKeep;
    }

    private static bool IsPacket(byte[] receiveBuffer, int offset)
    {
        return receiveBuffer[offset] == 0xFE &&
               receiveBuffer[offset + 1] == 0xFF &&
               receiveBuffer[offset + 2] == 0x03 &&
               receiveBuffer[offset + 62] == 0xFF &&
               receiveBuffer[offset + 63] == 0xFE;
    }

}
