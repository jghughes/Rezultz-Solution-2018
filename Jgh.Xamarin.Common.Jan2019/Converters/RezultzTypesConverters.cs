using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NetStd.DataTypes.Mar2024;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Converters
{
    #region Rezultz model converters

    #region Search model null converters

    public sealed class DoNotGuardAgainstNullSearchQuerySuggestionItemConverter : IValueConverter
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

    public sealed class SearchQuerySuggestionItemNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is SearchQueryItem
                ? value
                : new SearchQueryItem
                {
                    TagAsInt = 0,
                    SearchQueryAsString = "SearchBoxSuggestionItem object binding is null"
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public sealed class SearchQuerySuggestionItemsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        return value is SearchQueryItem[]
		        ? value
                : new[]
                {
                    new SearchQueryItem
                    {
                        TagAsInt = 0,
                        SearchQueryAsString = "AllSearchQueryItems object binding is null"
                    }
                };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    #endregion

    #region Rezultz model null converters

    // Note If there is an error in any conversion, do not throw an exception. Instead, return DependencyProperty.UnsetValue, which will stop the data transfer.
    public sealed class ParticularsItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CboLookupItem)
                return value;

            return new CboLookupItem
            {
                Label = "CboLookupItem object binding is null",
                EnumString = string.Empty
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class ParticularsItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<CboLookupItem>)
                return value;

            var answer = new[]
            {
                new CboLookupItem
                {
                    Label = "ParticularsItems object binding is null",
                    EnumString = "ParticularsItems object binding is null",
                    Title = "ParticularsItems object binding is null"
                }
            };

            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class SeriesItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? new SeriesProfileItem
            {
                Label = "SeriesItem object binding is null",
                Title = "SeriesItem object binding is null",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class SeriesItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new[]
                   {
                       new SeriesProfileItem
                       {
                           Label = "SeriesItems object binding is null",
                           Title = "SeriesItems object binding is null"
                       }
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class EventItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new EventProfileItem
                   {
                       Label = "EventItem object binding is null",
                       Title = "EventItem object binding is null",
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class EventItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new[]
                   {
                       new EventProfileItem
                       {
                           Label = "EventProfiles object binding is null",
                           Title = "EventProfiles object binding is null",
                       }
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class RaceDefinitionItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new RaceSpecificationItem
                   {
                       Label = "RaceDefinitionItem object binding is null",
                       //Title = "RaceDefinitionItem object binding is null",
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class RaceDefinitionItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new[]
                   {
                       new RaceSpecificationItem
                       {
                           Label = "RaceDefinitionItems object binding is null",
                           //Title = "RaceDefinitionItems object binding is null",
                       }
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class ResultItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is IEnumerable<ResultItem>)
		        return value;

	        var answer = new[] { 
		        new ResultItem() 
		        { 
			        Bib = "object binding source is either null or not of type IEnumerable<Result>", 
			        Label = "object binding source is either null or not of type IEnumerable<Result>"
		        } };

	        Debug.WriteLine($"conversion={answer} after ResultItemsNullConverter.Convert() where value={value}");

            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class ResultItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is ResultItem)
		        return value;
            
            var answer = new ResultItem
                         {
                             Label = "Result object binding is null",
                         };


            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class ResultItemDoNotGuardAgainstNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is not ResultItem ? null : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public sealed class CohortAnalysisLineItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new List<PopulationCohortItem>
                   {
                       new()
                       {
                           NameOfCohort = "CohortAnalysisLineItems object binding is null"
                       }
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class CohortAnalysisLineItemNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ??
                   new PopulationCohortItem
                   {
                       Label = "PopulationCohortItem object binding is null"
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class ImageItemsNullConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        if (value is IEnumerable<UriItem>)
		        return value;

	        var answer = new List<UriItem> { new()
            {
                //Label = "object binding source is either null or not of type IEnumerable<UriItem>"
            } };

			return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public sealed class ClockItemsNullConverter : IValueConverter
    {
	    #region IValueConverter Members

	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is IEnumerable<TimeStampHubItem>)
			    return value;

		    var answer = new List<TimeStampHubItem>
		           {
			           new()
		           };

		    Debug.WriteLine($"conversion={answer} after ClockItemsNullConverter.Convert() where value={value}");

            return answer;
	    }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    return value;
	    }

	    #endregion
    }

    public sealed class ClockItemNullConverter : IValueConverter
    {
	    #region IValueConverter Members

	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    if (value is TimeStampHubItem)
			    return value;

		    var answer = new TimeStampHubItem();

		    return answer;
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    return value;
	    }

	    #endregion
    }

    public sealed class ClockHubItemDoNotGuardAgainstNullConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
            return value is not TimeStampHubItem ? null : value;
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    return value;
	    }
    }

    public sealed class ClockHubItemsDoNotGuardAgainstNullConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
            return value is not List<TimeStampHubItem> ? null : value;
	    }

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    return value;
	    }
    }

    #endregion

	#region Rezultz model projections

	//public sealed class ResultItemsToPrettyPrintedFavoriteStringsConverter : IValueConverter
	//{
	//	#region IValueConverter Members

	//	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	//	{
	//		if (value is not IEnumerable<ResultItem> results)
	//			return new List<ResultItem>
	//			{
	//				new()
 //                   {
	//					FirstName = "object binding source is either null or not of type IEnumerable<Result>",
	//					LastName = "object binding source is either null or not of type IEnumerable<Result>",
	//					Bib = "object binding source is either null or not of type IEnumerable<Result>"
	//				}
	//			};

	//		var answer = results.Where(z => z != null).Select(MakePrettyPrintedFavoriteString).ToArray();

	//		return answer;
	//	}

	//	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	//	{
	//		return value;
	//	}

	//	#endregion

	//	#region IValueConverter Members

	//	public string MakePrettyPrintedFavoriteString(ResultItem resultItem)
	//	{
	//		if (resultItem?.DerivedData == null ) return string.Empty;

	//		var myString = resultItem.DerivedData.PlaceCalculatedOverallInt.ToString();

	//		if (string.IsNullOrWhiteSpace(myString)) return "     ";

	//		if (myString.Length == 1) myString = "    " + myString;
	//		if (myString.Length == 2) myString = "   " + myString;
	//		if (myString.Length == 3) myString = "  " + myString;
	//		if (myString.Length == 4) myString = " " + myString;

	//		var answer = JghString.ConcatAsSentences(
	//		 myString,
	//			resultItem.FullName,
	//			resultItem.RaceGroup,
	//			resultItem.AgeGroup,
	//			resultItem.Bib);

	//		return answer;
	//	}
	//	#endregion

	//}

	#endregion

	#endregion
}