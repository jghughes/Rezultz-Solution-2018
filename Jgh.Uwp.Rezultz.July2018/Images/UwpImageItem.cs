using Windows.UI.Xaml.Media.Imaging;
using NetStd.DataTypes.Mar2024;

namespace Jgh.Uwp.Rezultz.July2018.Images
{
    // mickey mouse class to attach an BitmapImage to the UriItem that we can bind to as an Image.Source in a data template for an xaml Image
    public class UwpImageItem
    {
        #region ctor

        public UwpImageItem()
        {
            UriItem = new UriItem();

            ImageSource = new BitmapImage();
        }

        public UwpImageItem(UriItem uriItem)
        {
            UriItem = uriItem;

            ImageSource = new BitmapImage();
        }

        #endregion

        #region props

        public UriItem UriItem { get; set; } // plain vanilla

        public BitmapImage ImageSource { get; set; } // Windows.UI.Xaml.Media.Imaging i.e. for UWP

        #endregion

    }
}