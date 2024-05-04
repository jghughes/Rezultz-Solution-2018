using System.Threading;
using System.Threading.Tasks;

namespace NetStd.HttpRequestService.Feb2019;

public interface IHttpRequestService
{

    /// <summary>
    ///     HTTP GET method where the underlying HttpClient returns an object whose hidden representation is as a JSON string.
    ///     Only use this method naively for objects where successful JSON serialisation is guaranteed: in such cases convert the objects manually to byte[] and transmit them like that instead
    /// </summary>
    /// <typeparam name="T">The object returned in the GET. Transmitted in the form of a JSON string.</typeparam>
    /// <param name="uri">endpoint/controller/action?queries</param>
    /// <param name="accessToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Object of type T</returns>
    Task<T> GetObjectAsync<T>(string uri, string accessToken = "", CancellationToken cancellationToken = default);

    /// <summary>
    ///     HTTP POST method where the underlying HttpClient sends and returns objects whose hidden representation is as a JSON string,
    ///     Only use this method naively for objects where successful JSON serialisation is guaranteed: in such cases convert the objects manually to byte[] and transmit them like that instead
    /// </summary>
    /// <typeparam name="TSend">The object sent in the POST. Transmitted in the form of a JSON string.</typeparam>
    /// <typeparam name="TReturn">The object returned in the HttpResponse. Transmitted in the form of a JSON string.</typeparam>
    /// <param name="uri">endpoint/controller/action?queries</param>
    /// <param name="objectToSend">The object to send</param>
    /// <param name="accessToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Object of type TReturn</returns>
    Task<TReturn> PostObjectAsync<TSend, TReturn>(string uri, TSend objectToSend, string accessToken = "", CancellationToken cancellationToken = default);

    /// <summary>
    ///    HTTP PUT method where the underlying HttpClient sends and returns objects whose hidden representation is as a JSON string.
    ///    Only use this method naively for objects where successful JSON serialisation is guaranteed: in such cases convert the objects manually to byte[] and transmit them like that instead
    /// </summary>
    /// <typeparam name="TSend"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    /// <param name="uri"></param>
    /// <param name="objectToSend"></param>
    /// <param name="accessToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TReturn> PutObjectAsync<TSend, TReturn>(string uri, TSend objectToSend, string accessToken = "", CancellationToken cancellationToken = default);


    #region old

    ///// <summary>
    /////     HTTP GET method where the underlying HttpClient returns a string
    ///// </summary>
    ///// <param name="uri">endpoint/controller/action?queries</param>
    ///// <param name="accessToken"></param>
    ///// <param name="cancellationToken"></param>
    ///// <returns>Object of type T</returns>
    //Task<string> GetRawStringAsync(string uri, string accessToken = "", CancellationToken cancellationToken = default);

    ///// <summary>
    /////     HTTP POST method where the underlying HttpClient sends a raw string and returns an object whose underlying
    /////     representation is a JSON string.  JSON serialisation/deserialisation is automatic.
    /////     Do not use this method for objects containing payloads where successful JSON serialisation is not guaranteed.
    ///// </summary>
    ///// <typeparam name="TReturn">The object returned in the HttpResponse. Transmitted in the form of a JSON string.</typeparam>
    ///// <param name="uri">endpoint/controller/action?queries</param>
    ///// <param name="stringToSend">The raw string to send</param>
    ///// <param name="accessToken"></param>
    ///// <param name="cancellationToken"></param>
    ///// <returns>Object of type TReturn</returns>
    //Task<TReturn> PostRawStringAndReturnSerialisedObjectAsync<TReturn>(string uri, string stringToSend, string accessToken = "", CancellationToken cancellationToken = default);




    ///// <summary>
    /////     HTTP POST method where the underlying HttpClient sends and returns byte arrays.
    /////     This method converts the returned bytes to a string.
    /////     JSON deserialisation is automatic.
    /////     Do not use this method for objects containing returned objects where successful JSON serialisation is not guaranteed.
    ///// </summary>
    ///// <typeparam name="TSend">The object sent in the POST. Transmitted in the form of a JSON string.</typeparam>
    ///// <typeparam name="TReturn">The object returned in the HttpResponse. Transmitted in the form of a JSON string.</typeparam>
    ///// <param name="uri">endpoint/controller/action?queries</param>
    ///// <param name="objectToSend">The object to send</param>
    ///// <param name="accessToken"></param>
    ///// <param name="cancellationToken"></param>
    ///// <returns>Object of type TReturn</returns>
    //Task<TReturn> PostBytesAndReturnObjectUsingJsonAsync<TReturn>(string uri, byte[] objectToSend, string accessToken = "", CancellationToken cancellationToken = default);


    //   /// <summary>
    //   /// HTTP request that POSTs an object of a specified type as a serialised string.
    //   /// It expects to receive an HttpResponseMessage whose content is a string.
    //   /// It deserialises the string into the same type. 
    //   /// Serialisation and deserialisation is automatically in JSON.
    //   /// </summary>
    //   /// <typeparam name="TRequestAndResponse">The type of object conveyed in the POST and HttpResponse</typeparam>
    //   /// <param name="uri">endpoint/controller/action?queries</param>
    //   /// <param name="objectToSend">The object to post</param>
    //   /// <param name="accessToken"></param>
    //   /// <param name="cancellationToken"></param>
    //   /// <returns>Object of type TRequestAndResponse</returns>
    //Task<TRequestAndResponse> PostObjectAsync<TRequestAndResponse>(string uri, TRequestAndResponse objectToSend, string accessToken = "",
    // CancellationToken cancellationToken = default);

    //   /// <summary>
    //   /// HTTP request that PUTSs an object of a specified type as a serialised string.
    //   /// It expects to receive an HttpResponseMessage whose content is a string.
    //   /// It deserialises the string into the identical specified type. 
    //   /// Serialisation and deserialisation is automatically in JSON.
    //   /// </summary>
    //   /// <typeparam name="TRequestAndResponse">The type of object conveyed in the PUT and HttpResponse</typeparam>
    //   /// <param name="uri">endpoint/controller/action?queries</param>
    //   /// <param name="objectToSend">The object to PUT</param>
    //   /// <param name="accessToken"></param>
    //   /// <param name="cancellationToken"></param>
    //   /// <returns>Object of type TRequestAndResponse</returns>
    //Task<TRequestAndResponse> PutAsync<TRequestAndResponse>(string uri, TRequestAndResponse objectToSend, string accessToken = "",
    // CancellationToken cancellationToken = default);

    //   /// <summary>
    //   /// HTTP request that PUTSs an object of a specified request type as a serialised string.
    //   /// It expects to receive an HttpResponseMessage whose content is a string.
    //   /// It deserialises the string into the specified Result type. 
    //   /// Serialisation and deserialisation is automatically in JSON.
    //   /// </summary>
    //   /// <typeparam name="TRequest">The type of the object conveyed in the POST</typeparam>
    //   /// <typeparam name="TResponse">The type of object conveyed in the HttpResponse</typeparam>
    //   /// <param name="uri">endpoint/controller/action?queries</param>
    //   /// <param name="objectToSend">The object to PUT</param>
    //   /// <param name="accessToken"></param>
    //   /// <param name="cancellationToken"></param>
    //   /// <returns>Object of type TResponse</returns>
    //Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest objectToSend, string accessToken = "",
    // CancellationToken cancellationToken = default);

    #endregion
}