using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using WUnderground.Data.Historical;

namespace WUnderground.Data
{
    [DebuggerDisplay("{_date.Pretty}")]
    public class History
    {
        //local fields
        private DateData _date;
        private DateData _utcdate;
        private List<ObservationData> _observations;
        private List<DailySummary> _dailysummary;
        //shared functionality
        public static string FeatureRequestString(System.DateTime WhichDate)
        {
            return string.Format("{0}_{1:yyyyMMdd}", DataFeatureChoices.History.ToString().ToLower(), WhichDate);
        }
        //ctor
        protected internal History( XmlReader r)
        {
            if (!(r.Name == "history"))
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
                else if (r.Name == "observations")
                {
                    _observations = new List<ObservationData>();
                    r.ReadStartElement();
                    while (r.MoveToContent() == XmlNodeType.Element)
                    {
                        if (r.Name == "observation")
                        {
                            _observations.Add(new ObservationData(r));
                        }
                        else
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                    }
                    r.ReadEndElement();
                    // r.Skip()

                }
                else if (r.Name == "dailysummary")
                {
                    _dailysummary = new List<DailySummary>();
                    r.ReadStartElement();
                    while (r.MoveToContent() == XmlNodeType.Element)
                    {
                        if (r.Name == "summary")
                        {
                            _dailysummary.Add(new DailySummary(r));
                        }
                        else
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                    }
                    r.ReadEndElement();
                    //   r.Skip()
                }
                else
                {
                    //read the close tag
                    r.ReadEndElement();

                }
            }
            r.ReadEndElement();
        }
        //exposed properties
        public DateData Date
        {
            get { return _date; }
        }
        public DateData Utcdate
        {
            get { return _utcdate; }
        }
        public List<ObservationData> Observations
        {
            get { return _observations; }
        }
    }
}
