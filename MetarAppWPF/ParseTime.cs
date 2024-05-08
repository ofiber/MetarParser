using System;

namespace MetarAppWPF
{ 
    internal class ParseTimeToZulu
    {
        private static int UTC_OFFSET_EST = 5;      // UTC offset for EST
        private static int UTC_OFFSET_EDT = 4;      // UTC offset for EST during daylight savings
        private static int TIME_FORMAT = 24;    // Time format, 24 HR
        private static int TIME_FORMAT_AMPM = 12;    // Time format, 12 HR

        public static string ParseTime()
        {
            // Get the current time
            DateTime currentTime = DateTime.Now;

            // Is DST in effect
            bool isDST = TimeZoneInfo.Local.IsDaylightSavingTime(currentTime);

            // Convert the current time to a string
            string timeString = currentTime.ToString();

            // Parse the date from full date/time string
            string dateString = timeString.Substring(0, timeString.IndexOf(":") - 2);

            // Parse year from the date string
            string date = ParseYear(dateString);

            // Parse the month and day, add to the date string
            date += ParseMonths(dateString);
            date += ParseDay(dateString);

            // Parse the time from the full date/time string
            string time = timeString.Substring(timeString.IndexOf(":") - 2, 5);

            string when = timeString.Substring(timeString.Length - 2, 2);

            // Remove the colon from the time string
            time = time.Remove(2, 1);

            // Parse the hour to int
            int hour = int.Parse(time.Substring(0, 2));

            if(when == "PM")
                hour = (hour + TIME_FORMAT_AMPM) % TIME_FORMAT;

            // If timezone is in DST, convert to EDT, else conver to EST
            if(isDST)
                hour = (hour + UTC_OFFSET_EDT) % TIME_FORMAT;
            else
                hour = (hour + UTC_OFFSET_EST) % TIME_FORMAT;

            // If hour is less than 10, add a 0 to the beginning of the string
            if (hour < 10)
                time = time.Replace(time.Substring(0, 2), "0" + hour.ToString());
            else
                // Replace the hour in the time string
                time = time.Replace(time.Substring(0, 2), hour.ToString());

           // Console.WriteLine(date + "_" + time);

            // Return the current time in Zulu time
            return date + "_" + time + "Z";
        }

        private static string ParseDay(string date)
        {
            // Parse the day from the date string
            string day = date.Substring(date.IndexOf("/") + 1, date.LastIndexOf("/") - 2);

            // If the day is less than 10, add a 0 to the beginning of the string
            if (day.Length < 2)
                day = day.Insert(0, "0");

            // Return the day
            return day;
        }

        private static string ParseYear(string date)
        {
            // Return parsed the year from the date string
            return date.Substring(date.Length - 4, 4);
        }

        private static string ParseMonths(string date)
        {
            // Parse the month from the date string
            string month = date.Substring(0, date.IndexOf("/"));

            // If the month is less than 10, add a 0 to the beginning of the string
            if (month.Length < 2)
                month = month.Insert(0, "0");

            // Return the month
            return month;
        }
    }
}

