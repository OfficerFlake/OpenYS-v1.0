using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace OpenYS
{
    public static class OpenYS_Link
    {
        public class OYS_Link_Response
        {
            public string Reason = "";
            public string Response = null;
            public bool Success = false;

            public OYS_Link_Response(string _Reason, string _Response, bool _Success)
            {
                Reason = _Reason;
                Response = _Response; //yeah I could use "object" here but data is passed back from the server as a string anyway...
                Success = _Success;
            }
        }

        public static OYS_Link_Response Get(int YSFHQ_ID, string TargetUrl)
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["id"] = YSFHQ_ID.ToString();

                try
                {
                    //Key Verification:
                    //https://openys.ysfhq.com/api/keycheck.php?apikey=
                    //^^ NOT YET IMPLEMENTED!

                    string APIKey = Environment.GetAPIKey();

                    if (APIKey == "MISSINGNO." | APIKey == "")
                    {
                        return new OYS_Link_Response("NO APIKEY PROVIDED!", "", false);
                    }

                    var response = client.UploadValues("https://openys.ysfhq.com/api/get/" + TargetUrl + ".php?apikey=" + APIKey, values);
                    string responseString = Encoding.Default.GetString(response);

                    return new OYS_Link_Response("SUCCESS 200!", responseString, true);
                }
                catch (System.Net.WebException WebError)
                {
                    Debug.TestPoint();
                    HttpStatusCode ErrorCode = ((HttpWebResponse)WebError.Response).StatusCode;
                    
                    if (ErrorCode == HttpStatusCode.NotImplemented) //501
                    {
                        //Missing Username / Password
                        return new OYS_Link_Response("DATABASE CONNECTION ERROR 501!", "", false);
                    }
                    if (ErrorCode == HttpStatusCode.NotFound) //404
                    {
                        //User Not Found
                        return new OYS_Link_Response("USER NOT FOUND 404!", "", false);
                    }
                    if (ErrorCode == HttpStatusCode.Unauthorized) //401
                    {
                        //Bad API Key
                        return new OYS_Link_Response("REJECTED API KEY 401!", "", false);
                    }
                    //Generic Error?
                    return new OYS_Link_Response("GENERIC ERROR 500!", "", false);
                    //System.Console.WriteLine(WebError);
                }
            }
        }

        public static OYS_Link_Response Set(int YSFHQ_ID, string TargetUrl, object Value)
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["id"] = YSFHQ_ID.ToString();
                values["value"] = Value.ToString();

                try
                {
                    //Key Verification:
                    //https://openys.ysfhq.com/api/keycheck.php?apikey=
                    //^^ NOT YET IMPLEMENTED!

                    string APIKey = Environment.GetAPIKey();

                    if (APIKey == "MISSINGNO." | APIKey == "")
                    {
                        return new OYS_Link_Response("NO APIKEY PROVIDED!", "", false);
                    }

                    var response = client.UploadValues("https://openys.ysfhq.com/api/set/" + TargetUrl + ".php?apikey=" + APIKey, values);
                    string responseString = Encoding.Default.GetString(response);

                    return new OYS_Link_Response("SUCCESS 200!", responseString, true);
                }
                catch (System.Net.WebException WebError)
                {
                    Debug.TestPoint();
                    HttpStatusCode ErrorCode = ((HttpWebResponse)WebError.Response).StatusCode;

                    if (ErrorCode == HttpStatusCode.NotImplemented) //501
                    {
                        //Missing Username / Password
                        return new OYS_Link_Response("DATABASE CONNECTION ERROR 501!", "", false);
                    }
                    if (ErrorCode == HttpStatusCode.NotFound) //404
                    {
                        //User Not Found
                        return new OYS_Link_Response("USER NOT FOUND 404!", "", false);
                    }
                    if (ErrorCode == HttpStatusCode.Unauthorized) //401
                    {
                        //Bad API Key
                        return new OYS_Link_Response("REJECTED API KEY 401!", "", false);
                    }
                    //Generic Error?
                    return new OYS_Link_Response("GENERIC ERROR 500!", "", false);
                    //System.Console.WriteLine(WebError);
                }
            }
        }

        public static class Stats
        {
            public static int _id = -1;
            public static double total_flight_seconds = Double.NegativeInfinity;
        }

        public static void GetAllStats()
        {
            #region total_flight_seconds
            OYS_Link_Response total_flight_seconds = OpenYS_Link.Get(Stats._id, "stats_total_flight_seconds");
            if (total_flight_seconds.Success)
            {
                bool Failed = !Double.TryParse(total_flight_seconds.Response, out Stats.total_flight_seconds);
            }
            #endregion
        }
    }
}
