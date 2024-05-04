using System;

namespace RezultzSvc.Library01.Mar2024
{
    internal class StringHelpers
    {
        public static string Truncate(string inputString, int length)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return string.Empty;

            if (inputString.Length <= length)
                return inputString;

            var answer = inputString.AsSpan().Slice(0, length).ToString();

            return answer;

        }

        public static string Slice(string inputString, int start, int length)
        {
            if (string.IsNullOrWhiteSpace(inputString)) return string.Empty;

            if (inputString.Length <= length)
                return inputString;

            var answer = inputString.AsSpan().Slice(start, length).ToString();

            return answer;

        }

    }
}
