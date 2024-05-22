using Jgh.MvcParameters.Mar2024;
using Microsoft.AspNetCore.Mvc;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using RezultzSvc.Library02.Mar2024.SvcHelpers;
using RezultzSvc.WebApp04.Controller_Base;
using RezultzSvc.WebApp04.Controller_Interfaces;


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

namespace RezultzSvc.WebApp04.Controllers;

/// <summary>
///     A REST API is best developed in a service-first manner. Use hard coded strings for the names of
///     ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
///     endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
///     from Swagger into the URI strings used in your REST client to make HTTP calls to the API.
///     The inspiration for this service comes from Microsoft's 2017 reference project on Github.
///     https://github.com/microsoft/SmartHotel360-Backend/blob/master/Source/Backend/src/SmartHotel.Services.Bookings/Controllers/BookingsController.cs
/// </summary>
[ApiController]
[Produces("application/json")]
[Route(Routes.RaceResultsPublishingController)]
public class RaceResultsPublishingController : ControllerBaseJgh, IRaceResultsPublishingController
{
    #region ctor

    public RaceResultsPublishingController(ILogger<RaceResultsPublishingController> logger)
    {
        Logger1 = logger;

        _raceResultsPublishingServiceMethodsHelper = new RaceResultsPublishingServiceMethodsHelper();
    }

    #endregion

    #region fields

    public ILogger<RaceResultsPublishingController> Logger1 { get; }

    private readonly RaceResultsPublishingServiceMethodsHelper _raceResultsPublishingServiceMethodsHelper;

    #endregion

    #region actions

