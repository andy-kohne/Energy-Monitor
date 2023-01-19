namespace Brultech.ECM1240;

public class Rates
{
    public ulong Ch1NetAbsoluteWs { get; set; }
    public ulong Ch2NetAbsoluteWs { get; set; }
    public ulong Ch1NetPolarizedWs { get; set; }
    public ulong Ch2NetPolarizedWs { get; set; }
    public uint Aux1NetWs { get; set; }
    public uint Aux2NetWs { get; set; }
    public uint Aux3NetWs { get; set; }
    public uint Aux4NetWs { get; set; }
    public uint Aux5NetWs { get; set; }
    public double Ch1AbsoluteKwh { get; set; }
    public double Ch2AbsoluteKwh { get; set; }
    public double Ch1PolarizedKwh { get; set; }
    public double Ch2PolarizedKwh { get; set; }
    public double Aux1Kwh { get; set; }
    public double Aux2Kwh { get; set; }
    public double Aux3Kwh { get; set; }
    public double Aux4Kwh { get; set; }
    public double Aux5Kwh { get; set; }
    public uint PeriodLength { get; set; }
    public int Ch1AbsoluteRate { get; set; }
    public int Ch2AbsoluteRate { get; set; }
    public int Ch1PolarizedRate { get; set; }
    public int Ch2PolarizedRate { get; set; }
    public int Aux1Rate { get; set; }
    public int Aux2Rate { get; set; }
    public int Aux3Rate { get; set; }
    public int Aux4Rate { get; set; }
    public double Aux5Rate { get; set; }
    public int OtherRate { get; set; }
}