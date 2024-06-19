using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Jgh.Uwp.Common.July2018.Converters
{
    public static class FormStatus
    {
        public static readonly int Incomplete = 0;
        public static readonly int Invalid = 1;
        public static readonly int Complete = 2;
    }

    public sealed class TextToHeaderVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is string ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Value converter that retrieves the first error of the collection, or null if empty.
    /// </summary>
    public sealed class FirstErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var errors = value as ICollection<string>;
            return errors is not null && errors.Count > 0 ? errors.ElementAt(0) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Value converter that translates FormStatus.Complete to ValidFormStatus{commandParameter}Style
    ///     and the others to InvalidFormStatus{commandParameter}Style.
    /// </summary>
    public sealed class FormStatusToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var styleKey = value is int && (int) value == FormStatus.Complete
                ? string.Format(CultureInfo.CurrentCulture, "ValidFormStatus{0}Style", parameter)
                : string.Format(CultureInfo.CurrentCulture, "InvalidFormStatus{0}Style", parameter);

            Application.Current.Resources.TryGetValue(styleKey, out var style);

            return (Style) style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Value converter that translates FormStatus.Complete or FormStatus.Invalid to <see cref="Visibility.Visible" />
    ///     and FormStatus.Incomplete to <see cref="Visibility.Collapsed" />.
    /// </summary>
    public sealed class FormStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is int && (int) value == FormStatus.Incomplete ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Value converter that translates a boolean value to an invalid sign-in message.
    /// </summary>
    public sealed class IsSignInInvalidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool && (bool) value
                ? "Invalid username or password"
                : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}