    [HttpGet(Routes.GetIfPublisherIdIsRecognised)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync([FromQuery(Name = QueryParameters.FileNameFragment)] string fileNameFragment)
    {
        try
        {
            var isFound = await _raceResultsPublishingServiceMethodsHelper.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(fileNameFragment);

            return StatusCode(200, isFound);
        }
        catch (Exception ex)
        {
            var innermostException = JghExceptionHelpers.FindInnermostException(ex);

            switch (innermostException)
            {
                case JghAlertMessageException:
                    return StatusCode(400, innermostException.Message);
                default:
                    return StatusCode(500, ExceptionDto.FromException(ex));
            }
        }
    }

    [HttpGet(Routes.GetPublisherModuleProfile)]
    [ProducesResponseType(typeof(PublisherModuleProfileItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPublisherModuleProfileItemAsync([FromQuery(Name = QueryParameters.FileNameFragment)] string fileNameFragment)
    {
        try
        {
            var publisherProfileItem = await _raceResultsPublishingServiceMethodsHelper.GetPublisherModuleProfileItemAsync(fileNameFragment);

            var publisherProfileItemDto = PublisherModuleProfileItem.ToDataTransferObject(publisherProfileItem);

            return StatusCode(200, publisherProfileItemDto);
        }
        catch (Exception ex)
        {
            var innermostException = JghExceptionHelpers.FindInnermostException(ex);

            switch (innermostException)
            {
                case JghAlertMessageException:
                {
                    return StatusCode(400, innermostException.Message);
                }
                default:
                    return StatusCode(500, ExceptionDto.FromException(ex));
            }
        }
    }

    [HttpGet(Routes.GetAllPublisherModuleId)]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFileNameFragmentsOfAllPublishingProfilesAsync()
    {
        try
        {
            var fileNameFragmentsAsStringArray = await _raceResultsPublishingServiceMethodsHelper.GetFileNameFragmentsOfAllPublishingProfilesAsync();

            return StatusCode(200, fileNameFragmentsAsStringArray);
        }
        catch (Exception ex)
        {
            var innermostException = JghExceptionHelpers.FindInnermostException(ex);

            switch (innermostException)
            {
                case JghAlertMessageException:
                {
                    return StatusCode(400, innermostException.Message);
                }
                default:
                    return StatusCode(500, ExceptionDto.FromException(ex));
            }
        }
    }

    [HttpGet(Routes.GetIllustrativeExampleOfRawDataset)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIllustrativeExampleOfDatasetExpectedForProcessingAsync([FromQuery(Name = QueryParameters.FileNameWithExtension)] string fileNameWithExtension)
    {
        try
        {
            var fileContentsAsRawString = await _raceResultsPublishingServiceMethodsHelper.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(fileNameWithExtension);

            return StatusCode(200, fileContentsAsRawString);
        }
        catch (Exception ex)
        {
            var innermostException = JghExceptionHelpers.FindInnermostException(ex);

            switch (innermostException)
            {
                case JghAlertMessageException:
                {
                    return StatusCode(400, innermostException.Message);
                }
                default:
                    return StatusCode(500, ExceptionDto.FromException(ex));
            }
        }
    }

    [HttpPost(Routes.ComputeResultsForSingleEvent)]
    [ProducesResponseType(typeof(PublisherOutputItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync([FromBody] PublisherInputItemDto publisherInputItemDto)
    {
        try
        {
            var publisherInputItem = PublisherInputItem.FromDataTransferObject(publisherInputItemDto);

            var publisherOutputItem = await _raceResultsPublishingServiceMethodsHelper.ConvertPreviouslyUploadedDatasetsToResultsForSingleEventAsync(publisherInputItem);

            var publisherOutputItemDto = PublisherOutputItem.ToDataTransferObject(publisherOutputItem);

            return StatusCode(200, publisherOutputItemDto);
        }
        catch (Exception ex)
        {
            var innermostException = JghExceptionHelpers.FindInnermostException(ex);

            switch (innermostException)
            {
                case JghAlertMessageException:
                {
                    return StatusCode(400, innermostException.Message);
                }
                default:
                    return StatusCode(500, ExceptionDto.FromException(ex));
            }
        }
    }

    [HttpPut(Routes.SaveFileToBeProcessedSubsequently)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFileToBeProcessedSubsequentlyAsync([FromQuery(Name = QueryParameters.DatasetIdentifier)] string identifierOfDataset,
        [FromQuery(Name = QueryParameters.Account)] string accountName,
        [FromQuery(Name = QueryParameters.Container)] string containerName,
        [FromQuery(Name = QueryParameters.FileNameWithExtension)] string datasetFileName, [FromBody] string datasetAsRawString)
    {
        try
        {
            var datasetAsStringAsBytes = JghConvert.ToBytesUtf8FromString(datasetAsRawString);

            var didSucceed = await _raceResultsPublishingServiceMethodsHelper.UploadDatasetAsBytesAsync(accountName, containerName, datasetFileName, datasetAsStringAsBytes);

            return await Task.FromResult(Ok(didSucceed));
        }
        catch (Exception ex)
        {
            var innermostException = JghExceptionHelpers.FindInnermostException(ex);

            switch (innermostException)
            {
                case JghAlertMessageException:
                {
                    return StatusCode(400, innermostException.Message);
                }
                default:
                    return StatusCode(500, ExceptionDto.FromException(ex));
            }
        }
    }

    [HttpPut(Routes.PublishFileOfCompletedResultsForSingleEvent)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFileOfCompletedResultsForSingleEventAsync([FromQuery(Name = QueryParameters.Account)] string accountName,
        [FromQuery(Name = QueryParameters.Container)] string containerName,
        [FromQuery(Name = QueryParameters.FileNameWithExtension)] string datasetFileNameWithExtension, [FromBody] string completedResultsAsRawStringOfXml)
    {
        try
        {
            var datasetAsStringAsBytes = JghConvert.ToBytesUtf8FromString(completedResultsAsRawStringOfXml);

            var didSucceed = await _raceResultsPublishingServiceMethodsHelper.UploadDatasetAsBytesAsync(accountName, containerName, datasetFileNameWithExtension, datasetAsStringAsBytes);

            return StatusCode(200, didSucceed);
        }
        catch (Exception ex)
        {
            var innermostException = JghExceptionHelpers.FindInnermostException(ex);

            switch (innermostException)
            {
                case JghAlertMessageException:
                {
                    return StatusCode(400, innermostException.Message);
                }
                default:
                    return StatusCode(500, ExceptionDto.FromException(ex));
            }
        }
    }

    #endregion
}