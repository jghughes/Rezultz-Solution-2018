using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;

namespace RezultzSvc.Library02.Mar2024.SvcHelpers
{
    // this class is a cut and paste from RezultzSvc.Library01.Mar2024. Don't change it

    internal class AzureStorageServiceMethodsHelper
    {
        #region ctor

        public AzureStorageServiceMethodsHelper(IAzureStorageAccessor azureStorageAccessorInstance)
        {
            _azureStorageAccessor = azureStorageAccessorInstance ?? new AzureStorageAccessor();
        }

        #endregion

        #region fields

        private readonly IAzureStorageAccessor _azureStorageAccessor;

        #endregion

        #region methods

        public async Task<bool> GetIfContainerExistsAsync(string accountName, string containerName)
        {
                var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);

                if (accountConnectionString is null)
                    throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

                Response<bool> answer =
                    await _azureStorageAccessor.GetIfContainerExistsAsync(accountConnectionString,
                        containerName, CancellationToken.None);

                return answer;
        }

        public async Task<string[]> GetNamesOfBlobsInContainerAsync(string accountName, string containerName, string requiredSubstring, bool mustPrintDescriptionAsOpposedToBlobName)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var blobItems = await _azureStorageAccessor.ListBlobsInContainerAsync(
                accountConnectionString, containerName, requiredSubstring, CancellationToken.None);

            var answer = blobItems.Select(z => z.Name).ToArray();

            return answer;
        }

        public async Task<bool> GetIfBlobExistsAsync(string accountName, string containerName, string blobName)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var answer = await _azureStorageAccessor.GetIfBlobExistsAsync(accountConnectionString,
                containerName,
                blobName, CancellationToken.None);

            return answer;
        }

        public async Task<bool> UploadToBlockBlobAsStringAsync(string accountName, string containerName, string blobName, bool createContainerIfNotExists, string content)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);
                
            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            Response<BlobContentInfo> response = await _azureStorageAccessor.UploadStringAsync(
                accountConnectionString,
                containerName, blobName, createContainerIfNotExists, content, CancellationToken.None);

            // ReSharper disable once UnusedVariable
            var answer = response.Value.VersionId;

            // todo. in a future version lets return the version id of saved blob so that we can uniquely identify it later if necessary

            return true;
        }

        public async Task<bool> UploadToBlockBlobAsBytesAsync(string accountName, string containerName, string blobName,   bool createContainerIfNotExists, byte[] bytes)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            Response<BlobContentInfo> response = await _azureStorageAccessor.UploadBytesAsync(
                accountConnectionString,
                containerName, blobName, createContainerIfNotExists, bytes, CancellationToken.None);

            // ReSharper disable once UnusedVariable
            var answer = response.Value.VersionId;

            // todo. in a future version lets return the version id of saved blob so that we can uniquely identify it later if necessary

            return true;
        }

        public async Task<byte[]> DownloadBlockBlobAsBytesAsync(string accountName, string containerName, string blobName)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            Response<BlobDownloadResult> response = await _azureStorageAccessor.DownloadAsync(accountConnectionString, containerName, blobName, CancellationToken.None);

            if (response.GetRawResponse().Status != 200)
                throw new Jgh404Exception("Blob not found.");

            byte[] answer = response.Value.Content.ToArray();

            return answer;
        }

        public async Task<string> GetAbsoluteUriOfBlockBlobAsync(string accountName, string containerName, string blobName)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var answer = await _azureStorageAccessor.GetAbsoluteUriOfBlobAsync(
                accountConnectionString,
                containerName, blobName, CancellationToken.None);

            return answer;
        }

        public async Task<bool> BlockBlobDeleteIfExistsAsync(string accountName, string containerName, string blobName)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(accountName);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var answer = await _azureStorageAccessor.DeleteBlobIfExistsAsync(
                accountConnectionString,
                containerName, blobName, CancellationToken.None);

            return answer;
        }

        #endregion

    }
}
