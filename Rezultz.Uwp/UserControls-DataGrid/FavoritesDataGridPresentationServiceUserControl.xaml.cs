using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.DataGridInterfaces;

/* To see the magic of how this Control works ...
    is populated, go to FavoritesStylePageViewModelBase.MultiFactorFavoritesRefilterButtonOnClickAsync(). In there ...
    Step 1: instantiate and call DataGridDesignerForFavorites.GetNonEmptyColumnSpecificationItemsForFavoritesStyleFormat() to generate the desired array of columnSpecificationItems.
    Step 2: insert the columnSpecificationItems into this RadDataGrid by calling FavoritesDataGridPresentationServiceInstance.GenerateDataGridColumnCollectionManuallyAsync(columnSpecificationItems) right here.
    Step 3: Call FavoritesStylePageViewModelBase.DataGridOfFavoritesVm.PopulatePresenterAsync() to fill DataGridOfFavoritesVm.ItemsSource.
    if you look in PageContentsForFavoritesStylePageUserControl (that contains this control), you will see that FavoritesStylePageViewModelBase.DataGridOfFavoritesVm is specified as the datacontext for this control.
    This means that its ItemsSource property provides the row collection for the RadDataGrid.
*/

namespace Rezultz.Uwp.UserControls_DataGrid
{
    public sealed partial class FavoritesDataGridPresentationServiceUserControl : UserControl, IFavoritesDataGridPresentationService
    {
        private object _previouslyTappedItem;

        public FavoritesDataGridPresentationServiceUserControl()
        {
            this.InitializeComponent();
        }

        public async Task<bool> GenerateDataGridColumnCollectionManuallyAsync(IEnumerable<ColumnSpecificationItem> columnSpecificationItems)
        {
            TelerikRadDataGridUserControlHelpers.ManuallyGenerateDataGridColumns(columnSpecificationItems, XamlElementTelerikRadDataGridUserControl);

            return await Task.FromResult(true);
        }

        private void ThisRadDataGridUserControl_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (XamlElementTelerikRadDataGridUserControl.SelectedItem == _previouslyTappedItem)
            {
                // the firing of this event precedes the invocation of the EventToCommandBehavior.
                // setting this to null de-colorizes the lineItem upon a repeat-tap, which is what a user
                // would naturally expect. it also has the couldn't-dream-of-more-perfect side-effect
                // of neutralising the OnSelectionChangedCommand chain of events in the vm

                XamlElementTelerikRadDataGridUserControl.SelectedItem = null;
            }

            _previouslyTappedItem = XamlElementTelerikRadDataGridUserControl.SelectedItem;
        }

    }
}
