using System;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;
using ServiceReference3;

// ReSharper disable RedundantNameQualifier


namespace RezultzSvc.Clients.Wcf.Mar2023.ServiceClients;

public class LeaderboardResultsServiceClientWcf : ILeaderboardResultsServiceClient
{
    private const string Locus2 = nameof(LeaderboardResultsServiceClientWcf);
    private const string Locus3 = "[RezultzSvc.Clients.Wcf.Mar2023]";

    #region field

    private ServiceReference3.LeaderboardResultsSvcClient _svcProxy;

    #endregion

    #region ctor stuff

    public LeaderboardResultsServiceClientWcf()
    {
        const string failure = "Unable to instantiate LeaderboardResultsServiceClientWcf.";
        const string locus = "[ctor]";

        try
        {
            // this is a placeholder. would be nice to new up the _svcProxy here once and for all, rather than in every method
            // but nobody does this. maybe in future.... 

            _svcProxy = null;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion

    #region svc calls

    public async Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfServiceIsAnsweringAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            return await _svcProxy.GetIfServiceIsAnsweringAsync();
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference3.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException faultEx)
        {
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        catch (CommunicationException commProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception ex)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        finally
        {
            await CloseSvcProxy();
        }

        #endregion
    }

    public async Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetServiceEndpointsInfoAsync]";

        var startTimestamp = DateTime.Now;


        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            return await _svcProxy.GetServiceEndpointsInfoAsync();
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference3.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException faultEx)
        {
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        catch (CommunicationException commProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception ex)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        finally
        {
            await CloseSvcProxy();
        }

        #endregion
    }

    public async Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfFileNameOfSeasonProfileIsRecognisedAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var answer = await _svcProxy.GetIfFileNameOfSeasonProfileIsRecognisedAsync(profileFileNameFragment);

            return answer;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference3.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsParagraphs(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException faultEx)
        {
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        catch (CommunicationException commProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception ex)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        finally
        {
            await CloseSvcProxy();
        }

        #endregion
    }

    public async Task<SeasonProfileDto> GetSeasonProfileAsync(string profileFileNameFragment, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetSeasonProfileAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var bytesCompressed = await _svcProxy.GetSeasonProfileAsync(profileFileNameFragment);

            var bytes = await JghCompression.DecompressAsync(bytesCompressed);

            var json = JghConvert.ToStringFromUtf8Bytes(bytes);

            var answer = JghSerialisation.ToObjectFromJson<SeasonProfileDto>(json);

            return answer;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference3.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException faultEx)
        {
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        catch (CommunicationException commProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            //var msg = JghString.ConcatAsSentences(Strings05.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception ex)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        finally
        {
            await CloseSvcProxy();
        }

        #endregion
    }

    public async Task<SeasonProfileDto[]> GetAllSeasonProfilesAsync(CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetAllSeasonProfilesAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var bytesCompressed = await _svcProxy.GetAllSeasonProfilesAsync();

            var bytes = await JghCompression.DecompressAsync(bytesCompressed);

            var json = JghConvert.ToStringFromUtf8Bytes(bytes);

            var answer = JghSerialisation.ToObjectFromJson<SeasonProfileDto[]>(json);

            return answer;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference3.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException faultEx)
        {
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        catch (CommunicationException commProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            //var msg = JghString.ConcatAsSentences(Strings05.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception ex)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        finally
        {
            await CloseSvcProxy();
        }

        #endregion
    }

    public async Task<EventProfileDto> PopulateSingleEventWithResultsAsync(EventProfileDto eventProfileDto, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[PopulateSingleEventWithResultsAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (eventProfileDto is null) throw new ArgumentNullException(nameof(eventProfileDto));
            if (string.IsNullOrWhiteSpace(eventProfileDto.DatabaseAccountName)) throw new ArgumentNullException(nameof(eventProfileDto.DatabaseAccountName));
            if (string.IsNullOrWhiteSpace(eventProfileDto.DataContainerName)) throw new ArgumentNullException(nameof(eventProfileDto.DataContainerName));

            var jsonOut = JghSerialisation.ToJsonFromObject(eventProfileDto);

            var bytesOut = JghConvert.ToBytesUtf8FromString(jsonOut);

            var bytesOutCompressed = await JghCompression.CompressAsync(bytesOut);

            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var bytesBackCompressed = await _svcProxy.PopulateSingleEventWithResultsAsync(bytesOutCompressed);

            var bytesBack = await JghCompression.DecompressAsync(bytesBackCompressed);

            var jsonBack = JghConvert.ToStringFromUtf8Bytes(bytesBack);

            var answer = JghSerialisation.ToObjectFromJson<EventProfileDto>(jsonBack);

            return answer;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference3.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException faultEx)
        {
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        catch (CommunicationException commProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception ex)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        finally
        {
            await CloseSvcProxy();
        }

        #endregion
    }

    public async Task<SeriesProfileDto> PopulateAllEventsInSingleSeriesWithAllResultsAsync(SeriesProfileDto seriesProfileDto, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[PopulateAllEventsInSingleSeriesWithAllResultsAsync]";

        var startTimestamp = DateTime.Now;


        try
        {
            if (seriesProfileDto is null) throw new ArgumentNullException(nameof(seriesProfileDto));

            var jsonOut = JghSerialisation.ToJsonFromObject(seriesProfileDto);

            var bytesOut = JghConvert.ToBytesUtf8FromString(jsonOut);

            var bytesOutCompressed = await JghCompression.CompressAsync(bytesOut);

            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var bytesBackCompressed = await _svcProxy.PopulateAllEventsInSingleSeriesWithAllResultsAsync(bytesOutCompressed);

            var bytesBack = await JghCompression.DecompressAsync(bytesBackCompressed);

            var jsonBack = JghConvert.ToStringFromUtf8Bytes(bytesBack);

            var answer = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(jsonBack);

            return answer;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference3.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException faultEx)
        {
            // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        catch (CommunicationException commProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception ex)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
        }
        finally
        {
            await CloseSvcProxy();
        }

        #endregion
    }

    #endregion

    #region svc proxy methods

    private void CreateSvcProxy()
    {
        _svcProxy = new LeaderboardResultsSvcClient(LeaderboardResultsSvcClient.EndpointConfiguration.MyHttpsCustomBinaryBinding_ILeaderboardResultsSvc);
        //when svc deployed to Azure and running over Https

        //_svcProxy = new ResultsSvcClient(ResultsSvcClient.EndpointConfiguration.MyHttpTextBinding_IResultsSvc);
        //when debugging with svc running in localhost over Http (not Https)
    }

    private async Task CloseSvcProxy()
    {
        //Closing the client gracefully closes the connection and cleans up resources at both ends of the wire
        //// see https://learn.microsoft.com/en-us/dotnet/framework/wcf/samples/use-close-abort-release-wcf-client-resources

        if (_svcProxy is null) return;

        try
        {
            await _svcProxy.CloseAsync(); // this will take time because it makes a call waits for the server to finish processing the request
        }
        catch (CommunicationException)
        {
            // anything?
            _svcProxy.Abort();
        }
        catch (TimeoutException)
        {
            // anything?
            _svcProxy.Abort();
        }
        catch (Exception)
        {
            // anything?
            _svcProxy.Abort();
            throw;
        }

        _svcProxy = null;
    }

    #endregion
}