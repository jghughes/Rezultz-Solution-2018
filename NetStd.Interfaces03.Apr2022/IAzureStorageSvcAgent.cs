

// ReSharper disable InconsistentNaming

using System.Threading;
using System.Threading.Tasks;

namespace NetStd.Interfaces03.Apr2022
{
    public interface IAzureStorageSvcAgent : ISvcAgentBase
    {
        Task<bool> GetIfContainerExistsAsync(string account, string container, CancellationToken ct);

        Task<string[]> GetNamesOfBlobsInContainerAsync(string account, string container, string contains, bool mustPrintDescriptionAsOpposedToBlobName, CancellationToken ct);

        Task<bool> GetIfBlobExistsAsync(string account, string container, string blob, CancellationToken ct);

        Task<bool> DeleteBlobIfExistsAsync(string account, string container, string blob, CancellationToken ct);

        Task<string> GetAbsoluteUriOfBlockBlobAsync(string account, string container, string blob, CancellationToken ct);

        Task<bool> UploadStringAsync(string account, string container, string blob, string payload, CancellationToken ct);

        Task<bool> UploadBytesAsync(string account, string container, string blob, byte[] payload, CancellationToken ct);

        Task<byte[]> DownloadBytesThrowingExceptionWithErrorMessageUponFailureAsync(string account, string container, string blob, CancellationToken ct);

    }
}