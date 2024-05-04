using System;

namespace NetStd.Goodies.Mar2022
{
    public class JghDateTime
    {
        #region format specifiers
  

        public const double ArbitrarilyLargeNum = 99999 * 1000;

        public const string Iso8601Pattern = "O"; // <2021-12-29T15:35:24.5737342-05:00> the only reliable and accurate format for date times involved in round-tripping. 
        public const string SortablePattern = "s"; // <2021-12-29T15:38:34> concise format for datetime in file names and blob names. not to be used when round-tripping
        public const string LongDatePattern = "D"; // <December 29, 2021> ho hum.
        public const string FullDatePattern = "F"; // <December 29, 2021 3:33:09 PM> ho hum.
        public const string Rfc1123Pattern = "R"; // <Wed, 29 Dec 2021 15:41:05 GMT>. AVOID! misleadingly adds a GMT suffix even if a time is not a UTC time!.
        public const string UniversalSortablePattern = "u";// <2021-12-29 15:43:54Z> AVOID! misleadingly adds a Z suffix even if a time is not a UTC time! FAILS arithmetic operations!
        public const string MilitaryPattern = "dddd MMMM dd, yyyy  HH':'mm':'ss";// NB. the ':' separator is incompatible with NTFS filenames, but it's human friendly
        public const string ShortDatePattern = "yyyy-MM-dd";


        //To round-trip DateTime values successfully, follow these steps:

        //If the values represent single moments of time, convert them from the local time to UTC by calling the ToUniversalTime method.
        //Convert the dates to their string representations by calling the ToString(String, IFormatProvider) or String.Format(IFormatProvider, String, Object[]) overload. Use the formatting conventions of the invariant culture by specifying CultureInfo.InvariantCulture as the provider argument.Specify that the value should round-trip by using the "O" or "R" standard format string.
        //    To restore the persisted DateTime values without data loss, follow these steps:

        //Parse the data by calling the ParseExact or TryParseExact overload.Specify CultureInfo.InvariantCulture as the provider argument, and use the same standard format string you used for the format argument during conversion.Include the DateTimeStyles.RoundtripKind value in the styles argument.
        //    If the DateTime values represent single moments in time, call the ToLocalTime method to convert the parsed date from UTC to local time.

        #endregion

        public static string ToTimeLocalhhmmss(long binaryDateTime)
        {
            string answer;

            try
            {
                answer = binaryDateTime == 0
                    ? string.Empty
                    : DateTime.FromBinary(binaryDateTime).ToLocalTime().ToString("HH:mm:ss"); 

            }
            catch (Exception)
            {
                return string.Empty;
            }

            return answer;
        }

        public static string ToTimeLocalhhmmssf(long binaryDateTime)
        {
            string answer;

            try
            {
                answer = binaryDateTime == 0
                    ? string.Empty
                    : DateTime.FromBinary(binaryDateTime).ToLocalTime().ToString("HH:mm:ss.f"); 

            }
            catch (Exception)
            {
                return string.Empty;
            }

            return answer;
        }

        public static string ToDateTimeLocalSortable(long binaryDateTime)
        {
            string answer;

            try
            {
                answer = binaryDateTime == 0
                    ? string.Empty
                    : DateTime.FromBinary(binaryDateTime).ToLocalTime().ToString(SortablePattern); // <2021-12-29T15:38:34> concise format for datetime in file names and blob names. not to be used when round-tripping
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return answer;
        }

        public static string ToDateLocalSortable(long binaryDateTime)
        {
            string answer;

            try
            {
                answer = binaryDateTime == 0
                    ? string.Empty
                    : DateTime.FromBinary(binaryDateTime).ToLocalTime().Date.ToString(ShortDatePattern); // <2021-12-29> concise format for date. not to be used when round-tripping
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return answer;
        }

        public static string ToLongDate(long binaryDateTime)
        {
            string answer;

            try
            {
                answer = binaryDateTime == 0
                    ? string.Empty
                    : DateTime.FromBinary(binaryDateTime).ToLocalTime().Date.ToString("D"); // <Friday, 18 April, 2008> human readable. use it only for display purposes
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return answer;
        }

        public static string ToDateTimeLocalIso8601(long binaryDateTime)
        {
            string answer;

            try
            {
                answer = binaryDateTime == 0
                    ? string.Empty
                    : DateTime.FromBinary(binaryDateTime).ToLocalTime().ToString(Iso8601Pattern);// <2021-12-29T15:35:24.5737342-05:00> the only reliable and accurate format for date times involved in round-tripping.
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return answer;
        }

        public static DateTime RoundedToTenthOfSecond(DateTime item)
        {
            var dateTimeExcludingSeconds = new DateTime(
                item.Year,
                item.Month,
                item.Day,
                item.Hour,
                item.Minute,
                0,
                DateTimeKind.Local);

            var secondsWithFraction = item.Second + (double)item.Millisecond / 1000;

            var secondsRoundedToNearestTenth = Math.Round(secondsWithFraction, 1);

            var roundedSecondsToAddBack = TimeSpan.FromSeconds(secondsRoundedToNearestTenth);

            var dateTimeToNearestTenth = dateTimeExcludingSeconds + roundedSecondsToAddBack;

            return dateTimeToNearestTenth;
        }
    }
}