using Jgh.MvcParameters.Mar2024;
using Microsoft.AspNetCore.Mvc;
using NetStd.AzureStorageAccess.July2018;
using NetStd.DataTransferObjects.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using RezultzSvc.Library01.Mar2024.SvcHelpers;
using RezultzSvc.WebApp02.Controller_Base;
using RezultzSvc.WebApp02.Controller_Interfaces;

namespace RezultzSvc.WebApp02.Controllers
{
    /// <summary>
    /// A REST API is best developed in a service-first manner. Use hard coded strings for the names of
    /// ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
    /// endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
    /// from Swagger into the URI strings used in your REST client to make HTTP calls to the API.
    ///
    /// The inspiration for this service comes from Microsoft's 2017 reference project on Github.
    /// https://github.com/microsoft/SmartHotel360-Backend/blob/master/Source/Backend/src/SmartHotel.Services.Bookings/Controllers/BookingsController.cs
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route(Routes.ParticipantRegistrationController)]
    public class ParticipantRegistrationController : ControllerBaseJgh, IParticipantRegistrationController
    {
        #region ctor

        public ParticipantRegistrationController(IAzureStorageAccessor azureStorageAccessorInstance, ILogger<ParticipantRegistrationController> logger)
        {
            _participantServiceMethodsHelperInstance = new ParticipantHubServiceMethodsHelper(azureStorageAccessorInstance);

            Logger1 = logger;
        }

        #endregion

        #region fields

        public ILogger<ParticipantRegistrationController> Logger1 { get; }

        private readonly ParticipantHubServiceMethodsHelper _participantServiceMethodsHelperInstance;

        #endregion

        #region actions

        [HttpGet(Routes.GetIfContainerExists)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetIfContainerExistsAsync(
            [FromQuery(Name = QueryParameters.Account)] string databaseAccount,
            [FromQuery(Name = QueryParameters.Container)] string dataContainer)
        {
            try
            {
               var answer = await _participantServiceMethodsHelperInstance.GetIfContainerExistsAsync(databaseAccount, dataContainer, CancellationToken.None);

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

        [HttpPost(Routes.PostParticipantItem)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostParticipantItemAsync(
            [FromQuery(Name = QueryParameters.Account)] string databaseAccount,
            [FromQuery(Name = QueryParameters.Container)] string dataContainer,
            [FromQuery(Name = QueryParameters.TablePartition)] string tablePartition,
            [FromQuery(Name = QueryParameters.TableRowKey)] string tableRowKey,
            [FromBody] string participantItemAsJson)
        {
            // NB: ONLY use this method for singletons i.e. for PINs and Check-ins and Profiles.
            // in our blobStorage schema for singletons we use tablePartition==RecordingModeEnum
            // and tableRowKey==Bib for check-in and profile, and tableRowKey==ParticipantModeSymbolPin for the one and only PIN.
            // why? because in our system, only singletons have row keys which can be deduced for lookups. 
            // things which aren't singletons have row keys that are complex compound keys that fail in round tripping 

            // currently, the tablePartition and tableRowKey parameters in this signature are superfluous because the _participantServiceMethodsHelperInstance method deduces
            // partition and row key from the contents of the ParticipantItem. i'm only leaving them in the signature for the sake of symmetry
            // with the signature of GetItemAsync(), and as placeholders for some unforeseen use in future


            try
            {
                await _participantServiceMethodsHelperInstance.PostItemAsync(databaseAccount, dataContainer, participantItemAsJson, CancellationToken.None);

                return StatusCode(200, participantItemAsJson);  // simply return what we sent
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


        [HttpPost(Routes.PostParticipantItemArray)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostParticipantItemArrayAsync(
            [FromQuery(Name = QueryParameters.Account)] string databaseAccount,
            [FromQuery(Name = QueryParameters.Container)] string dataContainer,
            [FromBody] byte[] participantItemArrayAsJsonAsCompressedBytes)
        {
            try
            {
                var decompressedBytesUtf8 =
                    await JghCompression.DecompressAsync(participantItemArrayAsJsonAsCompressedBytes);

                var participantItemArrayAsJson = JghConvert.ToStringFromUtf8Bytes(decompressedBytesUtf8);

                string answer = await _participantServiceMethodsHelperInstance.PostItemArrayAsyncV2(databaseAccount, dataContainer, participantItemArrayAsJson, CancellationToken.None);

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

        [HttpGet(Routes.GetParticipantItem)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParticipantItemAsync(
            [FromQuery(Name = QueryParameters.Account)] string databaseAccount,
            [FromQuery(Name = QueryParameters.Container)] string dataContainer,
            [FromQuery(Name = QueryParameters.TablePartition)] string tablePartition,
            [FromQuery(Name = QueryParameters.TableRowKey)] string tableRowKey)
        {
            try
            {
                string answer = await _participantServiceMethodsHelperInstance.GetItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, CancellationToken.None);

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

        [HttpGet(Routes.GetParticipantItemArray)]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetParticipantItemArrayAsync(
            [FromQuery(Name = QueryParameters.Account)] string databaseAccount,
            [FromQuery(Name = QueryParameters.Container)] string dataContainer)
        {
            try
            {
                var answer = await _participantServiceMethodsHelperInstance.GetArrayOfHubItemAsync(databaseAccount, dataContainer, CancellationToken.None);

                var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

                var answerAsCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

                return StatusCode(200, answerAsCompressedBytes);
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
