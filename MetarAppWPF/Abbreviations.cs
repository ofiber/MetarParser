using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarAppWPF
{
    internal class Abbreviations
    {
        public static readonly Dictionary<string, string> abbr = new Dictionary<string, string>
        {
            {"ALQDS", "All Quadrants" },
            {"OCNL", "Occasional" },
            {"LTGIC", "Lightning in clouds" },
            {"DSNT", "Distant" },
            {"MOV", "Moving" },
            {"LTGICCG", "Lightning in cloud and cloud to ground" },
            {"OHD", "Overhead" },
            {"LTG", "Lightning" },
            {"RMK", "Remarks" },
            {"CONS", "Continous" },
            {"LTGICCC", "Lightning in clouds and cloud to cloud" },
            {"LTGCW", "Lightning cloud to water" },
            {"PRESFR", "Pressure falling rapidly" }

        };
    }
}