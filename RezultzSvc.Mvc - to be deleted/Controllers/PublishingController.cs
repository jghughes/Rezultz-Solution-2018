using Jgh.MvcParameters.Mar2024;
using Microsoft.AspNetCore.Mvc;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using RezultzSvc.Library02.Mar2024.SvcHelpers;
using RezultzSvc.Mvc.Bases;
using RezultzSvc.Mvc.Controller_Base;
using RezultzSvc.Mvc.Controller_Interfaces;



//[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
//[ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
//[ProducesResponseType(StatusCodes.Status202Accepted)]
//[ProducesResponseType(StatusCodes.Status204NoContent)]
//[ProducesResponseType(StatusCodes.Status302Found)]
//[ProducesResponseType(StatusCodes.Status304NotModified)]
//[ProducesResponseType(StatusCodes.Status400BadRequest)]
//[ProducesResponseType(StatusCodes.Status401Unauthorized)]
//[ProducesResponseType(StatusCodes.Status403Forbidden)]
//[ProducesResponseType(StatusCodes.Status404NotFound)]
//[ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
//[ProducesResponseType(StatusCodes.Status500InternalServerError)]
//[ProducesResponseType(StatusCodes.Status501NotImplemented)]
//[ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]

namespace RezultzSvc.Mvc.Controllers;

/// <summary>
///     A REST API is best developed in a service-first manner. Use hard coded strings for the names of
///     ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
///     endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
///     from Swagger into the URI strings used in your REST client to make HTTP calls to the API.
///     The inspiration for this service comes from Microsoft's 2017 reference project on Github.
///     https://github.com/microsoft/SmartHotel360-Backend/blob/master/Source/Backend/src/SmartHotel.Services.Bookings/Controllers/BookingsController.cs
/// </summary>
[ApiController]
[Route(Routes.PublishingController)]
public class PublishingController : ControllerBaseJgh, IPublishingController
{
    #region ctor

    public PublishingController(ILogger<PublishingController> logger)
    {
        Logger1 = logger;

        _publishingServiceMethodsHelper = new PublishingServiceMethodsHelper();
    }

    #endregion

    #region fields

    public ILogger<PublishingController> Logger1 { get; }

    private readonly PublishingServiceMethodsHelper _publishingServiceMethodsHelper;

    #endregion

    #region actions

