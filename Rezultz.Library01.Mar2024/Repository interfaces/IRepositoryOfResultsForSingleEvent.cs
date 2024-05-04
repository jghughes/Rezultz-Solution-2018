using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
    public interface IRepositoryOfResultsForSingleEvent : IHasSingleEventCategories, IHasCohorts
    {

        Task<ResultItem[]> GetPlacedResultsAsync();

        Task<ResultItem[]> GetPlacedAndNonDnsResultsAsync();

        Task<EventProfileItem> GetEventToWhichThisRepositoryBelongsAsync();

        Task<bool> LoadRepositoryOfResultsFailingNoisilyAsync(string databaseAccountName, string dataContainerName, EventProfileItem eventProfileToWhichThisRepositoryBelongs);

    }
}



