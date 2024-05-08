using System.Collections.Generic;

namespace MetarAppWPF
{
    public static class WeatherMap
    {
        public static readonly Dictionary<string, string> weatherMap = new Dictionary<string, string>
        {
            {"RA", "Rain"},
            {"DZ", "Drizzle"},
            {"SN", "Snow"},
            {"SG", "Snow Grains"},
            {"IC", "Ice Crystals"},
            {"PL", "Ice Pellets"},
            {"GR", "Hail"},
            {"GS", "Small Hail"},
            {"UP", "Unknown Precipitation"},
            {"BR", "Mist"},
            {"FG", "Fog"},
            {"FU", "Smoke"},
            {"VA", "Volcanic Ash"},
            {"DU", "Widespread Dust"},
            {"SA", "Sand"},
            {"HZ", "Haze"},
            {"PO", "Well-Developed Dust/Sand Whirls"},
            {"SQ", "Squalls"},
            {"FC", "Funnel Cloud"},
            {"SS", "Sandstorm"},
            {"DS", "Duststorm"},
            {"MIFG", "Shallow Fog"},
            {"BCFG", "Patches of Fog"},
            {"SH", "Showers"},
            {"PY", "Spray"},
            {"TS", "Thunderstorm"},
            {"CC", "Cloud-cloud lightning"},
            {"CA", "Cloud-air lightning"},
            {"CG", "Cloud-ground lightning"},
            {"+", "Heavy"},
            {"-", "Light"}
        };

        public static readonly Dictionary<string, string> cloudMap = new Dictionary<string, string>
        {
            {"FEW", "Few"},
            {"SCT", "Scattered"},
            {"BKN", "Broken"},
            {"OVC", "Overcast"},
            {"CLR", "Clear"},
            {"NCD", "No Cloud Detected"}
        };

        public static readonly Dictionary<string, string> cumulonimbusMap = new Dictionary<string, string>
        {
            {"OHD", "Overhead"},
            {"NE", "Northeast"},
            {"NW", "Northwest"},
            {"SE", "Southeast"},
            {"SW", "Southwest"},
            {"N", "North"},
            {"S", "South"},
            {"E", "East"},
            {"W", "West"}
        };
    }
}