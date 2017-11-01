using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyMonitor.Data
{
    [Table("usage")]
    public class UsageReading
    {
        public long id { get; set; }
        public System.DateTime dateTimeUTC { get; set; }
        public int device { get; set; }
        public ushort resetCount { get; set; }
        public decimal volts { get; set; }
        public uint seconds { get; set; }
        public decimal ch1Current { get; set; }
        public decimal ch2Current { get; set; }
        public bool ch1Polarity { get; set; }
        public bool ch2Polarity { get; set; }
        public ulong ch1AbsWS { get; set; }
        public ulong ch2AbsWS { get; set; }
        public ulong ch1PolWS { get; set; }
        public ulong ch2PolWS { get; set; }
        public uint aux1WS { get; set; }
        public uint aux2WS { get; set; }
        public uint aux3WS { get; set; }
        public uint aux4WS { get; set; }
        public uint aux5WS { get; set; }
        public int ch1Watts { get; set; }
        public int ch2Watts { get; set; }
        public int aux1Watts { get; set; }
        public int aux2Watts { get; set; }
        public int aux3Watts { get; set; }
        public int aux4Watts { get; set; }
        public decimal aux5Watts { get; set; }
        public int otherLoads { get; set; }
    }
}