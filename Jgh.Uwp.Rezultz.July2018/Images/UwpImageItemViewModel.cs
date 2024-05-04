using NetStd.Prism.July2018;

namespace Jgh.Uwp.Rezultz.July2018.Images
{
    // mickey mouse class to serve as a vm, with the UwpImageItem as its sole prop
// for the author of this trick, see laurent bugnion, Silverlight 4, Chap 7, p170
    public class UwpImageItemViewModel : BindableBase // NB bindable base
    {

        #region ctors

        public UwpImageItemViewModel()
        {
            // ensure non-null
            Model = new UwpImageItem();
        }

        public UwpImageItemViewModel(UwpImageItem uwpImageItem)
        {
            Model = uwpImageItem;
        }

        #endregion

        #region prop

        public UwpImageItem Model { get; private set; }

        #endregion

    }
}