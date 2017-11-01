using System.Collections.Generic;
using System.Net;
using System.Xml;
using WUnderground.Data;

namespace WUnderground
{
    public class DataSet
    {
        public List<object> DataFeaturesContent { get; } = new List<object>();

        public string Version { get; private set; }

        public string TermsOfService { get; private set; }

        public List<string> DataFeatures { get; private set; } = new List<string>();

        public static DataSet Query(string apiKey, string[] features, string location)
        {
            //first do the fetch
            var featurestring = string.Join("/", features);
            var uri = $"https://api.wunderground.com/api/{apiKey}/{featurestring}/q/{location}.xml";
            var oRequest = (HttpWebRequest)WebRequest.Create(uri);
            using (var oResponse = (HttpWebResponse)oRequest.GetResponse())
            {
                //get an xml reader
                using (var oReader = XmlReader.Create(oResponse.GetResponseStream()))
                {
                    //create a return object
                    var ret = new DataSet();
                    //parse the xml
                    oReader.MoveToContent();
                    if (oReader.Name == "response")
                    {
                        oReader.ReadStartElement();
                        //iterate
                        while (oReader.MoveToContent() == XmlNodeType.Element)
                        {
                            switch (oReader.Name.ToLower())
                            {
                                case "version":
                                    oReader.ReadStartElement();
                                    ret.Version = oReader.ReadContentAsString();
                                    oReader.ReadEndElement();
                                    break;
                                case "termsofservice":
                                    oReader.ReadStartElement();
                                    ret.TermsOfService = oReader.ReadContentAsString();
                                    oReader.ReadEndElement();
                                    break;
                                case "features":
                                    ret.DataFeatures = new List<string>();
                                    oReader.ReadStartElement();
                                    while (oReader.MoveToContent() == XmlNodeType.Element)
                                    {
                                        oReader.ReadStartElement();
                                        ret.DataFeatures.Add(oReader.ReadContentAsString());
                                        oReader.ReadEndElement();
                                    }
                                    oReader.ReadEndElement();
                                    break;
                                case "current_observation":
                                    ret.DataFeaturesContent.Add(new CurrentObservationData(oReader));
                                    break;
                                case "history":
                                    ret.DataFeaturesContent.Add(new History(oReader));

                                    break;
                                default:
                                    oReader.Skip();
                                    break;
                            }
                            if (oReader.NodeType == XmlNodeType.Whitespace)
                            {
                                oReader.Read();
                            }
                        }
                    }
                    oReader.ReadEndElement();
                    return ret;
                }
            }
        }
    }
}
