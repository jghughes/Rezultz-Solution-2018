using System;
using System.Linq;
using System.Text;

namespace NetStd.Exceptions.Mar2024.Helpers
{
    internal class StringHelpers
    {
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
        internal static string Concat(params string[] arrayOfTexts)
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
        internal static string ConcatWithSeparator(string separator, params string[] arrayOfTexts)
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
        internal static string ConcatAsSentences(params string[] arrayOfTexts)
        {
            var answer = ConcatWithSeparator(" ", arrayOfTexts);

            return answer;
        }

        /// <summary>
        ///     Concatenates a variable length array of texts with each piece of text on a new line.
        ///     Skips texts that are null or whitespace.
        /// </summary>
        /// <returns>lines of text</returns>
        internal static string ConcatAsLines(params string[] arrayOfTexts)
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
        ///     Concatenates a variable length array of texts with a line-space between each piece of text.
        ///     Skips texts that are null or whitespace. The first parameter in the array will be
        ///     the top paragraph. The last parameter will be the bottom paragraph.
        /// </summary>
        /// <returns>paragraphs of text</returns>
        internal static string ConcatAsParagraphs(params string[] arrayOfTexts)
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

        private static string[] MakeACleanCollectionOfTextsExcludingTheLastItem(string[] arrayOfTexts, out string lastItem)
        {
            if (arrayOfTexts is null)
                throw new ArgumentNullException(nameof(arrayOfTexts));

            lastItem = null;

            // rinse out all the empties 
            var listOfTexts = arrayOfTexts
                .Where(z => !string.IsNullOrEmpty(z))
                .ToList();

            if (!listOfTexts.Any())
                return [];

            // temporarily remove the last on the list because we need this to be disaggregated
            lastItem = listOfTexts.Last();

            listOfTexts.RemoveAt(listOfTexts.Count - 1); //i.e.the last one

            return listOfTexts.ToArray();
        }

    }
}
