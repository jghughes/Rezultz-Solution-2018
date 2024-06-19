using System;
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
    public class HubServiceMethodsHelper: HubServiceMethodsHelperBase<TimeStampHubItemDto>
	{
		#region ctor

		public HubServiceMethodsHelper(IAzureStorageAccessor azureStorageAccessorInstance) : base(azureStorageAccessorInstance)
		{
		}

        #endregion

        #region methods

        public async Task<string> PostItemAsync(string databaseAccount, string dataContainer, string itemAsJson, CancellationToken cancellationToken)
        {
            // NB: ONLY use this method for uploading singletons i.e. for PINs and Check-ins.
            // in our blobStorage schema for singleton we use tablePartition==RecordingModeEnum
            // and tableRowKey==Bib for check-in (a pseudo singleton) and tableRowKey==ClockModeSymbolPin for the one and only PIN.

            var accountConnectionString =
                await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var containerName = MakeAzureContainerOrCosmosTableName(dataContainer, TableNameSuffix);

            var blobName = CreateClockHubItemBlobName(JghSerialisation.ToObjectFromJson<TimeStampHubItemDto>(itemAsJson));

            // the actual PIN is inside the Json in the ID field
            // the actual check-in ID is inside the Json in the ID field
            // and the same must go for all other singletons or pseudo singletons

            await AzureStorageAccessorInstance.UploadStringAsync(accountConnectionString, containerName, blobName, true, itemAsJson, cancellationToken);

            return itemAsJson;
        }

        public async Task<string> PostItemArrayAsyncV2(string databaseAccount, string dataContainer, string arrayOfItemsAsJson, CancellationToken cancellationToken)
        {
            var accountConnectionString =
                await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var containerName = MakeAzureContainerOrCosmosTableName(dataContainer, TableNameSuffix);
            // DatabaseTableName==ContainerName, TableNameSuffix is unused for blob mode of storage

            var arrayOfItemsAsObject = JghSerialisation.ToObjectFromJson<TimeStampHubItemDto[]>(arrayOfItemsAsJson).ToArray();

            var counter = arrayOfItemsAsObject.Length;

            List<Task<TimeStampHubItemDto>> uploadTasks = [];

            foreach (var itemAsObject in arrayOfItemsAsObject)
            {
                uploadTasks.Add(UploadItemToStorageAsync(itemAsObject, accountConnectionString, containerName, cancellationToken));
            }

            await JghParallel.WhenAllAsync(uploadTasks.ToArray());

            return $"{containerName}  {counter} items.";
        }

        public async Task<string> GetItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken cancellationToken)
        {
            // NB: ONLY use this method for downloading singletons i.e. for PINs and Check-ins?.
            // in our blobStorage schema for singletons we use tablePartition==RecordingModeEnum
            // and tableRowKey==Bib for check-in and tableRowKey==ClockModeSymbolPin for the one and only PIN.


            var accountConnectionString =
                await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(databaseAccount);

            if (accountConnectionString is null)
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var containerName = MakeAzureContainerOrCosmosTableName(dataContainer, TableNameSuffix);

            var keyCarrier = new TimeStampHubItemDto
            {
                Bib = tableRowKey,
                RecordingModeEnum = tablePartition
            };
            // in our database system, the row key to a singleton is in the Bib property and nowhere else, and the table partition is always equivalent to the stopwatch recording mode symbol 

            var blobName = CreateClockHubItemBlobName(keyCarrier);

            var response = await AzureStorageAccessorInstance.DownloadAsync(accountConnectionString, containerName, blobName, cancellationToken);

            if (response.GetRawResponse().Status != 200)
                throw new Jgh404Exception("Blob not found.");

            var clockHubItemDataTransferObjectAsJsonHopefully = JghConvert.ToStringFromUtf8Bytes(response.Value.Content.ToArray());
            // presumes this is a itemAsJson in json format where Bib==pin when downloading a PIN)

            return clockHubItemDataTransferObjectAsJsonHopefully;
        }


        #endregion

        #region helpers


        private static string CreateClockHubItemBlobName(TimeStampHubItemDto item)
        {
            var descriptionOfContents = MakeCleverDescriptionOfClockHubItemContents(item);

            var uniqueTableRowKey =
                ChooseCosmosTableRowKey(item.RecordingModeEnum, item.Bib, descriptionOfContents);

            var tablePartition = item.RecordingModeEnum;

            var blobName = MakeAzureBlobNameOutOfCosmosTablePartitionAndRowKey(tablePartition, uniqueTableRowKey);

            return blobName;
        }

        private static string MakeCleverDescriptionOfClockHubItemContents(TimeStampHubItemDto hubItem)
        {
            const string spacer = "  ";

            var sb = new StringBuilder();

            sb.Append(spacer);

            sb.Append(JghString.RightAlign(hubItem.Bib, 3, '0'));
            sb.Append(spacer);

            // convert to UTC so that everything in the database is stored in UTC worldwide, regardless of the timezone where the server happens to be running
            var xx2 = DateTime.FromBinary(hubItem.TimeStampBinaryFormat).ToUniversalTime().ToString(JghDateTime.Iso8601Pattern);
            xx2 = JghString.Replace(xx2, ' ', '-');
            sb.Append(xx2);
            sb.Append(spacer);

            sb.Append(StringHelpers.Truncate(hubItem.OriginatingItemGuid, 4));
            sb.Append("-");
            sb.Append(StringHelpers.Truncate(hubItem.Guid, 4));
            sb.Append(spacer);

            sb.Append(JghString.LeftAlign(hubItem.DatabaseActionEnum.ToLower(), 6, ' '));

            if (!string.IsNullOrWhiteSpace(hubItem.DnxSymbol))
            {
                sb.Append(spacer);
                sb.Append(hubItem.DnxSymbol);
            }

            if (hubItem.MustDitchOriginatingItem)
            {
                sb.Append(spacer);
                sb.Append(JghString.BooleanTrueToDitchOrBlank(true));
            }

            return sb.ToString();
        }

        private async Task<TimeStampHubItemDto> UploadItemToStorageAsync(TimeStampHubItemDto itemAsObject, string accountConnectionString, string containerName, CancellationToken cancellationToken)
        {
            var blobName = CreateClockHubItemBlobName(itemAsObject);

            await AzureStorageAccessorInstance.UploadStringAsync(accountConnectionString, containerName, blobName, true, JghSerialisation.ToJsonFromObject(itemAsObject), cancellationToken);

            return itemAsObject;
        }

        #endregion

    }
}