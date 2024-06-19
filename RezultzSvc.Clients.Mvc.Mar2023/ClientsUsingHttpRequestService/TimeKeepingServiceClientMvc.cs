using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Jgh.MvcParameters.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.HttpRequestService.Feb2019;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService
{
    /// <summary>
    /// A REST API is best developed in a service-first manner. Use hard coded strings for the names of
    /// ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
    /// endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
    /// from Swagger into the URI strings used in your REST client to make HTTP calls to the API. 
    /// </summary>
    public class TimeKeepingServiceClientMvc : ClientBaseMvc, ITimeKeepingServiceClient
    {
        private const string Locus2 = nameof(TimeKeepingServiceClientMvc);
        private const string Locus3 = "[RezultzSvc.Clients.Mvc.Mar2023]";

        #region ctor stuff 

        public TimeKeepingServiceClientMvc()
        {
            const string failure = "Unable to instantiate TimeKeepingServiceClientMvc.";
            const string locus = "[TimeKeepingServiceClientMvc]";

            try
            {
                ThisControllerRoute = $"{AppSettings.TimeKeepingControllerBaseUrl}/{Routes.TimeKeepingController}";
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        #endregion

        #region actions

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

                var queryParameters = new List<KeyValuePair<string, string>>
                {
                    new(QueryParameters.Account, databaseAccount),
                    new(QueryParameters.Container, dataContainer)
                };

                var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIfContainerExists, queryParameters);

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

        public async Task<string> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, TimeStampHubItemDto dataTransferObject, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[PostTimeStampItemAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (string.IsNullOrWhiteSpace(tablePartition)) throw new ArgumentNullException(nameof(tablePartition));
                if (string.IsNullOrWhiteSpace(tableRowKey)) throw new ArgumentNullException(nameof(tableRowKey));
                if (dataTransferObject is null) throw new ArgumentNullException(nameof(dataTransferObject));

                var queryParameters = new List<KeyValuePair<string, string>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                    new(QueryParameters.TablePartition, tablePartition),
                    new(QueryParameters.TableRowKey, tableRowKey),
                };

                var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PostTimeStampItem, queryParameters);

                var json = JghSerialisation.ToJsonFromObject(dataTransferObject);

                var answerAsString = await HttpRequestService.PostObjectAsync<string, string>(route, json, "", ct);

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

        public async Task<string> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, TimeStampHubItemDto[] dataTransferObject, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[PostTimeStampItemArrayAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (dataTransferObject is null) throw new ArgumentNullException(nameof(dataTransferObject));

                var queryParameters = new List<KeyValuePair<string, string>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                };

                var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.PostTimeStampItemArray, queryParameters);

                var json = JghSerialisation.ToJsonFromObject(dataTransferObject);

                var bytes = JghConvert.ToBytesUtf8FromString(json);

                var bytesCompressed = await JghCompression.CompressAsync(bytes);

                var answerAsString = await HttpRequestService.PostObjectAsync<byte[], string>(route, bytesCompressed, "", ct);

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

        public async Task<TimeStampHubItemDto> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetTimeStampItemAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (string.IsNullOrWhiteSpace(tablePartition)) throw new ArgumentNullException(nameof(tablePartition));
                if (string.IsNullOrWhiteSpace(tableRowKey)) throw new ArgumentNullException(nameof(tableRowKey));

                var queryParameters = new List<KeyValuePair<string, string>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                    new(QueryParameters.TablePartition, tablePartition),
                    new(QueryParameters.TableRowKey, tableRowKey),
                };

                var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetTimeStampItem, queryParameters);

                var answerAsJson = await HttpRequestService.GetObjectAsync<string>(route, "", ct);

                var answerAsDto = JghSerialisation.ToObjectFromJson<TimeStampHubItemDto>(answerAsJson);

                return answerAsDto;
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

        public async Task<TimeStampHubItemDto[]> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetTimeStampItemArrayAsync]";

            var startTimestamp = DateTime.Now;

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));

                var queryParameters = new List<KeyValuePair<string, string>>
                {
                    new( QueryParameters.Account, databaseAccount),
                    new( QueryParameters.Container, dataContainer),
                };

                var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetTimeStampItemArray, queryParameters);

                byte[] bytesCompressed = await HttpRequestService.GetObjectAsync<byte[]>(route, "", ct);

                byte[] bytes = await JghCompression.DecompressAsync(bytesCompressed);

                string json = JghConvert.ToStringFromUtf8Bytes(bytes);

                var answerAsDto = JghSerialisation.ToObjectFromJson<TimeStampHubItemDto[]>(json);

                return answerAsDto;
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
}