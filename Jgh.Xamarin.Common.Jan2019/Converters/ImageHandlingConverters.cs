using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Jgh.Xamarin.Common.Jan2019.Image_handling;
using NetStd.DataTypes.Mar2024;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Converters
{
    public class XamlImageItemsToXamarinViewModelsConverter : IValueConverter
    {
        #region helper

        private static XamarinImageItem TranslateImageItemToPlatformType(UriItem uriItem)
        {
            if (uriItem is null) return new XamarinImageItem();

            var answer = new XamarinImageItem {UriItem = uriItem};

            // loads the ImageAsBytes prop into the ImageSource prop, granting the option to bind the
            // ImageSource prop to the Image.Source in our datatemplate. currently redundant. here for didactic purposes only.
            // our ImageRepository automatically populates the UwpImageItem.Source prop with the uri string to the image stored in Azure
            // and we bind to that
            //answer.InstantiateImageSourceProperty(); // todo - not working - hangs

            return answer;
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is not UriItem[] imageItems) return new XamarinImageItemViewModel[0];

                var answer = imageItems
                    .Where(z => z is not null)
                    .Select(z => new XamarinImageItemViewModel(TranslateImageItemToPlatformType(z))).ToArray();

                return answer;
            }
            catch (Exception)
            {
                return new XamarinImageItemViewModel[0];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class XamlImageItemToXamarinViewModelConverter : IValueConverter
    {
        #region helper

        private static XamarinImageItem TranslateImageItemToPlatformType(UriItem uriItem)
        {
            if (uriItem is null) return new XamarinImageItem();

            var answer = new XamarinImageItem(uriItem);

            // loads the ImageAsBytes prop into the ImageSource prop, granting the option to bind the
            // ImageSource prop to the Image.Source in our datatemplate. currently redundant. here for didactic purposes only.
            // our ImageRepository automatically populates the UwpImageItem.Source prop with the uri string to the image stored in Azure
            // and we bind to that
            //answer.InstantiateImageSourceProperty(); // todo - not working? - hangs? to be checked

            return answer;
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is not UriItem imageItem) return new XamarinImageItemViewModel();

                var vm = new XamarinImageItemViewModel(TranslateImageItemToPlatformType(imageItem));

                return vm;
            }
            catch (Exception)
            {
                return new XamarinImageItemViewModel();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class XamlBytesToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not byte[] imageAsBytes)
                return string.Empty;

            // it HAS to be converted, otherwise it doesn't work!
            var bytesAsString = System.Text.Encoding.UTF8.GetString(imageAsBytes);

            return ImageSource.FromStream(() => new MemoryStream(System.Convert.FromBase64String(bytesAsString)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class XamlUriStringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uriAsString = value as string;

            if (string.IsNullOrWhiteSpace(uriAsString))
                return ImageSource.FromUri(new Uri("https://dummy"));

            var answer = ImageSource.FromUri(new Uri(uriAsString));

            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


}
