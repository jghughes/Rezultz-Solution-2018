using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Jgh.MvcParameters.Mar2024;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.HttpRequestService.Feb2019;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService;

/// <summary>
///     A REST API is best developed in a service-first manner. Use hard coded strings for the names of
///     ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
///     endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
///     from Swagger into the URI strings used in your REST client to make HTTP calls to the API.
/// </summary>
public class RaceResultsPublishingServiceClientMvc : ClientBaseMvc, IRaceResultsPublishingServiceClient
{
    private const string Locus2 = nameof(RaceResultsPublishingServiceClientMvc);
    private const string Locus3 = "[RezultzSvc.Clients.Mvc.Mar2023]";

    #region ctor stuff

    public RaceResultsPublishingServiceClientMvc()
    {
        const string failure = "Unable to instantiate RaceResultsPublishingServiceClientMvc.";
        const string locus = "[constructor]";

        try
        {
            ThisControllerRoute = $"{AppSettings.RaceResultsPublishingControllerBaseUrl}/{Routes.RaceResultsPublishingController}";
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion

    #region actions

    public async Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.FileNameFragment, fileNameFragment)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIfPublisherIdIsRecognised, queryParameters);

            var answerAsBool = await HttpRequestService.GetObjectAsync<bool>(route, "", ct);

            return answerAsBool;
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
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
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.FileNameFragment, fileNameFragment)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetPublisherModuleProfile, queryParameters);

            var answerAsPublisherModuleProfileDto = await HttpRequestService.GetObjectAsync<PublisherModuleProfileItemDto>(route, "", ct);

            return answerAsPublisherModuleProfileDto;
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
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
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var route = $"{ThisControllerRoute}/{Routes.GetAllPublisherModuleId}";

            var answerAsStringArray = await HttpRequestService.GetObjectAsync<string[]>(route, "", ct);

            return answerAsStringArray;
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

    public async Task<string> GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(string fileNameWithExtension, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIllustrativeExampleOfDatasetExpectedByPublisherAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.FileNameWithExtension, fileNameWithExtension)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIllustrativeExampleOfRawDataset, queryParameters);

            var answerAsString = await HttpRequestService.GetObjectAsync<string>(route, "", ct); //NB: don't use JSON or anything else here. contents of string are totally unpredictable and might not be serialiseable. return as is

            return answerAsString;
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

    public async Task<PublisherOutputItemDto> ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(PublisherInputItemDto publisherInputItemDto, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var route = $"{ThisControllerRoute}/{Routes.ComputeResultsForSingleEvent}";

            var answerAsPublisherOutputDto = await HttpRequestService.PostObjectAsync<PublisherInputItemDto, PublisherOutputItemDto>(route, publisherInputItemDto, "", ct);

            return answerAsPublisherOutputDto;
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

    public async Task<bool> SendFileOfRawDataToBeProcessedSubsequentlyAsync(string identifierOfDataset, EntityLocationDto storageLocation, string datasetAsRawString, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[SendFileOfRawDataToBeProcessedSubsequentlyAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.DatasetIdentifier, identifierOfDataset),
                new(QueryParameters.Account, storageLocation.AccountName),
                new(QueryParameters.Container, storageLocation.ContainerName),
                new(QueryParameters.FileNameWithExtension, storageLocation.EntityName)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.SaveFileToBeProcessedSubsequently, queryParameters);

            var answerAsBool = await HttpRequestService.PutObjectAsync<string, bool>(route, datasetAsRawString, "", ct);

            return answerAsBool;
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }
    
    public async Task<bool> SendFileOfCompletedResultsForSingleEventAsync(EntityLocationDto storageLocation, string completedResultsAsXml, CancellationToken ct)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[SendFileOfCompletedResultsForSingleEventAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, storageLocation.AccountName),
                new(QueryParameters.Container, storageLocation.ContainerName),
                new(QueryParameters.FileNameWithExtension, storageLocation.EntityName)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PublishFileOfCompletedResultsForSingleEvent, queryParameters);

            var didSucceed = await HttpRequestService.PutObjectAsync<string, bool>(route, completedResultsAsXml, "", ct);

            return didSucceed;
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

    #endregion
}