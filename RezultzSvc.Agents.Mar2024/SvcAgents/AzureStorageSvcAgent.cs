using System;
using System.Threading;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces03.Apr2022;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Agents.Mar2024.SvcAgents;

public class AzureStorageSvcAgent : IAzureStorageSvcAgent
{
    private const string Locus2 = nameof(AzureStorageSvcAgent);
    private const string Locus3 = "[RezultzSvc.Agents.Mar2024]";

    #region fields

    private readonly IAzureStorageServiceClient _myClient;

    #endregion

    #region ctor

    public AzureStorageSvcAgent(IAzureStorageServiceClient clientInstance)
    {
        _myClient = clientInstance;
    }

    #endregion

    #region methods

    /// <summary>
    ///     If the WCF or REST svc is responding returns True.
    ///     If the svc cannot be reached will throw an JghCommunicationFailureException with a curt message
    /// </summary>
    /// <param name="ct"></param>
    /// <returns>Task type bool if AOK</returns>
    /// <exception cref="AggregateException">outer wrapper containing all exceptions</exception>
    /// <exception cref="JghCarrierException">inner wrapper containing all exceptions</exception>
    /// <exception cref="JghCommunicationFailureException">
    ///     innermost exception type used for handled exceptions and commonplace
    ///     communication exceptions
    /// </exception>
    public async Task<bool> ThrowIfNoServiceConnectionAsync(CancellationToken ct = default)
    {
        const string failure = "Testing service availability.";
        const string locus = "[ThrowIfNoServiceConnectionAsync]";

        try
        {
            var answer = await _myClient.GetIfServiceIsAnsweringAsync(ct);

            if (answer)
                return true;

            throw new JghCommunicationFailureException(StringsSvcAgents.UnableToEstablishConnectionWithServer);
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to determine if remote server is answering.";
        const string locus = "[GetIfServiceIsAnsweringAsync]";

        try
        {
            var answer = await _myClient.GetIfServiceIsAnsweringAsync(ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    /// <summary>
    ///     Returns a pretty-printed array of line items of all available particulars for every endpoint
    ///     using WCF or REST service client as case may be.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to obtain details of service endpoints on server.";
        const string locus = "[GetServiceEndpointsInfoAsync]";

        try
        {
            var answer = await _myClient.GetServiceEndpointsInfoAsync(ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    /// <summary>
    ///     Determines if a container exists using WCF or REST service client as case may be.
    /// </summary>
    /// <param name="account"></param>
    /// <param name="container"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> GetIfContainerExistsAsync(string account, string container, CancellationToken ct = default)
    {
        const string failure = "Unable to determine if specified container exists in specified account in remote storage used by service.";
        const string locus = "[GetIfContainerExistsAsync]";

        try
        {
            var answer = await _myClient.GetIfContainerExistsAsync(account, container, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    /// <summary>
    ///     Pre-formatted selective enumeration of blob information using WCF or REST service client as case may be.
    /// </summary>
    /// <param name="account"></param>
    /// <param name="container"></param>
    /// <param name="contains"></param>
    /// <param name="mustPrintDescriptionAsOpposedToBlobName"></param>
    /// <param name="ct"></param>
    /// <returns>Task type string collection</returns>
    /// <exception cref="AggregateException">outer wrapper containing all exceptions</exception>
    /// <exception cref="JghCarrierException">inner wrapper containing all exceptions</exception>
    /// <exception cref="JghCommunicationFailureException">
    ///     innermost exception type used for handled exceptions and commonplace
    ///     communication exceptions
    /// </exception>
    public async Task<string[]> GetNamesOfBlobsInContainerAsync(string account, string container, string contains, bool mustPrintDescriptionAsOpposedToBlobName, CancellationToken ct = default)
    {
        const string failure = "Unable to obtain names of blobs in specified account and container in remote storage used by service.";
        const string locus = "[GetNamesOfBlobsInContainerAsync]";

        try
        {
            var answer =
                await _myClient.GetNamesOfBlobsInContainerAsync(account, container, contains, mustPrintDescriptionAsOpposedToBlobName,
                    ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    /// <summary>
    ///     Determines if a blob exists using WCF or REST service client as case may be.
    /// </summary>
    /// <param name="account"></param>
    /// <param name="container"></param>
    /// <param name="blob"></param>
    /// <param name="ct"></param>
    /// <returns>true if successful or if file did not exist in the first place</returns>
    /// <exception cref="AggregateException">Async exception wrapper for all exceptions</exception>
    /// <exception cref="ArgumentNullException">Innermost exception if any of the parameters are null.</exception>
    /// <exception cref="JghCommunicationFailureException">Innermost exception if internet connection appears to be limited.</exception>
    public async Task<bool> GetIfBlobExistsAsync(string account, string container, string blob, CancellationToken ct = default)
    {
        const string failure = "Unable to determine if blob exists in specified account and container in remote storage used by service.";
        const string locus = "[GetIfBlobExistsAsync]";

        try
        {
            var answer = await _myClient.GetIfBlobExistsAsync(account, container, blob, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    /// <summary>
    ///     Deletes blob if it exists
    /// </summary>
    /// <param name="account"></param>
    /// <param name="container"></param>
    /// <param name="blob"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> DeleteBlobIfExistsAsync(string account, string container, string blob, CancellationToken ct = default)
    {
        const string failure = "Unable to delete specified blob in specified account and container in remote storage used by service.";
        const string locus = "[DeleteBlobIfExistsAsync]";

        try
        {
            var answer = await _myClient.DeleteBlockBlobIfExistsAsync(account, container, blob, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<string> GetAbsoluteUriOfBlockBlobAsync(string account, string container, string blob, CancellationToken ct = default)
    {
        var answer = await _myClient.GetAbsoluteUriOfBlobAsync(account, container, blob, ct);

        return answer;
    }

    /// <summary>
    ///     Uploads string as a blob
    /// </summary>
    /// <param name="account"></param>
    /// <param name="container"></param>
    /// <param name="blob"></param>
    /// <param name="payload"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<bool> UploadStringAsync(string account, string container, string blob, string payload, CancellationToken ct = default)
    {
        const string failure = "Unable to upload payload to specified blob in specified account and container in remote storage used by service.";
        const string locus = "[UploadStringAsync]";

        try
        {
            JghFilePathValidator.ThrowArgumentExceptionIfStorageParticularsAreInvalid("dummy", "dummy", blob,
                StringsSvcAgents.RemoteCallPreventedMsg); // let Azure catch account and container name errors, but we want to intervene more strictly to prevent bob names being attempted that are incompatible with Rezultz

            var outcome = await _myClient.UploadStringToBlockBlobAsync(account, container, blob, false, payload, ct);

            if (outcome == false)
                throw new JghAlertMessageException("The upload operation returned False, indicating lack of success."); // dummy. we should never arrive here. if fail for any reason, UploadBytesToBlockBlobAsync() must throw 
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        return true;
    }

    /// <summary>
    ///     Uploads byte array as blob to Azure using WCF or REST service client as case may be
    ///     Compresses the array if requested, adding a .gz extension.
    /// </summary>
    /// <param name="account"></param>
    /// <param name="container"></param>
    /// <param name="blob"></param>
    /// <param name="payload"></param>
    /// <param name="ct"></param>
    /// <returns>true if successful</returns>
    /// <exception cref="AggregateException">Async exception wrapper for all exceptions</exception>
    /// <exception cref="Jgh404Exception">
    ///     Innermost exception if account and/or container cannot be located or
    ///     accessed.
    /// </exception>
    /// <exception cref="ArgumentNullException">Innermost exception if any of the parameters are null.</exception>
    /// <exception cref="JghCommunicationFailureException">
    ///     Innermost exception if internet connection appears to be limited or
    ///     the Wcf service operation returns a response of 'false' for any reason.
    /// </exception>
    public async Task<bool> UploadBytesAsync(string account, string container, string blob, byte[] payload, CancellationToken ct = default)
    {
        const string failure = "Unable to upload payload to specified blob in specified account and container in remote storage used by service.";
        const string locus = "[UploadBytesAsync]";

        try
        {
            JghFilePathValidator.ThrowArgumentExceptionIfStorageParticularsAreInvalid("dummy", "dummy", blob, StringsSvcAgents.RemoteCallPreventedMsg);
            // let Azure catch account and container name errors, but we want to intervene more strictly to prevent bob names being attempted that are incompatible with Rezultz

            var outcome = await _myClient.UploadBytesToBlockBlobAsync(account, container, blob, true, payload, ct);

            if (outcome == false) throw new JghAlertMessageException("The upload operation returned False, indicating lack of success."); // dummy. we should never arrive here. if fail for any reason, UploadBytesAsync() must throw 
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        return true;
    }


    /// <summary>
    ///     Gets a single Cloud blob as byte array using WCF or REST service client as case may be.
    ///     Automatically unzips blobs with .gz extension using System.IO.Compression library.
    ///     Throws aggregate exception as the wrapper for all inner exceptions.
    ///     JghCommunicationFailureException as an inner exception if request fails and/or
    ///     resource not obtained for any reason whatsoever including an invalidly formatted uri.
    ///     Inner exception is Jgh404Exception if the blob or its container is not found or if the security
    ///     credentials provided under the covers by the service are invalid.
    ///     Inner exception is JghCommunicationFailureException if connection is limited
    ///     Inner exception is InvalidOperationException if for any reason the unzipping of the blob (if any) fails.
    /// </summary>
    /// <param name="account"></param>
    /// <param name="container"></param>
    /// <param name="blob"></param>
    /// <param name="ct"></param>
    /// <returns>Blob as bytes</returns>
    /// <exception cref="AggregateException">Async exception wrapper for all exceptions</exception>
    /// <exception cref="Jgh404Exception">
    ///     Innermost exception if account and/or container cannot be located or
    ///     accessed.
    /// </exception>
    /// <exception cref="ArgumentNullException">Innermost exception if any of the parameters are null.</exception>
    /// <exception cref="JghCommunicationFailureException">
    ///     Innermost exception if internet connection appears to be limited or
    ///     the Wcf service operation returns a response of 'false' for any reason.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Innermost exception if the unzipping of the blob fails for any reason.
    /// </exception>
    public async Task<byte[]> DownloadBytesThrowingExceptionWithErrorMessageUponFailureAsync(string account, string container, string blob, CancellationToken ct)
    {
        const string failure = "Unable to download contents of specified blob in specified account and container in remote storage used by service.";
        const string locus = "[DownloadBlobAsBytesThrowingExceptionWithErrorMessageUponFailureAsync]";

        try
        {
            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

            var answerAsBytes = await _myClient.DownloadBlockBlobAsBytesAsync(account, container, blob, ct);

            try
            {
                if (blob.EndsWith(StandardFileTypeSuffix.Gz)) answerAsBytes = await JghCompression.DecompressAsync(answerAsBytes); // decompress if compressed
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unable to decompress file/blob. This can happen if the file/blob extension is misleading or if the the format used to compress the file is inconsistent with the format being used to decompress the file. A valid compression format is indicated by a .{StandardFileTypeSuffix.Gz} extension.  ",
                    ex);
            }

            return answerAsBytes;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion
}