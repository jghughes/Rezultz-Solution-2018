using System;
using System.Collections.Generic;
using System.Globalization;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Converters
{
    public sealed class ItemCountToContentViewHeightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
	        // in Xaml, the parameter must be set to the same as the row height specified or implicit for the ListView, or maybe a pixel or two larger perhaps. 

	        var desiredRowHeight = 40.0; // arbitrary default. 40 should be ample for typical fonts if parameter conversion fails 

            if (value is null)
                return desiredRowHeight;

            var conversionSucceeded = JghConvert.TryConvertToInt32((string)parameter, out var integerEquivalent, out _);

            if (conversionSucceeded)
	            desiredRowHeight = integerEquivalent;
            
            if (value is not IList<ResultItem> items || items.Count == 0) return 0;

            return items.Count * desiredRowHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}