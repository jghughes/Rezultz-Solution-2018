using System.Threading;
using System.Threading.Tasks;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using RezultzSvc.ClientInterfaces.Mar2024.ClientBase;


namespace RezultzSvc.ClientInterfaces.Mar2024.Clients
{
    public interface ILeaderboardResultsServiceClient : IServiceClientBase
    {
        Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment, CancellationToken ct);

        Task<SeasonProfileDto> GetSeasonProfileAsync(string profileFileNameFragment, CancellationToken ct);

        Task<SeasonProfileDto[]> GetAllSeasonProfilesAsync(CancellationToken ct);

        Task<EventProfileDto> PopulateSingleEventWithResultsAsync(EventProfileDto eventProfileDto, CancellationToken ct);

        Task<SeriesProfileDto> PopulateAllEventsInSingleSeriesWithAllResultsAsync(SeriesProfileDto seriesProfileDto, CancellationToken ct);

    }
}