using NetStd.Prism.July2018;

namespace Jgh.Xamarin.Common.Jan2019.Image_handling
{
    // mickey mouse class to serve as a vm, with the XamarinImageItem as its sole prop
    // for the author of this trick, see laurent bugnion, Silverlight 4, Chap 7, p170
    public class XamarinImageItemViewModel : BindableBase // NB bindable base
    {

        #region ctors

        public XamarinImageItemViewModel()
        {
            // ensure non-null
            Model = new XamarinImageItem();
        }

        public XamarinImageItemViewModel(XamarinImageItem uwpImageItem)
        {
            Model = uwpImageItem;
        }

        #endregion

        #region prop

        public XamarinImageItem Model { get; private set; }

        #endregion

    }
}