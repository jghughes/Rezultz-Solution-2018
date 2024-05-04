using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.JghExceptions;
using Newtonsoft.Json;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable InvocationIsSkipped
// ReSharper disable MemberCanBeMadeStatic.Local

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

namespace NetStd.HttpRequestService.Feb2019;

/// <summary>
///     This is a service for making HTTP requests to a RESTful API that returns typed objects as JSON.
/// </summary>
/// <remarks>
///     Content precisely cloned from NSwag generated client code.
///     "NSwag", "14.0.7.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0))"
///     Structure cloned from the SmartHotel360 HttpRequestService.cs
/// </remarks>
public partial class HttpRequestService02<TObject400BadRequest, TResponseObject500InternalServerError>(string baseUrl, HttpClient httpClient) 
{
    #region ctor

    // no explicit ctor required. I am using a primary ctor as you can see.

    // the user class provides the HttpClient instance. This is to allow the user to provide a custom HttpClient instance with custom settings
    // and to use the same HttpClient instance for multiple requests to the same website (the BaseUrl) to prevent socket exhaustion.


    #endregion

    #region public props

    // todo - add a property for the access token?

    /// <summary>
    ///     Name of website hosting the API e.g. https://servicename.azurewebsites.net. When debugging, include the protocol
    ///     and port number e.g. "http://localhost:1234/".
    /// </summary>
    public string BaseUrl { get; } = baseUrl ?? string.Empty;

    /// <summary>
    ///     Set to true to read the response as a string (lightweight). Default is false i.e. read and deserialise as a stream
    ///     (heavyweight anticipating possibility of large payload).
    /// </summary>
    public bool MustProcessResponseContentUsingReadAsStringAsync { get; set; }

    #endregion

    #region serializer stuff

    protected JsonSerializerSettings JsonSerializerSettings => _settings.Value;

    // ReSharper disable once StaticMemberInGenericType
    private static Lazy<JsonSerializerSettings> _settings = new(CreateSerializerSettings, true);

