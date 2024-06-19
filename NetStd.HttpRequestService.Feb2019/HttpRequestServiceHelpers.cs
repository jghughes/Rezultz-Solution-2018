using System;
using System.Collections.Generic;
using System.Linq;

namespace NetStd.HttpRequestService.Feb2019
{
    public class HttpRequestServiceHelpers
    {
        public static string AppendNameValuePairsToUriInQueryStringFormat(string controller, string action, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
        {
            if (string.IsNullOrWhiteSpace(controller))
                return string.Empty;

            if (string.IsNullOrWhiteSpace(action))
                return string.Empty;

            var route = $"{controller}/{action}";

            if (queryParameters is null)
                return route;

            var keyValuePairs = queryParameters.ToArray();

            if (!keyValuePairs.Any())
                return route;

            var filteredKeyValuePairs = keyValuePairs
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value)).ToArray(); // missing parameters, and null and whitespace parameter values are prohibited in .Net6 MVC (they were OK in .Net5). Controller will blow up.

            if (!filteredKeyValuePairs.Any())
                return route;

            if (!route.EndsWith("?"))
                route = string.Concat(route, "?");

            var queryParamsAsStrings = filteredKeyValuePairs
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}").ToArray();


            var formattedAsUri = string.Concat(route, string.Join("&", queryParamsAsStrings));

            return formattedAsUri;
        }

    }
}
