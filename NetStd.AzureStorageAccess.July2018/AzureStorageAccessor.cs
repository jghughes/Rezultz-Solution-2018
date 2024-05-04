using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;

namespace NetStd.AzureStorageAccess.July2018
{
    //	/// <summary>
    //	///     Multi-purpose client for access to Azure storage securely.
    //	///     Uses modern Azure.Storage.Blobs
    //	/// </summary>

    public class AzureStorageAccessor : IAzureStorageAccessor
    {
        private const string Locus2 = nameof(AzureStorageAccessor);
        private const string Locus3 = "[NetStd.AzureStorageAccess.July2018]";

        public const string InvalidStorageConnectionString = "Invalid storage account connection string.";

        #region methods

        ///  <summary>
        ///      Checks if container exists. Returns true or false.
        /// 		Throws informative exception if the remote process failed.
        ///  </summary>
        ///  <param name="accountConnectionString"></param>
        ///  <param name="container"></param>
        ///  <param name="ct"></param>
        ///  <returns>
        ///      type Response&lt;bool&gt;
        ///  </returns>
        ///  <exception cref="System.ArgumentNullException">storageAccountConnectionString, containerName</exception>
        ///   <exception cref="JghAzureRequestException ">invalidly formatted account name, security key, container name or blob name</exception>
        public async Task<Response<bool>> GetIfContainerExistsAsync(string accountConnectionString, string container, CancellationToken ct = default)
        {
            const string failure = "Unable to determine if container exists.";
            const string locus = "[GetIfContainerExistsAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString); // parse connection string and create URI. throws format exception if fail 

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative RequestFailedException if there are any problems whatsoever with the account credentials.
				 * When called behind a server-side service it doesn't throw - it inscrutibly returns false
				 */

                Response<bool> answer = await blobContainerClient.ExistsAsync(cancellationToken: ct);

                return answer;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate
                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion

        }

        ///  <summary>
        ///      Creates container if doesn't already exist
        /// 		Throws informative exception if the remote process failed.
        ///  </summary>
        ///  <param name="accountConnectionString"></param>
        ///  <param name="container"></param>
        ///  <exception cref="System.ArgumentNullException">storageAccountConnectionString, containerName</exception>
        ///    <exception cref="JghAzureRequestException ">invalidly formatted account name, security key, container name or blob name</exception>
        ///  <returns>type bool. True if existence is confirmed, otherwise false.</returns>
        public async Task<bool> CreateContainerAsync(string accountConnectionString, string container)
        {
            const string failure = "Unable to create container.";
            const string locus = "[CreateContainerAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);


                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                // ReSharper disable once UnusedVariable
                var createdInfo = await blobContainerClient.CreateIfNotExistsAsync(); // throws if fail

                return true;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate
                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion
        }

        ///  <summary>
        ///      Gets the blob names or pretty-prints the names with available metatdata in a descriptive sentence for each blob, pageblob and blockblob alike
        /// 		Throws informative exception if the remote process failed.
        ///  </summary>
        ///  <param name="accountConnectionString"></param>
        ///  <param name="container"></param>
        ///  <param name="contains">
        ///      selection criterion. optional substring that must exist within blobnames. use "*" or null or
        ///      string.Empty to select all
        ///  </param>
        ///  <param name="ct"></param>
        ///  <exception cref="System.ArgumentNullException">storageAccountConnectionString, containerName</exception>
        ///  <exception cref="JghAzureRequestException ">invalidly formatted account name, security key, or container name</exception>
        ///  <returns>type Response&lt;string[]&gt;, being the line-items in pretty-printed or name format</returns>
        public async Task<BlobItem[]> ListBlobsInContainerAsync(string accountConnectionString, string container, string contains, CancellationToken ct = default)
        {
            const string failure = "Unable to list blobs.";
            const string locus = "[ListBlobItemsInContainerAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                var blobItems = new List<BlobItem>();

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);


                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                // Call the listing operation and return pages of the specified size. default page size is 5,000

                IAsyncEnumerable<Page<BlobItem>> resultSegment = blobContainerClient.GetBlobsAsync(default, default, default, ct)
                    .AsPages(); // absence of await is intentional. we don't call Azure here. call azure in the awaitable foreach that follows

                // Enumerate the blobs returned for each page. (with await)

                await foreach (Page<BlobItem> blobPage in resultSegment.WithCancellation(ct))
                {
                    blobItems.AddRange(blobPage.Values.Where(blobItem => IsMatchingIdentity(contains, blobItem.Name)));
                }

                var answer = blobItems.ToArray();

                return answer;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate
                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion
        }

