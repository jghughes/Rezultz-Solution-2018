using System.Collections.Generic;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library02.Mar2024.DataGridInterfaces
{
    public interface ITimeStampEntriesInClustersDataGridPresentationService
    {
        Task<bool> GenerateDataGridColumnCollectionManuallyAsync(IEnumerable<ColumnSpecificationItem> columnSpecificationItems); // assigning columns to whatever datagrid that uses as its datacontext something like RezultzSingleEventLeaderboardPageVm.LeaderboardTablePresenter. otherwise empty
    }
}