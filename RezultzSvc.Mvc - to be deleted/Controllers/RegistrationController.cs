using Jgh.MvcParameters.Mar2024;
using Microsoft.AspNetCore.Mvc;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using RezultzSvc.Library01.Mar2024.SvcHelpers;
using RezultzSvc.Mvc.Bases;
using RezultzSvc.Mvc.Controller_Base;
using RezultzSvc.Mvc.Controller_Interfaces;


namespace RezultzSvc.Mvc.Controllers
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
    [Route(Routes.ParticipantRegistrationController)]
    public class RegistrationController : ControllerBaseJgh, IRegistrationController
    {
        #region ctor

        public RegistrationController(IAzureStorageAccessor azureStorageAccessorInstance, ILogger<RegistrationController> logger)
        {
            _participantServiceMethodsHelperInstance = new ParticipantHubServiceMethodsHelper(azureStorageAccessorInstance);

            Logger1 = logger;
        }

        #endregion

        #region fields

        public ILogger<RegistrationController> Logger1 { get; }

        private readonly ParticipantHubServiceMethodsHelper _participantServiceMethodsHelperInstance;

        #endregion

        #region actions

        [HttpGet(Routes.GetIfContainerExists)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetIfContainerExistsAsync(
            [FromQuery(Name = QueryParameters.DatabaseAccount)] string databaseAccount,
            [FromQuery(Name = QueryParameters.DataContainer)] string dataContainer)
        {
            try
            {
               var answer = await _participantServiceMethodsHelperInstance.GetIfContainerExistsAsync(databaseAccount, dataContainer, CancellationToken.None);

                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
            }
        }

        [HttpPost(Routes.PostParticipantItem)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostParticipantItemAsync(
            [FromQuery(Name = QueryParameters.DatabaseAccount)] string databaseAccount,
            [FromQuery(Name = QueryParameters.DataContainer)] string dataContainer,
            [FromQuery(Name = QueryParameters.TablePartition)] string tablePartition,
            [FromQuery(Name = QueryParameters.TableRowKey)] string tableRowKey,
            [FromBody] string participantItemAsJson)
        {
            // NB: ONLY use this method for singletons i.e. for PINs and Check-ins and Profiles.
            // in our blobStorage schema for singletons we use tablePartition==RecordingModeEnum
            // and tableRowKey==Identifier for check-in and profile, and tableRowKey==ParticipantModeSymbolPin for the one and only PIN.
            // why? because in our system, only singletons have row keys which can be deduced for lookups. 
            // things which aren't singletons have row keys that are complex compound keys that fail in round tripping 

            // currently, the tablePartition and tableRowKey parameters in this signature are superfluous because the _participantServiceMethodsHelperInstance method deduces
            // partition and row key from the contents of the ParticipantItem. i'm only leaving them in the signature for the sake of symmetry
            // with the signature of GetItemAsync(), and as placeholders for some unforeseen use in future


            try
            {
                await _participantServiceMethodsHelperInstance.PostItemAsync(databaseAccount, dataContainer, participantItemAsJson, CancellationToken.None);

                return Ok(participantItemAsJson);  // simply return what we sent
            }
            catch (Exception ex)
            {
                return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
            }
        }


        [HttpPost(Routes.PostParticipantItemArray)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostParticipantItemArrayAsync(
            [FromQuery(Name = QueryParameters.DatabaseAccount)] string databaseAccount,
            [FromQuery(Name = QueryParameters.DataContainer)] string dataContainer,
            [FromBody] byte[] participantItemArrayAsJsonAsCompressedBytes)
        {
            try
            {
                var decompressedBytesUtf8 =
                    await JghCompression.DecompressAsync(participantItemArrayAsJsonAsCompressedBytes);

                var participantItemArrayAsJson = JghConvert.ToStringFromUtf8Bytes(decompressedBytesUtf8);

                string answer = await _participantServiceMethodsHelperInstance.PostItemArrayAsyncV2(databaseAccount, dataContainer, participantItemArrayAsJson, CancellationToken.None);

                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
            }
        }

        [HttpGet(Routes.GetParticipantItem)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetParticipantItemAsync(
            [FromQuery(Name = QueryParameters.DatabaseAccount)] string databaseAccount,
            [FromQuery(Name = QueryParameters.DataContainer)] string dataContainer,
            [FromQuery(Name = QueryParameters.TablePartition)] string tablePartition,
            [FromQuery(Name = QueryParameters.TableRowKey)] string tableRowKey)
        {
            try
            {
                string answer = await _participantServiceMethodsHelperInstance.GetItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, CancellationToken.None);

                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
            }
        }

        [HttpGet(Routes.GetParticipantItemArray)]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetParticipantItemArrayAsync(
            [FromQuery(Name = QueryParameters.DatabaseAccount)] string databaseAccount,
            [FromQuery(Name = QueryParameters.DataContainer)] string dataContainer)
        {
            try
            {
                var answer = await _participantServiceMethodsHelperInstance.GetArrayOfHubItemAsync(databaseAccount, dataContainer, CancellationToken.None);

                var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

                var answerAsCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

                return Ok(answerAsCompressedBytes);
            }
            catch (Exception ex)
            {
                return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
            }
        }

        #endregion
    }
}