    private static JsonSerializerSettings CreateSerializerSettings()
    {
        var settings = new JsonSerializerSettings();

        UpdateJsonSerializerSettings(settings);

        return settings;
    }

    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings); // placeholder to implement in a partial class if desired

    #endregion

    #region THE MEAT - request service methods

    public async Task<TReturn> GetObjectAsync<TReturn>(MvcRouteSegmentItem uri, string accessToken = "", CancellationToken cancellationToken = default)
    {
        var urlBuilder = BuildMvcRoute(BaseUrl, uri.Controller, uri.Action, uri.QueryParameters);

        var client = httpClient;

        var disposeClient = false;

        try
        {
            using var request = new HttpRequestMessage();

            request.Method = new HttpMethod("GET");

            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            PrepareRequest(client, request, urlBuilder); // currently empty

            var url = urlBuilder.ToString();

            request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

            PrepareRequest(client, request, url); // currently empty

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            var disposeResponse = true;

            try
            {
                var allResponseHeaders = EnumerateAllResponseHeaders(response);

                ProcessResponse(client, response); // currently empty

                var answer = await HandleResponseAsync<TReturn>(response, allResponseHeaders, cancellationToken).ConfigureAwait(false); // throws exception if status code is not 200

                return answer;
            }
            finally
            {
                if (disposeResponse)
                    response.Dispose();
            }
        }
        finally
        {
            if (disposeClient)
                // ReSharper disable once HeuristicUnreachableCode
                client.Dispose();
        }
    }

    public async Task<TReturn> PostObjectAsync<TSend, TReturn>(MvcRouteSegmentItem uri, TSend objectToSend, string accessToken = "", CancellationToken cancellationToken = default)
    {
        var urlBuilder = BuildMvcRoute(BaseUrl, uri.Controller, uri.Action, uri.QueryParameters);

        var client = httpClient;

        var disposeClient = false;

        // Note: if you want to instantiate a different HttpClient instance per request, you can do so here, but don't forget to dispose it. edit the above code to use a new instance each time.

        try
        {
            using var request = new HttpRequestMessage();

            var json = JsonConvert.SerializeObject(objectToSend, _settings.Value);

            var content = new StringContent(json);

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json-patch+json");

            request.Content = content;

            request.Method = new HttpMethod("POST");

            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            PrepareRequest(client, request, urlBuilder);

            var url = urlBuilder.ToString();

            request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

            PrepareRequest(client, request, url);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            var disposeResponse = true;

            try
            {
                var allResponseHeaders = EnumerateAllResponseHeaders(response);

                ProcessResponse(client, response);

                var answer = await HandleResponseAsync<TReturn>(response, allResponseHeaders, cancellationToken).ConfigureAwait(false); // throws exception if status code is not 200

                return answer;
            }
            finally
            {
                if (disposeResponse)
                    response.Dispose();
            }
        }
        finally
        {
            if (disposeClient)
                // ReSharper disable once HeuristicUnreachableCode
                client.Dispose();
        }
    }

    public async Task<TReturn> PutObjectAsync<TSend, TReturn>(MvcRouteSegmentItem uri, TSend objectToSend, string accessToken = "", CancellationToken cancellationToken = default)
    {
        var urlBuilder = BuildMvcRoute(BaseUrl, uri.Controller, uri.Action, uri.QueryParameters);

        var client = httpClient;

        var disposeClient = false;

        // Note: if you want to instantiate a different HttpClient instance per request, you can do so here, but don't forget to dispose it. edit the above code to use a new instance each time.

        try
        {
            using var request = new HttpRequestMessage();

            var json = JsonConvert.SerializeObject(objectToSend, _settings.Value);

            var content = new StringContent(json);

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json-patch+json");

            request.Content = content;

            request.Method = new HttpMethod("PUT");

            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            PrepareRequest(client, request, urlBuilder);

            var url = urlBuilder.ToString();

            request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

            PrepareRequest(client, request, url);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            var disposeResponse = true;

            try
            {
                var allResponseHeaders = EnumerateAllResponseHeaders(response);

                ProcessResponse(client, response);

                var answer = await HandleResponseAsync<TReturn>(response, allResponseHeaders, cancellationToken).ConfigureAwait(false); // throws exception if status code is not 200

                return answer;
            }
            finally
            {
                if (disposeResponse)
                    response.Dispose();
            }
        }
        finally
        {
            if (disposeClient)
                // ReSharper disable once HeuristicUnreachableCode
                client.Dispose();
        }
    }

    #endregion

    #region MORE MEAT - helper methods

    /// <summary>
    /// </summary>
    /// <remarks>
    ///     As per the example of Microsoft's demo SmartHotel360 service all the service controller actions written by me have
    ///     a standard format.
    ///     They return two and only two types of Microsoft.AspNetCore.Mvc.ControllerBase.ObjectResponses: Ok and BadRequest.
    ///     Ok has HttpStatusCode=200
    ///     and BadRequest has HttpStatusCode=400. My convention is that the ctor of OK takes a generic serialisable object as
    ///     its only parameter
    ///     and the ctor of BadRequest takes object of type string. BadRequest is returned for everything and anything that
    ///     surfaces from a
    ///     controller action's business logic as an exception. The 400 string object is the exception message hence the
    ///     confidence in reading
    ///     the 400 Response as Type string in this method. This is one of the reasons this handler is valid only for MVC
    ///     services and only for MVC services
    ///     observing the same convention.
    /// </remarks>
    /// <typeparam name="TObject200Ok"></typeparam>
    /// <param name="responseMessageItem"></param>
    /// <param name="allResponseHeaders"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>T</returns>
    /// <exception cref="JghApiException"></exception>
    protected virtual async Task<TObject200Ok> HandleResponseAsync<TObject200Ok>(HttpResponseMessage responseMessageItem, IReadOnlyDictionary<string, IEnumerable<string>> allResponseHeaders, CancellationToken cancellationToken)
    {
        const string nullWasReturnedErrorMessage =
            "Error. Null response. The local client was not expecting to receive null as a response. It was expecting a response of type";

        var httpStatusCodeAsInt = (int) responseMessageItem.StatusCode;

        switch (httpStatusCodeAsInt)
        {
            case (int) HttpStatusCode.OK: // read (200) response as the expected object for the specific successful response for specified controller action
            {
                var responseResult = await ReadResponseAsync<TObject200Ok>(responseMessageItem, allResponseHeaders, cancellationToken).ConfigureAwait(false);

                // ToDo: nSwag includes the following code. Question. Is this desirable if the svc intentionally returns null or a nullable type? I guess we should do it the NSwag way and fix any of my services that return null when they shouldn't.
                if (responseResult.Object == null)
                    throw new JghApiException($"{nullWasReturnedErrorMessage} <{typeof(TObject200Ok).FullName}>.",
                        responseMessageItem.StatusCode, responseMessageItem.RequestMessage.ToString(), responseResult.Text, allResponseHeaders, null);

                return responseResult.Object;
            }
            case (int) HttpStatusCode.BadRequest: // // NB read (400) response as T400ResponseObjectBadRequest. The 400 decoration on all Rezultz controller actions dictate a string all for harmless alert messages.
                {
                var responseResult = await ReadResponseAsync<TObject400BadRequest>(responseMessageItem, allResponseHeaders, cancellationToken).ConfigureAwait(false);

                if (responseResult.Object == null)
                    throw new JghApiException($"{nullWasReturnedErrorMessage} <{typeof(TObject400BadRequest).FullName}>.",
                        responseMessageItem.StatusCode, responseMessageItem.RequestMessage.ToString(), responseResult.Text, allResponseHeaders, null);

                throw new JghApiException<TObject400BadRequest>($"The client received a status code ({httpStatusCodeAsInt}).", responseMessageItem.StatusCode, responseMessageItem.RequestMessage.ToString(), responseResult.Text, allResponseHeaders, responseResult.Object, null);
                }
            case (int) HttpStatusCode.InternalServerError: // NB read (500) response as T500ResponseObjectInternalServerError. The 500 decoration on all Rezultz controller actions dictate a SerialisableExceptionItem.
                {
                var responseResult = await ReadResponseAsync<TResponseObject500InternalServerError>(responseMessageItem, allResponseHeaders, cancellationToken).ConfigureAwait(false);

                if (responseResult.Object == null)
                    throw new JghApiException($"{nullWasReturnedErrorMessage} <{typeof(TResponseObject500InternalServerError).FullName}>.", responseMessageItem.StatusCode, responseMessageItem.RequestMessage.ToString(), responseResult.Text,
                        allResponseHeaders, null);

                throw new JghApiException<TResponseObject500InternalServerError>($"The client received an error status code ({httpStatusCodeAsInt}).", responseMessageItem.StatusCode, responseMessageItem.RequestMessage.ToString(), responseResult.Text, allResponseHeaders, responseResult.Object, null);
            }
            case (int) HttpStatusCode.Unauthorized or (int) HttpStatusCode.Forbidden or (int) HttpStatusCode.MethodNotAllowed: //system error
            {
                var responseBody = responseMessageItem.Content == null ? null : await responseMessageItem.Content.ReadAsStringAsync().ConfigureAwait(false);

                throw new JghApiException($"The server actively refused the call. Error status code ({httpStatusCodeAsInt}).",
                    responseMessageItem.StatusCode, responseMessageItem.RequestMessage.ToString(), responseBody, allResponseHeaders, null);
            }
            default:
            {
                var responseBody = responseMessageItem.Content == null ? null : await responseMessageItem.Content.ReadAsStringAsync().ConfigureAwait(false);

                throw new JghApiException($"The client received an unexpected error status code ({httpStatusCodeAsInt}).",
                    responseMessageItem.StatusCode, responseMessageItem.RequestMessage.ToString(), responseBody, allResponseHeaders, null);
            }
        }
    }

    protected virtual async Task<ObjectResponseResult<TObjectToBeDeserialised>> ReadResponseAsync<TObjectToBeDeserialised>(HttpResponseMessage httpResponseMessageItem, IReadOnlyDictionary<string, IEnumerable<string>> headers, CancellationToken cancellationToken)
    {
        if (httpResponseMessageItem?.Content == null)
            return new ObjectResponseResult<TObjectToBeDeserialised>(default, string.Empty);

        const string deserialisationFailedMessage = "Error. An unknown/unexpected type was received that could not be deserialised. The type being expected was";

        const string invisibleContentMessage = "(Sorry. Unable to show Http response content because it was processed with a StreamReader to economize on memory.)";

        if (MustProcessResponseContentUsingReadAsStringAsync)
        {
            var messageContentAsJson = await httpResponseMessageItem.Content.ReadAsStringAsync().ConfigureAwait(false);

            try
            {
                var messageContentAsDeserialisedObject = JsonConvert.DeserializeObject<TObjectToBeDeserialised>(messageContentAsJson, JsonSerializerSettings);

                return new ObjectResponseResult<TObjectToBeDeserialised>(messageContentAsDeserialisedObject, messageContentAsJson);
            }
            catch (JsonException exception)
            {
                var errorMessage = $"{deserialisationFailedMessage} {typeof(TObjectToBeDeserialised).FullName}.";

                throw new JghApiException(errorMessage, httpResponseMessageItem.StatusCode, httpResponseMessageItem.RequestMessage.ToString(), messageContentAsJson, headers, exception);
            }
        }

        try
        {
            using var responseStream = await httpResponseMessageItem.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var streamReader = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            var serializer = JsonSerializer.Create(JsonSerializerSettings);

            var messageContentAsDeserialisedObject = serializer.Deserialize<TObjectToBeDeserialised>(jsonTextReader);

            return new ObjectResponseResult<TObjectToBeDeserialised>(messageContentAsDeserialisedObject, invisibleContentMessage);
        }
        catch (JsonException exception)
        {
            var errorMessage = $"{deserialisationFailedMessage} {typeof(TObjectToBeDeserialised).FullName}.";

            throw new JghApiException(errorMessage, httpResponseMessageItem.StatusCode, httpResponseMessageItem.RequestMessage.ToString(), invisibleContentMessage, headers, exception);
        }
    }

    private Dictionary<string, IEnumerable<string>> EnumerateAllResponseHeaders(HttpResponseMessage response)
    {
        if (response == null)
            return new Dictionary<string, IEnumerable<string>>();

        var headersBeingConsolidated = response.Headers.ToDictionary(h => h.Key, h => h.Value);

        if (response.Content?.Headers == null)
            return headersBeingConsolidated;

        foreach (var item in response.Content.Headers) headersBeingConsolidated[item.Key] = item.Value;

        return headersBeingConsolidated;
    }

    public StringBuilder BuildMvcRoute(string baseUrl, string controller, string action, IEnumerable<KeyValuePair<string, object>> queryParameters = null)
    {
        var baseUrlRoute = string.IsNullOrWhiteSpace(baseUrl) ? "" : baseUrl.TrimEnd('/');

        var controllerRoute = string.IsNullOrWhiteSpace(controller) ? "" : $"/{controller.Trim('/')}";

        var actionRoute = string.IsNullOrWhiteSpace(action) ? "" : $"/{action.Trim('/').TrimEnd('?')}";

        var routeBuilder = new StringBuilder();

        routeBuilder.Append(baseUrlRoute);
        routeBuilder.Append(controllerRoute);
        routeBuilder.Append(actionRoute);

        if (queryParameters == null)
            return routeBuilder;

        var keyValuePairs = queryParameters.ToArray();

        if (!keyValuePairs.Any())
            return routeBuilder;

        var filteredKeyValuePairs = keyValuePairs
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key)).ToArray(); // missing parameters, and null and whitespace parameter values are prohibited in .Net6 MVC (they were OK in .Net5). Controller will blow up.

        if (!filteredKeyValuePairs.Any())
            return routeBuilder;

        routeBuilder.Append("?");

        var queries = filteredKeyValuePairs
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(ConvertLikelyObjectToString(kvp.Value, CultureInfo.InvariantCulture))}").ToArray();

        foreach (var query in queries)
        {
            routeBuilder.Append(query);
            routeBuilder.Append("&");
        }

        routeBuilder.Length--; // remove the last "&"

        return routeBuilder;
    }

    private string ConvertLikelyObjectToString(object value, CultureInfo cultureInfo)
    {
        switch (value)
        {
            case null:
                return "";
            case Enum:
            {
                var name = Enum.GetName(value.GetType(), value);

                if (name != null)
                {
                    var field = value.GetType().GetTypeInfo().GetDeclaredField(name);

                    if (field != null)
                        if (field.GetCustomAttribute(typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute)
                            return attribute.Value ?? name;

                    var converted = Convert.ToString(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), cultureInfo));

                    return converted;
                }

                break;
            }
            case bool booleanValue:
                return Convert.ToString(booleanValue, cultureInfo).ToLowerInvariant();
            case byte[] byteArray:
                return Convert.ToBase64String(byteArray);
            case string[] stringArray:
                return string.Join(",", stringArray);
            default:
            {
                // this is not a default case, but it is the last case in the switch statement
                if (value.GetType().IsArray)
                {
                    var valueArray = (Array) value;
                    var valueTextArray = new string[valueArray.Length];
                    for (var i = 0; i < valueArray.Length; i++) valueTextArray[i] = ConvertLikelyObjectToString(valueArray.GetValue(i), cultureInfo);
                    return string.Join(",", valueTextArray);
                }

                break;
            }
        }

        var result = Convert.ToString(value, cultureInfo);

        return result ?? "";
    }

    #endregion

    #region some empty partial methods for you to implement in a partial class if desired

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url);

    partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder);

    partial void ProcessResponse(HttpClient client, HttpResponseMessage response);

    #endregion
}

