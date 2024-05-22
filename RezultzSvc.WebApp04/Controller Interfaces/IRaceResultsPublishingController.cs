using Microsoft.AspNetCore.Mvc;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using RezultzSvc.WebApp04.Controller_Base;

// ReSharper disable InconsistentNaming

namespace RezultzSvc.WebApp04.Controller_Interfaces
{
    public interface IRaceResultsPublishingController : IControllerBaseJgh
    {
        Task<IActionResult> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment);

        Task<IActionResult> GetPublisherModuleProfileItemAsync(string fileNameFragment);

        Task<IActionResult> GetFileNameFragmentsOfAllPublishingProfilesAsync();

        Task<IActionResult> GetIllustrativeExampleOfDatasetExpectedForProcessingAsync(string fileNameWithExtension);

        Task<IActionResult> UploadFileToBeProcessedSubsequentlyAsync(string identifierOfDataset, string accountName, string containerName, string datasetFileName, string datasetAsRawString);

        Task<IActionResult> ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(PublisherInputItemDto publisherInputItemDto);

        Task<IActionResult> UploadFileOfCompletedResultsForSingleEventAsync(string accountName, string containerName, string datasetFileNameWithExtension, string completedResultsAsRawStringOfXml);
    }
}
