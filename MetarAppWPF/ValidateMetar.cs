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
        public static readonly Regex altimeter_regex = new Regex("(A|Q)(\\d{4})");

        string metar = "METAR KABC 021530Z 27007KT 10SM CLR 12/02 A2994 RMK AO2";

        private bool Validate()
        {
            Console.WriteLine(Station());

            return true;
        }

        private bool Station()
        {
            string station;

            MatchCollection matches = station_regex.Matches(metar);

            station = matches[0].ToString();

            if (string.IsNullOrEmpty(station))
            {
                Console.WriteLine(station);
                return true;  
            }            
            else
                return false;
        }

        //private bool Time()
        //{

        //}

        //private bool Wind()
        //{

        //}

        //private bool Visibility()
        //{

        //}

        //private bool Clouds()
        //{

        //}

        //private bool Temperature()
        //{

        //}

        //private bool Altimeter()
        //{

        //}
    }
}
