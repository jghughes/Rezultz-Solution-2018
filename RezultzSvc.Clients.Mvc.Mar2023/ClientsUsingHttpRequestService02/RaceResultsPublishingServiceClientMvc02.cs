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

namespace RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService02;

/// <summary>
///     A REST API is best developed in a service-first manner. Use hard coded strings for the names of
///     ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
///     endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
///     from Swagger into the URI strings used in your REST client to make HTTP calls to the API.
/// </summary>
public class RaceResultsPublishingServiceClientMvc02 : IRaceResultsPublishingServiceClient
{
    private const string Locus2 = nameof(RaceResultsPublishingServiceClientMvc02);
    private const string Locus3 = "[RezultzSvc.Clients.Mvc.Mar2023]";

    #region fields

    private string ThisControllerRoute { get; }

    private HttpRequestService02<string, ExceptionDto> HttpRequestService { get; } // IMPORTANT: this parameter originates in the MVC service description, the fault/exception type thrown by all the controller actions in the MVC service controllers (InternalServerError code: 500)

    #endregion

    #region ctor stuff

    public RaceResultsPublishingServiceClientMvc02()
    {
        const string failure = "Unable to instantiate RaceResultsPublishingServiceClientMvc02.";
        const string locus = "[constructor]";

        try
        {
            ThisControllerRoute = Routes.RaceResultsPublishingController;
            HttpRequestService = new HttpRequestService02<string, ExceptionDto>(AppSettings.RaceResultsPublishingControllerBaseUrl, MakeHttpClient());
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    private HttpClient MakeHttpClient()
    {
        // Note: it is best practice to reuse the same instance of HttpClient in all requests to a given website (the BaseUrl)
        // for the lifetime of the application so that the underlying TCP connections are reused and socket exhaustion is prevented.
        // To do this, you would need to inject a common HttpClient from a DI container rather than instantiate a new instance here.
        // This is best practice and thread-safe and efficient. The problem is that if I use a DI container I can't straightforwardly
        // customise each HttpClient/HttpClientHandler like we see here. Oh well.

        // For debugging, we bypass certificate validation, allowing the HttpClient to accept any server certificate, regardless
        // of its validity or trust. We do this because we are using localhost for debugging which has a self-signed certificate

#if DEBUG
        var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });
#else
        var httpClient = new HttpClient();
#endif

        return httpClient;
    }

    #endregion

    #region actions

    public async Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfServiceIsAnsweringAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetIfServiceIsAnswering, null);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

            return await HttpRequestService.GetObjectAsync<bool>(route, "", ct);
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
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, badRequest.Message, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
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
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetServiceEndpointsInfo, null);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

            return await HttpRequestService.GetObjectAsync<string[]>(route, "", ct);
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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
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
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            var queryParameters = new List<KeyValuePair<string, object>>
            {
                new(QueryParameters.FileNameFragment, fileNameFragment)
            };

            //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIfPublisherIdIsRecognised, queryParameters);

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetIfPublisherIdIsRecognised, queryParameters);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

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

            var queryParameters = new List<KeyValuePair<string, object>>
            {
                new(QueryParameters.FileNameFragment, fileNameFragment)
            };

            //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetPublisherModuleProfile, queryParameters);

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetPublisherModuleProfile, queryParameters);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

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

            //var route = $"{ThisControllerRoute}/{Routes.GetAllPublisherModuleId}";

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetAllPublisherModuleId, null);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

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

            var queryParameters = new List<KeyValuePair<string, object>>
            {
                new(QueryParameters.FileNameWithExtension, fileNameWithExtension)
            };

            //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIllustrativeExampleOfRawDataset, queryParameters);

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetIllustrativeExampleOfRawDataset, queryParameters);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = false;

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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

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

            //var route = $"{ThisControllerRoute}/{Routes.ComputeResultsForSingleEvent}";

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.ComputeResultsForSingleEvent, null);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true; // true because we want a full stack trace from the publishing modules, otherwise we won't know what went wrong in sufficient detail

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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

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

            var queryParameters = new List<KeyValuePair<string, object>>
            {
                new(QueryParameters.DatasetIdentifier, identifierOfDataset),
                new(QueryParameters.Account, storageLocation.AccountName),
                new(QueryParameters.Container, storageLocation.ContainerName),
                new(QueryParameters.FileNameWithExtension, storageLocation.EntityName)
            };

            //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.SaveFileToBeProcessedSubsequently, queryParameters);

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.SaveFileToBeProcessedSubsequently, queryParameters);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

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

            var queryParameters = new List<KeyValuePair<string, object>>
            {
                new(QueryParameters.Account, storageLocation.AccountName),
                new(QueryParameters.Container, storageLocation.ContainerName),
                new(QueryParameters.FileNameWithExtension, storageLocation.EntityName)
            };

            //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PublishFileOfCompletedResultsForSingleEvent, queryParameters);

            var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.PublishFileOfCompletedResultsForSingleEvent, queryParameters);

            HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

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
        catch (JghApiException<string> alert)
        {
            var msg = Helpers.MakeAlertMessage(alert);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException<ExceptionDto> ex)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ServerExceptionThrown, Helpers.MakeExceptionMessage(ex), JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (JghApiException apiException)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.ApiExceptionThrown, apiException.Description, JghString.MakeWaitTimeMsg(startTimestamp));

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