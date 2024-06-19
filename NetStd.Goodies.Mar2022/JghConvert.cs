using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NetStd.Goodies.Mar2022
{
    public class JghConvert
    {
        public static byte[] ToBytesUtf8FromString(string theString)
        {
            if (theString is null) return null;

            const string failure = "Unable to convert string into bytes.";
            const string locus = "[TransformStringToBytesUtf8]";

            byte[] bytes;

            try
            {
                // Create a UTF-8 encoding.
                var utf8 = new UTF8Encoding(true, true);

                bytes = utf8.GetBytes(theString);
            }
            catch (Exception ex)
            {
                throw new Exception($"{failure} {locus}", ex);
            }

            return bytes;
        }

        public static Stream ToStreamFromString(string theString)
        {
            return new MemoryStream(JghConvert.ToBytesUtf8FromString(theString));
        }

        public static string ToStringFromUtf8Bytes(byte[] utf8EncodedBytes)
        {
            if (utf8EncodedBytes is null) return null;

            const string failure = "Unable to convert bytes into string.";
            const string locus = "[ToStringFromUtf8Bytes]";

            try
            {
                // Create a UTF-8 encoding.
                var utf8 = new UTF8Encoding(true, true);

                var answer = utf8.GetString(utf8EncodedBytes, 0, utf8EncodedBytes.Length);

                return answer;
            }
            catch (Exception ex)
            {
                throw new Exception($"{failure} {locus}", ex);
            }
        }

        /// <summary>
        ///     Converts an integer representing the size of an object e.g. 23,000 bytes
        ///     into a string description in the highest sensible unit of measure e.g. 23 KB.
        /// </summary>
        /// <param name="length">The integer.</param>
        /// <returns>The size.</returns>
        public static string SizeOfBytesInHighestUnitOfMeasure(long length)
        {
            // for reasons utterly inexplicable at the time of writing in Nov 2017, System.Math library compiles just fine in .NetStd but is dysfunctional. Unbelievable.
            // so i copied the source code from http://referencesource.microsoft.com/#mscorlib/system/Math.cs,a4407e67b9a5afad as best i could. but the results is way too simple and inferior. hopefully it's sufficient
            //var signAsPlusOrMinusOne = JghMath.Sign(length);
            var signAsPlusOrMinusOne = Math.Sign(length);

            var prefixes = new[] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" };

            var size = (double)Math.Abs(length);
            //var size = (double) JghMath.Abs(length);

            int i;

            for (i = 0; size > 1000; i++)
                size /= 1024.0;

            size *= signAsPlusOrMinusOne;

            var sizeAsString = size.ToString(JghFormatSpecifiers.DecimalFormat1Dp);

            var answer = $"{sizeAsString} {prefixes[i]}";

            //var answer = string.Format(i < 2 ? "{0:##0} {1}" : "{0:##0.#} {1}", size, prefixes[i]);

            return answer;
        }

        public static long SizeOfStringInMbytes(string theString)
        {
            var sizeInMb = SizeOfStringInBytes(theString) / 1024 * 1024;

            return sizeInMb;

        }

        public static long SizeOfStringInBytes(string theString)
        {
            const string failure = "Unable to convert string into Mb.";
            const string locus = "[TransformStringToMegaBytesUtf8]";

            try
            {
                var stringAsBytes = JghConvert.ToBytesUtf8FromString(theString);

                var size = stringAsBytes?.LongLength ?? 0;

                return size;
            }
            catch (Exception ex)
            {
                throw new Exception($"{failure} {locus}", ex);
            }
        }

        public static int ToInt32(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return 0;

            var conversionSucceeded = int.TryParse(theString, out var parseResult);

            return conversionSucceeded ? parseResult : 0;
        }

        public static bool ToBool(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return false;

            return (JghString.TmLr(theString) == "1") | (JghString.TmLr(theString) == "true") |
                   (JghString.TmLr(theString) == "yes") | (JghString.TmLr(theString) == "y");
        }

        public static double ToDouble(string theString)
        {
            if (string.IsNullOrWhiteSpace(theString))
                return 0;

            var conversionSucceeded = double.TryParse(theString, out var parseResult);

            return conversionSucceeded ? parseResult : 0;
        }

        public static bool TryConvertToDouble(string inputString, out double doubleEquivalent, out string conversionReport)
        {
            const double defaultDoubleResult = 0; // max = 9,223,372,036,854,775,807 i.e. nine thousand trillion

            try
            {
                if (string.IsNullOrWhiteSpace(inputString)) //check for empty or whitespace XML text first of all
                {
                    doubleEquivalent = defaultDoubleResult;
                    conversionReport = "The value is missing or blank.";
                    return false;
                }

                doubleEquivalent = Convert.ToDouble(inputString.Trim());

                if (Math.Abs(doubleEquivalent) < double.PositiveInfinity)
                {
                    conversionReport = "Success.";
                    return true;
                }
                else
                {
                    conversionReport =
                        $"The value <{inputString}> is lengthier than the maximum value permitted for a number of type double.";
                    doubleEquivalent = defaultDoubleResult;
                    return false;

                }
            }
            catch (FormatException)
            {
                conversionReport =
                    $"The value <{inputString}> is not in a format recognizable as a type of double.  Format exception.";
                doubleEquivalent = defaultDoubleResult;
                return false;
            }
        }

        public static bool TryConvertToInt64(string inputString, out long integerEquivalent, out string conversionReport)
        {
            const long defaultIntegerResult = 0; // max = 9,223,372,036,854,775,807 i.e. nine thousand trillion

            try
            {
                if (string.IsNullOrWhiteSpace(inputString)) //check for empty or whitespace XML text first of all
                {
                    integerEquivalent = defaultIntegerResult;
                    conversionReport = "The value is missing or blank.";
                    return false;
                }

                integerEquivalent = Convert.ToInt64(inputString.Trim());
                conversionReport = "Success.";
                return true;
            }
            catch (OverflowException)
            {
                integerEquivalent = defaultIntegerResult;
                conversionReport =
                    $"Overflow exception. The value <{inputString}> is outside the range of this type of integer. The maximum is 9,223,372,036,854,775,807 i.e. nine thousand trillion.";
                return false;
            }
            catch (FormatException)
            {
                conversionReport =
                    $"The value <{inputString}> is not in a format recognizable as a type of integer.  Format exception.";
                integerEquivalent = defaultIntegerResult;
                return false;
            }
        }

        public static bool TryCleanAndConvertToInt32(string inputString, out int integerEquivalent, out string conversionReport)
        {

            string cleanDigitsAsText = Regex.Replace(inputString, "[^0-9]", "");

            var isSuccess = TryConvertToInt32(cleanDigitsAsText, out integerEquivalent, out conversionReport);

            return isSuccess;

            // note. this is what we would do to rinse digits out of a string
            // string cleanAlpha = Regex.Replace(thestring, "[^a-zA-Z]", ""); // neat example


        }

        public static bool TryConvertToInt32(string inputString, out int integerEquivalent, out string conversionReport)
        {
            const int defaultIntegerResult = 0; // max = 2,147,483,647 i.e. two billion

            try
            {
                if (string.IsNullOrWhiteSpace(inputString))
                {
                    integerEquivalent = defaultIntegerResult;
                    conversionReport = "The value is missing or blank.";
                    return false;
                }

                integerEquivalent = Convert.ToInt32(inputString.Trim());
                conversionReport = "Success.";
                return true;
            }
            catch (OverflowException)
            {
                integerEquivalent = defaultIntegerResult;
                conversionReport =
                    $"The value <{inputString}> is outside the permitted range of an integer. The maximum is 2,147,483,647 i.e. two billion. Overflow exception.";
                return false;
            }
            catch (FormatException)
            {
                conversionReport =
                    $"The value <{inputString}> is not in a format recognizable as an integer.  Format exception.";
                integerEquivalent = defaultIntegerResult;
                return false;
            }
        }

        public static int ToAgeFromYearOfBirth(int yearOfBirth)
        {
            var now = DateTime.Now;

            var age = now.Year - yearOfBirth;

            return age;

        }

    }
}