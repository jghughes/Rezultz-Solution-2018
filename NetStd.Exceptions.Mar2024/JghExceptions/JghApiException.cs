using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NetStd.Exceptions.Mar2024.JghExceptions;

public class JghApiException : Exception
{
    #region ctor

    public JghApiException(string message, HttpStatusCode statusCode, string request, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Request = request;
        Response = response;
        Headers = headers;
    }

    #endregion

    #region props

    public HttpStatusCode StatusCode { get; }

    public string Response { get; }

    public string Request { get; }

    public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

    public string Description => MakeExceptionDescription(this);

    #endregion

    #region methods

    public override string ToString()
    {
        return $"HTTP Response: \n\n{Response}\n\n{base.ToString()}";
    }

    private static string MakeExceptionDescription(JghApiException ex)
    {
        string FormatHeaders(IReadOnlyDictionary<string, IEnumerable<string>> headers)
        {
            var builder = new StringBuilder();

            foreach (var pair in headers) builder.AppendLine($"{pair.Key}: {string.Join(", ", pair.Value)}");
            var answer = builder.ToString();

            return answer;
        }

        static string FormatResponseMessageContent(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "(blank)";

            var maxPermittedLength = 4 * 1024; // arbitrary limit of 4k characters

            var length = text.Length;

            var possiblyTruncatedString = text.Substring(0, length >= maxPermittedLength ? maxPermittedLength : length);

            return $"{possiblyTruncatedString}";
        }

        var prettyHeaders = FormatHeaders(ex.Headers);
        var prettyContent = FormatResponseMessageContent(ex.Response);

        var sb = new StringBuilder();

        // if we are here, its a not regular alert message from the API, the Http service most likely on client side, although not definitely so,
        // has thrown an exception, so we need to get the HttpStatusCode and the first 512 characters of the response message content

        sb.AppendLine($"Message: {ex.Message}");
        sb.AppendLine($"StatusCode: {(int) ex.StatusCode}");
        sb.AppendLine($"StatusCodeEnum: {ex.StatusCode}");
        sb.AppendLine(prettyHeaders);
#if DEBUG
        sb.AppendLine($"Request (debug only): {ex.Request}");
#endif
        sb.AppendLine($"Content: {prettyContent}");

        var answer = sb.ToString();

        return answer;
    }

    #endregion
}

public class JghApiException<TResult> : JghApiException
{
    #region ctor

    public JghApiException(string message, HttpStatusCode statusCode, string request, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
        : base(message, statusCode, request, response, headers, innerException)
    {
        Result = result;
    }

    #endregion

    #region props

    public TResult Result { get; }

    #endregion
}