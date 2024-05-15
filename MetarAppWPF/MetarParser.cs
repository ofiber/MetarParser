#pragma warning disable CS8600
#pragma warning disable CS8604

using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace MetarAppWPF
{
    class MetarParser
    {
        private static string parsedMetar = "";
        private static StringBuilder encodedMetar = new StringBuilder();
        private static StringBuilder decodedMetar = new StringBuilder();

        //private static TextBox tB = MainWindow.icaoTb;

        public static bool Validate(string metar) { return ValidateMetar(metar); }

        public static void GetParsedMetar(string metar)
        {
            var win = new MainWindow();

            Console.SetOut(new ControlWriter(win.metarTbEnc));
                        
            Dictionary<string, string> weatherMap = WeatherMap.weatherMap;
            Dictionary<string, string> cloudMap = WeatherMap.cloudMap;
            Dictionary<string, string> cbMap = WeatherMap.cumulonimbusMap;
            Dictionary<string, string> ic = ICAODict.icaoDict;

            Regex station_regex = new Regex("METAR\\s(\\w{4})");
            Regex time_regex = new Regex("\\b(\\d{6}Z)\\b");
            Regex wind_regex = new Regex("\\b(\\d{3}|VRB)(\\d{2}|\\d{2}G\\d{2})KT\\b");
            Regex peak_wind_regex = new Regex("\\b(PK WND)\\b\\s\\b(\\d{5}\\/\\d{4})\\b");
            Regex windshear_regex = new Regex("\\bWS\\s(RWY\\d{2}[L|R|C])|(RWY\\d{2}\b)");
            Regex windshift_regex = new Regex("\\bWSHFT\\s\\d{4}\\b");
            Regex visibility_regex = new Regex("\\b(\\d{1,2})SM\\b|\\b(\\d{4})\\b|\\b(([1-9]\\s)?([1-9]\\/[1-9])SM)|CAVOK\\b");
            Regex twr_vis_regex = new Regex("\\bTWR VIS [1-9]+\\s[1-9]\\/[1-9]|TWR VIS [1-9]+\\b");
            Regex surface_vis_regex = new Regex("\\bSFC\\sVIS\\s((\\d*\\s\\d\\/\\d)|(\\d\\/\\d))\\b");
            Regex surface_vis_rng_regex = new Regex("\\b\\d\\/\\dV\\d\\b");
            Regex clouds_regex = new Regex("(FEW|SCT|BKN|OVC)((\\d{3})(CB)?)|(CLR)|(NCD)");
            Regex temperature_regex = new Regex("\\bT(\\d{4})(\\d{4})\\b");
            Regex alt_temp_regex = new Regex("((M?\\d{2})\\/(M?\\d{2}))");
            Regex altimeter_regex = new Regex("(A|Q)(\\d{4})");
            Regex remarks_regex = new Regex("RMK\\s(.*)");
            Regex weather_regex = new Regex("(-|\\+)?\\b(MIFG|BCFG|BR|FG|DZ|RA|SN|SG|IC|PL|GR|GS|UP|BR|HZ|FU|VA|CG|DU|SA|PY|PO|SQ|FC|SS|DS)\\b");            
            Regex percip_regex = new Regex("\\b(P\\d{4})\\b");
            Regex percip_regex2 = new Regex("\\b(6\\d{4})\\b");
            Regex percip_regex3 = new Regex("\\b(7\\d{4})\\b");
            Regex rvr_regex = new Regex("\\bR\\d{2}\\/\\d{4}V\\d{4}FT\\b");
            Regex cumulonimbus_regex = new Regex("MOV\\D((OHD|NW|NE|SW|SE|N|E|W|S))");
            Regex cbMovement_regex = new Regex("[NEWS]+");
            Regex nosig_regex = new Regex("\\bNOSIG\\b");

            if (string.IsNullOrEmpty(metar) || metar == "-1")
                GetMetar(ref metar);

            PrintHeader(metar);

            MatchCollection matches;
            GetStation(out matches, metar, station_regex, ic);
            GetTime(out matches, metar, time_regex);
            GetWinds(out matches, metar, wind_regex);
            GetPeakWind(out matches, metar, peak_wind_regex);
            GetWindshear(out matches, metar, windshear_regex);
            GetWindshift(out matches, metar, windshift_regex);
            int x = GetVisibility(out matches, metar, visibility_regex);
            GetTowerVis(out matches, metar, twr_vis_regex);
            GetSurfaceVis(out matches, metar, surface_vis_regex);
            GetSurfaceVisRng(out matches, metar, surface_vis_rng_regex);
            GetRVR(out matches, metar, rvr_regex);

            if(x!= -1)
                GetClouds(out matches, metar, clouds_regex, cloudMap);

            GetTempAndDewpoint(out matches, metar, temperature_regex, alt_temp_regex);
            GetAltimeter(out matches, metar, altimeter_regex);
            GetWeather(out matches, metar, remarks_regex, weather_regex, weatherMap);
            GetRecentPercip(out matches, metar, percip_regex, percip_regex2, percip_regex3);
            GetCumulonimbusActivity(out matches, metar, cumulonimbus_regex, cbMovement_regex, cbMap);
            GetMaintenence(out matches, metar, remarks_regex);
            GetNoSig(out matches, metar, nosig_regex);
        }

        private static bool ValidateMetar(string metar)
        {
            Regex station_regex = new Regex("METAR\\s(\\w{4})");
            MatchCollection m;

            if (metar.Length < 12)
            {
                return false;
            }
            else
            {
                if (!metar.Contains("METAR"))
                    metar = "METAR " + metar;

                m = station_regex.Matches(metar);

                string i = m[0].Value;

                if (ICAODict.icaoDict.ContainsKey(i))
                    return true;
                else
                    return false;
            }
        }

        private static void PrintHeader(string metar)
        {
            //Console.Clear();

            encodedMetar.AppendLine("\t Encoded METAR ");
            encodedMetar.AppendLine();
            encodedMetar.AppendLine(metar);
            encodedMetar.AppendLine();
            decodedMetar.AppendLine("\t Decoded METAR");
            decodedMetar.AppendLine();
        }

        private static void GetMetar(ref string metar)
        {
            string temp;
            do
            {
                Console.WriteLine("API request failed, please enter METAR manually");

                decodedMetar.Append("METAR: ");
                temp = Console.ReadLine();

            } while (!ValidateMetar(temp));

            metar = temp;

            Console.Clear();
        }

        private static void GetStation(out MatchCollection matches, string metar, Regex station_regex, Dictionary<string, string> icaoList)
        {
            string station = "";

            if (!metar.Contains("METAR"))
                metar = "METAR " + metar;

            matches = station_regex.Matches(metar);

            if(matches.Count > 0)
                station = matches[0].Groups[1].Value;
                        
            decodedMetar.AppendLine("\nICAO: " + station);

            string name = icaoList[station];
                        
            decodedMetar.AppendLine("Station Name: " + name);
        }

        private static void GetTime(out MatchCollection matches, string metar, Regex time_regex)
        {
            string time = "";

            matches = time_regex.Matches(metar);

            if(matches.Count > 0)
                time = matches[0].Groups[1].Value;

            if (string.IsNullOrEmpty(time))
                time = "N/A";
            else
            {
                time = time.Remove(0, 2);
                time = time.Insert(4, " ");
            }

            decodedMetar.AppendLine("Time: " + time);
        }

        private static void GetWinds(out MatchCollection matches, string metar, Regex wind_regex)
        {
            string wind_speed = "";
            string wind_direction = "";

            matches = wind_regex.Matches(metar);

            if (matches.Count > 0)
            {
                wind_direction = matches[0].Groups[1].Value;
                wind_speed = matches[0].Groups[2].Value;
            }

            if (!string.IsNullOrEmpty(wind_direction) && !string.IsNullOrEmpty(wind_speed))
            {
                if (wind_speed.Contains("G"))
                {
                    wind_speed = wind_speed.Remove(2, 1);                    
                    decodedMetar.AppendLine("Wind: " + wind_direction + " @ " + wind_speed.Substring(0, 2) + " gusting " + wind_speed.Substring(2, 2));
                }
                else if (wind_speed == "00" && wind_direction == "000")
                {
                    decodedMetar.AppendLine("Wind: Calm");
                }
                else
                {
                    if (wind_direction == "VRB")
                    {                        
                        decodedMetar.AppendLine("Wind: Variable @ " + wind_speed);
                    }
                    else
                        decodedMetar.AppendLine("Wind: " + wind_direction + " @ " + wind_speed);
                }
            }
        }

        private static void GetPeakWind(out MatchCollection matches, string metar, Regex peak_wind_regex)
        {
            string pkWind = "";

            matches = peak_wind_regex.Matches(metar);

            if(matches.Count > 0)
                pkWind = matches[0].Groups[2].Value;

            if (!string.IsNullOrEmpty(pkWind))
            {
                string wind = pkWind.Substring(0, pkWind.IndexOf("/"));
                wind = wind.Insert(3, " @ ");

                string time = pkWind.Substring(pkWind.IndexOf("/"), pkWind.Length - pkWind.IndexOf("/"));
                time = time.Remove(0, 1);
                time = time + "Z";

                decodedMetar.AppendLine("Peak Wind: " + wind + " at " + time);
            }
        }

        private static void GetWindshear(out MatchCollection matches, string metar, Regex windshear_regex)
        {
            string windshear = "";

            matches = windshear_regex.Matches(metar);

            if(matches.Count > 0)
                windshear = matches[0].Groups[1].Value;

            if (!string.IsNullOrEmpty(windshear))
                decodedMetar.AppendLine("Windshear: " + windshear.Substring(0, 3) + " " + windshear.Substring(3, windshear.Length - 1));
        }

        private static void GetWindshift(out MatchCollection matches, string metar, Regex windshift_regex)
        {
            string windshift = "";

            matches = windshift_regex.Matches(metar);

            if (matches.Count > 0)
                windshift = matches[0].Value;
            else
                windshift = "-1";

            if (!string.IsNullOrEmpty(windshift))
                decodedMetar.AppendLine("Windshift: " + windshift.Substring(windshift.IndexOf(" ") + 1) + " Z");
        }

        private static int GetVisibility(out MatchCollection matches, string metar, Regex visibility_regex)
        {
            string visibility = "";
            int i = 0;
            matches = visibility_regex.Matches(metar);

            if (matches.Count == 0)
            {
                decodedMetar.AppendLine("Visibility: N/A");
                return 1;
            }

            while (string.IsNullOrEmpty(visibility))
            {
                visibility = matches[i].Value;
                i++;
            }

            if (!string.IsNullOrEmpty(visibility))
            {
                if(visibility == "CAVOK")
                {
                    decodedMetar.AppendLine("Visibility: CAVOK (Ceiling and Visibility OK)");
                    return -1;
                }

                if (visibility.Contains("/"))
                {
                    visibility = visibility.Remove(visibility.Length - 2, 2);

                    decodedMetar.AppendLine("Visibility: " + visibility + " miles");
                }
                else if(visibility.Contains("SM"))
                {
                    visibility = visibility.Remove(visibility.IndexOf("S"), 2);
                    decodedMetar.AppendLine("Visibility: " + visibility + " miles");
                }
                else
                {
                    if (int.Parse(visibility) > 1000)
                        decodedMetar.AppendLine("Visibility: " + int.Parse(visibility) / 1000 + " km");
                    else if (int.Parse(visibility) > 100 && int.Parse(visibility) <= 999)
                    {
                        visibility = visibility.Remove(0, 1);
                        decodedMetar.AppendLine("Visibility: " + visibility + " meters");
                    }
                }
            }

            return 1;
        }

        private static void GetTowerVis(out MatchCollection matches, string metar, Regex twr_vis_regex)
        {
            string twr_vis = "";

            matches = twr_vis_regex.Matches(metar);

            if(matches.Count > 0)
                twr_vis = matches[0].Value;
            
            if(!string.IsNullOrEmpty(twr_vis) && twr_vis.Length > 7)
            {
                twr_vis = twr_vis.Remove(0, 7);

                decodedMetar.AppendLine("Tower Visibility: " + twr_vis + " miles");
            }
        }

        private static void GetSurfaceVis(out MatchCollection matches, string metar, Regex surface_vis_regex)
        {
            string surface_vis = "";

            matches = surface_vis_regex.Matches(metar);
            if (matches.Count > 0)
                surface_vis = matches[0].Value;

            if (!string.IsNullOrEmpty(surface_vis))
            {
                surface_vis = surface_vis.Remove(0, 8);
                decodedMetar.AppendLine("Surface Visibility: " + surface_vis + " miles");
            }
        }

        private static void GetSurfaceVisRng(out MatchCollection matches, string metar, Regex surface_vis_rng_regex)
        {
            string surface_vis_rng = "";

            matches = surface_vis_rng_regex.Matches(metar);

            if (matches.Count > 0)
                surface_vis_rng = matches[0].Value;
            
            if (!string.IsNullOrEmpty(surface_vis_rng))
            {
                decodedMetar.AppendLine("Surface Visibility Range: " + surface_vis_rng.Substring(0, surface_vis_rng.IndexOf("V"))
                    + " - " +
                    surface_vis_rng.Substring(surface_vis_rng.IndexOf("V") + 1,
                        surface_vis_rng.Length - surface_vis_rng.IndexOf("V") + 1) + " miles");
            }
        }

        private static void GetRVR(out MatchCollection matches, string metar, Regex rvr_regex)
        {
            string full = "", runway, rvr;

            matches = rvr_regex.Matches(metar);

            if (matches.Count > 0)
                full = matches[0].Value;

            if (!string.IsNullOrEmpty(full))
            {
                runway = full.Substring(1, full.IndexOf("/") - 1);

                rvr = full.Substring(full.IndexOf("/") + 1);

                rvr = rvr.Replace("V", "ft - ");
                rvr = rvr.Replace("FT", "ft");
                
                decodedMetar.AppendLine("RWY " + runway + " Visual Range: " + rvr);
            }
        }

        private static void GetClouds(out MatchCollection matches, string metar, Regex clouds_regex, Dictionary<string, string> cloudMap)
        {
            string[] cloudAlt = new string[5] { "", "", "", "", "" };
            string[] cloudType = new string[5] { "", "", "", "", "" };
            string c = "";

            matches = clouds_regex.Matches(metar);

            if (matches.Count > 0)
                c = matches[0].Value;
            
            int i = 1;

            if (c != "CLR" && c != "NCD")
            {
                i = 0;

                foreach (Match match in matches)
                {
                    cloudType[i] = match.Groups[1].Value;
                    cloudAlt[i] = match.Groups[2].Value;
                    i++;
                }

                decodedMetar.Append("Cloud Coverage: ");

                int j = 0;
                string x;
                bool cb = false;

                while (!string.IsNullOrEmpty(cloudType[j]))
                {
                    if (j != 0)
                        decodedMetar.Append(", ");

                    decodedMetar.Append(cloudMap[cloudType[j]] + " at ");

                    x = cloudAlt[j];

                    if (x.Contains("CB"))
                    {
                        x = x.Remove(x.IndexOf("CB"), 2);
                        cb = true;
                    }
                    while (x[0] == '0')
                        x = x.Remove(0, 1);

                    if (int.Parse(x) > 1)
                        decodedMetar.Append(int.Parse(x) * 100 + "ft");
                    else
                        decodedMetar.Append(int.Parse(x) + "00ft");
                    if (cb)
                        decodedMetar.Append(" (Cumulonimbus)");

                    cb = false;
                    j++;
                }

                decodedMetar.AppendLine();
            }
            else if(c == "NCD")
                decodedMetar.AppendLine("Cloud Coverage: No Cloud Detected");
            else
                decodedMetar.AppendLine("Cloud Coverage: Clear");
        }

        private static void GetTempAndDewpoint(out MatchCollection matches, string metar, Regex temperature_regex, Regex alt_temp_regex)
        {
            string temperature = "", dewpoint = "";

            matches = temperature_regex.Matches(metar);
            
            if (matches.Count > 0)
            {
                temperature = matches[0].Groups[1].Value;
                dewpoint = matches[0].Groups[2].Value;
            }

            if (!string.IsNullOrEmpty(temperature) && !string.IsNullOrEmpty(dewpoint))
            {
                if (temperature != "0000")
                {
                    if (temperature[0] == '1')
                    {
                        temperature = temperature.Remove(0, 1);

                        while (temperature[0] == '0')
                            temperature = temperature.Remove(0, 1);

                        temperature = "-" + temperature;
                    }
                    else
                    {
                        while (temperature[0] == '0')
                            temperature = temperature.Remove(0, 1);
                    }
                }
                else
                    temperature = "00";

                if (dewpoint != "0000")
                {
                    if (dewpoint[0] == '1')
                    {
                        dewpoint = dewpoint.Remove(0, 1);

                        while (dewpoint[0] == '0')
                            dewpoint = dewpoint.Remove(0, 1);

                        dewpoint = "-" + dewpoint;
                    }
                    else
                    {
                        while (dewpoint[0] == '0')
                            dewpoint = dewpoint.Remove(0, 1);
                    }
                }
                else
                    dewpoint = "00";
                if (temperature.Length == 2 && temperature[0] == '-')
                    temperature = temperature.Insert(temperature.Length - 1, "0");
                else if (temperature.Length == 1)
                    temperature = temperature.Insert(0, "0");
                if (dewpoint.Length == 2 && dewpoint[0] == '-')
                    dewpoint = dewpoint.Insert(dewpoint.Length - 1, "0");
                else if (dewpoint.Length == 1)
                    dewpoint = dewpoint.Insert(0, "0");

                temperature = temperature.Insert(temperature.Length - 1, ".");
                dewpoint = dewpoint.Insert(dewpoint.Length - 1, ".");
            }
            else
            {
                matches = alt_temp_regex.Matches(metar);

                temperature = matches[0].Groups[2].Value;
                dewpoint = matches[0].Groups[3].Value;

                if (temperature[0] == 'M')
                {
                    temperature = temperature.Remove(0, 1);

                    if (temperature[0] == '0')
                        temperature = temperature.Remove(0, 1);

                    temperature = "-" + temperature;
                }

                if (dewpoint[0] == 'M')
                {
                    dewpoint = dewpoint.Remove(0, 1);

                    if (dewpoint[0] == '0')
                        dewpoint = dewpoint.Remove(0, 1);

                    dewpoint = "-" + dewpoint;
                }

                if (temperature[0] == '0')
                    temperature = temperature.Remove(0, 1);

                if (dewpoint[0] == '0')
                    dewpoint = dewpoint.Remove(0, 1);
            }
            if (!string.IsNullOrEmpty(temperature) && !string.IsNullOrEmpty(dewpoint))
            {
                decodedMetar.AppendLine("Temperature: " + temperature + " C");
                decodedMetar.AppendLine("Dewpoint: " + dewpoint + " C");
            }
        }

        private static void GetAltimeter(out MatchCollection matches, string metar, Regex altimeter_regex)
        {
            string altimeter = "";

            matches = altimeter_regex.Matches(metar);

            if(matches.Count > 0)
                altimeter = matches[0].Groups[1].Value + matches[0].Groups[2].Value;

            if (!string.IsNullOrEmpty(altimeter))
            {
                if (altimeter[0] == 'A')
                {
                    altimeter = altimeter.Remove(0, 1);
                    decodedMetar.AppendLine("Altimeter: " + altimeter.Insert(2, ".") + " inHg");
                }
                else if (altimeter[0] == 'Q')
                {
                    altimeter = altimeter.Remove(0, 1);

                    if (altimeter[0] == '0')
                        altimeter = altimeter.Remove(0, 1);

                    decodedMetar.AppendLine("Altimeter: " + altimeter + " hPa");
                }
            }
        }

        private static void GetWeather(out MatchCollection matches, string metar, Regex remarks_regex, Regex weather_regex, Dictionary<string, string> weatherMap)
        {
            string remarks = "";
            string[] weather = new string[10] { "", "", "", "", "", "", "", "", "", "" };
            string[] weatherType = new string[10] { "", "", "", "", "", "", "", "", "", "" };

            matches = remarks_regex.Matches(metar);

            if(matches.Count > 0)
                remarks = matches[0].Groups[1].Value;

            int i = 0;
            string modMetar = metar.Remove(0, 11);
            
            foreach (Match match in weather_regex.Matches(modMetar))
            {
                weatherType[i] = match.Groups[1].Value;
                weather[i] = match.Groups[2].Value;
                i++;
            }

            int j = 0;

            if (!string.IsNullOrEmpty(weather[0]))
            {
                decodedMetar.Append("Weather: ");

                while (!string.IsNullOrEmpty(weather[j]))
                {
                    if (j != 0)
                        decodedMetar.Append(", ");
                    if (weatherType[j] == "-")
                        decodedMetar.Append("Light ");
                    else if (weatherType[j] == "+")
                        decodedMetar.Append("Heavy ");

                    decodedMetar.Append(weatherMap[weather[j]]);

                    j++;
                }
            }
            else
                decodedMetar.Append("Weather: N/A");

            decodedMetar.AppendLine();
        }

        private static void GetRecentPercip(out MatchCollection matches, string metar, Regex percip_regex, Regex percip_regex2, Regex percip_regex3)
        {
            string percip = "";
            bool oneHr = false, threeHr = false;

            matches = percip_regex.Matches(metar);

            if (matches.Count > 0)
                percip = matches[0].Groups[1].Value;

            if (!string.IsNullOrEmpty(percip))
            {
                if (percip[0] == 'P')
                    percip = percip.Remove(0, 1);

                percip = percip.Insert(percip.Length - 2, ".");

                if (percip[0] == '0')
                    percip = percip.Remove(0, 1);

                decodedMetar.Append("Percipitation: 1HR " + percip + "\"");
                oneHr = true;
            }

            percip = "";

            matches = percip_regex2.Matches(metar);

            if (matches.Count > 0)
                percip = matches[0].Groups[1].Value;

            if (!string.IsNullOrEmpty(percip))
            {
                if (percip[0] == '6')
                    percip = percip.Remove(0, 1);

                percip = percip.Insert(percip.Length - 2, ".");

                if (percip[0] == '0')
                    percip = percip.Remove(0, 1);

                if (oneHr)
                    decodedMetar.Append(", 3HR " + percip + "\"");
                else
                    decodedMetar.Append("Percipitation: 3HR " + percip + "\"");

                threeHr = true;
            }

            percip = "";

            matches = percip_regex3.Matches(metar);

            if (matches.Count > 0)
                percip = matches[0].Groups[1].Value;

            if (!string.IsNullOrEmpty(percip))
            {
                if (percip[0] == '7')
                    percip = percip.Remove(0, 1);

                percip = percip.Insert(percip.Length - 2, ".");

                if (percip[0] == '0')
                    percip = percip.Remove(0, 1);

                if (oneHr || threeHr)
                    decodedMetar.Append(", 6HR " + percip + "\"");
                else
                    decodedMetar.Append("Percipitation: 6HR " + percip + "\"");
            }
            decodedMetar.AppendLine();
        }

        private static void GetCumulonimbusActivity(out MatchCollection matches, string metar, Regex cumulonimbusRegex, Regex cbMovementRegex, Dictionary<string, string> cbMap)
        {
            string where = "", movement = "";

            matches = cumulonimbusRegex.Matches(metar);

            if (matches.Count > 0)
                movement = matches[0].Groups[1].Value;

            if (metar.Contains(" CB "))
            {
                where = metar.Substring(metar.IndexOf("AO2"), metar.IndexOf("MOV") - metar.IndexOf("AO2"));
                where = where.Substring(where.IndexOf("CB") + 3, where.IndexOf("MOV") - where.IndexOf("CB") - 4);

                List<string> where2 = new List<string>();

                while (!string.IsNullOrEmpty(where))
                {
                    if (where.Contains("-"))
                    {
                        where2.Add(where.Substring(where.LastIndexOf("-") + 1));
                        where = where.Substring(0, where.LastIndexOf("-"));
                    }
                    else
                    {
                        where2.Add(where);
                        where = "";
                    }
                }

                if (!string.IsNullOrEmpty(where2[0]))
                {
                    decodedMetar.Append("Cumulonimbus activity ");

                    int i = where2.Count - 1;

                    while (i >= 0)
                    {
                        decodedMetar.Append(cbMap[where2[i]] + " ");
                        i--;
                    }

                    decodedMetar.AppendLine("moving " + cbMap[movement]);
                }
            }
        }

        private static void GetMaintenence(out MatchCollection matches, string metar, Regex remarks_regex)
        {
            string remarks = "";

            matches = remarks_regex.Matches(metar);

            if (matches.Count > 0)
                remarks = matches[0].Groups[1].Value;

            if (!string.IsNullOrEmpty(remarks) && remarks.Contains("$"))
                decodedMetar.AppendLine("\n\t!! Maintenence Check Required !!");
        }

        private static void GetNoSig(out MatchCollection matches, string metar, Regex nosig_regex)
        {
            string nosig = "";

            matches = nosig_regex.Matches(metar);

            if (matches.Count > 0)
                nosig = matches[0].Value;

            if (!string.IsNullOrEmpty(nosig))
                decodedMetar.AppendLine("No significant weather changes expected in next ~2 hrs");
        }

        public static string GetEncodedMetarAsString() {  return encodedMetar.ToString(); }
        public static string GetDecodedMetarAsString() { return decodedMetar.ToString(); }

        public static void ResetMetarStrings() { decodedMetar.Clear(); encodedMetar.Clear(); }
    }
}