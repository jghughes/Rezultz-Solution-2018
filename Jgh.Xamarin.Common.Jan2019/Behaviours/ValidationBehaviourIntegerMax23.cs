using System;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Behaviours
{
	/// <summary>
	/// highlights the color of the text in the entry to Red if the entry is wrong (but doesn't explain why)
	/// Entry must be an integer between 0 and 23.
	/// </summary>
	public class ValidationBehaviourIntegerMax23 : ValidationBehavior
	{
        private static class Range
        {
            public const int LowerLimit = 0;
            public const int UpperLimit = 23;
        }

        internal override void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var isValid = IsWithinValidRange(args.NewTextValue);

            ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }

        // cut and paste from NetStd.Goodies.July2018.NavigationContextQueryStringHelper
        public static bool IsWithinValidRange(string candidateIntegerAsString)
        {
            var xx = TryConvertToInt32(candidateIntegerAsString, out var resultingInteger,
                out var _);

            if (xx == false) return false;

            var yy = IsWithinSpecifiedRange(resultingInteger);

            if (yy == false) return false;

            return true;
        }

        // cut and paste from project NetStd.StringHelpers.July2018.JghString

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

        public static bool IsWithinSpecifiedRange(int candidateMetadataId)
        {
            return (candidateMetadataId >= Range.LowerLimit && candidateMetadataId <= Range.UpperLimit);
        }

    }
}