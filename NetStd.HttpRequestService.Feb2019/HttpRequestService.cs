using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Goodies.Mar2022;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NetStd.HttpRequestService.Feb2019;

/// <summary>
///     This class provides a wrapper around common functionality for HTTP actions.
///     Learn more at https://docs.microsoft.com/windows/uwp/networking/httpclient
///     It is NOT a generic http client. It is designed for communicating with MVC controllers that intrinsically receive
///     and return JSON. The inspiration for the bulk of this service comes from Microsoft's 2017 reference project on
///     Github.
///     Accordingly, it deploys the underlying HttpClient to only send StringContent class and only receive string content.
///     https://github.com/microsoft/SmartHotel360-Mobile/blob/master/Source/SmartHotel.Clients/SmartHotel.Clients/Services/Request/RequestService.cs
///     for excellent ideas about how to make this client cleverer see https://www.youtube.com/watch?v=Lxyb_DOhGnk&amp;
///     also follow the blog and Nuget packages of Paul Betts who is a guru. Each new httpclient in each method is wrapped
///     in a using statement so that having exited the scope of the using statement, httpClient.Dispose() will be called
///     automatically, thus freeing up system resources (the underlying socket, and memory)
///     // used for the object).
/// </summary>
public class HttpRequestService : IHttpRequestService
{
    #region methods

    public async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string tokenEndpoint, string scope)
    {
        using var singleUseHttpClient = new HttpClient();

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", scope }
        };

        var requestContent = new FormUrlEncodedContent(requestBody);

        var response = await singleUseHttpClient.PostAsync(tokenEndpoint, requestContent);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

        if (tokenResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize OAuth token.");
        }

        return tokenResponse.AccessToken;
    }

    public async Task<TReturn> GetObjectAsync<TReturn>(string uri, string accessToken = "", CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response;

        using (var httpClient = CreateHttpClient(accessToken))
        {
            response = await httpClient.GetAsync(uri, cancellationToken);
        }

        await HandleResponseAsync(response);

        var serialized = await response.Content.ReadAsStringAsync();

        var result = JghSerialisation.ToObjectFromJson<TReturn>(serialized);

        return result;
    }

    public async Task<TReturn> PostObjectAsync<TSend, TReturn>(string uri, TSend objectToSend, string accessToken = "", CancellationToken cancellationToken = default)
    {
        var serialized = JghSerialisation.ToJsonFromObject(objectToSend);

        var content = new StringContent(serialized, Encoding.UTF8, "application/json");

        HttpResponseMessage response;

        using (var httpClient = CreateHttpClient(accessToken))
        {
            response = await httpClient.PostAsync(uri, content, cancellationToken);
        }

        await HandleResponseAsync(response);

        var responseData = await response.Content.ReadAsStringAsync();

        var result = JghSerialisation.ToObjectFromJson<TReturn>(responseData);

        return result;
    }

    public async Task<TReturn> PutObjectAsync<TSend, TReturn>(string uri, TSend objectToSend, string accessToken = "", CancellationToken cancellationToken = default)
    {
        var serialized = JghSerialisation.ToJsonFromObject(objectToSend);

        var content = new StringContent(serialized, Encoding.UTF8, "application/json");

        HttpResponseMessage response;

        using (var httpClient = CreateHttpClient(accessToken))
        {
            response = await httpClient.PutAsync(uri, content, cancellationToken);
        }

        await HandleResponseAsync(response);

        var responseData = await response.Content.ReadAsStringAsync();

        var result = JghSerialisation.ToObjectFromJson<TReturn>(responseData);

        return result;
    }

    
    #endregion

    #region type

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }

    #endregion

    #region helpers

    private HttpClient CreateHttpClient(string token = "")
    {
        var httpClient = MakeClient();

        AddRequestHeaders(httpClient);

        AddAuthorizationHeader(httpClient, token);

        return httpClient;
    }

    private HttpClient MakeClient()
    {
#if DEBUG
        //bypass certificate validation, allowing the HttpClient to accept any server certificate, regardless of its validity or trust. we do this because we are using localhost for debugging which has a self-signed certificate
        var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        });

#else
        var httpClient = new HttpClient();
#endif

        return httpClient;
    }

    private void AddRequestHeaders(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private void AddAuthorizationHeader(HttpClient httpClient, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
            return;
        }
        // Remove any existing Authorization headers to avoid duplicates
        if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task HandleResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        // oh,oh. if we arrive here, the server has returned an unsuccessful status code. we have to interpret it if we can and throw something meaningful

        var content = response.Content is null ? string.Empty : await response.Content.ReadAsStringAsync();

        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.MethodNotAllowed or HttpStatusCode.Unauthorized)
        {
            var responseMsg =
                $"The server actively refused the call.\r\n\r\nIt responded with HttpStatusCode {(int)response.StatusCode}={response.StatusCode}\r\n\r\nThe reason phrase was <{response.ReasonPhrase}>\r\n\r\nThe content was <{response.Content}>\r\n\r\nThe Http request was:\r\n\r\n{response.RequestMessage}";

            throw new Exception(responseMsg);
        }

        // the status code could be anything - it could be an error, a warning, or a valid alert message. Just rethrow as a HttpRequestException
        throw new HttpRequestException(content); // the meat
    }

    #endregion
}