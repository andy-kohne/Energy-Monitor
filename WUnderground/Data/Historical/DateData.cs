using System;
using System.Diagnostics;
using System.Xml;

namespace WUnderground.Data.Historical
{
    [DebuggerDisplay("{Pretty}")]
    public class DateData
    {
        //local fields
        private string _pretty;
        private string _tzName;
        private int _year;
        private int _month;
        private int _day;
        private int _hour;
        private int _minute;
        //exposed properties
        public string Pretty
        {
            get { return _pretty; }
        }
        public string TzName
        {
            get { return _tzName; }
        }
        public int Year
        {
            get { return _year; }
        }
        public int Month
        {
            get { return _month; }
        }
        public int Day
        {
            get { return _day; }
        }
        public int Hour
        {
            get { return _hour; }
        }
        public int Minute
        {
            get { return _minute; }
        }
        public System.DateTime Standardized
        {
            get { return new System.DateTime(_year, _month, _day, _hour, _minute, 0); }
        }
        //ctor
        protected internal DateData(XmlReader r)
        {
            if (!(r.Name.ToLower() == "date" | r.Name.ToLower() == "utcdate"))
                throw new Exception();
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
                    if (string.Compare(whichField, "pretty", true) == 0)
                    {
                        _pretty = r.ReadContentAsString();
                    }
                    else if (string.Compare(whichField, "year", true) == 0)
                    {
                        _year = r.ReadContentAsInt();
                    }
                    else if (string.Compare(whichField, "mon", true) == 0)
                    {
                        _month = r.ReadContentAsInt();
                    }
                    else if (string.Compare(whichField, "mday", true) == 0)
                    {
                        _day = r.ReadContentAsInt();
                    }
                    else if (string.Compare(whichField, "hour", true) == 0)
                    {
                        _hour = r.ReadContentAsInt();
                    }
                    else if (string.Compare(whichField, "min", true) == 0)
                    {
                        _minute = r.ReadContentAsInt();
                    }
                    else if (string.Compare(whichField, "tzname", true) == 0)
                    {
                        _tzName = r.ReadContentAsString();
                    }
                    else
                    {
                        throw new Exception(string.Format("Found unexpected element '{0}' as child of {1}", whichField, this.GetType().ToString()));
                    }
                }
                //read the close tag
                r.ReadEndElement();
            }
            r.ReadEndElement();
        }
    }
}