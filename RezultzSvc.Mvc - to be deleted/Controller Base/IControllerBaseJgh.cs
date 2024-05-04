using Microsoft.AspNetCore.Mvc;

namespace RezultzSvc.Mvc.Bases
{
	public interface IControllerBaseJgh
	{
		Task<IActionResult> GetIfServiceIsAnsweringAsync();

		Task<IActionResult> GetServiceEndpointsInfoAsync();

	}
}