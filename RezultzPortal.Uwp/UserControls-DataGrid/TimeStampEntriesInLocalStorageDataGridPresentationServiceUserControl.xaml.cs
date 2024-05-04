using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.DataGridInterfaces;

namespace RezultzPortal.Uwp.UserControls_DataGrid
{
    public sealed partial class TimeStampEntriesInLocalStorageDataGridPresentationServiceUserControl : ITimeStampEntriesInLocalStorageDataGridPresentationService
    {
        private object _previouslyTappedItem;

        public TimeStampEntriesInLocalStorageDataGridPresentationServiceUserControl()
        {
            InitializeComponent();
        }

        public async Task<bool> GenerateTableColumnCollectionManuallyAsync(IEnumerable<ColumnSpecificationItem> columnSpecificationItems)
        {
            // assigning columns to whatever datagrid that uses as its datacontext something like RezultzSingleEventLeaderboardPageVm.DataGridOfLeaderboardVm. otherwise empty

            TelerikRadDataGridUserControlHelpers.ManuallyGenerateDataGridColumns(columnSpecificationItems,
                ThisTelerikRadDataGridUserControl);

            return await Task.FromResult(true);
        }

        private void ThisTelerikRadDataGridUserControl_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ThisTelerikRadDataGridUserControl.SelectedItem == _previouslyTappedItem)
                // the firing of this event precedes the invocation of the EventToCommandBehavior.
                // setting this to null de-colorizes the lineItem upon a repeat-tap, which is what a user
                // would naturally expect. it also has the couldn't-dream-of-more-perfect side-effect
                // of neutralising the OnSelectionChangedCommand chain of events in the vm

                ThisTelerikRadDataGridUserControl.SelectedItem = null;

            _previouslyTappedItem = ThisTelerikRadDataGridUserControl.SelectedItem;
        }
    }
}
