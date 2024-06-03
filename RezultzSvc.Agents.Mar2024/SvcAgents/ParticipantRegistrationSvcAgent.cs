using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Agents.Mar2024.Bases;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Agents.Mar2024.SvcAgents
{
    public class ParticipantRegistrationSvcAgent : SvcAgentBase, IRegistrationSvcAgent
    {
        private const string Locus2 = nameof(ParticipantRegistrationSvcAgent);
        private const string Locus3 = "[RezultzSvc.Agents.Mar2024]";

        #region fields

        private readonly IParticipantRegistrationServiceClient _myClient;

        #endregion


        #region ctor stuff

        public ParticipantRegistrationSvcAgent(IParticipantRegistrationServiceClient clientInstance)
    {
        const string failure = "Unable to instantiate service agent.";
        const string locus = "[ParticipantRegistrationSvcAgent]";

        try
        {
            ClientBase = clientInstance;

            _myClient = clientInstance;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        #endregion

        #region helper

        public static Tuple<string, string> MakeDataLocationForStorageOfParticipantDataOnRemoteHub(SeriesProfileItem thisSeriesProfileItem, EventProfileItem thisEventProfileItem)
    {
        #region null checks

        if (thisSeriesProfileItem == null) throw new JghAlertMessageException("SeriesItem is null.");

        if (thisSeriesProfileItem.ContainerForParticipantHubItemData == null) throw new JghAlertMessageException("ContainerForParticipantHubData is null.");

        if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForParticipantHubItemData.AccountName)) throw new JghAlertMessageException("ContainerForParticipantHubData.AccountName is null.");

        if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForParticipantHubItemData.ContainerName)) throw new JghAlertMessageException("ContainerForParticipantHubData.ContainerName is null.");

        if (thisEventProfileItem == null) throw new JghAlertMessageException("EventItem is null.");

        #endregion

        var databaseAccount = thisSeriesProfileItem.ContainerForParticipantHubItemData.AccountName;

        var dataContainer = thisSeriesProfileItem.ContainerForParticipantHubItemData.ContainerName;

        var answer = new Tuple<string, string>(databaseAccount, dataContainer);

        return answer;
    }

        #endregion

        #region svc methods

        public async Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
    {
        const string failure = "Unable to determine if specified container exists in specified account in remote storage used by service.";
        const string locus = "[GetIfContainerExistsAsync]";

        try
        {
            var answer = await _myClient.GetIfContainerExistsAsync(databaseAccount, dataContainer, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public async Task<string> PostParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, ParticipantHubItem participantItem, CancellationToken ct)
    {
        const string failure = "Unable to upload a Participant item.";
        const string locus = "[PostParticipantItemAsync]";

        try
        {
            var dataTransferObject = ParticipantHubItem.ToDataTransferObject(participantItem);

            var answer =
                await _myClient.PostParticipantItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, dataTransferObject, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public async Task<ParticipantHubItem> GetParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct = default)
    {
        const string failure = "Unable to download a Participant item.";
        const string locus = "[GetParticipantItemAsync]";

        try
        {
            var dataTransferObject = await _myClient.GetParticipantItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, ct);

            var answer = ParticipantHubItem.FromDataTransferObject(dataTransferObject);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public async Task<string> PostParticipantItemArrayAsync(string databaseAccount, string dataContainer, ParticipantHubItem[] itemArray, CancellationToken ct = default)
    {
        const string failure = "Unable to upload a payload of multiple Participant items.";
        const string locus = "[PostParticipantItemArrayAsync]";

        try
        {
            var dataTransferObject = ParticipantHubItem.ToDataTransferObject(itemArray);

            var answer = await _myClient.PostParticipantItemArrayAsync(databaseAccount, dataContainer, dataTransferObject, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        public async Task<ParticipantHubItem[]> GetParticipantItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
    {
        const string failure = "Unable to download a payload of multiple Participant items.";
        const string locus = "[GetParticipantItemArrayAsync]";

        try
        {
            var dataTransferObject = await _myClient.GetParticipantItemArrayAsync(databaseAccount, dataContainer, ct);

            var answer = ParticipantHubItem.FromDataTransferObject(dataTransferObject);

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