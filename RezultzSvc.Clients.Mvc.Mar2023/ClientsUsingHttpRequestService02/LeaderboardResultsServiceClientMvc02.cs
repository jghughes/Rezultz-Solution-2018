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
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService02
{
    /// <summary>
    /// A REST API is best developed in a service-first manner. Use hard coded strings for the names of
    /// ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
    /// endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
    /// from Swagger into the URI strings used in your REST client to make HTTP calls to the API. 
    /// </summary>
    public class LeaderboardResultsServiceClientMvc02 : ILeaderboardResultsServiceClient
	{
		private const string Locus2 = nameof(LeaderboardResultsServiceClientMvc02);
		private const string Locus3 = "[RezultzSvc.Clients.Mvc.Mar2023]";

        #region fields

        private string ThisControllerRoute { get; }

        private HttpRequestService02<string, ExceptionDto> HttpRequestService { get; } // IMPORTANT: this parameter originates in the MVC service description, the fault/exception type thrown by all the controller actions in the MVC service controllers (InternalServerError code: 500)

        #endregion

        #region ctor stuff 

        public LeaderboardResultsServiceClientMvc02()
		{
			const string failure = "Unable to instantiate LeaderboardResultsServiceClientMvc02.";
			const string locus = "[LeaderboardResultsServiceClientMvc02]";

			try
			{
                //ThisControllerRoute = $"{AppSettings.LeaderboardResultsControllerBaseUrl}/{Routes.LeaderboardResultsController}";
                ThisControllerRoute = Routes.LeaderboardResultsController;
                HttpRequestService = new HttpRequestService02<string, ExceptionDto>(AppSettings.LeaderboardResultsControllerBaseUrl, MakeHttpClient());

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

        #region methods

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

        public async Task<bool> GetIfFileNameOfSeasonProfileIsRecognisedAsync(string profileFileNameFragment, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[GetIfFileNameOfSeasonProfileIsRecognisedAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (profileFileNameFragment == null) throw new ArgumentNullException(nameof(profileFileNameFragment));

				var queryParameters = new List<KeyValuePair<string, object>>
				{
					new(QueryParameters.SeasonId, profileFileNameFragment),
				};

				//var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIfFileNameOfSeasonProfileIsRecognised, queryParameters);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetIfFileNameOfSeasonProfileIsRecognised, queryParameters);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = true;

                var answer = await HttpRequestService.GetObjectAsync<bool>(route, "", ct);

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

        public async Task<SeasonProfileDto> GetSeasonProfileAsync(string profileFileNameFragment, CancellationToken ct)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetSeasonProfileAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (profileFileNameFragment == null) throw new ArgumentNullException(nameof(profileFileNameFragment));

                var queryParameters = new List<KeyValuePair<string, object>>
                {
                    new(QueryParameters.SeasonId, profileFileNameFragment),
                };

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetSeasonProfile, queryParameters);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetSeasonProfile, queryParameters);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = false;

                byte[] answerCompressed = await HttpRequestService.GetObjectAsync<byte[]>(route, "", ct);

                byte[] answerDecompressed = await JghCompression.DecompressAsync(answerCompressed);

                string answerAsJson = JghConvert.ToStringFromUtf8Bytes(answerDecompressed);

                var answer = JghSerialisation.ToObjectFromJson<SeasonProfileDto>(answerAsJson);

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

        public async Task<SeasonProfileDto[]> GetAllSeasonProfilesAsync(CancellationToken ct)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetAllSeasonProfilesAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetAllSeasonProfiles);


                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.GetAllSeasonProfiles, null);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = false;

                byte[] answerCompressed = await HttpRequestService.GetObjectAsync<byte[]>(route, "", ct);

                byte[] answerDecompressed = await JghCompression.DecompressAsync(answerCompressed);

                string answerAsJson = JghConvert.ToStringFromUtf8Bytes(answerDecompressed);

                var answer = JghSerialisation.ToObjectFromJson<SeasonProfileDto[]>(answerAsJson);

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

        public async Task<EventProfileDto> PopulateSingleEventWithResultsAsync(EventProfileDto eventProfileDto, CancellationToken ct)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[PopulateSingleEventWithResultsAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (eventProfileDto == null) throw new ArgumentNullException(nameof(eventProfileDto));
				if (string.IsNullOrWhiteSpace(eventProfileDto.DatabaseAccountName)) throw new ArgumentNullException(nameof(eventProfileDto.DatabaseAccountName));
                if (string.IsNullOrWhiteSpace(eventProfileDto.DataContainerName)) throw new ArgumentNullException(nameof(eventProfileDto.DataContainerName));

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PopulateSingleEventWithResults);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.PopulateSingleEventWithResults, null);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = false;

                var jsonOut = JghSerialisation.ToJsonFromObject(eventProfileDto);

                var bytesOut = JghConvert.ToBytesUtf8FromString(jsonOut);

                var bytesOutCompressed = await JghCompression.CompressAsync(bytesOut);

				var bytesBackCompressed = await HttpRequestService.PostObjectAsync<byte[], byte[]>(route, bytesOutCompressed, "", ct);

                var bytesBack = await JghCompression.DecompressAsync(bytesBackCompressed);

                var jsonBack = JghConvert.ToStringFromUtf8Bytes(bytesBack);

                var answer = JghSerialisation.ToObjectFromJson<EventProfileDto>(jsonBack);

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

        public async Task<SeriesProfileDto> PopulateAllEventsInSingleSeriesWithAllResultsAsync(SeriesProfileDto seriesProfileDto, CancellationToken ct)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[PopulateAllEventsInSingleSeriesWithAllResultsAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (seriesProfileDto == null) throw new ArgumentNullException(nameof(seriesProfileDto));

                //var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PopulateAllEventsInSingleSeriesWithAllResults);

                var route = new MvcRouteSegmentItem(ThisControllerRoute, Routes.PopulateAllEventsInSingleSeriesWithAllResults, null);

                HttpRequestService.MustProcessResponseContentUsingReadAsStringAsync = false;

                var jsonOut = JghSerialisation.ToJsonFromObject(seriesProfileDto);

                var bytesOut = JghConvert.ToBytesUtf8FromString(jsonOut);

                var bytesOutCompressed = await JghCompression.CompressAsync(bytesOut);

				var bytesBackCompressed = await HttpRequestService.PostObjectAsync<byte[], byte[]>(route, bytesOutCompressed, "", ct);

                byte[] bytesBack = await JghCompression.DecompressAsync(bytesBackCompressed);

                string jsonBack = JghConvert.ToStringFromUtf8Bytes(bytesBack);

                var answer = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(jsonBack);

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
