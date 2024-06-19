using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using NetStd.DataTypes.Mar2024;

namespace Jgh.Uwp.Rezultz.July2018.Images
{
    // Note If there is an error in any conversion, do not throw an exception. Instead, return DependencyProperty.UnsetValue, which will stop the data transfer.

    public class XamlImageItemToUwpViewModelConverter : IValueConverter
    {
        #region helper

        private static UwpImageItem TranslateImageItemToPlatformType(UriItem uriItem)
        {
            if (uriItem is null) return new UwpImageItem();

            var answer = new UwpImageItem(uriItem);

            // loads the ImageAsBytes prop into the ImageSource prop, granting the option to bind the
            // ImageSource prop to the Image.Source in our datatemplate. currently redundant. here for didactic purposes only.
            // our ImageRepository automatically populates the UwpImageItem.Source prop with the uri string to the image stored in Azure
            // and we bind to that
            //answer.InstantiateImageSourceProperty(); // todo - not working - hangs

            return answer;
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value is not UriItem imageItem) return new UwpImageItemViewModel();

                var vm = new UwpImageItemViewModel(TranslateImageItemToPlatformType(imageItem));

                return vm;
            }
            catch (Exception)
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
            //return value;
        }

        #endregion
    }
}