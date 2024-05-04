using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Jgh.ConnectionStrings.Mar2024;
using NetStd.AzureStorageAccess.July2018;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;

namespace RezultzSvc.Library01.Mar2024.SvcHelpers
{
    public class LeaderboardResultsServiceMethodsHelper
    {
        private const string Locus2 = nameof(LeaderboardResultsServiceMethodsHelper);
        private const string Locus3 = "[RezultzSvc.Library01.Mar2024]";

        #region ctor

        public LeaderboardResultsServiceMethodsHelper()
        {
            _azureStorageServiceMethodsHelperInstance = new AzureStorageServiceMethodsHelper(new AzureStorageAccessor());

        }

        #endregion

        #region fields

        private readonly AzureStorageServiceMethodsHelper _azureStorageServiceMethodsHelperInstance;

        #endregion

        #region svc methods

        public async Task<bool> GetIfSeasonIdIsRecognisedAsync(string profileFileNameFragment)
        {
            if (string.IsNullOrEmpty(profileFileNameFragment))
                return false;

            var entryPoint = ConnectionStringRepository.GetStorageHierarchyEntryPoint();

            var seasonIdBlobName = string.Concat(AzureStorageObjectNames.SeasonProfileFilenamePrefix, profileFileNameFragment, AzureStorageObjectNames.SeasonProfileFileNameExtension);

            var answer = await _azureStorageServiceMethodsHelperInstance.GetIfBlobExistsAsync(entryPoint.AzureStorageAccountName, entryPoint.AzureStorageContainerName, seasonIdBlobName);


            return answer;
        }

        /// <summary>
        ///     Returns deep copy of SeasonProfileItem document, inclusive of constituent Series data
        /// </summary>
        /// <param name="profileFileNameFragment"></param>
        /// <returns></returns>
        public async Task<SeasonProfileDto> GetSeasonProfileAsync(string profileFileNameFragment)
        {
            const string failure = "Unable to do what this method does.";
            const string locus = "[GetSeasonProfileAsync]";

            if (string.IsNullOrEmpty(profileFileNameFragment))
                throw new JghSeasonDataFile404Exception("Unable proceed. FragmentInFileNameOfAssociatedProfileFile is null or empty.");

            #region get the Season profile document associated with the given FragmentInFileNameOfAssociatedProfileFile

            var entryPoint = ConnectionStringRepository.GetStorageHierarchyEntryPoint();

            var seasonIdBlobName = string.Concat(AzureStorageObjectNames.SeasonProfileFilenamePrefix, profileFileNameFragment, AzureStorageObjectNames.SeasonProfileFileNameExtension);

            var seasonIdDocumentAsBytesAsBinary = await
                _azureStorageServiceMethodsHelperInstance.DownloadBlockBlobAsBytesAsync(
                    entryPoint.AzureStorageAccountName,
                    entryPoint.AzureStorageContainerName,
                    seasonIdBlobName);

            var seasonIdDocumentAsBytes = seasonIdDocumentAsBytesAsBinary.ToArray();

            var seasonIdDocumentAsJson = JghConvert.ToStringFromUtf8Bytes(seasonIdDocumentAsBytes);

            var seasonItem = JghSerialisation.ToObjectFromJson<SeasonProfileDto>(seasonIdDocumentAsJson);


            #endregion

            #region get one or more targeted series settings Season documents and load up the seasonData with them

            if (IsManifestlyInvalidSeasonItemDocument(seasonItem, out var errorMessage))
            {
                throw new
                    Jgh404Exception(
                        JghString.ConcatAsParagraphs($"FragmentInFileNameOfAssociatedProfileFile document is deficient. Unable to proceed. <{profileFileNameFragment}>",
                            errorMessage));
            }

            var populatedTargetItems = await PopulateSeasonItemWithSeriesItemDataTransferObjects(seasonItem, failure, locus);

            #endregion

            seasonItem.SeriesProfileCollection = populatedTargetItems;


            return seasonItem;
        }


