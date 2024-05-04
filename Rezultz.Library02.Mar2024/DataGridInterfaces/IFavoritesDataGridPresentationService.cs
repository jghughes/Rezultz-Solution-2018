using System.Collections.Generic;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library02.Mar2024.DataGridInterfaces
{
    public interface IFavoritesDataGridPresentationService
    {
        Task<bool> GenerateDataGridColumnCollectionManuallyAsync(IEnumerable<ColumnSpecificationItem> columnSpecificationItems);
    }
}