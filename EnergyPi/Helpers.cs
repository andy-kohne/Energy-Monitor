using Brultech.ECM1240;
using EnergyMonitor.Data.Entity;

namespace EnergyPi;

public static class Helpers
{
    public static Packet ToPacket(this UsageReading stored)
    {
        return new Packet(GetDeviceId(stored.device), GetUnitId(stored.device), stored.volts, stored.ch1AbsWS,
            stored.ch2AbsWS,
            stored.ch1PolWS, stored.ch2PolWS, stored.ch1Current, stored.ch2Current, stored.seconds, stored.aux1WS, stored.aux2WS, stored.aux3WS,
            stored.aux4WS, stored.aux5WS,
            0m, (byte)stored.resetCount, stored.ch1Polarity, stored.ch2Polarity);
    }

    public static byte GetUnitId(int serial)
    {
        return 1;
    }

    public static ushort GetDeviceId(int serial)
    {
        return Convert.ToUInt16(serial);
    }

}