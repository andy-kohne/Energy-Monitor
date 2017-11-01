using System;
using System.Diagnostics;
using System.Xml;

namespace WUnderground.Data.Historical
{
    [DebuggerDisplay("{_date.Pretty}")]
    public class ObservationData
    {
        //local fields
        private DateData _date;
        private DateData _utcdate;
        private decimal? _tempm;
        private decimal? _tempi;
        private decimal? _dewptm;
        private decimal? _dewpti;
        private int? _hum;
        private decimal? _wspdm;
        private decimal? _wspdi;
        private decimal? _wgustm;
        private decimal? _wgusti;
        private int? _wdird;
        private string _wdire;
        private decimal? _pressurem;
        private decimal? _pressurei;
        private decimal? _windchillm;
        private decimal? _windchilli;
        private decimal? _heatindexm;
        private decimal? _heatindexi;
        private decimal? _precipratem;
        private decimal? _precipratei;
        private decimal? _preciptotalm;
        private decimal? _preciptotali;
        private string _solarradiation;
        private string _uv;
        private string _softwaretype;
        private decimal? _vism;
        private decimal? _visi;
        private string _conds;
        private string _icon;
        private bool? _fog;
        private bool? _rain;
        private bool? _snow;
        private bool? _hail;
        private bool? _thunder;
        private bool? _tornado;

        private string _metar;

        //ctor
        public ObservationData(XmlReader r)
        {
            if (!(r.Name == "observation"))
                throw new Exception();
            r.ReadStartElement();
            while (r.MoveToContent() == XmlNodeType.Element)
            {
                if (r.Name.ToLower() == "date")
                {
                    _date = new DateData(r);
                }
                else if (r.Name.ToLower() == "utcdate")
                {
                    _utcdate = new DateData(r);
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
                        if (string.Compare(whichField, "tempm", true) == 0)
                        {
                            _tempm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "tempi", true) == 0)
                        {
                            _tempi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "dewptm", true) == 0)
                        {
                            _dewptm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "dewpti", true) == 0)
                        {
                            _dewpti = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "hum", true) == 0)
                        {
                            _hum = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "wspdm", true) == 0)
                        {
                            _wspdm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "wspdi", true) == 0)
                        {
                            _wspdi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "wgustm", true) == 0)
                        {
                            _wgustm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "wgusti", true) == 0)
                        {
                            _wgusti = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "wdird", true) == 0)
                        {
                            _wdird = r.ReadContentAsInt();
                        }
                        else if (string.Compare(whichField, "wdire", true) == 0)
                        {
                            _wdire = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "pressurem", true) == 0)
                        {
                            _pressurem = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "pressurei", true) == 0)
                        {
                            _pressurei = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "windchillm", true) == 0)
                        {
                            _windchillm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "windchilli", true) == 0)
                        {
                            _windchilli = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "heatindexm", true) == 0)
                        {
                            _heatindexm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "heatindexi", true) == 0)
                        {
                            _heatindexi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precip_ratem", true) == 0 | string.Compare(whichField, "precipm", true) == 0)
                        {
                            _precipratem = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precip_ratei", true) == 0 | string.Compare(whichField, "precipi", true) == 0)
                        {
                            _precipratei = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precip_totalm", true) == 0)
                        {
                            _preciptotalm = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "precip_totali", true) == 0)
                        {
                            _preciptotali = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "vism", true) == 0)
                        {
                            _vism = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "visi", true) == 0)
                        {
                            _visi = r.ReadContentAsDecimal();
                        }
                        else if (string.Compare(whichField, "solarradiation", true) == 0)
                        {
                            _solarradiation = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "uv", true) == 0)
                        {
                            _uv = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "softwaretype", true) == 0)
                        {
                            _softwaretype = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "conds", true) == 0)
                        {
                            _conds = r.ReadContentAsString();
                        }
                        else if (string.Compare(whichField, "icon", true) == 0)
                        {
                            _icon = r.ReadContentAsString();
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
                        else if (string.Compare(whichField, "metar", true) == 0)
                        {
                            _metar = r.ReadContentAsString();

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
        //exposed properties
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
        public string Metar
        {
            get { return _metar; }
        }
        public DateData Date
        {
            get { return _date; }
        }
        public DateData Utcdate
        {
            get { return _utcdate; }
        }
        public decimal? Tempm
        {
            get { return _tempm; }
        }
        public decimal? Tempi
        {
            get { return _tempi; }
        }
        public decimal? Dewptm
        {
            get { return _dewptm; }
        }
        public decimal? Dewpti
        {
            get { return _dewpti; }
        }
        public int? Hum
        {
            get { return _hum; }
        }
        public decimal? Wspdm
        {
            get { return _wspdm; }
        }
        public decimal? Wspdi
        {
            get { return _wspdi; }
        }
        public decimal? Wgustm
        {
            get { return _wgustm; }
        }
        public decimal? Wgusti
        {
            get { return _wgusti; }
        }
        public int? Wdird
        {
            get { return _wdird; }
        }
        public string Wdire
        {
            get { return _wdire; }
        }
        public decimal? Pressurem
        {
            get { return _pressurem; }
        }
        public decimal? Pressurei
        {
            get { return _pressurei; }
        }
        public decimal? Windchillm
        {
            get { return _windchillm; }
        }
        public decimal? Windchilli
        {
            get { return _windchilli; }
        }
        public decimal? Heatindexm
        {
            get { return _heatindexm; }
        }
        public decimal? Heatindexi
        {
            get { return _heatindexi; }
        }
        public decimal? Precipratem
        {
            get { return _precipratem; }
        }
        public decimal? Precipratei
        {
            get { return _precipratei; }
        }
        public decimal? Preciptotalm
        {
            get { return _preciptotalm; }
        }
        public decimal? Preciptotali
        {
            get { return _preciptotali; }
        }
        public string Solarradiation
        {
            get { return _solarradiation; }
        }
        public string Uv
        {
            get { return _uv; }
        }
        public string Softwaretype
        {
            get { return _softwaretype; }
        }
        public decimal? Vism
        {
            get { return _vism; }
        }
        public decimal? Visi
        {
            get { return _visi; }
        }
        public string Conds
        {
            get { return _conds; }
        }
        public string Icon
        {
            get { return _icon; }
        }
    }
}