        ///   <summary>
        ///       Checks if blob exists. Returns true or false. To return true, both the container and the blob have to exist.
        /// 		Throws informative exception if the remote process failed.
        ///   </summary>
        ///   <param name="accountConnectionString"></param>
        ///   <param name="container"></param>
        ///   <param name="blob"></param>
        ///   <param name="ct"></param>
        ///   <returns>
        ///       type Response&lt;bool&gt;
        ///   </returns>
        ///   <exception cref="System.ArgumentNullException">storageAccountConnectionString, containerName, blobname</exception>
        ///   <exception cref="JghAzureRequestException">invalidly formatted account name, security key or container name, or some other problem</exception>
        ///   <exception cref="System.Exception"></exception>
        public async Task<Response<bool>> GetIfBlobExistsAsync(string accountConnectionString, string container, string blob, CancellationToken ct = default)
        {
            const string failure = "Unable to determine if blob exists.";
            const string locus = "[GetIfBlobExistsAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                /*
				 * WARNING I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                var blobClient = blobContainerClient.GetBlobClient(blob); // append blob name to URI

                /*
				 * WARNING I have determined empirically that when called on the client-side the following method
				 * throws an informative RequestFailedException if there are any problems whatsoever with the account credentials.
				 * When called behind a server-side service it doesn't throw - it inscrutibly returns false
				 */

                Response<bool> answer = await blobClient.ExistsAsync(ct);

                return answer;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate
                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion
        }

