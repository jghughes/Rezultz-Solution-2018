using System.Threading;
using System.Threading.Tasks;
using NetStd.DataTypes.Mar2024;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace NetStd.Interfaces03.Apr2022;

public interface IRaceResultsPublishingSvcAgent : ISvcAgentBase
{
    public Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment, CancellationToken ct = default);

    public Task<PublisherModuleProfileItem> GetPublishingProfileAsync(string fileNameFragment, CancellationToken ct = default);

    public Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync(CancellationToken ct = default);

    public Task<string> GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(string entityFileName, CancellationToken ct = default);

    public Task<PublisherOutputItem> GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(string fileNameFragmentOfAssociatedPublishingProfile, string seriesLabelAsEventIdentifier, string eventLabelAsEventIdentifier,
        SeriesProfileItem seriesProfile, PublisherImportFileTargetItem[] filesToBeFetchedForProcessing, CancellationToken ct = default);

    public Task<bool> UploadDatasetFileToBeProcessedSubsequentlyAsync(string identifierOfDataset, EntityLocationItem storageLocation, string datasetAsRawString, CancellationToken ct = default);

    public Task<bool> UploadFileOfCompletedResultsForSingleEventAsync(EntityLocationItem storageLocation, string datasetAsRawString, CancellationToken ct = default);

    #region old

    //public Task<bool> GetIfConversionModuleSpecificationFilenameIsRecognisedAsync(string nameOfXmlSpecificationFileEnteredByUser, CancellationToken ct = default);

    //public Task<string[]> GetConversionModuleSpecificationFileNamesAsync(CancellationToken ct = default);

    //public Task<PublisherModuleProfileItem> GetConversionModuleSpecificationItemAsync(string nameOfXmlSpecificationFileEnteredByUser, CancellationToken ct = default);

    //public Task<PublisherModuleProfileItem[]> GetConversionModuleSpecificationItemArrayAsync(CancellationToken ct);

    //public Task<ComputerOutputItem> ConvertDatasetsToArrayOfResultItemDataTransferObjectsAsync(
    //    ComputerDatasetTargetItem[] arrayOfDatasetToBeProcessed,
    //    PublisherModuleProfileItem computerProfileItem,
    //    SeriesProfileItem seriesProfileItem,
    //    string eventLabelServingAsEventIdentifier,
    //    CancellationToken ct = default);

    #endregion
}