        /// <summary>
        ///     Returns shallow copies of all SeasonProfileItem documents in
        ///     the StorageHierarchyEntryPoint container, each of them
        ///     exclusive of constituent Series Season
        /// </summary>
        /// <returns></returns>
        public async Task<SeasonProfileDto[]> GetAllSeasonProfilesAsync(CancellationToken ct)
        {
            //const string failure = "Unable to do what this method does.";
            //const string locus = "[GetAllSeasonProfilesAsync]";

            var entryPoint = ConnectionStringRepository.GetStorageHierarchyEntryPoint();

            if (entryPoint == null || string.IsNullOrWhiteSpace(entryPoint.AzureStorageAccountName))
                throw new JghAzureRequestException(JghString.ConcatAsSentences(StringsRezultzSvc.ServiceIntervenedMsg,
                    StringsRezultzSvc.AccountNameUnauthorisedMsg));

            var accountConnectionString = await ConnectionStringRepository.GetAzureStorageAccountConnectionStringAsync(entryPoint.AzureStorageAccountName);

            var blobServiceClient = new BlobServiceClient(accountConnectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(entryPoint.AzureStorageContainerName); //append container name to URI

            var asyncPageableBlobItems = blobContainerClient.GetBlobsAsync(); // todo add ct here

            var listOfDataTransferObjects = new List<SeasonProfileDto>();

            await foreach (var blobItem in asyncPageableBlobItems)
            {
                if (!blobItem.Name.EndsWith(".json")) continue; // belt and braces. to eliminate rogue contents of container

                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name); // append blob name to URI

                var blobDownloadInfo = await blobClient.DownloadAsync(ct);

                var contentAsString = await JghStreamHandlers.ReadStreamToStringAsync(blobDownloadInfo.Value.Content); // presumes this blob is a valid json string!!

                var theSeasonItemDataTransferObject = JghSerialisation.ToObjectFromJson<SeasonProfileDto>(contentAsString);

                if (theSeasonItemDataTransferObject == null) continue;

                listOfDataTransferObjects.Add(theSeasonItemDataTransferObject);
            }

            return listOfDataTransferObjects.ToArray();
        }

        /// <summary>
        ///     Returns serialised EventItem inclusive of constituent ArrayOfResultItemForEvent[]
        /// </summary>
        /// <param name="eventProfileDto"></param>
        /// <returns></returns>
        public async Task<EventProfileDto> PopulateSingleEventWithResultsAsync(EventProfileDto eventProfileDto)
        {
            if (eventProfileDto == null)
                throw new JghSeasonDataFile404Exception("Unable proceed. EventItem is null.");

            var populatedEventItem = await InsertPreprocessedResultsIntoEventItemAsync(eventProfileDto);

            return populatedEventItem;
        }

        /// <summary>
        ///     Returns serialised SeriesItem inclusive of all constituent results for all events.
        ///     This version of the method does the remote I/O concurrently.
        /// </summary>
        /// <param name="seriesProfileDto"></param>
        /// <returns></returns>
        public async Task<SeriesProfileDto> PopulateAllEventsInSingleSeriesWithAllResultsAsync(SeriesProfileDto seriesProfileDto)
        {
            if (seriesProfileDto == null)
                throw new JghSeasonDataFile404Exception("Unable proceed. SeriesItem is null.");

            List<EventProfileDto> allEventsInSeries = new();
            List<Task<EventProfileDto>> allEventsInSeriesTasks = new();

            foreach (var thisEventItem in seriesProfileDto.EventProfileCollection
                         .Where(z => z != null))
            {

                var advertisedDateOfEvent = DateTime.TryParse(thisEventItem.AdvertisedDateAsString, out var dateTime) ? dateTime.Date : DateTime.Today;

                if (advertisedDateOfEvent <= DateTime.Now)
                {
                    var thisPopulatedEventItem = InsertPreprocessedResultsIntoEventItemAsync(thisEventItem);

                    allEventsInSeriesTasks.Add(thisPopulatedEventItem);
                }
                else
                {
                    // it's in the future. do nothing

                    allEventsInSeries.Add(thisEventItem);
                }
            }

            var xx = await JghParallel.WhenAllAsync(allEventsInSeriesTasks.ToArray());

            allEventsInSeries.AddRange(xx);

            allEventsInSeries = allEventsInSeries.OrderBy(z => z.AdvertisedDateAsString).ToList();

            seriesProfileDto.EventProfileCollection = allEventsInSeries.ToArray();


            return seriesProfileDto;
        }


        #endregion

        #region helpers

        private async Task<SeriesProfileDto[]> PopulateSeasonItemWithSeriesItemDataTransferObjects(SeasonProfileDto seasonData, string failure, string locus)
        {
            List<SeriesProfileDto> populatedTargetItemsAsList = new();

            foreach (var seriesSeasonDocumentTarget in seasonData.SeriesProfileFileLocationCollection)
            {
                try
                {
                    var xx = await GetSeriesItemAsync(seriesSeasonDocumentTarget, failure, locus);

                    if (xx != null)
                    {
                        populatedTargetItemsAsList.Add(xx);
                    }
                }
                catch 
                {

                    // do nothing
                }
            }

            return populatedTargetItemsAsList.ToArray();
        }

