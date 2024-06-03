using System;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;
using ServiceReference5;

// ReSharper disable RedundantNameQualifier


namespace RezultzSvc.Clients.Wcf.Mar2023.ServiceClients
{
    public class RaceResultsPublishingServiceClientWcf : IRaceResultsPublishingServiceClient
    {
        private const string Locus2 = nameof(RaceResultsPublishingServiceClientWcf);
        private const string Locus3 = "[RezultzSvc.Clients.Wcf.Mar2023]";

        #region field

        private ServiceReference5.RaceResultsPublishingSvcClient _svcProxy;

        #endregion

        #region ctor stuff

        public RaceResultsPublishingServiceClientWcf()
    {
        const string failure = "Unable to instantiate RaceResultsPublishingServiceClientWcf.";
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
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
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
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
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

        public async Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var answerAsBool = await _svcProxy.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(fileNameFragment);

            //var answerAsBool = await JghCompressionHelper.ConvertJsonAsCompressedBytesToObjectAsync<bool>(boolAsJsonAsCompressedBytesBack);

            return answerAsBool;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
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

        public async Task<PublisherModuleProfileItemDto> GetPublisherModuleProfileItemAsync(string fileNameFragment, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetPublisherModuleProfileItemAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var publisherProfileDtoAsJson = await _svcProxy.GetPublishingProfileAsync(fileNameFragment);

            var publishingProfileDto = JghSerialisation.ToObjectFromJson<PublisherModuleProfileItemDto>(publisherProfileDtoAsJson);

            return publishingProfileDto;

            //var publisherProfileDtoAsJsonAsCompressedBytesBack = await _svcProxy.GetPublishingProfileAsync(fileNameFragment);

            //var publishingProfileDto = await JghCompressionHelper.ConvertJsonAsCompressedBytesToObjectAsync<PublisherModuleProfileItemDto>(publisherProfileDtoAsJsonAsCompressedBytesBack);

            //return publishingProfileDto;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsParagraphs(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghPublisherServiceFaultException(msg));
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

        public async Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync(CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetFileNameFragmentsOfAllPublishingProfilesAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var fragmentsAsStringArray = await _svcProxy.GetFileNameFragmentsOfAllPublishingProfilesAsync();

            //var answerAsStringArray = await JghCompressionHelper.ConvertJsonAsCompressedBytesToObjectAsync<string[]>(fragmentsAsStringArray);

            return fragmentsAsStringArray;

            //var fragmentsAsStringArrayAsJsonAsCompressedBytesBack = await _svcProxy.GetFileNameFragmentsOfAllPublishingProfilesAsync();

            //var answerAsStringArray = await JghCompressionHelper.ConvertJsonAsCompressedBytesToObjectAsync<string[]>(fragmentsAsStringArrayAsJsonAsCompressedBytesBack);

            //return answerAsStringArray;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
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

        public async Task<string> GetIllustrativeExampleOfSourceDatasetExpectedByPublishingServiceAsync(string fileNameWithExtension, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIllustrativeExampleOfDatasetExpectedByPublisherAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var fileContentsAsString = await _svcProxy.GetIllustrativeExampleOfDatasetExpectedForProcessingAsync(fileNameWithExtension);

            //var fileContentsAsString = JghConvert.ToStringFromUtf8Bytes(await JghCompression.DecompressAsync(fileContentsAsStringAsCompressedBytesBack));

            return fileContentsAsString;

            //var fileContentsAsStringAsCompressedBytesBack = await _svcProxy.GetIllustrativeExampleOfDatasetExpectedForProcessingAsync(fileNameWithExtension);

            //var fileContentsAsString = JghConvert.ToStringFromUtf8Bytes(await JghCompression.DecompressAsync(fileContentsAsStringAsCompressedBytesBack));

            //return fileContentsAsString;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
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

        public async Task<bool> UploadSourceDatasetToBeProcessedSubsequentlyAsync(string identifierOfDataset, EntityLocationDto storageLocation, string datasetAsRawString, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[SendFileOfRawDataToBeProcessedSubsequentlyAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var didSucceed =
                await _svcProxy.UploadDatasetToBeProcessedSubsequentlyAsync(identifierOfDataset, storageLocation.AccountName, storageLocation.ContainerName, storageLocation.EntityName, datasetAsRawString);

            return didSucceed;

        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghPublisherServiceFaultException(msg));
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

        public async Task<PublisherOutputItemDto> ProcessPreviouslyUploadedSourceDataIntoPublishableResultsForSingleEventAsync(PublisherInputItemDto publisherInputItemDto, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var publisherInputDtoAsJson = JghSerialisation.ToJsonFromObject(publisherInputItemDto);

            var publisherOutputDtoAsJson = await _svcProxy.GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(publisherInputDtoAsJson);

            var publisherOutputDto = JghSerialisation.ToObjectFromJson<PublisherOutputItemDto>(publisherOutputDtoAsJson);

            return publisherOutputDto;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghPublisherServiceFaultException(msg));
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

        public async Task<bool> UploadPublishableResultsForSingleEventAsync(EntityLocationDto storageLocation, string completedResultsAsXml, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[SendFileOfCompletedResultsForSingleEventAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

            CreateSvcProxy();

            var didSucceed =
                await _svcProxy.UploadFileOfCompletedResultsForSingleEventAsync(storageLocation.AccountName, storageLocation.ContainerName, storageLocation.EntityName, completedResultsAsXml);

            return didSucceed;

            //var completedResultsAsXmlFileContentsAsCompressedBytesOut = await JghCompression.CompressAsync(JghConvert.ToBytesUtf8FromString(completedResultsAsXml));

            //var boolAsJsonAsCompressedBytesBack =
            //    await _svcProxy.UploadFileOfCompletedResultsForSingleEventAsync(storageLocation.AccountName, storageLocation.ContainerName, storageLocation.EntityItemName, completedResultsAsXmlFileContentsAsCompressedBytesOut);

            //var didSucceed = await JghCompressionHelper.ConvertJsonAsCompressedBytesToObjectAsync<bool>(boolAsJsonAsCompressedBytesBack);

            //return didSucceed;
        }

        #region catch

        catch (TimeoutException timeout)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (FaultException<ServiceReference5.JghFault> rfc7807Fault)
        {
            var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghPublisherServiceFaultException(msg));
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
        _svcProxy = new RaceResultsPublishingSvcClient(RaceResultsPublishingSvcClient.EndpointConfiguration.MyHttpsCustomBinaryBinding_IRaceResultsPublishingSvc);
        //when svc deployed to Azure and running over Https

        //_svcProxy = new RaceResultsPublishingSvcClient(RaceResultsPublishingSvcClient.EndpointConfiguration.MyHttpTextBinding_IRaceResultsPublishingSvc);
        //when debugging with svc running in localhost over Http (not Https)

    }

        private async Task CloseSvcProxy()
    {
        //Closing the client gracefully closes the connection and cleans up resources at both ends of the wire
        //// see https://learn.microsoft.com/en-us/dotnet/framework/wcf/samples/use-close-abort-release-wcf-client-resources

        if (_svcProxy == null) return;

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
}