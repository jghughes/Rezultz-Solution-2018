using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        #region fields

        private readonly AzureStorageServiceMethodsHelper _azureStorageServiceMethodsHelperInstance;

        #endregion

        #region ctor

        public LeaderboardResultsServiceMethodsHelper()
    {
        _azureStorageServiceMethodsHelperInstance = new AzureStorageServiceMethodsHelper(new AzureStorageAccessor());
    }

        #endregion

        #region svc methods

        public async Task<bool> GetIfSeasonIdIsRecognisedAsync(string profileFileNameFragment)
    {
        var failure = "Unable to do what this method does";
        const string locus = nameof(GetIfSeasonIdIsRecognisedAsync);

        try
        {
            if (string.IsNullOrEmpty(profileFileNameFragment))
                return false;

            var entryPoint = ConnectionStringRepository.GetStorageHierarchyEntryPoint();

            var seasonIdBlobName = string.Concat(AzureStorageObjectNames.SeasonProfileFilenamePrefix, profileFileNameFragment, AzureStorageObjectNames.SeasonProfileFileNameExtension);

            var answer = await _azureStorageServiceMethodsHelperInstance.GetIfBlobExistsAsync(entryPoint.AzureStorageAccountName, entryPoint.AzureStorageContainerName, seasonIdBlobName);

            return answer;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        /// <summary>
        ///     Returns deep copy of SeasonProfileItem document, inclusive of constituent Series data
        /// </summary>
        /// <param name="profileFileNameFragment"></param>
        /// <returns></returns>
        public async Task<SeasonProfileDto> GetSeasonProfileAsync(string profileFileNameFragment)
    {
        var failure = "Unable to do what this method does";
        const string locus = nameof(GetSeasonProfileAsync);

        try
        {
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
                throw new
                    Jgh404Exception(
                        JghString.ConcatAsParagraphs($"FragmentInFileNameOfAssociatedProfileFile document is deficient. Unable to proceed. <{profileFileNameFragment}>",
                            errorMessage));

            var populatedTargetItems = await PopulateSeasonItemWithSeriesItemDataTransferObjects(seasonItem, failure, locus);

            #endregion

            seasonItem.SeriesProfileCollection = populatedTargetItems;


            return seasonItem;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }


        /// <summary>
        ///     Returns shallow copies of all SeasonProfileItem documents in
        ///     the StorageHierarchyEntryPoint container, each of them
        ///     exclusive of constituent Series Season
        /// </summary>
        /// <returns></returns>
        public async Task<SeasonProfileDto[]> GetAllSeasonProfilesAsync(CancellationToken ct)
    {
        var failure = "Unable to do what this method does";
        const string locus = nameof(GetAllSeasonProfilesAsync);

        try
        {
            var entryPoint = ConnectionStringRepository.GetStorageHierarchyEntryPoint();

            if (entryPoint is null || string.IsNullOrWhiteSpace(entryPoint.AzureStorageAccountName))
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

                if (theSeasonItemDataTransferObject is null) continue;

                listOfDataTransferObjects.Add(theSeasonItemDataTransferObject);
            }

            return listOfDataTransferObjects.ToArray();
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        /// <summary>
        ///     Returns serialised EventItem inclusive of constituent ArrayOfResultItemForEvent[]
        /// </summary>
        /// <param name="eventProfileDto"></param>
        /// <returns></returns>
        public async Task<EventProfileDto> PopulateSingleEventWithResultsAsync(EventProfileDto eventProfileDto)
    {
        var failure = "Unable to do what this method does";
        const string locus = nameof(PopulateSingleEventWithResultsAsync);

        try
        {
            if (eventProfileDto is null)
                throw new JghSeasonDataFile404Exception("Unable proceed. EventItem is null.");

            var populatedEventItem = await InsertPreprocessedResultsIntoEventItemAsync(eventProfileDto);

            return populatedEventItem;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        /// <summary>
        ///     Returns serialised SeriesItem inclusive of all constituent results for all events.
        ///     This version of the method does the remote I/O concurrently.
        /// </summary>
        /// <param name="seriesProfileDto"></param>
        /// <returns></returns>
        public async Task<SeriesProfileDto> PopulateAllEventsInSingleSeriesWithAllResultsAsync(SeriesProfileDto seriesProfileDto)
    {
        var failure = "Unable to do what this method does";
        const string locus = nameof(PopulateAllEventsInSingleSeriesWithAllResultsAsync);

        try
        {
            if (seriesProfileDto is null)
                throw new JghSeasonDataFile404Exception("Unable proceed. SeriesItem is null.");

            List<EventProfileDto> allEventsInSeries = [];
            List<Task<EventProfileDto>> allEventsInSeriesTasks = [];

            foreach (var thisEventItem in seriesProfileDto.EventProfileCollection
                         .Where(z => z is not null))
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

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region helpers

        private async Task<SeriesProfileDto[]> PopulateSeasonItemWithSeriesItemDataTransferObjects(SeasonProfileDto seasonData, string failure, string locus)
    {
        try
        {
            List<SeriesProfileDto> populatedTargetItemsAsList = [];

            foreach (var seriesSeasonDocumentTarget in seasonData.SeriesProfileFileLocationCollection)
                try
                {
                    var xx = await GetSeriesItemAsync(seriesSeasonDocumentTarget, failure, locus);

                    if (xx is not null) populatedTargetItemsAsList.Add(xx);
                }
                catch
                {
                    // do nothing
                }

            return populatedTargetItemsAsList.ToArray();
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        private async Task<SeriesProfileDto> GetSeriesItemAsync(EntityLocationDto databaseOfSeriesItemDocument, string failure, string locus)
    {
        if (databaseOfSeriesItemDocument is null) throw new ArgumentNullException(nameof(databaseOfSeriesItemDocument));

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
            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<Jgh404Exception>(ex) || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghCommunicationFailureException>(ex))
            {
                var e = JghExceptionHelpers.FindInnermostException(ex);

                ex = new JghSettingsData404Exception(
                    JghString.ConcatAsParagraphs(
                        $"Unable to find expected series settings document named <{databaseOfSeriesItemDocument.EntityName}>.",
                        e.Message));

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        private async Task<EventProfileDto> InsertPreprocessedResultsIntoEventItemAsync(EventProfileDto eventDataTransferObject)
    {
        var failure = "Unable to do what this method does";
        const string locus = nameof(InsertPreprocessedResultsIntoEventItemAsync);

        try
        {
            var resultsForThisEvent = new List<ResultDto>();

            var arrayOfPreprocessedResultsDataItemNames = eventDataTransferObject.XmlFileNamesForPublishedResults.Split(',');

            foreach (var dataItemName in arrayOfPreprocessedResultsDataItemNames.Where(z => !string.IsNullOrWhiteSpace(z)))
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


                ResultDto[] arrayOfResultInThisBlob = [];

                if (dataItemName.EndsWith(".xml"))
                {
                    var text = JghConvert.ToStringFromUtf8Bytes(resultsDocumentInUnknownFormat);

                    arrayOfResultInThisBlob = JghSerialisation.ToObjectFromXml<ResultDto[]>(text, [typeof(ResultDto)]); //Note: this will throw if the xml is not well formed and if the container of repeating elements is not named as expected by the DataContractSerializer i.e. "ArrayOf" + the name of the repeating element.

                    //var xx = XDocument.Parse(text);

                    //var zz = xx.Element(ResultDto.XeRootForContainerOfSimpleStandAloneArray)?.Elements(ResultDto.XeResult);

                    //if(zz!=null)
                    //    arrayOfResultInThisBlob = zz.Select(z => JghSerialisation.ToObjectFromXml<ResultDto>(z.ToString(), new[] { typeof(ResultDto) })).ToArray();
                }
                else
                {
                    var text = JghConvert.ToStringFromUtf8Bytes(resultsDocumentInUnknownFormat);

                    // todo - add json handling here

                }

                resultsForThisEvent.AddRange(arrayOfResultInThisBlob);
            }

            eventDataTransferObject.PublishedResultsForEvent = resultsForThisEvent.ToArray();


            return eventDataTransferObject;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        private bool IsManifestlyInvalidSeasonItemDocument(SeasonProfileDto thisSeasonData, out string errorMessage)
    {
        var failure = "Unable to do what this method does";
        const string locus = nameof(PopulateSingleEventWithResultsAsync);

        try
        {
            var sb = new StringBuilder();

            if (thisSeasonData is null)
            {
                sb.Append("FragmentInFileNameOfAssociatedProfileFile is null. This is a database error.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(thisSeasonData.Title)) sb.AppendLine("SeasonProfileItem.Title is null. This property is mandatory.");

                if (string.IsNullOrWhiteSpace(thisSeasonData.Label)) sb.AppendLine("SeasonProfileItem.Label is null. This property is mandatory.");

                if (string.IsNullOrWhiteSpace(thisSeasonData.Organizer?.Title)) sb.AppendLine("Organizer.Title is null. This property is mandatory.");

                if (string.IsNullOrWhiteSpace(thisSeasonData.Organizer?.Label)) sb.AppendLine("Organizer.Label is null. This property is mandatory.");

                foreach (var item in thisSeasonData.SeriesProfileFileLocationCollection)
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


            if (sb.Length <= 0)
            {
                errorMessage = string.Empty;
                return false;
            }

            errorMessage = sb.ToString();

            return true;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion
    }
}