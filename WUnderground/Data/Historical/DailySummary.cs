using System;
using System.Xml;

namespace WUnderground.Data.Historical
{
    public class DailySummary
    {
        private DateData _date;
        private bool? _fog;
        private bool? _rain;
        private bool? _snow;
        private bool? _hail;
        private bool? _thunder;
        private bool? _tornado;
        private decimal? _snowfallm;
        private decimal? _snowfalli;
        private decimal? _monthtodatesnowfallm;
        private decimal? _monthtodatesnowfalli;
        private decimal? _since1julsnowfallm;
        private decimal? _since1julsnowfalli;
        private decimal? _snowdepthm;
        private decimal? _snowdepthi;
        private decimal? _meantempm;
        private decimal? _meantempi;
        private decimal? _meandewptm;
        private decimal? _meandewpti;
        private decimal? _meanpressurem;
        private decimal? _meanpressurei;
        private decimal? _meanwindspdm;
        private decimal? _meanwindspdi;
        private decimal? _meanwdire;
        private decimal? _meanwdird;
        private decimal? _meanvism;
        private decimal? _meanvisi;
        private decimal? _humidity;
        private decimal? _maxtempm;
        private decimal? _maxtempi;
        private decimal? _mintempm;
        private decimal? _mintempi;
        private decimal? _maxhumidity;
        private decimal? _minhumidity;
        private decimal? _maxdewptm;
        private decimal? _maxdewpti;
        private decimal? _mindewptm;
        private decimal? _mindewpti;
        private decimal? _maxpressurem;
        private decimal? _maxpressurei;
        private decimal? _minpressurem;
        private decimal? _minpressurei;
        private decimal? _maxwspdm;
        private decimal? _maxwspdi;
        private decimal? _minwspdm;
        private decimal? _minwspdi;
        private decimal? _maxvism;
        private decimal? _maxvisi;
        private decimal? _minvism;
        private decimal? _minvisi;
        private int? _gdegreedays;
        private int? _heatingdegreedays;
        private int? _coolingdegreedays;
        private decimal? _precipm;
        private decimal? _precipi;
        private string _precipsource;
        private int? _heatingdegreedaysnormal;
        private int? _monthtodateheatingdegreedays;
        private int? _monthtodateheatingdegreedaysnormal;
        private int? _since1sepheatingdegreedays;
        private int? _since1sepheatingdegreedaysnormal;
        private int? _since1julheatingdegreedays;
        private int? _since1julheatingdegreedaysnormal;
        private int? _coolingdegreedaysnormal;
        private int? _monthtodatecoolingdegreedays;
        private int? _monthtodatecoolingdegreedaysnormal;
        private int? _since1sepcoolingdegreedays;
        private int? _since1sepcoolingdegreedaysnormal;
        private int? _since1jancoolingdegreedays;

        private int? _since1jancoolingdegreedaysnormal;





