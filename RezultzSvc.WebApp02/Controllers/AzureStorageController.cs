using Jgh.MvcParameters.Mar2024;
using Microsoft.AspNetCore.Mvc;
using NetStd.AzureStorageAccess.July2018;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using RezultzSvc.Library01.Mar2024.SvcHelpers;
using RezultzSvc.WebApp02.Controller_Base;
using RezultzSvc.WebApp02.Controller_Interfaces;

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

namespace RezultzSvc.WebApp02.Controllers
{
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
    [Route(Routes.AzureStorageController)]
    public class AzureStorageController : ControllerBaseJgh, IAzureStorageController
    {
        #region ctor

        public AzureStorageController(IAzureStorageAccessor azureStorageAccessorInstance, ILogger<AzureStorageController> logger)
        {
            Logger1 = logger;

            _azureStorageServiceMethodsHelperInstance = new AzureStorageServiceMethodsHelper(azureStorageAccessorInstance);
        }

        #endregion

        #region fields

        public ILogger<AzureStorageController> Logger1 { get; }

        private readonly AzureStorageServiceMethodsHelper _azureStorageServiceMethodsHelperInstance;

        #endregion

        #region actions

        [HttpGet(Routes.GetIfContainerExists)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetIfContainerExistsAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container)
        {
            try
            {
                bool answer = await
                    _azureStorageServiceMethodsHelperInstance.GetIfContainerExistsAsync(account, container);

                return StatusCode(200, answer);
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

        [HttpGet(Routes.GetNamesOfBlobsInContainer)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNamesOfBlobsInContainerAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container,
            [FromQuery(Name = QueryParameters.RequiredSubstring)] string requiredSubstring,
            [FromQuery(Name = QueryParameters.MustPrintDescriptionAsOpposedToBlobName)]
            bool mustPrintDescriptionAsOpposedToBlobName)
        {
            try
            {
                var answer = await
                    _azureStorageServiceMethodsHelperInstance.GetNamesOfBlobsInContainerAsync(account, container, requiredSubstring, mustPrintDescriptionAsOpposedToBlobName);

                return StatusCode(200, answer);
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

        [HttpGet(Routes.GetIfBlobExists)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetIfBlobExistsAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container,
            [FromQuery(Name = QueryParameters.Blob)] string blob)
        {
            try
            {
                var answer = await _azureStorageServiceMethodsHelperInstance.GetIfBlobExistsAsync(account, container, blob);

                return StatusCode(200, answer);
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

        [HttpGet(Routes.GetAbsoluteUriOfBlockBlob)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAbsoluteUriOfBlockBlobAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container,
            [FromQuery(Name = QueryParameters.Blob)] string blob)
        {
            try
            {
                string answer = await _azureStorageServiceMethodsHelperInstance.GetAbsoluteUriOfBlockBlobAsync(account, container, blob);

                return StatusCode(200, answer);
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


        [HttpGet(Routes.DeleteBlockBlobIfExists)] // use of the Get is intentional, means the client can use an commonplace Get method to delete a blob, doesn't need  a dedicated Delete method
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBlockBlobIfExistsAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container,
            [FromQuery(Name = QueryParameters.Blob)] string blob)
        {
            try
            {
                var answer = await _azureStorageServiceMethodsHelperInstance.BlockBlobDeleteIfExistsAsync(account, container, blob);

                return StatusCode(200, answer);
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



        [HttpPost(Routes.UploadBytesToBlockBlob)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadBytesToBlockBlobAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container,
            [FromQuery(Name = QueryParameters.Blob)] string blob,
            [FromQuery(Name = QueryParameters.CreateContainerIfNotExist)] bool createContainerIfNotExist,
            [FromBody] byte[] bytesToUpload)
        {
            try
            {
                var answer = await
                    _azureStorageServiceMethodsHelperInstance.UploadToBlockBlobAsBytesAsync(account, container, blob, createContainerIfNotExist, bytesToUpload);

                return StatusCode(200, answer);
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

        [HttpPost(Routes.UploadStringToBlockBlob)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadStringToBlockBlobAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container,
            [FromQuery(Name = QueryParameters.Blob)] string blob,
            [FromQuery(Name = QueryParameters.CreateContainerIfNotExist)] bool createContainerIfNotExist,
            [FromBody] string stringToUpload)
        {
            try
            {
                var answer = await _azureStorageServiceMethodsHelperInstance.UploadToBlockBlobAsStringAsync(account, container, blob, createContainerIfNotExist, stringToUpload);

                // todo. in a future version lets return the version id of saved blob so that we can uniquely identify it later if necessary

                return StatusCode(200, answer);
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

        [HttpGet(Routes.DownloadBlockBlobAsBytes)]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadBlockBlobAsBytesAsync(
            [FromQuery(Name = QueryParameters.Account)] string account,
            [FromQuery(Name = QueryParameters.Container)] string container,
            [FromQuery(Name = QueryParameters.Blob)] string blob)
        {
            try
            {
                var answer = await _azureStorageServiceMethodsHelperInstance.DownloadBlockBlobAsBytesAsync(account, container, blob);

                return StatusCode(200, answer);
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
}
