#pragma warning disable CS8603 // Possible null reference return.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarAppWPF
{
    class ValidateMetar
    {
        // Minimum METAR: METAR KABC 021530Z 27007KT 10SM CLR 12/02 A2994 RMK AO2
        //                METAR KABC 021530Z 27007KT 10SM FEW060 12/02 A2994 RMK AO2

        public static readonly Regex station_regex = new Regex("METAR\\s(\\w{4})");
        public static readonly Regex time_regex = new Regex("\\b(\\d{6}Z)\\b");
        public static readonly Regex wind_regex = new Regex("\\b(\\d{3}|VRB)(\\d{2}|\\d{2}G\\d{2})KT\\b");
        public static readonly Regex visibility_regex = new Regex("\\b(\\d{1,2})SM\\b|\\b(\\d{4})\\b|\\b(([1-9]\\s)?([1-9]\\/[1-9])SM)|CAVOK\\b");
        public static readonly Regex clouds_regex = new Regex("(FEW|SCT|BKN|OVC)((\\d{3})(CB)?)|(CLR)|(NCD)");
        public static readonly Regex temperature_regex = new Regex("\\bT(\\d{4})(\\d{4})\\b");
        public static readonly Regex alt_temp_regex = new Regex("((M?\\d{2})\\/(M?\\d{2}))");
        public static readonly Regex altimeter_regex = new Regex("(A|Q)(\\d{4})");

        public static Dictionary<string, string> icao = ICAODict.icaoDict;

        public static bool Validate(string metar)
        {
            if (string.IsNullOrEmpty(metar))
                return false;

            // If user enters a METAR that doesn't start with "METAR", insert it
            if(metar.Contains("METAR") == false)
            {
               metar = metar.Insert(0, "METAR ");
            }

            string[] arr = {
                ValidateStation(metar),
                ValidateTime(metar),
                ValidateWind(metar),
                ValidateVisibility(metar),
                ValidateClouds(metar),
                ValidateTemperature(metar),
                ValidateAltimeter(metar)
            };

            // Check if all values are valid
            return GetValidation(arr);
        }

        private static bool GetValidation(string[] arr)
        {
            foreach (string s in arr)
            {
                if (s == null)
                    return false;
            }

            return true;
        }

        private static string ValidateStation(string metar)
        {
            Match match = station_regex.Match(metar);

            if (match.Success)
            {
                string station = match.Groups[1].Value;

                if (icao.ContainsKey(station))
                    return icao[station];
                else
                    return null;
            }
            else
                return null;
        }

        private static string ValidateTime(string metar)
        {
            Match match = time_regex.Match(metar);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
                return null;
        }

        private static string ValidateWind(string metar)
        {
            Match match = wind_regex.Match(metar);

            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            else
                return null;
        }

        private static string ValidateVisibility(string metar)
        {
            Match match = visibility_regex.Match(metar);

            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            else
                return null;
        }

        private static string ValidateClouds(string metar)
        {
            Match match = clouds_regex.Match(metar);

            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            else
                return null;
        }

        private static string ValidateTemperature(string metar)
        {
            Match match = temperature_regex.Match(metar);

            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            else
            {
                match = alt_temp_regex.Match(metar);

                if (match.Success)
                {
                    return match.Groups[0].Value;
                }
                else
                    return null;
            }

        }

        private static string ValidateAltimeter(string metar)
        {
            Match match = altimeter_regex.Match(metar);

            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            else
                return null;
        }
    }
}