        protected internal DailySummary(XmlReader r)
        {
            if (!(r.Name == "summary"))
                throw new Exception();
            r.ReadStartElement();
            while (r.MoveToContent() == XmlNodeType.Element)
            {
                if (r.Name.ToLower() == "date")
                {
                    _date = new DateData(r);

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
                        if (string.Compare(whichField, "snowfallm", true) == 0)
                        {
                            _snowfallm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "snowfalli", true) == 0)
                        {
                            _snowfalli = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "monthtodatesnowfallm", true) == 0)
                        {
                            _monthtodatesnowfallm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "monthtodatesnowfalli", true) == 0)
                        {
                            _monthtodatesnowfalli = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "fog", true) == 0)
                        {
                            _fog = r.ReadContentAsBoolean();
                        }
                        else if (string.Compare(whichField, "rain", true) == 0)
                        {
                            _rain = r.ReadContentAsBoolean();
                        }
                        else if (string.Compare(whichField, "snow", true) == 0)
                        {
                            _snow = r.ReadContentAsBoolean();
                        }
                        else if (string.Compare(whichField, "thunder", true) == 0)
                        {
                            _thunder = r.ReadContentAsBoolean();
                        }
                        else if (string.Compare(whichField, "tornado", true) == 0)
                        {
                            _tornado = r.ReadContentAsBoolean();
                        }
                        else if (string.Compare(whichField, "hail", true) == 0)
                        {
                            _hail = r.ReadContentAsBoolean();
                        }
                        else if (string.Compare(whichField, "since1julsnowfallm", true) == 0)
                        {
                            _since1julsnowfallm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "since1julsnowfalli", true) == 0)
                        {
                            _since1julsnowfalli = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "snowdepthm", true) == 0)
                        {
                            _snowdepthm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "snowdepthi", true) == 0)
                        {
                            _snowdepthi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meantempm", true) == 0)
                        {
                            _meantempm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meantempi", true) == 0)
                        {
                            _meantempi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meandewptm", true) == 0)
                        {
                            _meandewptm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meandewpti", true) == 0)
                        {
                            _meandewpti = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanpressurem", true) == 0)
                        {
                            _meanpressurem = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanpressurei", true) == 0)
                        {
                            _meanpressurei = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanwindspdm", true) == 0)
                        {
                            _meanwindspdm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanwindspdi", true) == 0)
                        {
                            _meanwindspdi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanwdire", true) == 0)
                        {
                            _meanwdire = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanwdird", true) == 0)
                        {
                            _meanwdird = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanvism", true) == 0)
                        {
                            _meanvism = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "meanvisi", true) == 0)
                        {
                            _meanvisi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "humidity", true) == 0)
                        {
                            _humidity = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxtempm", true) == 0)
                        {
                            _maxtempm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxtempi", true) == 0)
                        {
                            _maxtempi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "mintempm", true) == 0)
                        {
                            _mintempm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "mintempi", true) == 0)
                        {
                            _mintempi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxhumidity", true) == 0)
                        {
                            _maxhumidity = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "minhumidity", true) == 0)
                        {
                            _minhumidity = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxdewptm", true) == 0)
                        {
                            _maxdewptm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxdewpti", true) == 0)
                        {
                            _maxdewpti = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "mindewptm", true) == 0)
                        {
                            _mindewptm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "mindewpti", true) == 0)
                        {
                            _mindewpti = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxpressurem", true) == 0)
                        {
                            _maxpressurem = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxpressurei", true) == 0)
                        {
                            _maxpressurei = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "minpressurem", true) == 0)
                        {
                            _minpressurem = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "minpressurei", true) == 0)
                        {
                            _minpressurei = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxwspdm", true) == 0)
                        {
                            _maxwspdm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxwspdi", true) == 0)
                        {
                            _maxwspdi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "minwspdm", true) == 0)
                        {
                            _minwspdm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "minwspdi", true) == 0)
                        {
                            _minwspdi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxvism", true) == 0)
                        {
                            _maxvism = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "maxvisi", true) == 0)
                        {
                            _maxvisi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "minvism", true) == 0)
                        {
                            _minvism = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "minvisi", true) == 0)
                        {
                            _minvisi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precipm", true) == 0)
                        {
                            _precipm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precipi", true) == 0)
                        {
                            _precipi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "gdegreedays", true) == 0)
                        {
                            _gdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "heatingdegreedays", true) == 0)
                        {
                            _heatingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "coolingdegreedays", true) == 0)
                        {
                            _coolingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "precipsource", true) == 0)
                        {
                            _precipsource = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "heatingdegreedaysnormal", true) == 0)
                        {
                            _heatingdegreedaysnormal = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "monthtodateheatingdegreedays", true) == 0)
                        {
                            _monthtodateheatingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "monthtodateheatingdegreedaysnormal", true) == 0)
                        {
                            _monthtodateheatingdegreedaysnormal = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1sepheatingdegreedays", true) == 0)
                        {
                            _since1sepheatingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1sepheatingdegreedaysnormal", true) == 0)
                        {
                            _since1sepheatingdegreedaysnormal = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1julheatingdegreedays", true) == 0)
                        {
                            _since1julheatingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1julheatingdegreedaysnormal", true) == 0)
                        {
                            _since1julheatingdegreedaysnormal = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "coolingdegreedaysnormal", true) == 0)
                        {
                            _coolingdegreedaysnormal = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "monthtodatecoolingdegreedays", true) == 0)
                        {
                            _monthtodatecoolingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "monthtodatecoolingdegreedaysnormal", true) == 0)
                        {
                            _monthtodatecoolingdegreedaysnormal = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1sepcoolingdegreedays", true) == 0)
                        {
                            _since1sepcoolingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1sepcoolingdegreedaysnormal", true) == 0)
                        {
                            _since1sepcoolingdegreedaysnormal = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1jancoolingdegreedays", true) == 0)
                        {
                            _since1jancoolingdegreedays = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "since1jancoolingdegreedaysnormal", true) == 0)
                        {
                            _since1jancoolingdegreedaysnormal = r.ReadContentAsInt();
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

        public DateData Date
        {
            get { return _date; }
        }

        public bool? Fog
        {
            get { return _fog; }
        }

        public bool? Rain
        {
            get { return _rain; }
        }

        public bool? Snow
        {
            get { return _snow; }
        }

        public bool? Hail
        {
            get { return _hail; }
        }

        public bool? Thunder
        {
            get { return _thunder; }
        }

        public bool? Tornado
        {
            get { return _tornado; }
        }

        public decimal? Snowfallm
        {
            get { return _snowfallm; }
        }

        public decimal? Snowfalli
        {
            get { return _snowfalli; }
        }

        public decimal? Monthtodatesnowfallm
        {
            get { return _monthtodatesnowfallm; }
        }

        public decimal? Monthtodatesnowfalli
        {
            get { return _monthtodatesnowfalli; }
        }

        public decimal? Since1julsnowfallm
        {
            get { return _since1julsnowfallm; }
        }

        public decimal? Since1julsnowfalli
        {
            get { return _since1julsnowfalli; }
        }

        public decimal? Snowdepthm
        {
            get { return _snowdepthm; }
        }

        public decimal? Snowdepthi
        {
            get { return _snowdepthi; }
        }

        public decimal? Meantempm
        {
            get { return _meantempm; }
        }

        public decimal? Meantempi
        {
            get { return _meantempi; }
        }

        public decimal? Meandewptm
        {
            get { return _meandewptm; }
        }

        public decimal? Meandewpti
        {
            get { return _meandewpti; }
        }

        public decimal? Meanpressurem
        {
            get { return _meanpressurem; }
        }

        public decimal? Meanpressurei
        {
            get { return _meanpressurei; }
        }

        public decimal? Meanwindspdm
        {
            get { return _meanwindspdm; }
        }

        public decimal? Meanwindspdi
        {
            get { return _meanwindspdi; }
        }

        public decimal? Meanwdire
        {
            get { return _meanwdire; }
        }

        public decimal? Meanwdird
        {
            get { return _meanwdird; }
        }

        public decimal? Meanvism
        {
            get { return _meanvism; }
        }

        public decimal? Meanvisi
        {
            get { return _meanvisi; }
        }

        public decimal? Humidity
        {
            get { return _humidity; }
        }

        public decimal? Maxtempm
        {
            get { return _maxtempm; }
        }

        public decimal? Maxtempi
        {
            get { return _maxtempi; }
        }

        public decimal? Mintempm
        {
            get { return _mintempm; }
        }

        public decimal? Mintempi
        {
            get { return _mintempi; }
        }

        public decimal? Maxhumidity
        {
            get { return _maxhumidity; }
        }

        public decimal? Minhumidity
        {
            get { return _minhumidity; }
        }

        public decimal? Maxdewptm
        {
            get { return _maxdewptm; }
        }

        public decimal? Maxdewpti
        {
            get { return _maxdewpti; }
        }

        public decimal? Mindewptm
        {
            get { return _mindewptm; }
        }

        public decimal? Mindewpti
        {
            get { return _mindewpti; }
        }

        public decimal? Maxpressurem
        {
            get { return _maxpressurem; }
        }

        public decimal? Maxpressurei
        {
            get { return _maxpressurei; }
        }

        public decimal? Minpressurem
        {
            get { return _minpressurem; }
        }

        public decimal? Minpressurei
        {
            get { return _minpressurei; }
        }

        public decimal? Maxwspdm
        {
            get { return _maxwspdm; }
        }

        public decimal? Maxwspdi
        {
            get { return _maxwspdi; }
        }

        public decimal? Minwspdm
        {
            get { return _minwspdm; }
        }

        public decimal? Minwspdi
        {
            get { return _minwspdi; }
        }

        public decimal? Maxvism
        {
            get { return _maxvism; }
        }

        public decimal? Maxvisi
        {
            get { return _maxvisi; }
        }

        public decimal? Minvism
        {
            get { return _minvism; }
        }

        public decimal? Minvisi
        {
            get { return _minvisi; }
        }

        public int? Gdegreedays
        {
            get { return _gdegreedays; }
        }

        public int? Heatingdegreedays
        {
            get { return _heatingdegreedays; }
        }

        public int? Coolingdegreedays
        {
            get { return _coolingdegreedays; }
        }

        public decimal? Precipm
        {
            get { return _precipm; }
        }

        public decimal? Precipi
        {
            get { return _precipi; }
        }

        public string Precipsource
        {
            get { return _precipsource; }
        }

        public int? Heatingdegreedaysnormal
        {
            get { return _heatingdegreedaysnormal; }
        }

        public int? Monthtodateheatingdegreedays
        {
            get { return _monthtodateheatingdegreedays; }
        }

        public int? Monthtodateheatingdegreedaysnormal
        {
            get { return _monthtodateheatingdegreedaysnormal; }
        }

        public int? Since1sepheatingdegreedays
        {
            get { return _since1sepheatingdegreedays; }
        }

        public int? Since1sepheatingdegreedaysnormal
        {
            get { return _since1sepheatingdegreedaysnormal; }
        }

        public int? Since1julheatingdegreedays
        {
            get { return _since1julheatingdegreedays; }
        }

        public int? Since1julheatingdegreedaysnormal
        {
            get { return _since1julheatingdegreedaysnormal; }
        }

        public int? Coolingdegreedaysnormal
        {
            get { return _coolingdegreedaysnormal; }
        }

        public int? Monthtodatecoolingdegreedays
        {
            get { return _monthtodatecoolingdegreedays; }
        }

        public int? Monthtodatecoolingdegreedaysnormal
        {
            get { return _monthtodatecoolingdegreedaysnormal; }
        }

        public int? Since1sepcoolingdegreedays
        {
            get { return _since1sepcoolingdegreedays; }
        }

        public int? Since1sepcoolingdegreedaysnormal
        {
            get { return _since1sepcoolingdegreedaysnormal; }
        }

        public int? Since1jancoolingdegreedays
        {
            get { return _since1jancoolingdegreedays; }
        }

        public int? Since1jancoolingdegreedaysnormal
        {
            get { return _since1jancoolingdegreedaysnormal; }
        }

    }
}
