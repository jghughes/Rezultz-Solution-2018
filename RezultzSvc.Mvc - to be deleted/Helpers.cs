using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace RezultzSvc.Mvc
{
	internal class Helpers
	{

		public static string[] PrettyPrintControllerInfo(ControllerBase controller)
		{
			var connectionInfo = controller.HttpContext.Connection;

			RouteValueDictionary routeData = controller.ControllerContext.RouteData.Values;

			IFeatureCollection features = controller.ControllerContext.HttpContext.Features;

			var answers = new List<string>
            {
                "---- HttpContext.Connection.Connection ----",
                $"Local IP address: [{connectionInfo.LocalIpAddress}]",
                $"Local port: [{connectionInfo.LocalPort}]",
                $"Remote IP address: [{connectionInfo.RemoteIpAddress}]",
                $"Remote port: [{connectionInfo.RemotePort}]",
                "",
                "---- ControllerContext.RouteData ----"
            };

            var i = 1;
			foreach (var dataItem in routeData)
			{
				answers.Add($"{i} Key: [{dataItem.Key}] Value: [{dataItem.Value}]");
				i += 1;
			}
			answers.Add("");

			answers.Add("---- ControllerContext.HttpContext.Features ----");
			i = 1;
			foreach (var feature in features)
			{
				answers.Add($"{i} Key: [{feature.Key}] Value: [{feature.Value}]");
				i += 1;
			}

			return answers.ToArray();
		}

	}
}