    [HttpGet(Routes.GetIfFileNameOfPublishingProfileIsRecognised)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetIfFileNameOfPublishingProfileIsRecognisedAsync([FromQuery(Name = QueryParameters.FileNameFragment)] string profileFileNameFragment)
    {
        try
        {
            var isFound = await _publishingServiceMethodsHelper.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(profileFileNameFragment);

            var boolAsJsonAsCompressedBytesBack = await JghCompressionHelper.ConvertObjectToJsonAsCompressedBytesAsync(isFound);

            return Ok(boolAsJsonAsCompressedBytesBack);
        }
        catch (Exception ex)
        {
            return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
        }
    }

    [HttpGet(Routes.GetPublishingProfile)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPublishingProfileAsync([FromQuery(Name = QueryParameters.FileNameFragment)] string profileFileNameFragment)
    {
        try
        {
            var publisherProfileItem = await _publishingServiceMethodsHelper.GetPublishingProfileAsync(profileFileNameFragment);

            var publisherProfileDtoAsJsonAsCompressedBytesBack = await JghCompressionHelper.ConvertObjectToJsonAsCompressedBytesAsync(PublisherModuleProfileItem.ToDataTransferObject(publisherProfileItem));

            return Ok(publisherProfileDtoAsJsonAsCompressedBytesBack);
               
        }
        catch (Exception ex)
        {
            return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
        }
    }

    [HttpGet(Routes.GetFileNameFragmentsOfAllPublishingProfiles)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFileNameFragmentsOfAllPublishingProfilesAsync()
    {
        try
        {
            var fileNameFragmentsAsStringArray = await _publishingServiceMethodsHelper.GetFileNameFragmentsOfAllPublishingProfilesAsync();

            var fileNameFragmentsAsStringArrayAsJsonAsCompressedBytesBack = await JghCompressionHelper.ConvertObjectToJsonAsCompressedBytesAsync(fileNameFragmentsAsStringArray);

            return Ok(fileNameFragmentsAsStringArrayAsJsonAsCompressedBytesBack);
        }
        catch (Exception ex)
        {
            return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
        }
    }

    [HttpGet(Routes.GetIllustrativeExampleOfDatasetExpectedForProcessing)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetIllustrativeExampleOfDatasetExpectedForProcessingAsync([FromQuery(Name = QueryParameters.FileNameFragment)] string exampleFileName)
    {
        try
        {
            var fileContentsAsRawString = await _publishingServiceMethodsHelper.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(exampleFileName);

            var fileContentsAsStringAsCompressedBytesBack = await JghCompression.CompressAsync(JghConvert.ToBytesUtf8FromString(fileContentsAsRawString));

            return Ok(fileContentsAsStringAsCompressedBytesBack);
        }
        catch (Exception ex)
        {
            return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
        }
    }

    [HttpGet(Routes.UploadDatasetToBeProcessedSubsequently)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadDatasetToBeProcessedSubsequentlyAsync([FromQuery(Name = QueryParameters.DatasetIdentifier)] string identifierOfDataset,
        [FromQuery(Name = QueryParameters.AccountName)] string accountName,
        [FromQuery(Name = QueryParameters.ContainerName)] string containerName,
        [FromQuery(Name = QueryParameters.FileNameFragment)] string datasetFileName,
        [FromBody] byte[] datasetAsRawStringAsCompressedBytes)
    {
        var datasetAsStringAsBytes = await JghCompression.DecompressAsync(datasetAsRawStringAsCompressedBytes);

        var didSucceed = await _publishingServiceMethodsHelper.UploadDatasetAsBytesAsync(accountName, containerName, datasetFileName, datasetAsStringAsBytes);

        var boolAsJsonAsCompressedBytesBack = await JghCompressionHelper.ConvertObjectToJsonAsCompressedBytesAsync(didSucceed);

        return Ok(boolAsJsonAsCompressedBytesBack);
    }

    [HttpGet(Routes.ProcessPreviouslyUploadedDatasetsIntoResultsForSingleEvent)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPreviouslyUploadedDatasetsIntoResultsForSingleEventAsync([FromBody] byte[] publisherInputDtoAsJsonAsCompressedBytes)
    {
        try
        {
            var publisherInputItem = PublisherInputItem.FromDataTransferObject(await JghCompressionHelper.ConvertJsonAsCompressedBytesToObjectAsync<PublisherInputDto>(publisherInputDtoAsJsonAsCompressedBytes));

            var publisherOutputItem = await _publishingServiceMethodsHelper.ConvertPreviouslyUploadedDatasetsToResultsForSingleEventAsync(publisherInputItem);

            var publisherOutputDtoAsJsonAsCompressedBytesBack = await JghCompressionHelper.ConvertObjectToJsonAsCompressedBytesAsync(PublisherOutputItem.ToDataTransferObject(publisherOutputItem));
            
            return Ok(publisherOutputDtoAsJsonAsCompressedBytesBack);
        }
        catch (Exception ex)
        {
            return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
        }
    }

    [HttpGet(Routes.UploadFileOfCompletedResultsForSingleEvent)]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadFileOfCompletedResultsForSingleEventAsync(
        [FromQuery(Name = QueryParameters.AccountName)] string accountName,
        [FromQuery(Name = QueryParameters.ContainerName)] string containerName,
        [FromQuery(Name = QueryParameters.FileNameFragment)] string datasetFileName,
        [FromBody] byte[] completedResultsAsXmlFileContentsAsCompressedBytes)
    {

        var didSucceed = await _publishingServiceMethodsHelper.UploadDatasetAsBytesAsync(accountName, containerName, datasetFileName, await JghCompression.DecompressAsync(completedResultsAsXmlFileContentsAsCompressedBytes));

        var boolAsJsonAsCompressedBytesBack = await JghCompressionHelper.ConvertObjectToJsonAsCompressedBytesAsync(didSucceed);

        return Ok(boolAsJsonAsCompressedBytesBack);
    }

    #endregion
}