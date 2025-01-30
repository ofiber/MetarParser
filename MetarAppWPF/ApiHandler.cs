//#pragma warning disable SYSLIB0014 // Disable warning for WebRequest obselescence
#pragma warning disable CS8600     // Disable warning for nullable reference type

using System;
using System.Net;
using System.IO;
using System.Net.Http;

namespace MetarAppWPF
{
    internal class ApiHandler
    {
        private static bool PerformWebRequest(string url, out string response)
        {
            // API response string
            response = string.Empty;

            try
            {
                HttpClient client = new HttpClient();
                using HttpResponseMessage res = client.GetAsync(url).Result;

                response = res.Content.ReadAsStringAsync().Result;
                //Console.WriteLine(res.Content.ReadAsStringAsync().Result);

                //// Create a new web request
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                //// Set the request headers
                //request.Method = "GET";

                //// Get the response
                //using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                //{

                //    if(webResponse.ContentLength == 0)
                //    {
                //        return false;
                //    }

                //    // Read the response stream
                //    using (Stream stream = webResponse.GetResponseStream())
                //    {
                //        // Read the response
                //        using (StreamReader reader = new StreamReader(stream))
                //        {
                //            // Set the response string
                //            response = reader.ReadToEnd();
                //        }
                //    }
                //}

                // Return true if the request was successful
                return true;
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return false
                Console.WriteLine("Failed to perform web request: " + ex.Message);
                return false;
            }
        }

        public static string RequestMetar(string icaoCode)
        {
            // API URL
            string url = "https://aviationweather.gov/api/data/metar?ids=&format=raw&taf=false&hours=0&date=";

            string response = ""; // API response string

            // Get the current time and parse it to Zulu time in the format "YYYYMMDD_HHMM"
            string time = ParseTimeToZulu.ParseTime();

            // Append the time to the URL
            url += time;

            // If the ICAO code is empty, use Atlanta as the default
            if (string.IsNullOrEmpty(icaoCode))
                icaoCode = "KATL";

            // Convert the ICAO code to uppercase
            icaoCode = icaoCode.ToUpper();

            // Insert the ICAO code into the URL
            url = url.Insert(47, icaoCode);

            // If the web request fails, return -1, otherwise return the response
            if (!PerformWebRequest(url, out response))
                return "-1";

            if (string.IsNullOrEmpty(response))
            {
                while (string.IsNullOrEmpty(response))
                {
                    url = url.Replace(time, "");
                    time = UpdateHour(time);
                    url += time;

                    if (!PerformWebRequest(url, out response))
                        return "-1";
                }

                
            }

            return response;
        }

        private static string UpdateHour(string time)
        {
            string hr = time.Substring(time.IndexOf("_") + 1, 2);

            int hour = int.Parse(hr);

            hour -= 1;

            if (hour < 10)
                hr = "0" + hour.ToString();
            else
                hr = hour.ToString();

            time = time.Remove(time.IndexOf("_") + 1, 2);
            return time.Insert(time.IndexOf("_") + 1, hr);

        }
    }
}