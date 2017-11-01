namespace Brultech.ECM1240
{
    public class PacketReceivedArgs
    {
        public readonly Packet Packet;

        public PacketReceivedArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}