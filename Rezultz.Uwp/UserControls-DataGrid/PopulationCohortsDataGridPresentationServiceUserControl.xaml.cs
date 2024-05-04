using System.Collections.Generic;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.DataGridInterfaces;

/* To see the magic of how this Control is populated...
    go to PopulationCohortsPageViewModel.CboLookupKindOfCohortOnSelectionChangedAsync(). In there ...
    Step 1: instantiate and call PopulationCohortsDataGridDesigner.GetNonEmptyColumnSpecificationItemsForPopulationCohortItemDisplayObjects() to generate the desired array of columnSpecificationItems.
    Step 2: insert the columnSpecificationItems into this RadDataGrid by calling PopulationCohortsDataGridPresentationServiceInstance.GenerateDataGridColumnCollectionManuallyAsync(columnSpecificationItems) right here.
    Step 3: Call CohortAnalysisDataGridPresenter.PopulatePresenterAsync() to fill CohortAnalysisDataGridPresenter.ItemsSource.
    if you look in ParticipantCohortAnalysisFormatPageUserControl (that contains this control), you will see that PopulationCohortsPageViewModel.CohortAnalysisDataGridPresenter is specified as the datacontext for this control.
    This means that its ItemsSource property provides the row collection for the RadDataGrid.
*/

namespace Rezultz.Uwp.UserControls_DataGrid
{
    public sealed partial class PopulationCohortsDataGridPresentationServiceUserControl : IPopulationCohortsDataGridPresentationService
    {
        public PopulationCohortsDataGridPresentationServiceUserControl()
        {
            this.InitializeComponent();
        }

        public async Task<bool> GenerateDataGridColumnCollectionManuallyAsync(IEnumerable<ColumnSpecificationItem> columnSpecificationItems)
        {
            TelerikRadDataGridUserControlHelpers.ManuallyGenerateDataGridColumns(columnSpecificationItems, XamlElementTelerikRadDataGridUserControl);

            return await Task.FromResult(true);
        }
    }
}
