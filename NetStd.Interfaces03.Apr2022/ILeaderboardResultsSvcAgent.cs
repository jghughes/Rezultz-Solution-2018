using System.Threading;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;


// ReSharper disable InconsistentNaming

namespace NetStd.Interfaces03.Apr2022
{
    public interface ILeaderboardResultsSvcAgent : ISvcAgentBase
    {
        Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment, CancellationToken ct = default);

        Task<SeasonProfileItem> GetSeasonProfileAsync(string profileFileNameFragment, CancellationToken ct = default);

        Task<SeasonProfileItem[]> GetAllSeasonProfilesAsync(CancellationToken ct = default);

        Task<EventProfileItem> PopulateSingleEventWithResultsAsync(string databaseAccount, string dataContainer, EventProfileItem eventProfile, CancellationToken ct = default);

        Task<SeriesProfileItem> PopulateAllEventsInSingleSeriesWithAllResultsAsync(string databaseAccount, string dataContainer, SeriesProfileItem seriesProfile, CancellationToken ct = default);
    }
}