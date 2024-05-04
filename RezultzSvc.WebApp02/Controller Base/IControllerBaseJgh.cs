using Microsoft.AspNetCore.Mvc;

namespace RezultzSvc.WebApp02.Controller_Base
{
	public interface IControllerBaseJgh
	{
		Task<IActionResult> GetIfServiceIsAnsweringAsync();

		Task<IActionResult> GetServiceEndpointsInfoAsync();

	}
}