        private async Task<SeriesProfileDto> GetSeriesItemAsync(EntityLocationDto databaseOfSeriesItemDocument, string failure, string locus)
        {
            if (databaseOfSeriesItemDocument == null) throw new ArgumentNullException(nameof(databaseOfSeriesItemDocument));

            try
            {
                //     Throws JghCommunicationFailureException as an inner exception if request fails and/or
                //     resource not obtained for any reason whatsoever including an invalidly formatted uri.

                var seriesSettingsDocumentAsBytesAsBinary = await _azureStorageServiceMethodsHelperInstance.DownloadBlockBlobAsBytesAsync(
                    databaseOfSeriesItemDocument.AccountName,
                    databaseOfSeriesItemDocument.ContainerName,
                    databaseOfSeriesItemDocument.EntityName);

                var seriesSettingsDocumentAsBytes = seriesSettingsDocumentAsBytesAsBinary.ToArray();

                var json = JghConvert.ToStringFromUtf8Bytes(seriesSettingsDocumentAsBytes);

                var answer = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(json);

                return answer;
            }
            catch (Exception ex)
            {
                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<Jgh404Exception>(ex: ex) || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghCommunicationFailureException>(ex: ex))
                {
                    var e = JghExceptionHelpers.FindInnermostException(ex: ex);

                    ex = new JghSettingsData404Exception(
                        message: JghString.ConcatAsParagraphs(
                            $"Unable to find expected series settings document named <{databaseOfSeriesItemDocument.EntityName}>.",
                            e.Message));

                    throw JghExceptionHelpers.ConvertToCarrier(failureDescription: failure, locusDescription: locus, locus2Description: Locus2, locus3Description: Locus3, ex: ex);
                }

                throw JghExceptionHelpers.ConvertToCarrier(failureDescription: failure, locusDescription: locus, locus2Description: Locus2, locus3Description: Locus3, ex: ex);
            }

        }

        private async Task<EventProfileDto> InsertPreprocessedResultsIntoEventItemAsync(EventProfileDto eventDataTransferObject)
        {
            var resultsForThisEvent = new List<ResultDto>();

            var arrayOfPreprocessedResultsDataItemNames = eventDataTransferObject.XmlFileNamesForPublishedResults.Split(',');

            foreach (string dataItemName in arrayOfPreprocessedResultsDataItemNames.Where(z =>  !string.IsNullOrWhiteSpace(z)))
            {
                var blobDoesExist = await _azureStorageServiceMethodsHelperInstance.GetIfBlobExistsAsync(
                    eventDataTransferObject.DatabaseAccountName,
                    eventDataTransferObject.DataContainerName,
                    dataItemName);


                if (!blobDoesExist) continue;

                var resultsDocumentInUnknownFormatAsBinary = await
                    _azureStorageServiceMethodsHelperInstance.DownloadBlockBlobAsBytesAsync(
                        eventDataTransferObject.DatabaseAccountName,
                        eventDataTransferObject.DataContainerName,
                        dataItemName);

                var resultsDocumentInUnknownFormat = resultsDocumentInUnknownFormatAsBinary.ToArray();


                ResultDto[] arrayOfResultInThisBlob;

                if (dataItemName.EndsWith(".xml"))
                {
                    arrayOfResultInThisBlob =
                        JghSerialisation.ToObjectFromXml<ResultDto[]>(JghConvert.ToStringFromUtf8Bytes(resultsDocumentInUnknownFormat),
                            new[] { typeof(ResultDto[]) });
                }
                else
                {
                    arrayOfResultInThisBlob =
                        JghSerialisation.ToObjectFromJson<ResultDto[]>(JghConvert.ToStringFromUtf8Bytes(resultsDocumentInUnknownFormat));
                }

                resultsForThisEvent.AddRange(arrayOfResultInThisBlob);
            }

            eventDataTransferObject.PublishedResultsForEvent = resultsForThisEvent.ToArray();


            return eventDataTransferObject;
        }

        private bool IsManifestlyInvalidSeasonItemDocument(SeasonProfileDto thisSeasonData, out string errorMessage)
        {

            var sb = new StringBuilder();

            if (thisSeasonData == null)
            {
                sb.Append("FragmentInFileNameOfAssociatedProfileFile is null. This is a database error.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(thisSeasonData.Title))
                {
                    sb.AppendLine("SeasonProfileItem.Title is null. This property is mandatory.");
                }

                if (string.IsNullOrWhiteSpace(thisSeasonData.Label))
                {
                    sb.AppendLine("SeasonProfileItem.Label is null. This property is mandatory.");
                }

                if (string.IsNullOrWhiteSpace(thisSeasonData.Organizer?.Title))
                {
                    sb.AppendLine("Organizer.Title is null. This property is mandatory.");
                }

                if (string.IsNullOrWhiteSpace(thisSeasonData.Organizer?.Label))
                {
                    sb.AppendLine("Organizer.Label is null. This property is mandatory.");
                }

                foreach (var item in thisSeasonData.SeriesProfileFileLocationCollection)
                {
                    if (!JghFilePathValidator.IsValidBlobLocationSpecification(
                            item.AccountName,
                            item.ContainerName,
                            item.EntityName,
                            out var errorDescription))
                    {

                        sb.AppendLine("Specification of EntityLocation in SeriesProfileFileLocationCollection is deficient." + " ");
                        sb.Append(errorDescription);
                    }
                }
            }


            if (sb.Length <= 0)
            {
                errorMessage = string.Empty;
                return false;
            }

            errorMessage = sb.ToString();

            return true;

        }

        #endregion

    }
}