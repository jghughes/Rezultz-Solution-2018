using System.Threading;
using System.Threading.Tasks;
using RezultzSvc.ClientInterfaces.Mar2024.ClientBase;

namespace RezultzSvc.ClientInterfaces.Mar2024.Clients
{
    public interface IAzureStorageServiceClient : IServiceClientBase
    {
        Task<bool> GetIfContainerExistsAsync(string account, string container, CancellationToken ct);

        Task<string[]> GetNamesOfBlobsInContainerAsync(string account, string container, string requiredSubstring, bool mustPrintDescriptionAsOpposedToBlobName, CancellationToken ct);

        Task<bool> GetIfBlobExistsAsync(string account, string container, string blob, CancellationToken ct);

        Task<string> GetAbsoluteUriOfBlobAsync(string account, string container, string blob, CancellationToken ct);

        Task<bool> DeleteBlockBlobIfExistsAsync(string account, string container, string blob, CancellationToken ct);

        Task<bool> UploadBytesToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, byte[] bytesToUpload, CancellationToken ct);

        Task<bool> UploadStringToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, string stringToUpload, CancellationToken ct);

        Task<byte[]> DownloadBlockBlobAsBytesAsync(string account, string container, string blob, CancellationToken ct);
    }
}