using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.DataGridInterfaces;

namespace RezultzPortal.Uwp.UserControls_DataGrid
{
    /// <summary>
    ///     To see the magic of how this Control is populated, go to
    ///     PortalParticipantAdminPagesViewModel.RefreshDisplayOfAllEntryCollectionVmsAsync(). In there ...
    ///     Step 1: create a grid designer and call
    ///     DataGridDesignerForItemsInLocalStorage.GetNonEmptyColumnSpecificationItemsForRawParticipantHubItemEntries() to generate the
    ///     desired array of columnSpecificationItems.
    ///     Step 2: insert the columnSpecificationItems into this here RadDataGrid by calling
    ///     ParticipantEntriesInLocalStorageDataGridPresentationService.GenerateDataGridColumnCollectionManuallyAsync(columnSpecificationItems)
    ///     right here.
    ///     Step 3: create a presenter and call DataGridOfItemsInLocalStorage.PopulatePresenterAsync() to fill the ItemsSource
    ///     property of the presenter.
    ///     If you look in RegisterParticipantsLaunchPage (that contains this control), you will see that
    ///     PortalParticipantAdminPagesVm.DataGridOfItemsInLocalStorage is specified in XAML as the datacontext for the Telerik
    ///     datagrid.
    ///     This means that its ItemsSource property provides the row collection for the RadDataGrid.
    /// </summary>
    public sealed partial class ParticipantEntriesInLocalStorageDataGridPresentationServiceUserControl : IParticipantEntriesInLocalStorageDataGridPresentationService
    {
        private object _previouslyTappedItem;

        public ParticipantEntriesInLocalStorageDataGridPresentationServiceUserControl()
        {
            InitializeComponent();
        }

        public async Task<bool> GenerateDataGridColumnCollectionManuallyAsync(IEnumerable<ColumnSpecificationItem> columnSpecificationItems)
        {
            TelerikRadDataGridUserControlHelpers.ManuallyGenerateDataGridColumns(columnSpecificationItems, XamlElementTelerikRadDataGridUserControl);

            return await Task.FromResult(true);
        }

        private void ThisTelerikRadDataGridUserControl_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (XamlElementTelerikRadDataGridUserControl.SelectedItem == _previouslyTappedItem)
                // the firing of this event precedes the invocation of the EventToCommandBehavior.
                // setting this to null de-colorizes the lineItem upon a repeat-tap, which is what a user
                // would naturally expect. it also has the couldn't-dream-of-more-perfect side-effect
                // of neutralising the OnSelectionChangedCommand chain of events in the vm

                XamlElementTelerikRadDataGridUserControl.SelectedItem = null;

            _previouslyTappedItem = XamlElementTelerikRadDataGridUserControl.SelectedItem;
        }
    }
}
