using System;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;
using ServiceReference4;

// ReSharper disable RedundantNameQualifier

namespace RezultzSvc.Clients.Wcf.Mar2023.ServiceClients
{
    public class TimeKeepingServiceClientWcf : ITimeKeepingServiceClient
    {
        private const string Locus2 = nameof(TimeKeepingServiceClientWcf);
        private const string Locus3 = "[RezultzSvc.Clients.Wcf.Mar2023]";

        #region ctor stuff 

        public TimeKeepingServiceClientWcf()
        {
            const string failure = "Unable to instantiate TimeKeepingServiceClientWcf.";
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

        #region field

        private ServiceReference4.TimeKeepingSvcClient _svcProxy;

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

                ct.ThrowIfCancellationRequested(); // last chance to cancel before the call is made

                CreateSvcProxy();

                return await _svcProxy.GetIfServiceIsAnsweringAsync();
            }

            #region catch

            catch (TimeoutException timeout)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException<ServiceReference4.JghFault> rfc7807Fault)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException faultEx)
            {
                // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            catch (CommunicationException commProblem)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (OperationCanceledException cancelledException)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.OperationAborted, JghExceptionHelpers.FindInnermostException(cancelledException).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAlertMessageException(msg));
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed,
                    JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            finally
            {
                await CloseSvcProxyAsync();
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

                ct.ThrowIfCancellationRequested(); // last chance to cancel before the call is made

                CreateSvcProxy();

                return await _svcProxy.GetServiceEndpointsInfoAsync();
            }

            #region catch

            catch (TimeoutException timeout)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException<ServiceReference4.JghFault> rfc7807Fault)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(),
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException faultEx)
            {
                // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            catch (CommunicationException commProblem)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (OperationCanceledException cancelledException)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.OperationAborted, JghExceptionHelpers.FindInnermostException(cancelledException).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAlertMessageException(msg));
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed,
                    JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            finally
            {
                await CloseSvcProxyAsync();
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
                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(dataContainer));
                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(dataContainer));

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                ct.ThrowIfCancellationRequested(); // last chance to cancel before the call is made

                CreateSvcProxy();

                return await _svcProxy.GetIfContainerExistsAsync(databaseAccount, dataContainer);
            }

            #region catch

            catch (TimeoutException timeout)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
            }
            catch (FaultException<ServiceReference4.JghFault> rfc7807Fault)
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
            catch (OperationCanceledException cancelledException)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.OperationAborted, JghExceptionHelpers.FindInnermostException(cancelledException).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAlertMessageException(msg));
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghAzureRequestException(msg));
            }
            finally
            {
                await CloseSvcProxyAsync();
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
                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (string.IsNullOrWhiteSpace(tablePartition)) throw new ArgumentNullException(nameof(tablePartition));
                if (string.IsNullOrWhiteSpace(tableRowKey)) throw new ArgumentNullException(nameof(tableRowKey));
                if (dataTransferObject is null) throw new ArgumentNullException(nameof(dataTransferObject));

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                ct.ThrowIfCancellationRequested(); // last chance to cancel before the call is made

                var json = JghSerialisation.ToJsonFromObject(dataTransferObject);

                CreateSvcProxy();

                return await _svcProxy.PostTimeStampItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, json);
            }

            #region catch

            catch (TimeoutException timeout)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException<ServiceReference4.JghFault> rfc7807Fault)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException faultEx)
            {
                // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            catch (CommunicationException commProblem)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (OperationCanceledException cancelledException)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.OperationAborted, JghExceptionHelpers.FindInnermostException(cancelledException).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAlertMessageException(msg));
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed,
                    JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            finally
            {
                await CloseSvcProxyAsync();
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
                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (dataTransferObject is null) throw new ArgumentNullException(nameof(dataTransferObject));

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                ct.ThrowIfCancellationRequested(); // last chance to cancel before the call is made

                var json = JghSerialisation.ToJsonFromObject(dataTransferObject);

                var bytes = JghConvert.ToBytesUtf8FromString(json);

                var bytesCompressed = await JghCompression.CompressAsync(bytes);

                CreateSvcProxy();

                return await _svcProxy.PostTimeStampItemArrayAsync(databaseAccount, dataContainer, bytesCompressed);
            }

            #region catch

            catch (TimeoutException timeout)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException<ServiceReference4.JghFault> rfc7807Fault)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException faultEx)
            {
                // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            catch (CommunicationException commProblem)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (OperationCanceledException cancelledException)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.OperationAborted, JghExceptionHelpers.FindInnermostException(cancelledException).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAlertMessageException(msg));
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed,
                    JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            finally
            {
                await CloseSvcProxyAsync();
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
                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));
                if (string.IsNullOrWhiteSpace(tablePartition)) throw new ArgumentNullException(nameof(tablePartition));
                if (string.IsNullOrWhiteSpace(tableRowKey)) throw new ArgumentNullException(nameof(tableRowKey));

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                ct.ThrowIfCancellationRequested(); // last chance to cancel before the call is made

                CreateSvcProxy();

                var json = await _svcProxy.GetTimeStampItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey);

                var answer = JghSerialisation.ToObjectFromJson<TimeStampHubItemDto>(json);

                return answer;
            }

            #region catch

            catch (TimeoutException timeout)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException<ServiceReference4.JghFault> rfc7807Fault)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException faultEx)
            {
                // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            catch (CommunicationException commProblem)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (OperationCanceledException cancelledException)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.OperationAborted, JghExceptionHelpers.FindInnermostException(cancelledException).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAlertMessageException(msg));
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed,
                    JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            finally
            {
                await CloseSvcProxyAsync();
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
                if (string.IsNullOrWhiteSpace(databaseAccount)) throw new ArgumentNullException(nameof(databaseAccount));
                if (string.IsNullOrWhiteSpace(dataContainer)) throw new ArgumentNullException(nameof(dataContainer));

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                ct.ThrowIfCancellationRequested(); // last chance to cancel before the call is made

                CreateSvcProxy();

                byte[] bytesCompressed = await _svcProxy.GetTimeStampItemArrayAsync(databaseAccount, dataContainer);

                byte[] bytes = await JghCompression.DecompressAsync(bytesCompressed);

                string json = JghConvert.ToStringFromUtf8Bytes(bytes);

                var answer = JghSerialisation.ToObjectFromJson<TimeStampHubItemDto[]>(json);

                return answer;
            }

            #region catch

            catch (TimeoutException timeout)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException<ServiceReference4.JghFault> rfc7807Fault)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.ProblemEncountered, rfc7807Fault.Detail.Narrative, _svcProxy?.Endpoint.Address.ToString(), JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (FaultException faultEx)
            {
                // Catch unrecognized faults. This handler receives exceptions thrown by WCF services when ServiceDebugBehavior.IncludeExceptionDetailInFaults is set to true.
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed, JghExceptionHelpers.FindInnermostException(faultEx).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            catch (CommunicationException commProblem)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallBounced, JghExceptionHelpers.FindInnermostException(commProblem).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghCommunicationFailureException(msg));
            }
            catch (OperationCanceledException cancelledException)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.OperationAborted, JghExceptionHelpers.FindInnermostException(cancelledException).Message,
                    JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAlertMessageException(msg));
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsWcfClients.CallFailed,
                    JghExceptionHelpers.FindInnermostException(ex).Message, JghString.MakeWaitTimeMsg(startTimestamp));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3,
                    new JghAzureRequestException(msg));
            }
            finally
            {
                await CloseSvcProxyAsync();
            }


            #endregion
        }

        #endregion

        #region svc proxy methods

        private void CreateSvcProxy()
        {
            _svcProxy = new TimeKeepingSvcClient(TimeKeepingSvcClient.EndpointConfiguration.MyHttpsCustomBinaryBinding_ITimeKeepingSvc);
        }

        private async Task CloseSvcProxyAsync()
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

}

