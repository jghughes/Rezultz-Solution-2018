using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Agents.Mar2024.Bases;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace RezultzSvc.Agents.Mar2024.SvcAgents
{
    public class LeaderboardResultsSvcAgent : SvcAgentBase, ILeaderboardResultsSvcAgent
    {
        private const string Locus2 = nameof(LeaderboardResultsSvcAgent);
        private const string Locus3 = "[RezultzSvc.Agents.Mar2024]";

        #region fields

        private readonly ILeaderboardResultsServiceClient _myServiceClient;

        #endregion

        #region ctor stuff

        public LeaderboardResultsSvcAgent(ILeaderboardResultsServiceClient serviceClientInstance)
    {
        const string failure = "Unable to instantiate service agent.";
        const string locus = "[LeaderboardResultsSvcAgent]";

        try
        {
            ClientBase = serviceClientInstance;

            _myServiceClient = serviceClientInstance;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        #endregion

        #region methods

        public async Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment, CancellationToken ct = default)
    {
        const string failure = "Unable to determine if filename of file containing profile of season is recognised.";
        const string locus = "[GetIfFileNameOfSeasonProfileIsRecognisedAsync]";

        try
        {
            var answer = await _myServiceClient.GetIfFileNameOfSeasonProfileIsRecognisedAsync(profileFileNameFragment, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        /// <summary>
        ///     Returns deep copy of SeasonProfileItem document, inclusive of constituent Series data
        /// </summary>
        /// <param name="profileFileNameFragment"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<SeasonProfileItem> GetSeasonProfileAsync(string profileFileNameFragment, CancellationToken ct = default)
    {
        const string failure = "Unable to obtain file containing profile of season.";
        const string locus = "[GetSeasonProfileAsync]";

        try
        {
            var dataTransferItem = await _myServiceClient.GetSeasonProfileAsync(profileFileNameFragment, ct);

            var item = SeasonProfileItem.FromDataTransferObject(dataTransferItem);

            return item;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        /// <summary>
        ///     Returns shallow copies of all SeasonProfileItem documents in
        ///     the StorageHierarchyEntryPoint container, each of them
        ///     exclusive of constituent Series data
        /// </summary>
        /// <returns></returns>
        public async Task<SeasonProfileItem[]> GetAllSeasonProfilesAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to obtain files containing profiles of all seasons.";
        const string locus = "[GetAllSeasonProfilesAsync]";

        try
        {
            var dataTransferItem = await _myServiceClient.GetAllSeasonProfilesAsync(ct);

            var item = SeasonProfileItem.FromDataTransferObject(dataTransferItem);

            return item;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public async Task<EventProfileItem> PopulateSingleEventWithResultsAsync(string databaseAccount, string dataContainer, EventProfileItem eventProfile, CancellationToken ct = default)
    {
        const string failure = "Unable to obtain results for specified event.";
        const string locus = "[PopulateSingleEventWithResultsAsync]";

        try
        {
            var dataTransferObjectOut = EventProfileItem.ToDataTransferObject(eventProfile);

            dataTransferObjectOut.DatabaseAccountName = databaseAccount;

            dataTransferObjectOut.DataContainerName = dataContainer;

            var dataTransferObjectBack = await _myServiceClient.PopulateSingleEventWithResultsAsync(dataTransferObjectOut, ct);

            var answer = EventProfileItem.FromDataTransferObject(dataTransferObjectBack);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public async Task<SeriesProfileItem> PopulateAllEventsInSingleSeriesWithAllResultsAsync(string databaseAccount, string dataContainer, SeriesProfileItem seriesProfile, CancellationToken ct = default)
    {
        const string failure = "Unable to obtain results for all events in specified series.";
        const string locus = "[PopulateAllEventsInSingleSeriesWithAllResultsAsync]";

        try
        {
            var dataTransferObjectOut = SeriesProfileItem.ToDataTransferObject(seriesProfile);

            foreach (var thisEventItem in dataTransferObjectOut.EventProfileCollection
                         .Where(z => z is not null))
            {
                thisEventItem.DatabaseAccountName = databaseAccount;
                thisEventItem.DataContainerName = dataContainer;
            }

            var dataTransferObjectBack = await _myServiceClient.PopulateAllEventsInSingleSeriesWithAllResultsAsync(dataTransferObjectOut, ct);

            var answer = SeriesProfileItem.FromDataTransferObject(dataTransferObjectBack);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        #endregion
    }
}