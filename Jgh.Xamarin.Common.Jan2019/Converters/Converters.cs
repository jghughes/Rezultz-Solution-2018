using System;
using System.Globalization;
using NetStd.Goodies.Mar2022;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Converters
{
    #region Debug converters

    public sealed class DebugBreakpointValueConverter : IValueConverter
    {
        /// No-Operation converter for ad hoc use in debug mode to locate the source object 
        /// and contents of a problematic binding, using a breakpoint at the problem binding. 
        /// Hover over the “value” parameter to see what is being passed from the source to the target.

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is string myString)
		        return myString;

            return "debug binding is null"; // NB. ensure the breakpoint is here

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is string myString)
		        return myString;

	        return "debug binding is null";
        }

    }

    public sealed class DebugStringNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? "source string being debugged is null" : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? string.Empty;
        }
    }

    #endregion
    
    #region Text converters

    public sealed class NullTextToWhiteSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is string myString)
		        return myString;

	        return "";
        }
    }

    public sealed class TextGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return "String binding is null";

            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string myString)
                return myString;

            return "";
        }
    }

    public sealed class StaticTextGuardAgainstNullConverter
    {
        public static string Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public static object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is string myString)
		        return myString;

	        return "";
        }
    }

    public sealed class TextDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? null : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is string myString)
		        return myString;

	        return "";
        }
    }

    public sealed class TextItemsGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string[])
                return value;

            return new[] {"Text items binding is null"};
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? new[] {string.Empty};
        }
    }

    public sealed class TextPadRight6Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    const string padding = "      ";

		    if (value is not string myString) return padding;

		    if (string.IsNullOrWhiteSpace(myString)) return padding;

		    return myString + padding;
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is string myString)
			    return myString.Trim();

		    return "";
	    }
    }

    public sealed class TextToTextMin1Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return " ";

		    return JghString.RightAlign(myString,1, ' ');
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is string myString)
			    return myString.Trim();

		    return "";
	    }
    }

    public sealed class TextToTextMin2Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return " ";

		    return JghString.RightAlign(myString,2,' ');
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is string myString)
			    return myString.Trim();

		    return "";
	    }
    }

    public sealed class TextToTextMin3Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return "   ";

		    return JghString.RightAlign(myString,3,  ' ');
	    }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is string myString)
			    return myString.Trim();

		    return "";
	    }
    }

    public sealed class TextToTextMin5Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return "     ";

		    return JghString.RightAlign(myString, 5,' ');
	    }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is string myString)
			    return myString.Trim();

		    return "";
	    }
    }

    public sealed class TextToTextMin7Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return "       ";

		    return JghString.RightAlign(myString, 7, ' ');
	    }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    //if (value is null) return "";

		    if (value is not int) return "number not integer";

		    return ((int)value).ToString();
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return 0;
            
		    var isInt = JghConvert.TryConvertToInt32(myString, out var answer, out _);

		    return isInt ? answer : 0;
	    }
    }

    public sealed class IntegerToTextMin3Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not int myInt) return "number not integer";

		    return JghString.ToStringMin3(myInt);
	    }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return 0;

		    var isInt = JghConvert.TryConvertToInt32(myString, out var answer, out _);

		    return isInt ? answer : 0;
	    }
    }

    public sealed class DecimalToTextMin7Converter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not decimal myDecimal) return "number not decimal";

		    return JghString.ToStringMin7(myDecimal);
	    }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is not string myString) return 0;

		    var isInt = JghConvert.TryConvertToInt32(myString, out var answer, out _);

		    return isInt ? answer : 0;
	    }
    }


    #endregion

    #region DateTime converters

    public sealed class DateTimeToTextConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is null) return "null value";

		    if (value is not DateTime myDateTime) return "wrong value";

		    return myDateTime.ToLocalTime().ToString("HH:mm:ss");
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    // todo this might be an editable field where the field is displayed and edited in DateTime format
		    throw new ArgumentException("DateTimeToTextConverter.ConvertBack not implemented");
	    }
    }
    public sealed class DateTimeBinaryToLocalTimeTextConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is null) return "null value";

		    if (value is not long dtAsBinary) return "wrong value";

		    return JghDateTime.ToTimeLocalhhmmss(dtAsBinary);
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    // todo this might be an editable field where the field is displayed and edited in DateTime format
		    throw new ArgumentException("DateTimeBinaryToLocalTimeTextConverter.ConvertBack not implemented");
	    }
    }

    #endregion

    #region Visibility converters

    //public sealed class XamlBooleanToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is bool && (bool) value ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is Visibility && (Visibility) value == Visibility.Visible;
    //    }
    //}

    //public sealed class XamlStaticBooleanToVisibilityConverter
    //{
    //    public static Visibility Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is bool && (bool) value ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public static bool ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is Visibility && (Visibility) value == Visibility.Visible;
    //    }
    //}

    //public sealed class XamlTextToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        string myString = null;

    //        if (value is string s)
    //            myString = s;

    //        return string.IsNullOrWhiteSpace(myString) ? Visibility.Collapsed : Visibility.Visible;
    //        //if (value is string)
    //        //    return Visibility.Visible;

    //        //return Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new ArgumentException("TextToVisibilityConverter.ConvertBack not implemented");
    //    }
    //}

    //public sealed class XamlBooleanNegationToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is bool && !(bool) value ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new ArgumentException("BooleanNegationToVisibilityConverter.ConvertBack not implemented");
    //        //return value is Visibility && (Visibility) value != Visibility.Visible;
    //    }
    //}

    //public sealed class XamlNullToVisibleConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is null ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new ArgumentException("NullToVisibleConverter.ConvertBack not implemented");

    //        //return value as Visibility? != Visibility.Visible;
    //    }
    //}

    //public sealed class XamlNotNullToVisibleConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value is not null ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new ArgumentException("NotNullToVisibleConverter.ConvertBack not implemented");

    //        //return value is Visibility && (Visibility) value == Visibility.Visible;
    //    }
    //}

    //public sealed class XamlInvertVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (targetType != typeof(Visibility))
    //            throw new InvalidOperationException("Converter can only convert to value of type Visibility.");

    //        var vis = (Visibility) value;

    //        return vis == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new ArgumentException("InvertVisibilityConverter.ConvertBack not implemented");
    //    }
    //}

    #endregion

    #region Bool converters

    public sealed class TrueToPlusSignConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if ((bool) value)
			    return "+";

		    return " ";
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    throw new ArgumentException("NullToFalseConverter.ConvertBack not implemented");
	    }
    }

    public sealed class NullToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ArgumentException("NullToFalseConverter.ConvertBack not implemented");
        }
    }

    public sealed class AnyToTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is object[] items && items.Length != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ArgumentException("NullToFalseConverter.AnyToTrueConverter not implemented");
        }
    }

    #endregion

}