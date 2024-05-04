using Jgh.MvcParameters.Mar2024;
using Microsoft.AspNetCore.Mvc;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.SeasonSeriesProfiles;
using RezultzSvc.Library01.Mar2024.SvcHelpers;
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
	[Route(Routes.LeaderboardResultsController)]
	public class RaceResultsController : ControllerBaseJgh, IResultsController
	{
		#region ctor

		public RaceResultsController(ILogger<RaceResultsController> logger)
		{
			Logger1 = logger;
			_leaderboardResultsServiceMethodsHelper = new LeaderboardResultsServiceMethodsHelper();

		}

		#endregion

		#region fields

		public ILogger<RaceResultsController> Logger1 { get; }

		private readonly LeaderboardResultsServiceMethodsHelper _leaderboardResultsServiceMethodsHelper;

		#endregion

		#region actions

        [HttpGet(Routes.GetIfFileNameOfSeasonProfileIsRecognised)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetIfSeasonIdIsRecognisedAsync([FromQuery(Name = QueryParameters.SeasonId)] string seasonId)
        {
            try
            {
                var answer = await _leaderboardResultsServiceMethodsHelper.GetIfSeasonIdIsRecognisedAsync(seasonId);

                return Ok(answer);
            }
            catch (Exception ex)
            {
                return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
            }
        }

		[HttpGet(Routes.GetSeasonProfile)]
		[ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetSeasonItemAsync([FromQuery(Name = QueryParameters.SeasonId)] string seasonId)
		{
			try
			{
                var seasonItemDataTransferObject = await _leaderboardResultsServiceMethodsHelper.GetSeasonProfileAsync(seasonId);

                var answer = JghSerialisation.ToJsonFromObject(seasonItemDataTransferObject);

				var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

                var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

				return Ok(answerCompressedBytes);
			}
			catch (Exception ex)
			{
				return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
			}
		}

        [HttpGet(Routes.GetAllSeasonProfiles)]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSeasonItemArrayAsync()
        {
            try
            {
                var seasonItemDataTransferObjects = await _leaderboardResultsServiceMethodsHelper.GetAllSeasonProfilesAsync(CancellationToken.None);

                var answer = JghSerialisation.ToJsonFromObject(seasonItemDataTransferObjects);

                var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

                var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

                return Ok(answerCompressedBytes);
            }
            catch (Exception ex)
            {
                return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
            }
        }

        [HttpPost(Routes.PopulateSingleEventWithResults)]
		[ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PopulateSingleEventWithPreprocessedResultsAsync([FromBody] byte[] eventItemAsJsonAsCompressedBytes)
		{
			try
			{
                var eventItemAsJsonAsBytes =
                    await JghCompression.DecompressAsync(eventItemAsJsonAsCompressedBytes);

                string eventItemAsJson = JghConvert.ToStringFromUtf8Bytes(eventItemAsJsonAsBytes);

                var eventItem = JghSerialisation.ToObjectFromJson<EventProfileDto>(eventItemAsJson);

				var populatedEventItem = await _leaderboardResultsServiceMethodsHelper.PopulateSingleEventWithResultsAsync(eventItem);

                var answer = JghSerialisation.ToJsonFromObject(populatedEventItem);

                var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

				var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

				return Ok(answerCompressedBytes);
			}
			catch (Exception ex)
			{
				return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
			}
		}

		[HttpPost(Routes.PopulateAllEventsInSingleSeriesWithAllResults)]
		[ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> PopulateAllEventsInSeriesWithAllPreprocessedResultsAsync([FromBody] byte[] seriesItemAsJsonAsCompressedBytes)
		{
			try
			{
                var seriesItemAsJsonAsBytes = await JghCompression.DecompressAsync(seriesItemAsJsonAsCompressedBytes);

                string seriesItemAsJson = JghConvert.ToStringFromUtf8Bytes(seriesItemAsJsonAsBytes);

                var seriesItem = JghSerialisation.ToObjectFromJson<SeriesProfileDto>(seriesItemAsJson);

				var populatedSeriesItem = await _leaderboardResultsServiceMethodsHelper.PopulateAllEventsInSingleSeriesWithAllResultsAsync(seriesItem);

                var answer = JghSerialisation.ToJsonFromObject(populatedSeriesItem);

                var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

				var answerCompressedBytes = await JghCompression.CompressAsync(bytesUtf8);

				return Ok(answerCompressedBytes);
			}
			catch (Exception ex)
			{
				return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
			}
		}

		#endregion
	}
}