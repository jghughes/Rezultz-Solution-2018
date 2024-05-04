using System;

namespace NetStd.Goodies.Mar2022
{
    public class JghTimeSpan
    {
        #region format specifiers

        public const string GeneralShortPattern = "g";
        public const string GeneralLongPattern = "G";
        public const string HhmmssfPattern = @"hh\:mm\:ss\.f";
        public const string HhmmssffPattern = @"hh\:mm\:ss\.ff";
        public const string DdhhmmssfPattern = @"dd\.hh\:mm\:ss\.f";
        public const string DdhhmmssffPattern = @"dd\.hh\:mm\:ss\.ff";

        #endregion

        public static string ToPrettyDurationFromTicks(long durationInTicks)
        {
            string answer;

            try
            {
                if (durationInTicks == 0)
                {
                    answer = string.Empty;
                }
                else
                {
                    var theTimeSpan = TimeSpan.FromTicks(durationInTicks);

                    answer = theTimeSpan.ToString(theTimeSpan.Days > 0 ? DdhhmmssfPattern : HhmmssfPattern);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return answer;
        }

        public static TimeSpan ToTimeSpanFromSeconds(double durationInSeconds, int roundingDecimalPlaces)
        {

            double JghGuardAgainstExceedingTimeSpanMaxAsMilliseconds(double milliseconds)
            {
                const string failure = "Guard against overflow.";
                const string locus = "[JghGuardAgainstExceedingTimeSpanMaxAsMilliseconds]";

                // be sure not to make epsilon any smaller than it already is here!

                const double epsilon = 100000;

                try
                {
                    var maximumPermissableMilliseconds = TimeSpan.MaxValue.TotalMilliseconds - epsilon;

                    return milliseconds > maximumPermissableMilliseconds ? maximumPermissableMilliseconds : milliseconds;
                }
                catch (Exception ex)
                {
                    throw new Exception(failure + " " + locus, ex);
                }
            }

            var milliseconds = Math.Round(durationInSeconds, roundingDecimalPlaces) * 1000;

            var answer = TimeSpan.FromMilliseconds(JghGuardAgainstExceedingTimeSpanMaxAsMilliseconds(milliseconds));

            return answer;
        }

        public static double ToTotalSeconds(string errorProneTimespanAsString)
        {
            if (string.IsNullOrWhiteSpace(errorProneTimespanAsString))
                return 0;

            if (!TimeSpan.TryParse(errorProneTimespanAsString, out var successfullyParsedTimeSpan)) return 0;

            var answerAsSeconds = successfullyParsedTimeSpan.TotalSeconds;

            return answerAsSeconds;
        }

        public static string ToDurationOrDnxV2(string errorProneTimespanAsString, string dnxString)
        {
            string answer;

            //if (string.IsNullOrWhiteSpace(dnxString))
            //{
            var isValidTimeSpan = TimeSpan.TryParse(errorProneTimespanAsString, out _);

            answer = isValidTimeSpan ? errorProneTimespanAsString : string.Empty; // important to be string.Empty if invalid
            //}
            //else
            //{
            //    answer = string.Empty; // important
            //}

            return answer;
        }
        public static string ToDurationOrDnx(string errorProneTimespanAsString, string dnxString)
        {
            string answer;

            if (string.IsNullOrWhiteSpace(dnxString))
            {
                var isValidTimeSpan = TimeSpan.TryParse(errorProneTimespanAsString, out _);

                answer = isValidTimeSpan ? errorProneTimespanAsString : string.Empty; // important to be string.Empty if invalid
            }
            else
            {
                answer = string.Empty; // important
            }

            return answer;
        }

        public static string ToDurationOrBlankIfNearZero(TimeSpan thisTimeSpan, int doubleRoundingPlaces, string formatString)
        {
            const double epsilonSeconds = 0.01; // arbitrarily small

            var timeSpanAsSeconds = thisTimeSpan.TotalSeconds;

            if (Math.Abs(timeSpanAsSeconds) < Math.Abs(epsilonSeconds))
                return string.Empty;

            var roundedSeconds = Math.Round(timeSpanAsSeconds, doubleRoundingPlaces);

            //if (JghMath.Abs(timeSpanAsSeconds) < JghMath.Abs(epsilonSeconds))
            //	return string.Empty;
            //var roundedSeconds = JghMath.Round(timeSpanAsSeconds, doubleRoundingPlaces);


            var ts = JghTimeSpan.ToTimeSpanFromSeconds(roundedSeconds, doubleRoundingPlaces);


            return ts.ToString(ts.Days > 0 ? JghTimeSpan.DdhhmmssfPattern : JghTimeSpan.HhmmssfPattern);
        }
    }
}