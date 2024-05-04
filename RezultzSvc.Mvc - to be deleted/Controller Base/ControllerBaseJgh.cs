using Jgh.MvcParameters.Mar2024;
using Microsoft.AspNetCore.Mvc;
using NetStd.Exceptions.Mar2024.Helpers;
using RezultzSvc.Mvc.Bases;


namespace RezultzSvc.Mvc.Controller_Base
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
	public class ControllerBaseJgh : ControllerBase , IControllerBaseJgh
	{
		#region actions

		[HttpGet(Routes.GetIfServiceIsAnswering)]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetIfServiceIsAnsweringAsync()
		{
			// if we have got this far, the svc is answering! this here is inside the svc!
			return await Task.FromResult(Ok(true));
		}

		[HttpGet(Routes.GetServiceEndpointsInfo)]
		[ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> GetServiceEndpointsInfoAsync()
		{
			try
			{
				var info = Helpers.PrettyPrintControllerInfo(this);

				return await Task.FromResult(Ok(info));
			}
			catch (Exception ex)
			{
				return BadRequest(JghExceptionHelpers.FindInnermostException(ex).Message);
			}
		}


		#endregion

	}
}