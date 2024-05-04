using System.Text;
using CoreWCF;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using RezultzSvc.Library01.Mar2024.SvcHelpers;
using RezultzSvc.WebApp01.Interfaces;

namespace RezultzSvc.WebApp01.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
partial class LeaderboardResultsSvcWcf : ILeaderboardResultsSvc
{
    #region ctor

    public LeaderboardResultsSvcWcf(ILogger<LeaderboardResultsSvcWcf> logger)
    {
        _leaderboardResultsServiceMethodsHelper = new LeaderboardResultsServiceMethodsHelper();

        _logger = logger;
        // logger registered by CoreWcf with DI as an instance in appsettings.json
    }

    #endregion

    #region fields

    private readonly ILogger<LeaderboardResultsSvcWcf> _logger;

    private readonly LeaderboardResultsServiceMethodsHelper _leaderboardResultsServiceMethodsHelper;

    #endregion

    #region svc methods

    /// <summary>
    ///     Returns true.
    /// </summary>
    /// <returns>Returns (true) if service answers. Otherwise exception will have been thrown at site of call to svc.</returns>
    public async Task<bool> GetIfServiceIsAnsweringAsync([Injected] HttpRequest httpRequest)
    {
        #region logging

        StringBuilder sb = new StringBuilder();

        sb.AppendLine();

        foreach (var header in httpRequest.Headers)
        {
            sb.AppendLine($"{header.Key} : {header.Value}");
        }

        _logger.LogInformation(sb.ToString());

        sb.AppendLine();

        _logger.LogInformation("GetIfServiceIsAnsweringAsync() was called. Returned True");

        sb.AppendLine();

        #endregion

        // if we have got this far, the svc is answering! this here is inside the svc!

        return await Task.FromResult(true);
    }

    /// <summary>
    ///     Probes Wcf service host to obtain visible particulars of svc endpoints, enumerated as a pretty-printed string array
    ///     of line items.
    /// </summary>
    /// <exception cref="FaultException"></exception>
    /// <returns>pretty-printed string array of line items of service endpoints and their descriptions</returns>
    public async Task<string[]> GetServiceEndpointsInfoAsync()
    {
        //const string failure = "Unable to do what this method does.";
        //const string locus = "[GetServiceEndpointsInfoAsync]";

        try
        {
            var context = OperationContext.Current;

            var answer = CoreWcfHelpers.PrettyPrintOperationContextInfo(context);

            return await Task.FromResult(answer);
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment)
    {
        try
        {
            var answer = await _leaderboardResultsServiceMethodsHelper.GetIfSeasonIdIsRecognisedAsync(profileFileNameFragment);

            return answer;

        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    /// <summary>
    ///     Returns serialised Profile document inclusive of constituent SeriesSettings documents.
    ///     Return value is a compressed byte[].
    /// </summary>
    /// <param name="profileFileNameFragment"></param>
    /// <returns></returns>
    public async Task<byte[]> GetSeasonProfileAsync(string profileFileNameFragment)
    {
        try
        {
            var metadataItemDataTransferObject = await _leaderboardResultsServiceMethodsHelper.GetSeasonProfileAsync(profileFileNameFragment);

            var answer = JghSerialisation.ToJsonFromObject(metadataItemDataTransferObject);

            var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

            var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

            return answerCompressedBytes;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<byte[]> GetAllSeasonProfilesAsync()
    {
        try
        {
            var metadataItemDataTransferObjects = await _leaderboardResultsServiceMethodsHelper.GetAllSeasonProfilesAsync(CancellationToken.None);

            var answer = JghSerialisation.ToJsonFromObject(metadataItemDataTransferObjects);

            var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

            var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

            return answerCompressedBytes;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    /// <summary>
    ///     Returns serialised EventItem inclusive of constituent ArrayOfResultPocoForEvent[].
    ///     Return value is a compressed byte[].
    ///     Input value is the serialised EventItem, not yet including constituent ResultPocoArray[] for the event.
    /// </summary>
    /// <param name="eventItemAsJsonAsCompressedBytes"></param>
    /// <returns></returns>
    public async Task<byte[]> PopulateSingleEventWithResultsAsync(byte[] eventItemAsJsonAsCompressedBytes)
    {
        try
        {
            var eventItemAsJsonAsBytes =
                await JghCompression.DecompressAsync(eventItemAsJsonAsCompressedBytes);

            string eventItemAsJson = JghConvert.ToStringFromUtf8Bytes(eventItemAsJsonAsBytes);

            var eventItem = JghSerialisation.ToObjectFromJson<EventProfileDto>(eventItemAsJson);

            var populatedEventItem = await _leaderboardResultsServiceMethodsHelper.PopulateSingleEventWithResultsAsync(eventItem);

            var answer = JghSerialisation.ToJsonFromObject(populatedEventItem);

            var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

            var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

            return answerCompressedBytes;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    /// <summary>
    ///     Returns serialised SeriesItem inclusive of all constituent results for all events.
    ///     Return value is a compressed byte[].
    ///     Input value is the serialised SeriesItem, not yet including constituent ResultPocoArrays[] for the series.
    /// </summary>
    /// <param name="seriesItemAsJsonAsCompressedBytes"></param>
    /// <returns></returns>
    public async Task<byte[]> PopulateAllEventsInSingleSeriesWithAllResultsAsync(byte[] seriesItemAsJsonAsCompressedBytes)
    {
        try
        {
            var seriesItemAsJsonAsBytes =
                await JghCompression.DecompressAsync(seriesItemAsJsonAsCompressedBytes);

            string seriesItemAsJson = JghConvert.ToStringFromUtf8Bytes(seriesItemAsJsonAsBytes);

            var seriesItem = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(seriesItemAsJson);

            var populatedSeriesItem = await _leaderboardResultsServiceMethodsHelper.PopulateAllEventsInSingleSeriesWithAllResultsAsync(seriesItem);

            var answer = JghSerialisation.ToJsonFromObject(populatedSeriesItem);

            var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

            var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

            return answerCompressedBytes;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }


    #endregion
}
