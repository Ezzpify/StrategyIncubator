using System;
using System.Text.RegularExpressions;

namespace StrategyIncubator
{
    class Functions
    {
        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(datetime - sTime).TotalSeconds;
        }

        public static string NullcheckStr(string str)
        {
            return str != null ? str : "Unknown";
        }

        public static int ConvertMinutesToMilliseconds(int minutes)
        {
            return (int)TimeSpan.FromMinutes(minutes).TotalMilliseconds;
        }

        public static string DiscordifyString(string str)
        {
            /*Add < > around links to avoid Discord making a link preview
            in the client which takes up a lot of space if there aremore 
            than just one link in the string*/
            str = Regex.Replace(str, @"(https?://[^\s]+)", "<$1>");

            /*Replace newline characters with a space to fit in as much
            text as possible from the summary to make it more compact*/
            str = Regex.Replace(str, @"\r\n?|\n", " ");

            /*Replaces double spaces with a single space*/
            str = Regex.Replace(str, @"\s+", " ");

            return str;
        }
    }
}
