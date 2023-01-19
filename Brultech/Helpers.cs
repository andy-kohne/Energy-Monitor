using Brultech.ECM1240;
using System.Text;

namespace Brultech;

public static class Helpers
{
    internal static ushort GetBigEndianUShort(this ReadOnlySpan<byte> bytes, int startIndex)
    {
        return Convert.ToUInt16(bytes[startIndex] << 8 | bytes[startIndex+1]);
    }

    internal static ushort GetUShort(this ReadOnlySpan<byte> bytes, int startIndex)
    {
        return (ushort)
            (Convert.ToUInt16(bytes[startIndex + 1] << 8) |
             Convert.ToUInt16(bytes[startIndex]));
    }

    internal static ulong Get5ByteCounter(this ReadOnlySpan<byte> bytes, int startIndex)
    {
        return (Convert.ToUInt64(bytes[startIndex + 4]) << 32) |
               (Convert.ToUInt64(bytes[startIndex + 3]) << 24) |
               (Convert.ToUInt64(bytes[startIndex + 2]) << 16) |
               (Convert.ToUInt64(bytes[startIndex + 1]) << 8) |
               (Convert.ToUInt64(bytes[startIndex]));
    }

    internal static uint Get4ByteCounter(this ReadOnlySpan<byte> bytes, int startIndex)
    {
        return (Convert.ToUInt32(bytes[startIndex + 3]) << 24) |
               (Convert.ToUInt32(bytes[startIndex + 2]) << 16) |
               (Convert.ToUInt32(bytes[startIndex + 1]) << 8) |
               (Convert.ToUInt32(bytes[startIndex]));
    }

    internal static uint Get3ByteCounter(this ReadOnlySpan<byte> bytes, int startIndex)
    {
        return (Convert.ToUInt32(bytes[startIndex + 2]) << 16) |
               (Convert.ToUInt32(bytes[startIndex + 1]) << 8) |
               (Convert.ToUInt32(bytes[startIndex]));
    }


    internal static ushort Get2ByteCounter(this ReadOnlySpan<byte> bytes, int startIndex)
    {
        return (ushort) ((ushort)(Convert.ToUInt16(bytes[startIndex + 1]) << 8) |
                         (ushort)(Convert.ToUInt16(bytes[startIndex])));
    }


    internal static uint CheckForRolloverOf3ByteCounter(this uint currentValue, uint previousValue)
    {
        //if the current value is less than the previous value, assume it has rolled over to zero
        if (currentValue < previousValue)
            return (0xffffffu - previousValue) + currentValue;
        else
            return currentValue - previousValue;
    }

    public static uint CheckForRolloverOf4ByteCounter(this uint currentValue, uint previousValue)
    {
        //if the current value is less than the previous value, assume it has rolled over to zero
        if (currentValue < previousValue)
            return (0xffffffffu - previousValue) + currentValue;
        else
            return currentValue - previousValue;
    }

    public static ulong CheckForRolloverOf5ByteCounter(this ulong currentValue, ulong previousValue)
    {
        //if the current value is less than the previous value, assume it has rolled over to zero
        if (currentValue < previousValue)
            return (0xffffffffffuL - previousValue) + currentValue;
        else
            return currentValue - previousValue;
    }

