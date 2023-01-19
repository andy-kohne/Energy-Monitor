using Brultech.ECM1240;

namespace Brultech;

public class PacketReceivedArgs
{
    public readonly Packet Packet;

    public PacketReceivedArgs(Packet packet)
    {
        Packet = packet;
    }
}