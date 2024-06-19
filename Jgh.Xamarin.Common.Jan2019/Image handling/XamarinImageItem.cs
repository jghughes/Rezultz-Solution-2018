using System;
using System.IO;
using System.Text;
using NetStd.DataTypes.Mar2024;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019.Image_handling
{

    // mickey mouse class to attach an ImageSource to the UriItem that we can bind to as an Image.Source in a datatemplate for an xaml Image
    public class XamarinImageItem
    {
        #region ctors

        public XamarinImageItem()
        {
            UriItem = new UriItem();

            ImageSource = MapByteArrayToImageSource([]); //i.e. non null
        }

        public XamarinImageItem(UriItem uriItem)
        {
            UriItem = uriItem;

            ImageSource = MapByteArrayToImageSource([]); //i.e. non null
        }

        #endregion

        #region props

        public UriItem UriItem { get; set; } // plain vanilla

        public ImageSource ImageSource { get; set; } // Xamarin.Forms

        #endregion

        #region helpers

        /// <summary>
        ///     Converts UriItem.ImageAsBytes into an ImageSource to instantiate the ImageSource prop of this class.
        ///     Intended and designed for use in a matching xaml Converter, not in this class, and even there it
        ///     is optional or even discouraged or redundant as you will see
        /// </summary>
        /// <returns></returns>
        //public void InstantiateImageSourceProperty()
        //{
        //    var fallback = MapByteArrayToImageSource(new byte[0]);

        //    try
        //    {
        //        if (UriItem?.ImageAsBytes is null) ImageSource = fallback;

        //        var answer = MapByteArrayToImageSource(UriItem?.ImageAsBytes);

        //        ImageSource = answer;
        //    }

        //    #region trycatch

        //    catch (Exception)
        //    {
        //        ImageSource = fallback;
        //    }

        //    #endregion
        //}

        /// <summary>
        ///     this static method could come in handy one day
        /// </summary>
        /// <param name="imageAsBytes"></param>
        /// <returns></returns>
        public static ImageSource MapByteArrayToImageSource(byte[] imageAsBytes)
        {
            if (imageAsBytes is null)
                return null;

            try
            {
                // it HAS to be converted, otherwise it doesn't work!
                var bytesAsString = Encoding.UTF8.GetString(imageAsBytes);

                var answer = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(bytesAsString)));

                return answer;
            }

            #region trycatch

            catch (Exception)
            {
                return null;
            }

            #endregion
        }

        #endregion
    }
}