    public static Rates CompareTo(this Packet currentPacket, Packet previousPacket)
    {
        //for sanity sake, ensure packets are from the same unit
        if (currentPacket.DeviceAddress != previousPacket.DeviceAddress)
            throw new Exception();

        var ret = new Rates();
        try
        {

            //determine the period length
            ret.PeriodLength = currentPacket.SecondsCounter.CheckForRolloverOf3ByteCounter(previousPacket.SecondsCounter);// Ecm1240Rates.CheckForRolloverOf3ByteCounter(SecondsCounter, previousPacket.SecondsCounter);
            //calculate the watt seconds consumed
            ret.Ch1NetAbsoluteWs = currentPacket.Ch1AbsoluteWsCounter.CheckForRolloverOf5ByteCounter(previousPacket.Ch1AbsoluteWsCounter); // Ecm1240Rates.CheckForRolloverOf5ByteCounter(Ch1AbsoluteWSCounter, previousPacket.Ch1AbsoluteWSCounter);
            ret.Ch2NetAbsoluteWs = currentPacket.Ch2AbsoluteWsCounter.CheckForRolloverOf5ByteCounter(previousPacket.Ch2AbsoluteWsCounter); // Ecm1240Rates.CheckForRolloverOf5ByteCounter(Ch2AbsoluteWSCounter, previousPacket.Ch2AbsoluteWSCounter);
            ret.Ch1NetPolarizedWs = currentPacket.Ch1PolarizedWsCounter.CheckForRolloverOf5ByteCounter(previousPacket.Ch1PolarizedWsCounter); // Ecm1240Rates.CheckForRolloverOf5ByteCounter(Ch1PolarizedWSCounter, previousPacket.Ch1PolarizedWSCounter);
            ret.Ch2NetPolarizedWs = currentPacket.Ch2PolarizedWsCounter.CheckForRolloverOf5ByteCounter(previousPacket.Ch2PolarizedWsCounter); // Ecm1240Rates.CheckForRolloverOf5ByteCounter(Ch2PolarizedWSCounter, previousPacket.Ch2PolarizedWSCounter);
            ret.Aux1NetWs = currentPacket.Aux1WsCounter.CheckForRolloverOf4ByteCounter(previousPacket.Aux1WsCounter); // Ecm1240Rates.CheckForRolloverOf4ByteCounter(Aux1WSCounter, previousPacket.Aux1WSCounter);
            ret.Aux2NetWs = currentPacket.Aux2WsCounter.CheckForRolloverOf4ByteCounter(previousPacket.Aux2WsCounter); //  Ecm1240Rates.CheckForRolloverOf4ByteCounter(Aux2WSCounter, previousPacket.Aux2WSCounter);
            ret.Aux3NetWs = currentPacket.Aux3WsCounter.CheckForRolloverOf4ByteCounter(previousPacket.Aux3WsCounter); //  Ecm1240Rates.CheckForRolloverOf4ByteCounter(Aux3WSCounter, previousPacket.Aux3WSCounter);
            ret.Aux4NetWs = currentPacket.Aux4WsCounter.CheckForRolloverOf4ByteCounter(previousPacket.Aux4WsCounter); //  Ecm1240Rates.CheckForRolloverOf4ByteCounter(Aux4WSCounter, previousPacket.Aux4WSCounter);
            ret.Aux5NetWs = currentPacket.Aux5WsCounter.CheckForRolloverOf4ByteCounter(previousPacket.Aux5WsCounter); //  Ecm1240Rates.CheckForRolloverOf4ByteCounter(Aux5WSCounter, previousPacket.Aux5WSCounter);
            //calculate the rates
            ret.Ch1AbsoluteRate = (int)(ret.Ch1NetAbsoluteWs / ret.PeriodLength);
            ret.Ch2AbsoluteRate = (int)(ret.Ch2NetAbsoluteWs / ret.PeriodLength);
            ret.Ch1PolarizedRate = (int)(ret.Ch1NetPolarizedWs / ret.PeriodLength);
            ret.Ch2PolarizedRate = (int)(ret.Ch2NetPolarizedWs / ret.PeriodLength);
            ret.Aux1Rate = (int)(ret.Aux1NetWs / ret.PeriodLength);
            ret.Aux2Rate = (int)(ret.Aux2NetWs / ret.PeriodLength);
            ret.Aux3Rate = (int)(ret.Aux3NetWs / ret.PeriodLength);
            ret.Aux4Rate = (int)(ret.Aux4NetWs / ret.PeriodLength);
            ret.Aux5Rate = (int)(ret.Aux5NetWs / ret.PeriodLength);
            ret.OtherRate = (int)ret.Ch1AbsoluteRate -
                            (int)ret.Ch2AbsoluteRate -
                            (int)ret.Aux1Rate -
                            (int)ret.Aux2Rate -
                            (int)ret.Aux3Rate -
                            (int)ret.Aux4Rate -
                            (int)ret.Aux5Rate;
            //calculate the kwh consumed
            ret.Ch1AbsoluteKwh = ret.Ch1NetAbsoluteWs / 3600000;
            ret.Ch2AbsoluteKwh = ret.Ch2NetAbsoluteWs / 3600000;
            ret.Ch1PolarizedKwh = ret.Ch1NetPolarizedWs / 3600000;
            ret.Ch2PolarizedKwh = ret.Ch2NetPolarizedWs / 3600000;
            ret.Aux1Kwh = ret.Aux1NetWs / 3600000;
            ret.Aux2Kwh = ret.Aux2NetWs / 3600000;
            ret.Aux3Kwh = ret.Aux3NetWs / 3600000;
            ret.Aux4Kwh = ret.Aux4NetWs / 3600000;
            ret.Aux5Kwh = ret.Aux5NetWs / 3600000;
        }
        catch (Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Message: {0}", ex.Message).AppendLine();
            sb.AppendFormat("StackTrace: {0}", ex.StackTrace).AppendLine();
            sb.AppendFormat("TargetSite: {0}", ex.TargetSite.ToString());
            Console.WriteLine(sb);
            throw;
        }

        return ret;
    }

}