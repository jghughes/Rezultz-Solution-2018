using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Interfaces03.Apr2022;

namespace RezultzSvc.Agents.Mar2024.Dummies
{
    public class AzureStorageSvcAgentDummy : IAzureStorageSvcAgent
    {
	    public Task<bool> ThrowIfNoServiceConnectionAsync(CancellationToken ct = default)
	    {
		    throw new NotImplementedException("AzureStorageSvcAgentDummy.ThrowIfNoServiceConnectionAsync()");
	    }

	    public Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
	    {
		    throw new NotImplementedException("zureStorageConnectorDummy.GetIfServiceIsAnsweringAsync()");

        }

	    public Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct = default)
	    {
		    throw new NotImplementedException("AzureStorageSvcAgentDummy.GetServiceEndpointsInfoAsync()");
	    }

	    public Task<bool> GetIfContainerExistsAsync(string account, string container,
            CancellationToken ct = default)
	    {
		    throw new NotImplementedException("AzureStorageSvcAgentDummy.GetIfContainerExistsAsync(string accountName, string containerName)");
	    }

	    public Task<string[]> GetNamesOfBlobsInContainerAsync(string account, string container,
            string contains, bool mustPrintDescriptionAsOpposedToBlobName, CancellationToken ct = default)
	    {
		    throw new NotImplementedException("AzureStorageSvcAgentDummy.GetNamesOfBlobsInContainerAsync(string accountName, string containerName, string requiredSubstring)");
	    }

        
	    public Task<bool> GetIfBlobExistsAsync(string account, string container, string blob,
            CancellationToken ct = default)
	    {
		    throw new NotImplementedException("AzureStorageSvcAgentDummy.GetIfBlobExistsAsync(string accountName, string containerName, string blobName)");
	    }


        public Task<bool> UploadBytesAsync(string account, string container, string blob, byte[] payload,
            CancellationToken ct = default)
        {
            throw new NotImplementedException("AzureStorageSvcAgentDummy.UploadBlobAsBytesAsync(string accountName, string containerName,....");
        }

        public Task<byte[]> DownloadBytesThrowingExceptionWithErrorMessageUponFailureAsync(string account, string container, string blob,
            CancellationToken ct = default)
        {
	        throw new NotImplementedException("AzureStorageSvcAgentDummy.DownloadBlobAsBytesThrowingExceptionWithErrorMessageUponFailureAsync(string account, string container, string blob, cancellationToken");
        }


        public Task<bool> UploadStringAsync(string account, string container, string blob, string payload,
            CancellationToken ct = default)
        {
	        throw new NotImplementedException("AzureStorageSvcAgentDummy.UploadBlobAsStringAsync(string accountName, string containerName,....");
        }


        public Task<string> GetAbsoluteUriOfBlockBlobAsync(string account, string container, string blob, CancellationToken ct = default)
        {
	        throw new NotImplementedException("AzureStorageSvcAgentDummy.GetAbsoluteUriOfBlockBlobAsync(target, cancellationToken");
        }

        public Task<bool> DeleteBlobIfExistsAsync(string account, string container, string blob, CancellationToken ct = default)
        {
	        throw new NotImplementedException();
        }


    }
}