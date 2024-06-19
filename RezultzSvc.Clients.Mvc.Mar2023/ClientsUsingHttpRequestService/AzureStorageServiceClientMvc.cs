using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Jgh.MvcParameters.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.HttpRequestService.Feb2019;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;


// ReSharper disable InconsistentNaming

namespace RezultzSvc.Clients.Mvc.Mar2023.ClientsUsingHttpRequestService
{
    /// <summary>
    ///     A MVC API is best developed in a service-first manner. Use hard coded strings for the names of
    ///     ApiController Route, HTTP action methods, and HTTP action parameters. Enable the API with Swagger to emit
    ///     endpoint information and use Swagger in your browser to read the information. Copy the hard coded strings
    ///     from Swagger into the URI strings used in your MVC client to make HTTP calls to the API.
    /// </summary>
    public class AzureStorageServiceClientMvc : ClientBaseMvc, IAzureStorageServiceClient
    {
        private const string Locus2 = nameof(AzureStorageServiceClientMvc);
        private const string Locus3 = "[RezultzSvc.Clients.Mvc.Mar2023]";

        #region ctor stuff

        public AzureStorageServiceClientMvc()
    {
        const string failure = "Unable to instantiate AzureStorageServiceClientMvc.";
        const string locus = "[AzureStorageServiceClientMvc]";

        try
        {
            ThisControllerRoute = $"{AppSettings.AzureStorageControllerBaseUrl}/{Routes.AzureStorageController}";
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        #endregion

        #region actions

        public async Task<bool> GetIfContainerExistsAsync(string account, string container, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfContainerExistsAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIfContainerExists, queryParameters);

            return await HttpRequestService.GetObjectAsync<bool>(route, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        public async Task<string[]> GetNamesOfBlobsInContainerAsync(string account, string container, string requiredSubstring, bool mustPrintDescriptionAsOpposedToBlobName, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetNamesOfBlobsInContainerAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(requiredSubstring)) throw new ArgumentNullException(nameof(requiredSubstring));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container),
                new(QueryParameters.RequiredSubstring, requiredSubstring),
                new(QueryParameters.MustPrintDescriptionAsOpposedToBlobName, mustPrintDescriptionAsOpposedToBlobName.ToString())
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetNamesOfBlobsInContainer, queryParameters);

            return await HttpRequestService.GetObjectAsync<string[]>(route, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        public async Task<bool> GetIfBlobExistsAsync(string account, string container, string blob, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetIfBlobExistsAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container),
                new(QueryParameters.Blob, blob)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetIfBlobExists, queryParameters);

            return await HttpRequestService.GetObjectAsync<bool>(route, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        public async Task<string> GetAbsoluteUriOfBlobAsync(string account, string container, string blob, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[GetAbsoluteUriOfBlockBlobAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container),
                new(QueryParameters.Blob, blob)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.GetAbsoluteUriOfBlockBlob, queryParameters);

            return await HttpRequestService.GetObjectAsync<string>(route, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        public async Task<bool> DeleteBlockBlobIfExistsAsync(string account, string container, string blob, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[DeleteBlockBlobIfExistsAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container),
                new(QueryParameters.Blob, blob)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.DeleteBlockBlobIfExists, queryParameters);

            return await HttpRequestService.GetObjectAsync<bool>(route, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        public async Task<bool> UploadBytesToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, byte[] bytesToUpload, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[UploadBytesToBlockBlobAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));
            if (bytesToUpload is null) throw new ArgumentNullException(nameof(bytesToUpload));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container),
                new(QueryParameters.Blob, blob),
                new(QueryParameters.CreateContainerIfNotExist, createContainerIfNotExist.ToString())
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.UploadBytesToBlockBlob, queryParameters);

            return await HttpRequestService.PostObjectAsync<byte[], bool>(route, bytesToUpload, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        public async Task<bool> UploadStringToBlockBlobAsync(string account, string container, string blob, bool createContainerIfNotExist, string stringToUpload, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[UploadStringToBlockBlobAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));
            if (string.IsNullOrWhiteSpace(stringToUpload)) throw new ArgumentNullException(nameof(stringToUpload));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container),
                new(QueryParameters.Blob, blob),
                new(QueryParameters.CreateContainerIfNotExist,
                    createContainerIfNotExist.ToString())
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.UploadStringToBlockBlob, queryParameters);

            return await HttpRequestService.PostObjectAsync<string, bool>(route, stringToUpload, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        public async Task<byte[]> DownloadBlockBlobAsBytesAsync(string account, string container, string blob, CancellationToken ct = default)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[DownloadBlockBlobAsBytesAsync]";

        var startTimestamp = DateTime.Now;

        try
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghCommunicationFailureException(StringsMar2023.NoConnection);

            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            if (string.IsNullOrWhiteSpace(container)) throw new ArgumentNullException(nameof(container));
            if (string.IsNullOrWhiteSpace(blob)) throw new ArgumentNullException(nameof(blob));

            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new(QueryParameters.Account, account),
                new(QueryParameters.Container, container),
                new(QueryParameters.Blob, blob)
            };

            var route = HttpRequestServiceHelpers.AppendNameValuePairsToUriInQueryStringFormat(ThisControllerRoute, Routes.DownloadBlockBlobAsBytes, queryParameters);

            return await HttpRequestService.GetObjectAsync<byte[]>(route, "", ct);
        }

        #region catch

        catch (InvalidOperationException invalidProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallInvalid, invalidProblem.Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (TaskCanceledException timeoutProblem)
        {
            var msg = JghString.ConcatAsSentences(StringsMar2023.CallTimedOut, JghExceptionHelpers.FindInnermostException(timeoutProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (HttpRequestException badRequest)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.HttpRequestExceptionThrown, JghExceptionHelpers.FindInnermostException(badRequest).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }
        catch (Exception unanticipatedProblem)
        {
            var msg = JghString.ConcatAsParagraphs(StringsMar2023.CallFailed, JghExceptionHelpers.FindInnermostException(unanticipatedProblem).Message, JghString.MakeWaitTimeMsg(startTimestamp));

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, new JghCommunicationFailureException(msg));
        }

        #endregion
    }

        #endregion
    }
}