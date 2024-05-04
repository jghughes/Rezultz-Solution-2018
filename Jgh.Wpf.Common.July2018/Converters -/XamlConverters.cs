using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NetStd.Objects03.Oct2022;
using NetStd.Objects03.Oct2022.RezultzItems;
using NetStd.Objects03.Oct2022.SeasonDataItems;
using NetStd.ViewModels01.April2022.UserControls;

namespace Jgh.Wpf.Common.July2018.Converters__
{

    #region Debug converters

    public class XamlDebugBreakpointValueConverter : IValueConverter
    {
        /// No-Operation converter for ad hoc use in debug mode to locate the source object 
        /// and contents of a problematic binding, using a breakpoint at the problem binding. 
        /// Hover over the “value” parameter to see what is being passed from the source to the target.
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // NB. ensure the breakpoint is here
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(
                "This ConvertBack method in the Debugging Converter has not been implemented and should never have been called");
        }

    }

    public class XamlDebugStringNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? "source string being debugged is null": strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? string.Empty;
        }
    }

    #endregion

    #region Text converters

    public class XamlNullTextToWhiteSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = value as string;

            return returnValue ?? string.Empty;
        }
    }
    public class XamlTextGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "String binding is null";

            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? string.Empty : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = value as string;

            return returnValue ?? string.Empty;
        }
    }

    public class XamlTextDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strValue = value as string;

            return string.IsNullOrWhiteSpace(strValue) ? null : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var returnValue = value as string;

            return returnValue ?? string.Empty;
        }
    }

    #endregion

    #region Visibility converters

    public class XamlBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool and true ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility and Visibility.Visible;
        }
    }

    public class XamlBooleanToReverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool and true ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility and Visibility.Visible;
        }
    }

    #endregion

    #region Search converters

    public class XamlDoNotGuardAgainstNullSearchQuerySuggestionItemConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not SearchQueryItem ? null : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class XamlSearchQuerySuggestionItemNullConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is SearchQueryItem ? value : new SearchQueryItem { TagAsInt = 0, SearchQueryAsString = "SearchBoxSuggestionItem binding is null" };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class XamlSearchQuerySuggestionItemsNullConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is SearchQueryItem[] ? value : new[] { new SearchQueryItem { TagAsInt = 0, SearchQueryAsString = "AllSearchQueryItems binding is null" } };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    #endregion

    #region Rezultz model converters

    public class XamlParticularsItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CboLookupItem)
                return value;

            return new CboLookupItem { Label = "CboLookupItem binding is null", EnumString = string.Empty };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlParticularsItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CboLookupItem[])
                return value;

            return new[]
            {
                new CboLookupItem
                {
                    Label = "ParticularsItems binding is null",
                    EnumString = "ParticularsItems binding is null",
                    Title = "ParticularsItems binding is null"
                }
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlSeriesItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? new SeriesItem { Label = "SeriesItem binding is null", Title = "SeriesItem binding is null" };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlSeriesItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new[] { new SeriesItem { Label = "SeriesItems binding is null", Title = "SeriesItems binding is null" } };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlEventItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new EventItem { Label = "EventItem binding is null", Title = "EventItem binding is null" };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlEventItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new[] { new EventItem { Label = "EventItems binding is null", Title = "EventItems binding is null" } };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlRaceSpecificationItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new RaceSpecificationItem
                   {
                       Label = "RaceSpecificationItem binding is null",
                       
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlRaceSpecificationItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new[]
                   {
                       new RaceSpecificationItem
                       {
                           Label = "RaceSpecificationItems binding is null",
                           
                       }
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlResultItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new List<ResultItem>
                   {
                       new()
                       {
                           FirstName = "ResultItems binding is null",
                           LastName = "ResultItems binding is null",
                           Bib = "ResultItems binding is null"
                       }
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class XamlCohortAnalysisLineItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new List<PopulationCohortItem> { new() { NameOfCohort = "CohortAnalysisLineItems binding is null" } };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    #endregion

}
