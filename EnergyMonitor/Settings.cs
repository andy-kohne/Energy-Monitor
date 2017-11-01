namespace EnergyMonitor
{
    public class Settings
    {
        public string[] AllowedRemotes { get; set; } = { };
        public int Port { get; set; } = 8082;

        public string WeatherStation { get; set; }
        public string WuApiKey { get; set; }
    }
}
