using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace NetStd.Goodies.Mar2022
{
	public static class JghString
	{
		#region replace

		public static string Replace(string text, char oldChar, char newChar)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var answer = text.Replace(oldChar, newChar);

            return answer;
        }

        public static string Replace(string text, string oldSubstring, string newSubstring)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (newSubstring == null)
                return text;

            var answer = text.Replace(oldSubstring, newSubstring);

            return answer;
        }

        public static string Replace(string text, Dictionary<string, string> substringMappingDictionary)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (substringMappingDictionary == null || substringMappingDictionary.Count == 0) return text;

            return substringMappingDictionary.Aggregate(text, (current, kvp) => Replace(current, kvp.Key, kvp.Value));
        }


        #endregion

        #region remove

        public static string Remove(char oldChar, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (oldChar == new char())
                return text;

            var charArray = text.ToCharArray();

            var sb = new StringBuilder();

            foreach (var c in charArray.Where(c => c != oldChar))
                sb.Append(c.ToString());

            var answer = sb.ToString();

            return answer;
        }


        #endregion

        #region equality

        public static bool AreEqualIgnoreOrdinalCase(string firstString, string secondString)
        {
            //return String.Equals(firstString.Trim(), secondString.Trim(), StringComparison.OrdinalIgnoreCase); don't do this!

            var xx = string.IsNullOrWhiteSpace(firstString) ? string.Empty : firstString.Trim();

            var yy = string.IsNullOrWhiteSpace(secondString) ? string.Empty : secondString.Trim();

            var areEqual = string.Equals(xx, yy, StringComparison.OrdinalIgnoreCase);

            return areEqual;
        }

        public static bool AreNotEqualIgnoreOrdinalCase(string firstString, string secondString)
        {
            return !AreEqualIgnoreOrdinalCase(firstString, secondString);
        }

        public static bool AreEqualAndNeitherIsNullOrWhiteSpace(string firstString, string secondString)
        {
            if (string.IsNullOrWhiteSpace(firstString) || string.IsNullOrWhiteSpace(secondString))
                return false;

            return string.Equals(firstString.Trim(), secondString.Trim(), StringComparison.Ordinal);
        }

        public static bool AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(string firstString, string secondString)
        {
            if (string.IsNullOrWhiteSpace(firstString) || string.IsNullOrWhiteSpace(secondString))
                return false;

            return string.Equals(firstString.Trim(), secondString.Trim(), StringComparison.OrdinalIgnoreCase);
        }

		#endregion

        #region contains

        public static bool JghContains(string fragment, string text, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(fragment) || string.IsNullOrEmpty(text))
                return true;

            return text.IndexOf(fragment, comparison) >= 0;
        }

        public static bool JghContains(string fragment, string text)
        {
            if (string.IsNullOrEmpty(fragment) || string.IsNullOrEmpty(text))
                return true;

            return text.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        public static bool JghStartsWith(string fragment, string text)
        {
            if (string.IsNullOrEmpty(fragment) || string.IsNullOrEmpty(text))
                return true;

            return text.StartsWith(fragment);
        }

        public static bool JghContainsEndingWithAnyOf(string inputString, params string[] suffixes)
        {
            return !string.IsNullOrWhiteSpace(inputString) && suffixes.Any(inputString.EndsWith);
        }


        #endregion

        #region concat

        /// <summary>
        ///     Concatenates a variable length array of texts..
        ///     Skips texts that are null or whitespace.
        /// </summary>
        /// <param name="arrayOfTexts">If any element in params array is null or empty, the element is skipped</param>
        /// <returns>concatenated strings</returns>
        /// <summary>
        ///     Concatenates a variable length array of texts..
        ///     Skips texts that are null or whitespace.
        /// </summary>
        /// <returns>concatenated texts</returns>
        public static string Concat(params string[] arrayOfTexts)
        {
            var answer = ConcatWithSeparator("", arrayOfTexts);

            return answer;
        }

		/// <summary>
		///     Concatenates a variable length array of texts with a separator between each piece of text.
		///     Skips texts that are null or whitespace.
		/// </summary>
		/// <param name="separator">If separator is null, an empty string (String.Empty) is used instead</param>
		/// <param name="arrayOfTexts">If any element in params array is null or empty, the element is skipped</param>
		/// <returns>concatenated strings</returns>
		/// <summary>
		///     Concatenates a variable length array of texts adding a separator in between each piece of text.
		///     Skips texts that are null or whitespace.
		/// </summary>
		/// <returns>concatenated texts</returns>
		public static string ConcatWithSeparator(string separator, params string[] arrayOfTexts)
		{
			separator ??= string.Empty;

			var sb = new StringBuilder();

			var collectionOfTexts =
				MakeACleanCollectionOfTextsExcludingTheLastItem(arrayOfTexts, out var lastItemOfText);

			foreach (var littleString in collectionOfTexts)
				sb.Append(littleString + separator);

			// Add back the last string without a trailing spacer
			if (!string.IsNullOrWhiteSpace(lastItemOfText))
				sb.Append(lastItemOfText);

            var answer = sb.ToString();

            return answer;
		}

        /// <summary>
        ///     Concatenates a variable length array of texts with a space between each piece of text.
        ///     Skips texts that are null or whitespace.
        /// </summary>
        /// <param name="arrayOfTexts">If any element in params array is null or empty, the element is skipped</param>
        /// <returns>concatenated strings</returns>
        /// <summary>
        ///     Concatenates a variable length array of texts adding a space in between each piece of text.
        ///     Skips texts that are null or whitespace.
        /// </summary>
        /// <returns>concatenated texts</returns>
        public static string ConcatAsSentences(params string[] arrayOfTexts)
		{
            var answer = ConcatWithSeparator(" ", arrayOfTexts);

            return answer;
        }

		/// <summary>
		///     Concatenates a variable length array of texts with each piece of text on a new line.
		///     Skips texts that are null or whitespace.
		/// </summary>
		/// <returns>lines of text</returns>
		public static string ConcatAsLines(params string[] arrayOfTexts)
		{
			var sb = new StringBuilder();

			var collectionOfTexts = MakeACleanCollectionOfTextsExcludingTheLastItem(arrayOfTexts, out var lastItemOfText);

			foreach (var pieceOfText in collectionOfTexts)
				sb.AppendLine(pieceOfText);

			// Add back the last string without a trailing spacer
			if (!string.IsNullOrWhiteSpace(lastItemOfText))
				sb.AppendLine(lastItemOfText);

            var answer = sb.ToString();

            return answer;
		}

        /// <summary>
        ///     Concatenates a variable length array of texts with a linespace between each piece of text.
        ///     Skips texts that are null or whitespace. The first parameter in the array will be
        ///     the top paragraph. The last parameter will be the bottom paragraph.
        /// </summary>
        /// <returns>paragraphs of text</returns>
        public static string ConcatAsParagraphs(params string[] arrayOfTexts)
		{
			var sb = new StringBuilder();

			var collectionOfTexts =
				MakeACleanCollectionOfTextsExcludingTheLastItem(arrayOfTexts, out var lastItemOfText);

			foreach (var item in collectionOfTexts)
			{
				sb.AppendLine(item);
				sb.AppendLine();
			}

			// Add back the last string without a trailing spacer
			if (!string.IsNullOrWhiteSpace(lastItemOfText))
				sb.AppendLine(lastItemOfText);

            var answer = sb.ToString();

			return answer;
		}

		#endregion

		#region validators


        public static bool IsOnlyDigits(string value)
        {
            var isValid = true;

            foreach (var c in value)
            {
                if (!char.IsDigit(c))
                    isValid = false;
            }

            return isValid;

        }

        public static bool IsOnlyLetters(string value)
        {
            var isValid = true;

            if (value == null)
            {
                isValid = false;
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
            }
            else
            {
                //process 1
                //isValid = Regex.IsMatch(value, @"^[a-zA-Z]+$");

                //process 2
                foreach (var c in value)
                {
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '-')
                        isValid = false;
                }

                //foreach (char c in value)
                //{
                //	if (!Char.IsLetter(c))
                //		isValid = false;
                //}

            }

            return isValid;
        }

        public static bool IsOnlyLettersOrDigits(string value)
        {

            var isValid = true;

            foreach (var c in value)
            {
                if (!Char.IsLetterOrDigit(c))
                    isValid = false;
            }

            return isValid;
        }

        public static bool IsOnlyLettersOrDigitsOrHyphen(string value)
        {

            var isValid = true;

            foreach (var c in value)
            {
                if (!(Char.IsLetterOrDigit(c) || c == '-'))
                    isValid = false;
            }

            return isValid;
        }
        public static bool IsOnlyLettersOrDigitsOrHyphenOrSpace(string value)
        {

            var isValid = true;

            foreach (var c in value)
            {
                if (!(Char.IsLetterOrDigit(c) || c == '-') || c == ' ')
                    isValid = false;
            }

            return isValid;
        }

        public static bool IsOnlyLettersOrHyphen(string value)
        {

            var isValid = true;

            foreach (var c in value)
            {
                if (!(Char.IsLetter(c) || c == '-'))
                    isValid = false;
            }

            return isValid;
        }

        public static bool IsOnlyLettersOrHyphenOrApostrophe(string value)
        {

            var isValid = true;

            foreach (var c in value)
            {
                if (!(Char.IsLetter(c) || c is '-' or '\''))
                    isValid = false;
            }

            return isValid;
        }
        public static bool IsOnlyLettersOrHyphenOrApostropheOrSpace(string value)
        {

            var isValid = true;

            foreach (var c in value)
            {
                if (!(Char.IsLetter(c) || c is '-' or '\'' or ' '))
                    isValid = false;
            }

            return isValid;
        }

        public static bool IsValidEmailAddress(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
                isValid = false;
            else
            {
                try
                {
                    _ = new MailAddress(value);
                }
                catch (FormatException)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        #endregion

        #region simple tools

        public static string TmLr(string inputString)
        {
            return inputString?.Trim().ToLower() ?? string.Empty;
        }

        public static string Substring(int startIndex, int length, string theString)
        {
            if (string.IsNullOrWhiteSpace(theString)) return string.Empty;

            return theString.Length >= length ? theString.Substring(startIndex, length) : theString;
        }

        public static string ToTrimmedString(string inputString)
        {
            return string.IsNullOrWhiteSpace(inputString) ? string.Empty : inputString.Trim();
        }

        public static string[] ToTrimmedStrings(string[] inputStrings)
        {
            if (inputStrings == null)
                return Array.Empty<string>();

            return inputStrings.Where(z => !string.IsNullOrWhiteSpace(z)).Select(x => x.Trim()).ToArray();
        }

        public static string[] ToTrimmedLowerCaseStrings(string[] inputStrings)
        {
            if (inputStrings == null)
                return Array.Empty<string>();

            return inputStrings.Where(z => !string.IsNullOrWhiteSpace(z)).Select(TmLr).ToArray();
        }


        public static string CleanAndConvertToLetterOrDigitOrHyphen(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return string.Empty;

            var inputStringAsChars = inputString.ToCharArray();
                
            StringBuilder answer = new();

            foreach (var thisChar in inputStringAsChars)
            {
                if (char.IsLetterOrDigit(thisChar) || thisChar == '-')
                {
                    answer.Append(thisChar);
                }
            }
            return answer.ToString();
        }

        public static string CleanAndConvertToLetterOrDigit(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return string.Empty;

            var inputStringAsChars = inputString.ToCharArray();

            StringBuilder answer = new();

            foreach (var thisChar in inputStringAsChars)
            {
                if (char.IsLetterOrDigit(thisChar))
                {
                    answer.Append(thisChar);
                }
            }
            return answer.ToString();
        }

        public static string Enclose(string inputString, char wrappingChar)
        {
            var answer = $"{wrappingChar}{inputString}{wrappingChar}";

            return answer;

        }

        public static string PadLeftOrBlankIfZero(int inputInteger, int totalWidth, char paddingChar)
        {
            var inputString = inputInteger == 0 ? "" : inputInteger.ToString();

            var answer = inputString.PadLeft(totalWidth, paddingChar);

            return answer;

        }

        public static string RightAlign(string inputString, int totalWidth)
        {
            inputString ??= string.Empty;

            var answer = inputString.PadLeft(totalWidth, ' ');

            return answer;

        }

        public static string RightAlign(string inputString, int totalWidth, char paddingChar)
        {
            inputString ??= string.Empty;

            var answer = inputString.PadLeft(totalWidth, paddingChar);

            return answer;

        }

        public static string LeftAlign(string inputString, int totalWidth)
        {
            inputString ??= string.Empty;

            var answer = inputString.PadRight(totalWidth, ' ');

            return answer;
        }

        public static string LeftAlign(string inputString, int totalWidth, char paddingChar)
        {
            inputString ??= string.Empty;

            var answer = inputString.PadRight(totalWidth, paddingChar);

            return answer;
        }

        public static string PadInBetween(string leftInputString, string rightInputString, int totalWidth, char paddingChar)
        {
            leftInputString ??= string.Empty;

            rightInputString ??= string.Empty;

            var gap = totalWidth - leftInputString.Length - rightInputString.Length;

            if (gap <= 0) return JghString.ConcatWithSeparator(" ", leftInputString, rightInputString);

            var sb = new StringBuilder();

            sb.Append(leftInputString);

            for (var i = 0; i < gap; i++)
            {
                sb.Append(paddingChar);
            }

            sb.Append(rightInputString);

            var answer = sb.ToString();

            return answer;
        }

        #endregion

		#region little reformatters and converters

        public static string ToStringMin2(int value)
        {
            var answer = value.ToString();

            if (answer.Length == 1) answer = "0" + answer;

            return answer;

        }

        public static string ToStringMin3(int value)
        {
            var answer = value.ToString();

            if (answer.Length == 1) answer = "00" + answer;
            if (answer.Length == 2) answer = "0" + answer;

            return answer;

        }
        
        public static string ToStringMin7(decimal value)
        {
            var answer = value.ToString(CultureInfo.InvariantCulture);

            if (answer.Length == 1) answer = "            " + answer;
            if (answer.Length == 2) answer = "          " + answer;
            if (answer.Length == 3) answer = "        " + answer;
            if (answer.Length == 4) answer = "      " + answer;
            if (answer.Length == 5) answer = "    " + answer;
            if (answer.Length == 6) answer = "  " + answer;

            return answer;

        }

        public static string ToStringOrBlankIfZero(int smallInt)
        {
            if (smallInt == 0) return string.Empty;

            return smallInt.ToString();
        }

        public static string ToStringOrBlankIfNearZero(double possiblySmallDouble, int roundingPlaces, string formatString)
        {
            const double epsilon = 0.05; // arbitrarily small

            if (Math.Abs(possiblySmallDouble) < Math.Abs(epsilon)) return string.Empty;

            return Math.Round(possiblySmallDouble, roundingPlaces).ToString(formatString);
        }

        public static string ToBracketedStringOrBlankIfNearZero(double possiblySmallDouble, int roundingPlaces, string formatString)
        {
            const double epsilon = 0.05; // arbitrarily small

            if (Math.Abs(possiblySmallDouble) < Math.Abs(epsilon)) return string.Empty;

            return $"({Math.Round(possiblySmallDouble, roundingPlaces).ToString(formatString)})";
        }

        public static string ToBestGuessStringOrBlankIfNearZero(string arbitraryString, int roundingPlaces, string decimalPlacesFormatSpecifier)
        {
            const double epsilon = 0.05; // arbitrarily small

            if (arbitraryString == null) return string.Empty;

            // we want to handle the valid possibilities that arbitraryString could be a timespan, a double, an int, or text.
            // what we do here is start with the most complex possibility and work down to the simplest possibility

            try
            {
                // it might be a valid timespan

                if (TimeSpan.TryParse(arbitraryString, out var validTimeSpan))
                {
                    var totalSeconds = validTimeSpan.TotalSeconds;

                    if (Math.Abs(totalSeconds) < Math.Abs(epsilon))
                        return string.Empty;

                    var totalSecondsRounded = Math.Round(totalSeconds, roundingPlaces);

                    var niceTimeSpan = TimeSpan.FromSeconds(totalSecondsRounded);

                    return niceTimeSpan.ToString(niceTimeSpan.Days > 0 ? JghTimeSpan.DdhhmmssfPattern : JghTimeSpan.HhmmssfPattern);
                }

                // it could be a double

                if (double.TryParse(arbitraryString, out var durationInSeconds))
                    return ToStringOrBlankIfNearZero(durationInSeconds, roundingPlaces, decimalPlacesFormatSpecifier);

                // it could be an integer

                if (int.TryParse(arbitraryString, out var bestGuessInt32))
                    return ToStringOrBlankIfZero(bestGuessInt32);

                //nope. it's a string

                return arbitraryString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string BooleanTrueToPOrBlank(bool value)
        {
            return value ? "p+" : "";
        }

        public static string BooleanTrueToSOrBlank(bool value)
        {
            return value ? "s+" : "";
        }

        public static string BooleanTrueToPushOrBlank(bool value)
        {
            return value ? "Push+" : "";
        }

        public static string BooleanTrueToSaveOrBlank(bool value)
        {
            return value ? "Save+" : "";
        }

        public static string BooleanTrueToDitchOrBlank(bool value)
        {
            return value ? "ditch" : "";
        }

        public static string BooleanTrueToDitchedOrBlank(bool value)
        {
            return value ? "ditched" : "";
        }

        public static string BooleanTrueToSeriesOrBlank(bool value)
        {
            return value ? "series" : "";
        }

        public static string BooleanTrueToSeriesOrOneOff(bool value)
        {
            return value ? "series" : "one-off";
        }

        public static string BooleanFalseToSingleOrBlank(bool value)
        {
            return value ? "" : "single";
        }

        public static string BooleanTrueToYesOrNo(bool value)
        {
            return value ? "yes" : "no";
        }

        public static string BooleanTrueToYesOrBlank(bool value)
        {
            return value ? "yes" : "";
        }

        public static string BoolToNoOrBlank(bool value)
        {
            return value ? "" : "no";
        }

        #endregion

        #region miscellaneous

        public static string ToTitleFormat(string inputString)
        {
            var trimmedString = ToTrimmedString(inputString);

            try
            {
                var lowerCaseCharacters = trimmedString.ToLower().ToCharArray();

                var upperCaseCharacters = trimmedString.ToUpper().ToCharArray();

                var charactersAsStrings = new string[trimmedString.Length];

                for (var i = 0; i < lowerCaseCharacters.Length; i++)
                {
                    var lcc = lowerCaseCharacters[i];
                    charactersAsStrings[i] = lcc.ToString();
                }

                var sb = new StringBuilder();

                for (var i = 0; i < charactersAsStrings.Count(); i++)
                {
                    if (i == 0 || charactersAsStrings[i - 1] == " " || charactersAsStrings[i - 1] == "-" ||
                        charactersAsStrings[i - 1] == "." ||
                        charactersAsStrings[i - 1] == "'")
                    {
                        sb.Append(upperCaseCharacters[i]);
                        continue;
                    }

                    sb.Append(lowerCaseCharacters[i]);
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                return trimmedString;
            }
        }

        public static string TakeFirstSentence(string longString)
        {
            if (string.IsNullOrWhiteSpace(longString)) return string.Empty;

            if (!longString.Contains(".")) return longString;

            var multipleSubstrings = longString.Split('.');

            var firstSubstring = multipleSubstrings[0];

            if (string.IsNullOrWhiteSpace(firstSubstring))
                return string.Empty;

            var answer = string.Concat(firstSubstring, ".");

            return answer;
        }

        public static string MakeWaitTimeMsg(DateTime startTimestamp)
        {
            var elapsedTimeMsg =
                $"Wait time was {(DateTime.Now - startTimestamp).TotalSeconds.ToString(JghFormatSpecifiers.DecimalFormat1Dp)} seconds.";
            return elapsedTimeMsg;
        }

        private static string[] MakeACleanCollectionOfTextsExcludingTheLastItem(string[] arrayOfTexts, out string lastItem)
		{
			if (arrayOfTexts == null)
				throw new ArgumentNullException(nameof(arrayOfTexts));

			lastItem = null;

            // rinse out all the empties 
            var listOfTexts = arrayOfTexts
                .Where(z => !string.IsNullOrEmpty(z))
                .ToList();
            //var listOfTexts = arrayOfTexts
            //    .Where(z => !string.IsNullOrWhiteSpace(z))
            //    .ToList();

            if (!listOfTexts.Any())
				return Array.Empty<string>();

			// temporarily remove the last on the list because we need this to be disaggregated
			lastItem = listOfTexts.Last();

			listOfTexts.RemoveAt(listOfTexts.Count - 1); //i.e.the last one

			return listOfTexts.ToArray();
		}

		public static string WrapTextInHtmlPreambleAsWebpage(string headTitle, string comment, string preformattedBodyText)
		{
			var sb = new StringBuilder();

			sb.AppendLine(@"<!DOCTYPE html>");
			sb.AppendLine($"{@" <!--"} {comment} {@"-->"}");
			sb.AppendLine(@"<html xmlns=""http://www.w3.org/TR/REC-html40"">");
			//sb.AppendLine(@"<html xmlns=""http://www.w3.org/1999/xhtml"">");
			sb.AppendLine(@"<head>");
			sb.AppendLine(@"<meta content=""IE=5.0000"" http-equiv=""X-UA-Compatible"">");
			sb.AppendLine($"{@" <title>"}{headTitle}{@"</title>"}");
			sb.AppendLine(@"<meta http-equiv=""Content-Type"" content=""text/html; charset=windows-1252"">");
			sb.AppendLine(@"<meta name=""GENERATOR"" content=""MSHTML 11.00.9600.17037"">");
			sb.AppendLine(@"</head>");
			sb.AppendLine(@"<body>");
			sb.AppendLine(@"<pre>");
			sb.AppendLine(preformattedBodyText);
			sb.AppendLine(@"</pre>");
			sb.AppendLine(@"</body>");
			sb.AppendLine(@"</html>");

			return sb.ToString();
		}

        /// <summary>
        ///     Strips out the file extension if any and substitutes an underscore for forbidden characters.
        ///     Only letters, digits, and underscores are permitted. Must be lowercase.
        /// </summary>
        /// <param name="inputString">account name</param>
        /// <returns>compliant string or "placeholder" if input string was empty or only conatined forbidden characters</returns>
        public static string MakeXamarinCompliantShortFileNameWithoutExtension(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return "placeholder";

            var answerCharsAsList = new List<char>();

            try
            {
                var inputchars = Path.GetFileNameWithoutExtension(inputString).ToLower().ToCharArray();

                foreach (var inputChar in inputchars)
                {
                    if (char.IsLetterOrDigit(inputChar))
                        answerCharsAsList.Add(inputChar);

                    answerCharsAsList.Add('_');
                }
            }
            catch (Exception)
            {
                return "placeholder";
            }

            return answerCharsAsList.ToString();
        }

		#endregion
    }
}