using System;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;
using ServiceReference1;

// ReSharper disable RedundantNameQualifier

namespace RezultzSvc.Clients.Wcf.Mar2023.ServiceClients
{
    public class AzureStorageServiceClientWcf : IAzureStorageServiceClient
	{
		private const string Locus2 = nameof(AzureStorageServiceClientWcf);
		private const string Locus3 = "[RezultzSvc.Clients.Wcf.Mar2023]";

        #region ctor stuff 

        public AzureStorageServiceClientWcf()
        {
            const string failure = "Unable to instantiate AzureStorageServiceClientWcf.";
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

        private ServiceReference1.AzureStorageSvcClient _svcProxy;

		#endregion

		#region svc calls

		public async Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct)
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
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct)
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
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<bool> GetIfContainerExistsAsync(string account, string container, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[GetIfContainerExistsAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));

				if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                return await _svcProxy.GetIfContainerExistsAsync(account, container);
			}
			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<string[]> GetNamesOfBlobsInContainerAsync(string account, string container, string requiredSubstring, bool mustPrintDescriptionAsOpposedToBlobName, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[GetNamesOfBlobsInContainerAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
                if (string.IsNullOrWhiteSpace(requiredSubstring)) throw new ArgumentNullException(nameof(requiredSubstring));

				if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                return await _svcProxy.GetNamesOfBlobsInContainerAsync(account, container, requiredSubstring, mustPrintDescriptionAsOpposedToBlobName);
			}

			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<bool> GetIfBlobExistsAsync(string account, string container, string blob, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[GetIfBlobExistsAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
                if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

				if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                var answer = await _svcProxy.GetIfBlobExistsAsync(account, container, blob);

				return answer;
			}

			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<string> GetAbsoluteUriOfBlobAsync(string account, string container, string blob, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[GetAbsoluteUriOfBlobAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
                if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

                if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                return await _svcProxy.GetAbsoluteUriOfBlockBlobAsync(account, container, blob);
			}

			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<bool> DeleteBlockBlobIfExistsAsync(string account, string container, string blob, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[DeleteBlockBlobIfExistsAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
                if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));


				if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                return await _svcProxy.DeleteBlockBlobIfExistsAsyncAsync(account, container, blob);
			}

			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<bool> UploadBytesToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, byte[] bytesToUpload, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[UploadBytesAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
                if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));
                if (bytesToUpload == null) throw new ArgumentNullException(nameof(bytesToUpload));


				if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                return await _svcProxy.UploadBytesToBlockBlobAsync(account, container, blob, createContainerIfNotExist, bytesToUpload);
			}

			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<bool> UploadStringToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, string stringToUpload, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[UploadStringAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
                if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));
                if (string.IsNullOrWhiteSpace(stringToUpload)) throw new ArgumentNullException(nameof(stringToUpload));

				if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                return await _svcProxy.UploadStringToBlockBlobAsync(account, container, blob, createContainerIfNotExist, stringToUpload);
			}

			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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

        public async Task<byte[]> DownloadBlockBlobAsBytesAsync(string account, string container, string blob, CancellationToken ct = default)
		{
			const string failure = "Unable to do what this method does.";
			const string locus = "[DownloadBytesAsync]";

			var startTimestamp = DateTime.Now;

			try
			{
                if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
                if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
                if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

				if (!NetworkInterface.GetIsNetworkAvailable())
					throw new JghCommunicationFailureException(StringsWcfClients.NoConnection);

                CreateSvcProxy();

                return await _svcProxy.DownloadBlockBlobAsync(account, container, blob);
			}

			#region catch

			catch (TimeoutException timeout)
			{
				var msg = JghString.ConcatAsSentences(StringsWcfClients.CallTimedOut, timeout.Message, JghString.MakeWaitTimeMsg(startTimestamp));
				throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
			}
			catch (FaultException<ServiceReference1.JghFault> rfc7807Fault)
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
            _svcProxy = new ServiceReference1.AzureStorageSvcClient(AzureStorageSvcClient.EndpointConfiguration.MyHttpsCustomBinaryBinding_IAzureStorageSvc);
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