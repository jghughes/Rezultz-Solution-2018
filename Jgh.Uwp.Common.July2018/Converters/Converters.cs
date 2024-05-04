using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using NetStd.Goodies.Mar2022;

namespace Jgh.Uwp.Common.July2018.Converters
{
    #region Debug converters

    public sealed class DebugBreakpointConverter : IValueConverter
    {
        /// No-Operation converter for ad hoc use in debug mode to locate the source object 
        /// and contents of a problematic binding, using a breakpoint at the problem binding. 
        /// Hover over the “value” parameter to see what is being passed from the source to the target.
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            return value; // NB. ensure the breakpoint is inserted here
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {

            var strValue = value as string;

            if (strValue == null)
            {
                throw new NotImplementedException(
                    "This ConvertBack method in the Debugging Converter has not been implemented and should never have been called");
            }

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;

        }
    }

    public sealed class DebugWriteLineConverter : IValueConverter
    {
        /// No-Operation converter for ad hoc use in debug mode to locate the source object 
        /// and contents of a problematic binding.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.WriteLine(
                $"Transiting DebugWriteLineConverter: Value={value} : TargetType={targetType} : Parameter={parameter} : Language={language}");

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException(
                "This ConvertBack method in the Debugging Converter has not been implemented and should never have been called");
        }
    }

    public sealed class DebugStringNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? "source string being debugged is null" : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value ?? string.Empty;
        }
    }

    #endregion

    #region Text converters

    public sealed class NullTextToWhiteSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString;

            return string.Empty;
        }
    }

    public sealed class TextGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "String binding is null";

            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString;

            return string.Empty;
        }
    }

    public sealed class StaticTextGuardAgainstNullConverter
    {
        public static string Convert(object value, Type targetType, object parameter, string language)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public static object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString;

            return string.Empty;
        }
    }

    public sealed class TextDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? null : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString;

            return string.Empty;
        }
    }

    public sealed class TextItemsGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string[])
                return value;

            return new[] { "Text items binding is null" };

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value ?? new[] { string.Empty };
        }
    }

    public sealed class ToUppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string output;

            if (value is string input)
            {
                output = input.ToUpperInvariant();
            }
            else
            {
                throw new ArgumentException("Value must be string.", nameof(value));
            }

            return output;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.ToUpperInvariant();

            return string.Empty;
        }
    }

    public sealed class TextPadRight6Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            const string padding = "      ";

            if (!(value is string myString)) return padding;

            if (string.IsNullOrWhiteSpace(myString)) return padding;

            return myString + padding;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.Trim();

            return "";
        }
    }

    public sealed class TextToTextMin1Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string myString)) return " ";

            return JghString.RightAlign(myString, 1, ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.Trim();

            return "";
        }
    }

    public sealed class TextToTextMin2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string myString)) return " ";

            return JghString.RightAlign(myString, 2, ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.Trim();

            return "";
        }
    }

    public sealed class TextToTextMin3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string myString)) return "   ";

            return JghString.RightAlign(myString, 3, ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.Trim();

            return "";
        }
    }

    public sealed class TextToTextMin5Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string myString)) return "     ";

            return JghString.RightAlign(myString, 5, ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.Trim();

            return "";
        }
    }

    public sealed class TextToTextMin7Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is string myString)) return "       ";

            return JghString.RightAlign(myString, 7, ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.Trim();

            return "";
        }
    }

    public sealed class GuidToAbbreviatedGuidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            const string abbreviationSuffix = "...";

            if (value is string myString)
            {
                return JghString.Substring(0, 4, myString) + abbreviationSuffix;
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string myString)
                return myString.Trim();

            return "";
        }
    }

    #endregion

    #region Number converters

    public sealed class IntegerToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string languag)
        {
            //if (value == null) return "";

            if (!(value is int)) return "number not integer";

            return ((int)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string languag)
        {
            if (!(value is string myString)) return 0;

            var isInt = JghConvert.TryConvertToInt32(myString, out var answer, out _);

            return isInt ? answer : 0;
        }
    }

    public sealed class IntegerToTextMin3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string languag)
        {
            if (!(value is int myInt)) return "number not integer";

            return JghString.ToStringMin3(myInt);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string languag)
        {
            if (!(value is string myString)) return 0;

            var isInt = JghConvert.TryConvertToInt32(myString, out var answer, out _);

            return isInt ? answer : 0;
        }
    }

    public sealed class DecimalToTextMin7Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string languag)
        {
            if (!(value is decimal myDecimal)) return "number not decimal";

            return JghString.ToStringMin7(myDecimal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string languag)
        {
            if (!(value is string myString)) return 0;

            var isInt = JghConvert.TryConvertToInt32(myString, out var answer, out _);

            return isInt ? answer : 0;
        }
    }


    #endregion

    #region DateTime converters

    public sealed class DateTimeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "null value";

            if (!(value is DateTime myDateTime)) return "wrong value";

            return myDateTime.ToLocalTime().ToString("HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // todo this might be an editable field where the field is displayed and edited in DateTime format
            throw new ArgumentException("DateTimeToTextConverter.ConvertBack not implemented");
        }
    }

    public sealed class DateTimeBinaryToLocalTimeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "null value";

            if (!(value is long dtAsBinary)) return "wrong value";

            return JghDateTime.ToTimeLocalhhmmss(dtAsBinary);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // todo this might be an editable field where the field is displayed and edited in DateTime format
            throw new ArgumentException("DateTimeBinaryToLocalTimeTextConverter.ConvertBack not implemented");
        }
    }

    #endregion

    #region Visibility converters

    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool && (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility) value == Visibility.Visible;
        }
    }

    public sealed class StaticBooleanToVisibilityConverter
    {
        public static Visibility Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool && (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public static bool ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility) value == Visibility.Visible;
        }
    }

    public sealed class TextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string myString = null;

            if (value is string s)
                myString = s;

            return string.IsNullOrWhiteSpace(myString) ? Visibility.Collapsed : Visibility.Visible;
            //if (value is string)
            //    return Visibility.Visible;

            //return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new ArgumentException("TextToVisibilityConverter.ConvertBack not implemented");
        }
    }

    public sealed class BooleanNegationToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool && !(bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new ArgumentException("BooleanNegationToVisibilityConverter.ConvertBack not implemented");
            //return value is Visibility && (Visibility) value != Visibility.Visible;
        }
    }

    public sealed class NullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new ArgumentException("NullToVisibleConverter.ConvertBack not implemented");

            //return value as Visibility? != Visibility.Visible;
        }
    }

    public sealed class NotNullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new ArgumentException("NotNullToVisibleConverter.ConvertBack not implemented");

            //return value is Visibility && (Visibility) value == Visibility.Visible;
        }
    }

    public sealed class InvertVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("Converter can only convert to value of type Visibility.");

            var vis = (Visibility) value;

            return vis == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new ArgumentException("InvertVisibilityConverter.ConvertBack not implemented");
        }
    }

    #endregion

    #region Miscellaneous converters

    public sealed class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool answer = false;    //default

            if (value is bool b)
                answer = !b;

            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            bool answer = false;    //default

            if (value is bool b)
                answer = !b;

            return answer;
        }
    }

    public sealed class NullToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new ArgumentException("NullToFalseConverter.ConvertBack not implemented");
        }
    }

    public sealed class AnyToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is object[] items && items.Length != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new ArgumentException("NullToFalseConverter.AnyToTrueConverter not implemented");
        }
    }

    #endregion
}