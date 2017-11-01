using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyMonitor.Data
{
    [Table("weather")]
    public class WeatherReading
    {
        public long id { get; set; }
        public string station { get; set; }
        public DateTime datetimeUtc { get; set; }
        public decimal? temperature { get; set; }
        public int? humidity { get; set; }
        public decimal? windspeed { get; set; }
        public int? winddir { get; set; }
        public decimal? windgust { get; set; }
    }
}