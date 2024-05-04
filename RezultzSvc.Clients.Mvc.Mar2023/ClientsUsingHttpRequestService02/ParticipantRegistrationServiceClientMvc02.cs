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
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService02
{
    /// <summary>
    /// A REST API is best developed in a service-first manner. Use hard coded strings for the names of
    /// ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
    /// endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
    /// from Swagger into the URI strings used in your REST client to make HTTP calls to the API. 
    /// </summary>
    public class ParticipantRegistrationServiceClientMvc02 : IParticipantRegistrationServiceClient
    {
        private const string Locus2 = nameof(ParticipantRegistrationServiceClientMvc02);
        private const string Locus3 = "[RezultzSvc.Clients.Mvc.Mar2023]";

        #region fields

        private string ThisControllerRoute { get; }

        private HttpRequestService02<string, ExceptionDto> HttpRequestService { get; } // IMPORTANT: this parameter originates in the MVC service description, the fault/exception type thrown by all the controller actions in the MVC service controllers (InternalServerError code: 500)

        #endregion

        #region ctor stuff 

        public ParticipantRegistrationServiceClientMvc02()
        {
            const string failure = "Unable to instantiate ParticipantRegistrationServiceClientMvc02.";
            const string locus = "[ParticipantRegistrationServiceClientMvc02]";

            try
            {
                //ThisControllerRoute = $"{AppSettings.ParticipantRegistrationControllerBaseUrl}/{Routes.ParticipantRegistrationController}";
                ThisControllerRoute = Routes.ParticipantRegistrationController;
                HttpRequestService = new HttpRequestService02<string, ExceptionDto>(AppSettings.ParticipantRegistrationControllerBaseUrl, MakeHttpClient());

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

        public async Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetIfContainerExistsAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));

                var queryParameters = new List<KeyValuePair<string, object>>
                {
                    new(QueryParameters.Account, databaseAccount),
                    new(QueryParameters.Container, dataContainer)
                };

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIfContainerExists, queryParameters);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetIfContainerExists, queryParameters);

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

        public async Task<string> PostParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, ParticipantHubItemDto dataTransferObject, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[PostParticipantItemAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (string.IsNullOrWhiteSpace(tablePartition)) throw new ArgumentNullException(nameof(tablePartition));
                if (string.IsNullOrWhiteSpace(tableRowKey)) throw new ArgumentNullException(nameof(tableRowKey));
                if (dataTransferObject == null) throw new ArgumentNullException(nameof(dataTransferObject));

                var queryParameters = new List<KeyValuePair<string, object>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                    new(QueryParameters.TablePartition, tablePartition),
                    new(QueryParameters.TableRowKey, tableRowKey),
                };

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PostParticipantItem, queryParameters);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.PostParticipantItem, queryParameters);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

                var json = JghSerialisation.ToJsonFromObject(dataTransferObject);

                return await HttpRequestService.PostObjectAsync<string, string>(route, json, "", ct);
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

        public async Task<string> PostParticipantItemArrayAsync(string databaseAccount, string dataContainer, ParticipantHubItemDto[] dataTransferObject, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[PostParticipantItemArrayAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (dataTransferObject == null) throw new ArgumentNullException(nameof(dataTransferObject));

                var queryParameters = new List<KeyValuePair<string, object>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                };

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PostParticipantItemArray, queryParameters);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.PostParticipantItemArray, queryParameters);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

                var json = JghSerialisation.ToJsonFromObject(dataTransferObject);

                var bytes = JghConvert.ToBytesUtf8FromString(json);

                var bytesCompressed = await JghCompression.CompressAsync(bytes);

                return await HttpRequestService.PostObjectAsync<byte[], string>(route, bytesCompressed, "", ct);

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

        public async Task<ParticipantHubItemDto> GetParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[PostParticipantItemAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (string.IsNullOrWhiteSpace(tablePartition)) throw new ArgumentNullException(nameof(tablePartition));
                if (string.IsNullOrWhiteSpace(tableRowKey)) throw new ArgumentNullException(nameof(tableRowKey));

                var queryParameters = new List<KeyValuePair<string, object>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                    new(QueryParameters.TablePartition, tablePartition),
                    new(QueryParameters.TableRowKey, tableRowKey),
                };

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetParticipantItem, queryParameters);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetParticipantItem, queryParameters);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

                var json = await HttpRequestService.GetObjectAsync<string>(route, "", ct);

                var answer = JghSerialisation.ToObjectFromJson<ParticipantHubItemDto>(json);

                return answer;
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

        public async Task<ParticipantHubItemDto[]> GetParticipantItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetParticipantItemArrayAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));

                var queryParameters = new List<KeyValuePair<string, object>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                };

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetParticipantItemArray, queryParameters);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetParticipantItemArray, queryParameters);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = false;

                byte[] bytesCompressed = await HttpRequestService.GetObjectAsync<byte[]>(route, "", ct);

                byte[] bytes = await JghCompression.DecompressAsync(bytesCompressed);

                string json = JghConvert.ToStringFromUtf8Bytes(bytes);

                var answer = JghSerialisation.ToObjectFromJson<ParticipantHubItemDto[]>(json);

                return answer;
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
}
