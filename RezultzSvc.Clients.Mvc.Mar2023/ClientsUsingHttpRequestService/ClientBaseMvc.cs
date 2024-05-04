using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jgh.MvcParameters.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.HttpRequestService.Feb2019;
using RezultzSvc.ClientInterfaces.Mar2024.ClientBase;

namespace RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService
{
    public class ClientBaseMvc : IServiceClientBase
    {
        private const string Locus2 = nameof(ClientBaseMvc);
        private const string Locus3 = "[RezultzSvc.Clients.Mvc.Mar2023]";

        #region ctor stuff 

        public ClientBaseMvc()
        {
            const string failure = "Unable to instantiate ClientBaseMvc.";
            const string locus = "[ClientBaseMvc]";

            try
            {
                HttpRequestService = new HttpRequestService();
                // do it like this. don't use ctor injection of an app-wide singleton. the instance must be unique to this service

            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        #endregion

        #region internal fields

        internal IHttpRequestService HttpRequestService { get; }

        internal string ThisControllerRoute { get; set; }

        #endregion

        #region REST call methods

        public async Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetIfServiceIsAnsweringAsync]";

            var startTimestamp = DateTime.Now;

            try
            {

                var route = $"{ThisControllerRoute}/{Routes.GetIfServiceIsAnswering}";

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
                var route = $"{ThisControllerRoute}/{Routes.GetServiceEndpointsInfo}";

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