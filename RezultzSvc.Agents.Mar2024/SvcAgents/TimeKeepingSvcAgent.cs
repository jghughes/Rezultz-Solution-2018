using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Agents.Mar2024.Bases;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Agents.Mar2024.SvcAgents;

public class TimeKeepingSvcAgent : SvcAgentBase, ITimeKeepingSvcAgent
{
    private const string Locus2 = nameof(TimeKeepingSvcAgent);
    private const string Locus3 = "[RezultzSvc.Agents.Mar2024]";

    #region fields

    private readonly ITimeKeepingServiceClient _myServiceClient;

    #endregion


    #region ctor stuff

    public TimeKeepingSvcAgent(ITimeKeepingServiceClient serviceClientInstance)
    {
        const string failure = "Unable to instantiate service agent.";
        const string locus = "[TimeKeepingSvcAgent]";

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

    #region static methods

    public static Tuple<string, string> MakeDataLocationForStorageOfClockDataOnRemoteHub(SeriesProfileItem thisSeriesProfileItem, EventProfileItem thisEventProfileItem)
    {
        #region null checks

        if (thisSeriesProfileItem == null) throw new JghAlertMessageException("SeriesItem is null.");

        if (thisSeriesProfileItem
                .ContainerForTimestampHubItemData == null)
            throw new JghAlertMessageException("ContainerForTimestampHubData is null.");

        if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForTimestampHubItemData.AccountName)) throw new JghAlertMessageException("ContainerForTimestampHubData.AccountName is null.");

        if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForTimestampHubItemData.ContainerName)) throw new JghAlertMessageException("ContainerForTimestampHubData.ContainerName is null.");

        if (thisEventProfileItem == null) throw new JghAlertMessageException("EventItem is null.");

        #endregion

        var databaseAccount = thisSeriesProfileItem.ContainerForTimestampHubItemData.AccountName;

        var dataContainer = string.Concat(thisSeriesProfileItem.ContainerForTimestampHubItemData.ContainerName, JghString.ToStringMin2(thisEventProfileItem.NumInSequence));

        var answer = new Tuple<string, string>(databaseAccount, dataContainer);

        return answer;
    }

    #endregion

    #region methods

    public async Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
    {
        const string failure = "Unable to determine if specified container exists in specified account in remote storage used by service.";
        const string locus = "[GetIfContainerExistsAsync]";

        try
        {
            var answer = await _myServiceClient.GetIfContainerExistsAsync(databaseAccount, dataContainer, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<string> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, TimeStampHubItem timeStampItem, CancellationToken ct)
    {
        const string failure = "Unable to upload a TimeStamp item.";
        const string locus = "[PostTimeStampItemAsync]";

        try
        {
            var dataTransferObject = TimeStampHubItem.ToDataTransferObject(timeStampItem);

            var answer =
                await _myServiceClient.PostTimeStampItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, dataTransferObject, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<TimeStampHubItem> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct = default)
    {
        const string failure = "Unable to download a TimeStamp item.";
        const string locus = "[GetTimeStampItemAsync]";

        try
        {
            var dataTransferObject = await _myServiceClient.GetTimeStampItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, ct);

            var answer = TimeStampHubItem.FromDataTransferObject(dataTransferObject);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<string> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, TimeStampHubItem[] itemArray, CancellationToken ct = default)
    {
        const string failure = "Unable to upload a payload of multiple TimeStamp items.";
        const string locus = "[PostTimeStampItemArrayAsync]";

        try
        {
            var dataTransferObject = TimeStampHubItem.ToDataTransferObject(itemArray);

            var answer =
                await _myServiceClient.PostTimeStampItemArrayAsync(databaseAccount, dataContainer, dataTransferObject, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<TimeStampHubItem[]> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
    {
        const string failure = "Unable to download a payload of multiple TimeStamp items.";
        const string locus = "[GetTimeStampItemArrayAsync]";

        try
        {
            var dataTransferObject = await _myServiceClient.GetTimeStampItemArrayAsync(databaseAccount, dataContainer, ct);

            var answer = TimeStampHubItem.FromDataTransferObject(dataTransferObject);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion
}