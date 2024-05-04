using System;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Behaviours
{
	/// <summary>
    /// highlights the color of the text in the entry to Red if the entry is wrong.
    /// Entry must be an integer. entry must be between 100 and 999.
    /// </summary>
    public class ValidationBehaviourMetadataId : ValidationBehavior
    {
	    private static class MetadataId
	    {
		    public const int LowerLimit = 100;
		    public const int UpperLimit = 999;
	    }
        internal override void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var isValid = IsSuperficiallyValidSettingsMetadataId(args.NewTextValue);

            ((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
        }

        // cut and paste from NetStd.Goodies.July2018.NavigationContextQueryStringHelper
        public static bool IsSuperficiallyValidSettingsMetadataId(string candidateMetadataIdAsString)
        {
            var numOfChars = candidateMetadataIdAsString.Length;

            if (numOfChars > 3 || numOfChars < 3)
                return false;

            var xx = TryConvertToInt32(candidateMetadataIdAsString, out var resultingInteger,
                out var _);

            if (xx == false) return false;

            var yy = SettingsMetadataIdIsWithinValidRange(resultingInteger);

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

        public static bool SettingsMetadataIdIsWithinValidRange(int candidateMetadataId)
        {
            return (candidateMetadataId >= MetadataId.LowerLimit && candidateMetadataId <= MetadataId.UpperLimit);
        }



    }
}
