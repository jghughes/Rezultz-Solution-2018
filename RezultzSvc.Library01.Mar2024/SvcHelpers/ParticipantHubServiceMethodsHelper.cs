using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;

namespace RezultzSvc.Library01.Mar2024.SvcHelpers
{
    public class ParticipantHubServiceMethodsHelper : HubServiceMethodsHelperBase<ParticipantHubItemDto>
    {
        #region ctor

        public ParticipantHubServiceMethodsHelper(IAzureStorageAccessor azureStorageAccessorInstance) : base(azureStorageAccessorInstance)
        {
        }

        #endregion

        #region methods

        public async Task<string> PostItemAsync(string databaseAccount, string dataContainer, string itemAsJson, CancellationToken cancellationToken)
        {
            // NB: ONLY use this method for uploading singletons i.e. for PINs and Checkins.
            // in our blobStorage schema for singletons we use tablePartition==RecordingModeEnum
            // and tableRowKey==Identifier for checkin (a pseudo singleton) and tableRowKey==ParticipantModeSymbolPin for the one and only PIN.

            var accountConnectionString =
                await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString == null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var containerName = MakeAzureContainerOrCosmosTableName(dataContainer, TableNameSuffix);

            var blobName = CreateParticipantHubItemBlobName(JghSerialisation.ToObjectFromJson<ParticipantHubItemDto>(itemAsJson));

            // the actual PIN is inside the Json in the bib field
            // the actual checkin bib is inside the Json in the bib field
            // and the same must go for all other singletons or pseudo singletons


            await AzureStorageAccessorInstance.UploadStringAsync(accountConnectionString, containerName, blobName,
                true, itemAsJson, cancellationToken);

            return itemAsJson;
        }
        
        public async Task<string> PostItemArrayAsyncV2(string databaseAccount, string dataContainer, string arrayOfItemsAsJson, CancellationToken cancellationToken)
        {
            var accountConnectionString =
                await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString == null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var containerName = MakeAzureContainerOrCosmosTableName(dataContainer, TableNameSuffix); // DatabaseTableName==ContainerName, TableNameSuffix is unused for blob mode of storage

            var arrayOfItemsAsObject = JghSerialisation.ToObjectFromJson<ParticipantHubItemDto[]>(arrayOfItemsAsJson).ToArray();

            var counter = arrayOfItemsAsObject.Length;

            List<Task<ParticipantHubItemDto>> uploadTasks = new();

            foreach (var itemAsObject in arrayOfItemsAsObject)
            {
                uploadTasks.Add(UploadItemToStorageAsync(itemAsObject, accountConnectionString, containerName, cancellationToken));
            }

            await JghParallel.WhenAllAsync(uploadTasks.ToArray());

            return $"{containerName}  {counter} items.";
        }

        public async Task<string> GetItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken cancellationToken)
        {
            // NB: ONLY use this method for downloading singletons i.e. for PINs and Check-ins.
            // in our blobStorage schema for singletons we use tablePartition==RecordingModeEnum
            // and tableRowKey==Identifier for check-in and tableRowKey==ParticipantModeSymbolPin for the one and only PIN.


            var accountConnectionString =
                await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString == null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var containerName = MakeAzureContainerOrCosmosTableName(dataContainer, TableNameSuffix);

            var keyCarrier = new ParticipantHubItemDto
            {
                Identifier = tableRowKey,
                RecordingModeEnum = tablePartition
            };
            // in our database system, the row key to a singleton is in the Identifier property and nowhere else, and the table partition is always equivalent to the RecordingModeEnum 

            var blobName = CreateParticipantHubItemBlobName(keyCarrier);

            var response = await AzureStorageAccessorInstance.DownloadAsync(accountConnectionString, containerName, blobName, cancellationToken);

            if (response.GetRawResponse().Status != 200)
                throw new Jgh404Exception("Blob not found.");

            var participantHubItemDataTransferObjectAsJsonHopefully = JghConvert.ToStringFromUtf8Bytes(response.Value.Content.ToArray());

            return participantHubItemDataTransferObjectAsJsonHopefully;
        }

        #endregion

        #region helpers

        private static string CreateParticipantHubItemBlobName(ParticipantHubItemDto keyCarrier)
        {

            var descriptionOfContents = MakeCleverDescriptionOfParticipantItemContents(keyCarrier);

            var uniqueTableRowKey =
                ChooseCosmosTableRowKey(keyCarrier.RecordingModeEnum, keyCarrier.Identifier, descriptionOfContents);

            var tablePartition = keyCarrier.RecordingModeEnum;

            var blobName = MakeAzureBlobNameOutOfCosmosTablePartitionAndRowKey(tablePartition, uniqueTableRowKey);

            return blobName;
        }

        private static string MakeCleverDescriptionOfParticipantItemContents(ParticipantHubItemDto hubItem)
        {
            const string spacer = "  ";

            var sb = new StringBuilder();

            sb.Append(spacer);
            sb.Append(JghString.LeftAlign($"{hubItem.LastName}, {hubItem.FirstName} {hubItem.MiddleInitial}", 30, '_'));
            sb.Append(spacer);
            sb.Append(JghString.RightAlign(hubItem.Identifier, 3, '0'));
            sb.Append(spacer);
            sb.Append(StringHelpers.Truncate(hubItem.OriginatingItemGuid, 4));
            sb.Append("-");
            sb.Append(StringHelpers.Truncate(hubItem.Guid, 4));
            sb.Append(spacer);
            sb.Append(JghString.LeftAlign(hubItem.DatabaseActionEnum.ToLower(), 6, ' '));

            if (hubItem.MustDitchOriginatingItem)
            {
                sb.Append(spacer);
                sb.Append(JghString.BooleanTrueToDitchOrBlank(true));
            }

            return sb.ToString();
        }

        private async Task<ParticipantHubItemDto> UploadItemToStorageAsync(ParticipantHubItemDto itemAsObject, string accountConnectionString, string containerName, CancellationToken cancellationToken)
        {
            var blobName = CreateParticipantHubItemBlobName(itemAsObject);

            await AzureStorageAccessorInstance.UploadStringAsync(accountConnectionString, containerName, blobName,
                true,
                JghSerialisation.ToJsonFromObject(itemAsObject), cancellationToken);

            return itemAsObject;
        }

        #endregion

    }
}