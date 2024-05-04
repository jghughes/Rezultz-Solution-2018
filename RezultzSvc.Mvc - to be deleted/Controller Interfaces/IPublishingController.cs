using Microsoft.AspNetCore.Mvc;
using RezultzSvc.Mvc.Bases;

// ReSharper disable InconsistentNaming

namespace RezultzSvc.Mvc.Controller_Interfaces
{
    public interface IPublishingController : IControllerBaseJgh
    {
        Task<IActionResult> GetIfFileNameOfPublishingProfileIsRecognisedAsync(string profileFileNameFragment);

        Task<IActionResult> GetPublishingProfileAsync(string profileFileNameFragment);

        Task<IActionResult> GetFileNameFragmentsOfAllPublishingProfilesAsync();

        Task<IActionResult> GetIllustrativeExampleOfDatasetExpectedForProcessingAsync(string exampleFileName);

        Task<IActionResult> UploadDatasetToBeProcessedSubsequentlyAsync(string identifierOfDataset, string accountName, string containerName, string datasetFileName, byte[] datasetAsRawStringAsCompressedBytes);

        Task<IActionResult> ProcessPreviouslyUploadedDatasetsIntoResultsForSingleEventAsync(byte[] publisherInputDtoAsJsonAsCompressedBytes);

        Task<IActionResult> UploadFileOfCompletedResultsForSingleEventAsync(string accountName, string containerName, string fileName, byte[] completedResultsAsXmlFileContentsAsCompressedBytes);
    }
}
