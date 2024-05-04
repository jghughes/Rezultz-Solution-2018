using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;

namespace NetStd.AzureStorageAccess.July2018
{
	/// <summary>
	///     Intended to access private accounts that require a storageAccountConnectionString with security credentials.
	///     Implementing classes, for example,should ideally employ Azure.Storage.Blob classes
	/// </summary>
	public interface IAzureStorageAccessor
	{
		Task<Response<bool>> GetIfContainerExistsAsync(string accountConnectionString, string container, CancellationToken ct);

		Task<bool> CreateContainerAsync(string accountConnectionString, string container); // not tested at time of writing Dec 2020

		Task<BlobItem[]> ListBlobsInContainerAsync(string accountConnectionString, string container, string contains, CancellationToken ct); // to do. convert this to a TaskRespone. problem is i'm struggling as to how to do it!

		Task<Response<bool>> GetIfBlobExistsAsync(string accountConnectionString, string container, string blob, CancellationToken ct);

        Task<Response<bool>> DeleteBlobIfExistsAsync(string accountConnectionString, string container, string blob, CancellationToken ct); // not tested at time of writing Dec 2020

        Task<string> GetAbsoluteUriOfBlobAsync(string accountConnectionString, string container, string blob, CancellationToken ct);

        Task<Response<BlobContentInfo>> UploadStringAsync(string accountConnectionString, string container, string blob, bool createContainerIfNotExist, string content, CancellationToken ct);

		Task<Response<BlobContentInfo>> UploadBytesAsync(string accountConnectionString, string container, string blob, bool createContainerIfNotExist, byte[] content, CancellationToken ct);

		Task<Response<BlobDownloadResult>> DownloadAsync(string accountConnectionString, string container, string blob, CancellationToken ct);




	}
}