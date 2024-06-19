using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.Objects;

namespace RezultzSvc.Library01.Mar2024.SvcHelpers
{
    public class HubServiceMethodsHelperBase<T> where T : IHubItemDataTransferObject
    {
        #region ctor

        public HubServiceMethodsHelperBase(IAzureStorageAccessor azureStorageAccessorInstance)
        {
            AzureStorageAccessorInstance = azureStorageAccessorInstance ?? new AzureStorageAccessor();
        }

        #endregion

        #region constants

        public const string TableNameSuffix = ""; // we'll need this if and when we move over to TableStorage

        #endregion

        #region fields

        protected IAzureStorageAccessor AzureStorageAccessorInstance;

        #endregion

        #region methods

        public async Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg, StringsRezultzSvc.AccountNameUnauthorisedMsg));

            Response<bool> answer =
                await AzureStorageAccessorInstance.GetIfContainerExistsAsync(accountConnectionString, dataContainer, CancellationToken.None);

            return answer;
        }

        public async Task<string> GetArrayOfHubItemAsync(string databaseAccount, string dataContainer, CancellationToken ct)
        {
            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var containerName = MakeAzureContainerOrCosmosTableName(dataContainer, TableNameSuffix);

            var blobServiceClient = new BlobServiceClient(accountConnectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName); //append container name to URI

            var asyncPageableBlobItems = blobContainerClient.GetBlobsAsync();

            var hubItemDictionary = new Dictionary<string, T>();

            await foreach (var blobItem in asyncPageableBlobItems)
            {
                if (!blobItem.Name.EndsWith(".json")) continue; // belt and braces. to eliminate rogue contents of container

                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name); // append blob name to URI

                var blobDownloadInfo = await blobClient.DownloadAsync(ct);

                var contentAsString = await JghStreamHandlers.ReadStreamToStringAsync(blobDownloadInfo.Value.Content); // presumes this is a string!!

                var item = JghSerialisation.ToObjectFromJson<T>(contentAsString); // and a single IHubItemDataTransferObject

                if (item is null || string.IsNullOrWhiteSpace(item.Guid) || string.IsNullOrWhiteSpace(item.OriginatingItemGuid)) continue;

                var bothGuids = JghString.Concat(item.OriginatingItemGuid, item.Guid);

                if (hubItemDictionary.ContainsKey(bothGuids)) // using this compound-key is how we filter out exact duplicates which tend to proliferate
                    hubItemDictionary[bothGuids] = item; // overwrite : don't just ignore
                else
                    hubItemDictionary.Add(bothGuids, item);
            }

            var hubItems = hubItemDictionary.Select(z => z.Value);

            var answerAsJson = JghSerialisation.ToJsonFromObject(hubItems);

            return answerAsJson;
        }


        #endregion

        #region internal methods

        /// <summary>
        ///     Cosmos Table name or Azure Storage container name.
        /// </summary>
        /// <param name="dataContainer"></param>
        /// <param name="tableNameSuffix"></param>
        /// <returns></returns>
        internal string MakeAzureContainerOrCosmosTableName(string dataContainer, string tableNameSuffix)
        {
            // for Blob storage, DatabaseTableName==containerName i.e. a container is the equivalent of a single table in a database

            var fragments = new[]
            {
                dataContainer,
                tableNameSuffix
            };

            var containerName =
                JghString.ConcatWithSeparator("", fragments); // suffix is placeholder. unused in blob storage arrangement as yet

            return containerName;
        }

        /// <summary>
        ///     Make Azure blob name. Intended for
        ///		creating or locating a single, unique blob if using using Azure
        ///		storage or a database line-item entity if using Cosmos TableStorage.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="rowKey"></param>
        /// <returns></returns> 
        internal static string MakeAzureBlobNameOutOfCosmosTablePartitionAndRowKey(string partition, string rowKey)
        {
            var blobName = $"{partition} {rowKey}.json";

            // defensive - important for round tripping from Azure to local file system
            blobName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', blobName);

            return blobName;
        }

        ///  <summary>
        ///      Make Cosmos Table entry row key or mid-section of Azure blob name
        /// 		based on meaningful, articulate description of HubItem contents.
        /// 		In singleton cases, the row key is the EnumString "PIN", which emanates
        /// 		from either the ParticipantService or the ClockService, or it is a participant's
        /// 		ID when checking-in using ParticipantService.
        ///  </summary>
        ///  <param name="recordingModeEnum"></param>
        ///  <param name="hubItemIdentifier"></param>
        ///  <param name="lineItemDescription"></param>
        ///  <returns></returns>
        internal static string ChooseCosmosTableRowKey(string recordingModeEnum, string hubItemIdentifier,
            string lineItemDescription)
        {
            string answer = recordingModeEnum switch
            {
                //EnumStrings.RecordingModePin => EnumStrings.RecordingModePin,

                //EnumStrings.RecordingModeCheckin => JghString.RightAlign(hubItemIdentifier, 5, '0'),

                _ => lineItemDescription
            };

            return answer;
        }


        #endregion

    }
}