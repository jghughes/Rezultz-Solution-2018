using System.Collections.Generic;

namespace NetStd.HttpRequestService.Feb2019;

public readonly struct MvcRouteSegmentItem(string controller, string action, List<KeyValuePair<string, object>> queryParameters)
{
    public string Controller { get; } = controller ?? string.Empty;
    public string Action { get; } = action ?? string.Empty;
    public List<KeyValuePair<string, object>> QueryParameters { get; } = queryParameters ?? new List<KeyValuePair<string, object>>();
}