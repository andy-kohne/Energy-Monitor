using System;
using System.Diagnostics;
using System.Xml;

namespace WUnderground.Data
{
    [DebuggerDisplay("{_observation_time}")]
    public class CurrentObservationData
    {
        //child classes
        public class ImageData
        {
            //local fields
            private string _url;
            private string _title;
            private string _link;
            //ctor
            protected internal ImageData( XmlReader r)
            {
                r.ReadStartElement();
                while (r.MoveToContent() == XmlNodeType.Element)
                {
                    //capture the name of the field
                    string whichField = r.Name;
                    //read the open tag
                    r.ReadStartElement();
                    //ensure vlaue exists
                    if (r.HasValue)
                    {
                        if (string.Compare(whichField, "url", true) == 0)
                        {
                            _url = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "title", true) == 0)
                        {
                            _title = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "link", true) == 0)
                        {
                            _link = r.ReadContentAsString();
                        }
                    }
                    //read the close tag
                    r.ReadEndElement();
                }
                //read the close tag
                r.ReadEndElement();

            }
            //exposed properties
            public string Url
            {
                get { return _url; }
            }
            public string Title
            {
                get { return _title; }
            }
            public string Link
            {
                get { return _link; }
            }
        }
        public class DisplayLocationData
        {
            //local fields
            private string _full;
            private string _city;
            private string _state;
            private string _stateName;
            private string _country;
            private string _countryIso3166;
            private string _zip;
            private decimal _latitude;
            private decimal _longitude;
            private decimal _elevation;
            //ctor
            protected internal DisplayLocationData( XmlReader r)
            {
                r.ReadStartElement();
                while (r.MoveToContent() == XmlNodeType.Element)
                {
                    //capture the name of the field
                    string whichField = r.Name;
                    //read the open tag
                    r.ReadStartElement();
                    //ensure vlaue exists
                    if (r.HasValue)
                    {
                        if (string.Compare(whichField, "full", true) == 0)
                        {
                            _full = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "city", true) == 0)
                        {
                            _city = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "state", true) == 0)
                        {
                            _state = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "state_name", true) == 0)
                        {
                            _stateName = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "country", true) == 0)
                        {
                            _country = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "country_iso3166", true) == 0)
                        {
                            _countryIso3166 = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "zip", true) == 0)
                        {
                            _zip = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "latitude", true) == 0)
                        {
                            _latitude = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "longitude", true) == 0)
                        {
                            _longitude = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "elevation", true) == 0)
                        {
                            _elevation = r.ReadContentAsDecimal();
                        }
                    }
                    //read the close tag
                    r.ReadEndElement();
                }
                //read the close tag
                r.ReadEndElement();
            }
            //exposed properties
            public string Full
            {
                get { return _full; }
            }
            public string City
            {
                get { return _city; }
            }
            public string State
            {
                get { return _state; }
            }
            public string StateName
            {
                get { return _stateName; }
            }
            public string Country
            {
                get { return _country; }
            }
            public string CountryIso3166
            {
                get { return _countryIso3166; }
            }
            public string Zip
            {
                get { return _zip; }
            }
            public decimal Latitude
            {
                get { return _latitude; }
            }
            public decimal Longitude
            {
                get { return _longitude; }
            }
            public decimal Elevation
            {
                get { return _elevation; }
            }
        }
        public class ObservationLocationData
        {
            //local fields
            private string _full;
            private string _city;
            private string _state;
            private string _country;
            private string _countryIso3166;
            private decimal _latitude;
            private decimal _longitude;
            private string _elevation;
            //ctor
            protected internal ObservationLocationData( XmlReader r)
            {
                r.ReadStartElement();
                while (r.MoveToContent() == XmlNodeType.Element)
                {
                    //capture the name of the field
                    string whichField = r.Name;
                    //read the open tag
                    r.ReadStartElement();
                    //ensure vlaue exists
                    if (r.HasValue)
                    {
                        if (string.Compare(whichField, "full", true) == 0)
                        {
                            _full = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "city", true) == 0)
                        {
                            _city = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "state", true) == 0)
                        {
                            _state = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "country", true) == 0)
                        {
                            _country = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "country_iso3166", true) == 0)
                        {
                            _countryIso3166 = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "latitude", true) == 0)
                        {
                            _latitude = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "longitude", true) == 0)
                        {
                            _longitude = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "elevation", true) == 0)
                        {
                            _elevation = r.ReadContentAsString();
                        }

                    }
                    //read the close tag
                    r.ReadEndElement();

                }
                //read the close tag
                r.ReadEndElement();

            }
            //exposed properties
            public string Full
            {
                get { return _full; }
            }
            public string City
            {
                get { return _city; }
            }
            public string State
            {
                get { return _state; }
            }
            public string Country
            {
                get { return _country; }
            }
            public string CountryIso3166
            {
                get { return _countryIso3166; }
            }
            public decimal Latitude
            {
                get { return _latitude; }
            }
            public decimal Longitude
            {
                get { return _longitude; }
            }
            public string Elevation
            {
                get { return _elevation; }
            }
        }
        //local fields
        private ImageData _image;
        private DisplayLocationData _display_location;
        private ObservationLocationData _observation_locaion;
        private object _estimated;
        private string _station_id;
        private string _observation_time;
        private System.DateTime? _observation_time_rfc822;
        private ulong? _observation_epoch;
        private System.DateTime? _local_time_rfc822;
        private ulong? _local_epoch;
        private string _local_tz_short;
        private string _local_tz_long;
        private string _local_tz_offset;
        private string _weather;
        private string _temperature_string;
        private decimal? _temp_f;
        private decimal? _temp_c;
        private string _relative_humidity;
        private string _wind_string;
        private string _wind_dir;
        private int? _wind_degrees;
        private decimal? _wind_mph;
        private decimal? _wind_gust_mph;
        private decimal? _wind_kph;
        private decimal? _wind_gust_kph;
        private int? _pressure_mb;
        private decimal? _pressure_in;
        private string _pressure_trend;
        private string _dewpoint_string;
        private int? _dewpoint_f;
        private int? _dewpoint_c;
        private string _heat_index_string;
        private decimal? _heat_index_f;
        private decimal? _heat_index_c;
        private string _windchill_string;
        private int? _windchill_f;
        private int? _windchill_c;
        private string _feelslike_string;
        private int? _feelslike_f;
        private int? _feelslike_c;
        private decimal? _visibility_mi;
        private decimal? _visibility_km;
        private string _solarradiation;
        private int? _uV;
        private string _precip_1hr_string;
        private decimal? _precip_1hr_in;
        private decimal? _precip_1hr_metric;
        private string _precip_today_string;
        private decimal? _precip_today_in;
        private decimal? _precip_today_metric;
        private string _icon;
        private string _icon_url;
        private string _forecast_url;
        private string _history_url;
        private string _ob_url;
        //ctor
        protected internal CurrentObservationData( XmlReader r)
        {
            if (!(r.Name == "current_observation"))
                throw new Exception();
            r.ReadStartElement();
            while (r.MoveToContent() == XmlNodeType.Element)
            {
                if (r.Name == "image")
                {
                    _image = new ImageData(r);
                }
                else if (r.Name == "display_location")
                {
                    _display_location = new DisplayLocationData(r);
                }
                else if (r.Name == "observation_location")
                {
                    _observation_locaion = new ObservationLocationData(r);

                }
                else
                {
                    //capture the name of the field
                    string whichField = r.Name;
                    //read the open tag
                    r.ReadStartElement();
                    //ensure vlaue exists
                    if (r.HasValue)
                    {
                        if (string.Compare(whichField, "estimated", true) == 0)
                        {
                            //o.caseId = r.ReadContentAsString()
                        }
                        else if (string.Compare(whichField, "station_id", true) == 0)
                        {
                            _station_id = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "observation_time", true) == 0)
                        {
                            _observation_time = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "observation_time_rfc822", true) == 0)
                        {
                            _observation_time_rfc822 = r.ReadContentAsDateTime();
                        }
                        else if (string.Compare(whichField, "observation_epoch", true) == 0)
                        {
                            _observation_epoch = Convert.ToUInt64(r.ReadContentAsString());
                        }
                        else if (string.Compare(whichField, "local_time_rfc822", true) == 0)
                        {
                            _local_time_rfc822 = r.ReadContentAsDateTime();
                        }
                        else if (string.Compare(whichField, "local_epoch", true) == 0)
                        {
                            _local_epoch = Convert.ToUInt64(r.ReadContentAsString());
                        }
                        else if (string.Compare(whichField, "local_tz_short", true) == 0)
                        {
                            _local_tz_short = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "local_tz_long", true) == 0)
                        {
                            _local_tz_long = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "local_tz_offset", true) == 0)
                        {
                            _local_tz_offset = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "weather", true) == 0)
                        {
                            _weather = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "temperature_string", true) == 0)
                        {
                            _temperature_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "temp_f", true) == 0)
                        {
                            _temp_f = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "temp_c", true) == 0)
                        {
                            _temp_c = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "relative_humidity", true) == 0)
                        {
                            _relative_humidity = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "wind_string", true) == 0)
                        {
                            _wind_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "wind_dir", true) == 0)
                        {
                            _wind_dir = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "wind_degrees", true) == 0)
                        {
                            _wind_degrees = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "wind_mph", true) == 0)
                        {
                            _wind_mph = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "wind_gust_mph", true) == 0)
                        {
                            _wind_gust_mph = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "wind_kph", true) == 0)
                        {
                            _wind_kph = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "wind_gust_kph", true) == 0)
                        {
                            _wind_gust_kph = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "pressure_mb", true) == 0)
                        {
                            _pressure_mb = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "pressure_in", true) == 0)
                        {
                            _pressure_in = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "pressure_trend", true) == 0)
                        {
                            _pressure_trend = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "dewpoint_string", true) == 0)
                        {
                            _dewpoint_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "dewpoint_f", true) == 0)
                        {
                            _dewpoint_f = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "dewpoint_c", true) == 0)
                        {
                            _dewpoint_c = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "heat_index_string", true) == 0)
                        {
                            _heat_index_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "heat_index_f", true) == 0)
                        {
                            if (r.Value == "NA")
                            {
                                r.Read();
                            }
                            else
                            {
                                _heat_index_f = r.ReadContentAsDecimal();
                            }
                        }
                        else if (string.Compare(whichField, "heat_index_c", true) == 0)
                        {
                            if (r.Value == "NA")
                            {
                                r.Read();
                            }
                            else
                            {
                                _heat_index_c = r.ReadContentAsDecimal();
                            }
                        }
                        else if (string.Compare(whichField, "windchill_string", true) == 0)
                        {
                            _windchill_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "windchill_f", true) == 0)
                        {
                            _windchill_f = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "windchill_c", true) == 0)
                        {
                            _windchill_c = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "feelslike_string", true) == 0)
                        {
                            _feelslike_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "feelslike_f", true) == 0)
                        {
                            _feelslike_f = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "feelslike_c", true) == 0)
                        {
                            _feelslike_c = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "visibility_mi", true) == 0)
                        {
                            _visibility_mi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "visibility_km", true) == 0)
                        {
                            _visibility_km = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "solarradiation ", true) == 0)
                        {
                            _solarradiation = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "UV", true) == 0)
                        {
                            _uV = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "precip_1hr_string", true) == 0)
                        {
                            _precip_1hr_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "precip_1hr_in", true) == 0)
                        {
                            _precip_1hr_in = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precip_1hr_metric", true) == 0)
                        {
                            _precip_1hr_metric = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precip_today_string", true) == 0)
                        {
                            _precip_today_string = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "precip_today_in", true) == 0)
                        {
                            _precip_today_in = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precip_today_metric", true) == 0)
                        {
                            _precip_today_metric = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "icon", true) == 0)
                        {
                            _icon = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "icon_url", true) == 0)
                        {
                            _icon_url = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "forecast_url", true) == 0)
                        {
                            _forecast_url = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "history_url", true) == 0)
                        {
                            _history_url = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "ob_url", true) == 0)
                        {
                            _ob_url = r.ReadContentAsString();

                        }
                        else
                        {
                            throw new Exception(string.Format("Found unexpected element '{0}' as child of {1}", whichField, this.GetType().ToString()));
                        }
                    }
                    //read the close tag
                    r.ReadEndElement();
                }

            }
            r.ReadEndElement();
        }
        //shared functionality
        public static object FeatureRequestString()
        {
            return DataFeatureChoices.Conditions.ToString().ToLower();
        }
        //exposed properties
        public ImageData Image
        {
            get { return _image; }
        }
        public DisplayLocationData Display_location
        {
            get { return _display_location; }
        }
        public ObservationLocationData Observation_locaion
        {
            get { return _observation_locaion; }
        }
        public object estimated
        {
            get { return _estimated; }
        }
        public string station_id
        {
            get { return _station_id; }
        }
        public string observation_time
        {
            get { return _observation_time; }
        }
        public System.DateTime? observation_time_rfc822
        {
            get { return _observation_time_rfc822; }
        }
        public ulong? observation_epoch
        {
            get { return _observation_epoch; }
        }
        public System.DateTime? local_time_rfc822
        {
            get { return _local_time_rfc822; }
        }
        public ulong? local_epoch
        {
            get { return _local_epoch; }
        }
        public string local_tz_short
        {
            get { return _local_tz_short; }
        }
        public string local_tz_long
        {
            get { return _local_tz_long; }
        }
        public string local_tz_offset
        {
            get { return _local_tz_offset; }
        }
        public string weather
        {
            get { return _weather; }
        }
        public string temperature_string
        {
            get { return _temperature_string; }
        }
        public decimal? temp_f
        {
            get { return _temp_f; }
        }
        public decimal? temp_c
        {
            get { return _temp_c; }
        }
        public string relative_humidity
        {
            get { return _relative_humidity; }
        }
        public string wind_string
        {
            get { return _wind_string; }
        }
        public string wind_dir
        {
            get { return _wind_dir; }
        }
        public int? wind_degrees
        {
            get { return _wind_degrees; }
        }
        public decimal? wind_mph
        {
            get { return _wind_mph; }
        }
        public decimal? wind_gust_mph
        {
            get { return _wind_gust_mph; }
        }
        public decimal? wind_kph
        {
            get { return _wind_kph; }
        }
        public decimal? wind_gust_kph
        {
            get { return _wind_gust_kph; }
        }
        public int? pressure_mb
        {
            get { return _pressure_mb; }
        }
        public decimal? pressure_in
        {
            get { return _pressure_in; }
        }
        public string pressure_trend
        {
            get { return _pressure_trend; }
        }
        public string dewpoint_string
        {
            get { return _dewpoint_string; }
        }
        public int? dewpoint_f
        {
            get { return _dewpoint_f; }
        }
        public int? dewpoint_c
        {
            get { return _dewpoint_c; }
        }
        public string heat_index_string
        {
            get { return _heat_index_string; }
        }
        public decimal? heat_index_f
        {
            get { return _heat_index_f; }
        }
        public decimal? heat_index_c
        {
            get { return _heat_index_c; }
        }
        public string windchill_string
        {
            get { return _windchill_string; }
        }
        public int? windchill_f
        {
            get { return _windchill_f; }
        }
        public int? windchill_c
        {
            get { return _windchill_c; }
        }
        public string feelslike_string
        {
            get { return _feelslike_string; }
        }
        public int? feelslike_f
        {
            get { return _feelslike_f; }
        }
        public int? feelslike_c
        {
            get { return _feelslike_c; }
        }
        public decimal? visibility_mi
        {
            get { return _visibility_mi; }
        }
        public decimal? visibility_km
        {
            get { return _visibility_km; }
        }
        public string solarradiation
        {
            get { return _solarradiation; }
        }
        public int? UV
        {
            get { return _uV; }
        }
        public string precip_1hr_string
        {
            get { return _precip_1hr_string; }
        }
        public decimal? precip_1hr_in
        {
            get { return _precip_1hr_in; }
        }
        public decimal? precip_1hr_metric
        {
            get { return _precip_1hr_metric; }
        }
        public string precip_today_string
        {
            get { return _precip_today_string; }
        }
        public decimal? precip_today_in
        {
            get { return _precip_today_in; }
        }
        public decimal? precip_today_metric
        {
            get { return _precip_today_metric; }
        }
        public string icon
        {
            get { return _icon; }
        }
        public string icon_url
        {
            get { return _icon_url; }
        }
        public string forecast_url
        {
            get { return _forecast_url; }
        }
        public string history_url
        {
            get { return _history_url; }
        }
        public string ob_url
        {
            get { return _ob_url; }
        }

    }
}
