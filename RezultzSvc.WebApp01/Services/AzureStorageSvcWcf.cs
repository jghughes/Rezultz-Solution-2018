using System.Text;
using CoreWCF;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using RezultzSvc.Library01.Mar2024.SvcHelpers;
using RezultzSvc.WebApp01.Interfaces;

namespace RezultzSvc.WebApp01.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    partial class AzureStorageSvcWcf : IAzureStorageSvc
    {
        #region ctor

        public AzureStorageSvcWcf(ILogger<AzureStorageSvcWcf> logger)
    {
        _azureStorageServiceMethodsHelperInstance =
            new AzureStorageServiceMethodsHelper(new AzureStorageAccessor());

        _logger = logger;
        // logger registered by CoreWcf with DI as an instance in appsettings.json

    }

        #endregion

        #region fields

        private readonly ILogger<AzureStorageSvcWcf> _logger;

        private readonly AzureStorageServiceMethodsHelper _azureStorageServiceMethodsHelperInstance;

        #endregion

        #region svc methods

        /// <summary>
        ///     Returns true.
        /// </summary>
        /// <returns>Returns (true) if service answers. Otherwise exception will have been thrown at site of call to svc.</returns>
        public async Task<bool> GetIfServiceIsAnsweringAsync([Injected] HttpRequest httpRequest)
    {
        #region logging

        StringBuilder sb = new StringBuilder();

        sb.AppendLine();

        foreach (var header in httpRequest.Headers)
        {
            sb.AppendLine($"{header.Key} : {header.Value}");
        }

        _logger.LogInformation(sb.ToString());

        sb.AppendLine();

        _logger.LogInformation("GetIfServiceIsAnsweringAsync() was called. Returned True");

        sb.AppendLine();

        #endregion

        // if we have got this far, the svc is answering! this here is inside the svc!

        return await Task.FromResult(true);
    }

        /// <summary>
        ///     Probes Wcf service host to obtain visible particulars of svc endpoints, enumerated as a pretty-printed
        ///     string array of line items.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        /// <returns>Pretty-printed string array of line items of service endpoints and their descriptions</returns>
        public async Task<string[]> GetServiceEndpointsInfoAsync()
    {
        //const string failure = "Unable to do what this method does.";
        //const string locus = "[GetServiceEndpointsInfoAsync]";

        try
        {
            var context = OperationContext.Current;

            var answer = CoreWcfHelpers.PrettyPrintOperationContextInfo(context);

            return await Task.FromResult(answer);
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

        /// <summary>
        ///     Determines if container exists.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <exception cref="FaultException">
        ///     Most likely caused by invalid Azure storage parameters or an unexpected
        ///     StorageException
        /// </exception>
        /// <returns>True if exists, otherwise false.</returns>
        public async Task<bool> GetIfContainerExistsAsync(string accountName, string containerName)
    {
        try
        {
            var answer = await
                _azureStorageServiceMethodsHelperInstance.GetIfContainerExistsAsync(accountName, containerName);

            return answer;
        }
        catch (Exception ex)
        {
            var innermost = JghExceptionHelpers.FindInnermostException(ex);

            var rfc7807 = new JghError(innermost);

            throw new FaultException<JghFault>(new JghFault(rfc7807), new FaultReason(innermost.Message));
        }
    }

        /// <summary>
        ///     Prints particulars of blobs in container as a pretty printed string array.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="contains"></param>
        /// <param name="mustPrintDescriptionAsOpposedToBlobName"></param>
        /// <exception cref="FaultException">
        ///     Most likely caused by invalid Azure storage parameters or an unexpected
        ///     StorageException
        /// </exception>
        /// <returns>string array, being the line-items in pretty-printed format</returns>
        public async Task<string[]> GetNamesOfBlobsInContainerAsync(string accountName, string containerName,
            string contains, bool mustPrintDescriptionAsOpposedToBlobName)
    {
        try
        {
            var answer = await
                _azureStorageServiceMethodsHelperInstance.GetNamesOfBlobsInContainerAsync(accountName,
                    containerName, contains, mustPrintDescriptionAsOpposedToBlobName);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

        /// <summary>
        ///     Checks if a blob exists in the specified account and container.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <exception cref="FaultException">
        ///     Most likely caused by invalid Azure storage parameters or an unexpected
        ///     StorageException
        /// </exception>
        /// <returns>True if exists, otherwise false.</returns>
        public async Task<bool> GetIfBlobExistsAsync(string accountName, string containerName, string blobName)
    {
        try
        {
            var answer = await
                _azureStorageServiceMethodsHelperInstance.GetIfBlobExistsAsync(accountName,
                    containerName, blobName);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

        /// <summary>
        ///     Marks blob for deletion if it exists
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <exception cref="FaultException">
        ///     Most likely caused by invalid Azure storage parameters or an unexpected
        ///     StorageException
        /// </exception>
        /// <returns>True if the blob did already exist and was deleted; otherwise false</returns>
        public async Task<bool> DeleteBlockBlobIfExistsAsync(string accountName, string containerName, string blobName)
    {
        try
        {
            var answer = await _azureStorageServiceMethodsHelperInstance.BlockBlobDeleteIfExistsAsync(accountName, containerName, blobName);

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }

        #endregion
    }

        /// <summary>
        ///     Obtains the absolute uri of blob. if it doesn't exist returns empty string.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <exception cref="FaultException">
        ///     Most likely caused by invalid Azure storage parameters or an unexpected
        ///     StorageException
        /// </exception>
        /// <returns>Absolute uri or empty string if blob doesn't exist.</returns>
        public async Task<string> GetAbsoluteUriOfBlockBlobAsync(string accountName, string containerName, string blobName)
    {
        try
        {
            var answer = await _azureStorageServiceMethodsHelperInstance.GetAbsoluteUriOfBlockBlobAsync(accountName, containerName, blobName);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

        /// <summary>
        ///     Uploads string to a blockblob, overwriting any existing blob of the same name.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="createContainerIfNotExists"></param>
        /// <param name="content">string</param>
        /// <exception cref="FaultException">
        ///     Most likely caused by invalid Azure storage parameters or an unexpected
        ///     StorageException
        /// </exception>
        /// <returns>True if upload is verified, otherwise false.</returns>
        public async Task<bool> UploadStringToBlockBlobAsync(string accountName, string containerName, string blobName, bool createContainerIfNotExists, string content)
    {
        try
        {
            var answer = await _azureStorageServiceMethodsHelperInstance.UploadToBlockBlobAsStringAsync(accountName, containerName, blobName, createContainerIfNotExists, content);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

        /// <summary>
        ///     Uploads byte array to a blockblob, overwriting any existing blob of the same name.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="createContainerIfNotExists"></param>
        /// <param name="content"></param>
        /// <exception cref="FaultException">
        ///     Most likely caused by invalid Azure storage parameters or an unexpected
        ///     StorageException
        /// </exception>
        /// <returns>True if upload is verified, otherwise false.</returns>
        public async Task<bool> UploadBytesToBlockBlobAsync(string accountName, string containerName, string blobName, bool createContainerIfNotExists, byte[] content)
    {
        try
        {
            var answer = await
                _azureStorageServiceMethodsHelperInstance.UploadToBlockBlobAsBytesAsync(accountName, containerName, blobName, createContainerIfNotExists, content);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

        /// <summary>
        ///     Secure call to retrieve blob as BinaryData.
        ///     Get blockblob from Azure storage.
        ///     Access security requires a valid account security key to be obtainable from
        ///     Jgh.Portable.AzureParticulars.May2017.AccessCredentialsForRecognizedAzureStorageAccounts.cs
        ///     Returns a Svc FaultException if the operation that powers this service throws an exception, namely
        ///     NetStd.AzureStorageAccess.July2018.AzureStorageAccessor.DownloadBlobAsBytesAsync().
        ///     Aggregate exception is wrapper for all inner exceptions thrown by this method. Innermost exception
        ///     is Jgh404Exception if blob is not found either because the container
        ///     or the blob doesn't exist or because the account security key is invalid.
        ///     StorageException is the innermost exception for remaining storage exceptions.
        ///     whatever the type of innermost exception, FaultException encapsulates only the message from the innermost
        ///     exception.
        ///     The message from a Jgh404Exception will contain the
        ///     string "404", thus providing a clue to its genesis to receivers.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <exception cref="FaultException">
        ///     Thrown if blob not found. Alternatively caused by invalid Azure storage parameters, or
        ///     an unexpected StorageException
        /// </exception>
        /// <returns>Type byte[]</returns>
        public async Task<byte[]> DownloadBlockBlobAsync(string accountName, string containerName, string blobName)
    {
        try
        {
            var answer = await _azureStorageServiceMethodsHelperInstance.DownloadBlockBlobAsBytesAsync(accountName, containerName, blobName);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

        #endregion

    }
}