        ///   <summary>
        ///       Deletes blob if it exists. Returns true if a deletion occurred.
        /// 		Returns false if the container or blob didn't exist and no deletion occurred.
        /// 		Throws informative exception if the remote process failed.
        ///   </summary>
        ///   <param name="accountConnectionString"></param>
        ///   <param name="container"></param>
        ///   <param name="blob"></param>
        ///   <param name="ct"></param>
        ///   <returns>
        ///       type bool
        ///   </returns>
        ///   <exception cref="System.ArgumentNullException">storageAccountConnectionString, containerName, blobname</exception>
        ///   <exception cref="JghAzureRequestException">invalidly formatted account name, security key or container name, or some other problem</exception>
        ///   <exception cref="System.Exception"></exception>
        public async Task<Response<bool>> DeleteBlobIfExistsAsync(string accountConnectionString, string container, string blob, CancellationToken ct = default)
        {
            const string failure = "Unable to delete blob.";
            const string locus = "[DeleteBlockBlobIfExistsAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                var blobClient = blobContainerClient.GetBlobClient(blob); // append blob name to URI

                /*
				 * If we have arrived here but the container doesn't exist throws RequestFailedException
				 */

                return await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, ct);
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate
                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion
        }

        /// <summary>
        ///     Obtains the absolute uri of blob. if it doesn't exist returns empty string
        /// </summary>
        /// <param name="accountConnectionString"></param>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <param name="ct"></param>
        /// <exception cref="System.ArgumentNullException">storageAccountConnectionString, containerName</exception>
        ///  <exception cref="JghAzureRequestException ">invalidly formatted account name, security key, container name or blob name</exception>
        /// <returns>Absolute uri or empty string if blob doesn't exist</returns>
        public async Task<string> GetAbsoluteUriOfBlobAsync(string accountConnectionString, string container, string blob, CancellationToken ct = default)
        {
            const string failure = "Unable to obtain uri of blob.";
            const string locus = "[GetAbsoluteUriOfBlockBlobAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);


                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                var blobClient = blobContainerClient.GetBlobClient(blob); // append blob name to URI

                /*
				 * If we have arrived here but the container doesn't exist throws RequestFailedException
				 */

                Response<bool> blobDoesExistResponse = await blobClient.ExistsAsync(ct);

                if (!blobDoesExistResponse.Value)
                    return null;

                var answer = blobClient.Uri.AbsoluteUri;

                return answer;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate
                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion
        }

        public async Task<Response<BlobContentInfo>> UploadStringAsync(string accountConnectionString, string container, string blob, bool createContainerIfNotExist, string content, CancellationToken ct = default)
        {
            const string failure = "Unable to upload.";
            const string locus = "[UploadBlobAsStringAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                if (content == null)
                    throw new ArgumentNullException(nameof(content));

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                if (createContainerIfNotExist)
                    await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: ct);

                var blobClient = blobContainerClient.GetBlobClient(blob); // append blob name to URI

                var apparentFileExtension = MimeTypeMap.GetExtensionFromName(blobClient.Name);

                var mostLikelyMimeTypeTitle = MimeTypeMap.GetMimeType(apparentFileExtension);

                /*
				 * If we have arrived here but the container doesn't exist throws RequestFailedException
				 */

                using var contentAsStream = JghConvert.ToStreamFromString(content);

                Response<BlobContentInfo> answer = await blobClient.UploadAsync(contentAsStream, new BlobHttpHeaders { ContentType = mostLikelyMimeTypeTitle }, cancellationToken: ct);

                return answer;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate
                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion
        }

        public async Task<Response<BlobContentInfo>> UploadBytesAsync(string accountConnectionString, string container, string blob, bool createContainerIfNotExist, byte[] content, CancellationToken ct = default)
        {
            const string failure = "Unable to upload.";
            const string locus = "[UploadBlobAsBytesAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                if (content == null)
                    throw new ArgumentNullException(nameof(content));

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                if (createContainerIfNotExist)
                    await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: ct);

                var blobClient = blobContainerClient.GetBlobClient(blob); // append blob name to URI

                var apparentFileExtension = MimeTypeMap.GetExtensionFromName(blobClient.Name);

                var mostLikelyMimeTypeTitle = MimeTypeMap.GetMimeType(apparentFileExtension);

                using var contentAsStream = new MemoryStream(content, false) { Position = 0 };

                /*
				 * If we have arrived here but the container doesn't exist throws RequestFailedException
				 */

                Response<BlobContentInfo> answer = await blobClient.UploadAsync(contentAsStream, new BlobHttpHeaders { ContentType = mostLikelyMimeTypeTitle }, cancellationToken: ct);

                return answer;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate

                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                var innermost = JghExceptionHelpers.FindInnermostException(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            #endregion
        }


        ///  <summary>
        ///      Get blob as BlobDownloadResult from Azure storage.
        ///     Throws informative exception if the remote process failed for any reason or blob not found.
        ///  </summary>
        ///  <param name="accountConnectionString"></param>
        ///  <param name="container"></param>
        ///  <param name="blob"></param>
        ///  <param name="ct"></param>
        ///  <exception cref="System.ArgumentNullException">storageAccountConnectionString, containerName, blob name</exception>
        ///   <exception cref="JghAzureRequestException ">invalidly formatted account name, security key, container name or blob name</exception>
        ///  <returns>type Response&lt;BlobDownloadInfo&gt;</returns>
        public async Task<Response<BlobDownloadResult>> DownloadAsync(string accountConnectionString, string container, string blob, CancellationToken ct = default)
        {
            const string failure = "Resource not found or unavailable.";
            const string locus = "[DownloadAsync]";

            try
            {
                if (accountConnectionString == null)
                    throw new ArgumentNullException(nameof(accountConnectionString));

                /*
				 * I have determined empirically that when called on the client-side the following method
				 * throws an informative FormatException if there are any problems parsing the connection string into a URI.
				 * e.g. if account key badly formatted. Its behaviour is totally different when called behind a server-side service.
				 * it doesn't throw. it leaves subsequent calls to Azure to blow up
				 */

                var blobServiceClient = new BlobServiceClient(accountConnectionString);


                var blobContainerClient = blobServiceClient.GetBlobContainerClient(container); //append container name to URI

                var blobClient = blobContainerClient.GetBlobClient(blob); // append blob name to URI

                /*
				 * If we have arrived here but the container doesn't exist throws RequestFailedException
				 */

                Response<BlobDownloadResult> answer = await blobClient.DownloadContentAsync(ct);

                return answer;
            }
            #region trycatch

            // if no connection, it tries and tries and then fails with a dumb AggregateException

            catch (RequestFailedException azureEx)
            {
                // a zillion things
                var firstSentence = JghString.TakeFirstSentence(azureEx.Message); // some Azure messages are too revealing. truncate

                var newEx = new JghAzureRequestException($"<AzureMessage: {firstSentence}> <AzureErrorCode: {azureEx.ErrorCode}>");

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (FormatException formatEx)
            {
                // parsing of the connection string failed
                var newEx = new JghAzureRequestException(JghString.ConcatAsSentences($"{InvalidStorageConnectionString} {formatEx.Message}"));
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, newEx);
            }
            catch (AggregateException aggEx)
            {
                // Azure frequently throws aggregate exceptions
                var innermost = JghExceptionHelpers.FindInnermostException(aggEx);
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, innermost);
            }

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region helpers

        private static bool IsMatchingIdentity(string contains, string blobName)
        {
            var isMatch = false;

            if (string.IsNullOrWhiteSpace(contains))
            {
                // wild card
                isMatch = true;
            }
            else
            {
                // wild card
                if (contains == "*")
                    isMatch = true;

                // matched
                if (blobName.Contains(contains))
                    isMatch = true;
            }

            return isMatch;
        }

        #endregion

    }
}
