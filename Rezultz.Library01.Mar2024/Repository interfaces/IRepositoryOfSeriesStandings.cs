using System.Collections.Generic;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace Rezultz.Library01.Mar2024.Repository_interfaces
{
    public interface IRepositoryOfSeriesStandings : IHasSeriesCategories, IHasCohorts
    {
        Task<SequenceContainerItem[]> GetPointsStandingsAsync();

        Task<SequenceContainerItem[]> GetTourStandingsAsync();

        Task<Dictionary<int, string>> GetTxxColumnHeadersAsync();

        Task<int> GetNumberOfEventsEligibleForPointsAsync();

        Task<bool> LoadRepositoryOfSequenceContainersAsync(string databaseAccountName, string dataContainerName, SeriesProfileItem seriesProfileToWhichThisRepositoryBelongs);

        Task<bool> GetIsInitialised();

    }
}