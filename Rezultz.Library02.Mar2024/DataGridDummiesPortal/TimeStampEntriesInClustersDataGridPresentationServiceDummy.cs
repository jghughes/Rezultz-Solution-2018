using System.Collections.Generic;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library02.Mar2024.DataGridInterfaces;

namespace Rezultz.Library02.Mar2024.DataGridDummiesPortal
{
    public class TimeStampEntriesInClustersDataGridPresentationServiceDummy : ITimeStampEntriesInClustersDataGridPresentationService
    {
        public async Task<bool> GenerateDataGridColumnCollectionManuallyAsync(IEnumerable<ColumnSpecificationItem> columnSpecificationItems)
        {
            // do nothing
            return await Task.FromResult(true);
        }

    }
}
