using System.Text;
using CoreWCF;
using NetStd.AzureStorageAccess.July2018;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using RezultzSvc.Library01.Mar2024.SvcHelpers;
using RezultzSvc.WebApp01.Interfaces;

namespace RezultzSvc.WebApp01.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
internal partial class TimeKeepingSvcWcf : ITimeKeepingSvc
{
    #region ctor

    public TimeKeepingSvcWcf(ILogger<TimeKeepingSvcWcf> logger)
    {
        _hubServiceMethodsHelperInstance = new HubServiceMethodsHelper(new AzureStorageAccessor());

        _logger = logger;
        // logger registered by CoreWcf with DI as an instance in appsettings.json
    }

    #endregion

    #region fields

    private readonly ILogger<TimeKeepingSvcWcf> _logger;

    private readonly HubServiceMethodsHelper _hubServiceMethodsHelperInstance;

    #endregion

    #region methods

    public async Task<bool> GetIfServiceIsAnsweringAsync([Injected] HttpRequest httpRequest)
    {
        #region logging

        var sb = new StringBuilder();

        sb.AppendLine();

        foreach (var header in httpRequest.Headers) sb.AppendLine($"{header.Key} : {header.Value}");

        _logger.LogInformation(sb.ToString());

        sb.AppendLine();

        _logger.LogInformation("GetIfServiceIsAnsweringAsync() was called. Returned True");

        sb.AppendLine();

        #endregion

        // if we have got this far, the svc is answering! this here is inside the svc!

        return await Task.FromResult(true);
    }

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

    public async Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer)
    {
        try
        {
            var answer = await _hubServiceMethodsHelperInstance.GetIfContainerExistsAsync(databaseAccount, dataContainer, CancellationToken.None);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<string> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, string clockItemAsJson)
    {
        // NB: ONLY use this method for singletons i.e. for PINs and Check-ins and Profiles.
        // in our blobStorage schema for singletons we use tablePartition==RecordingModeEnum
        // and tableRowKey==Identifier for check-in and profile, and tableRowKey==ClockModeSymbolPin for the one and only PIN.
        // why? because in our system, only singletons have row keys which can be deduced for lookups. 
        // things which aren't singletons have row keys that are complex compound keys that fail in round tripping 

        // currently, the tablePartition and tableRowKey parameters in this signature are superfluous because the _hubServiceMethodsHelperInstance method deduces
        // partition and row key from the contents of the clock item. i'm only leaving them in the signature for the sake of symmetry
        // with the signature of GetItemAsync(), and as placeholders for some unforeseen use in future

        try
        {
            await _hubServiceMethodsHelperInstance.PostItemAsync(databaseAccount, dataContainer, clockItemAsJson, CancellationToken.None);

            return clockItemAsJson;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<string> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, byte[] itemArrayAsJsonAsCompressedBytes)
    {
        try
        {
            var decompressedBytesUtf8 =
                await JghCompression.DecompressAsync(itemArrayAsJsonAsCompressedBytes);

            var itemArrayAsJson = JghConvert.ToStringFromUtf8Bytes(decompressedBytesUtf8);

            var answer = await _hubServiceMethodsHelperInstance.PostItemArrayAsyncV2(databaseAccount, dataContainer, itemArrayAsJson, CancellationToken.None);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<string> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey)
    {
        // NB: ONLY use this method for singletons i.e. for PINs and Check-ins and Profiles.
        // in our blobStorage schema for singletons we use tablePartition==RecordingModeEnum
        // and tableRowKey==Identifier for check-in and profile, and tableRowKey==ClockModeSymbolPin for the one and only PIN.
        // why? because in our system, only singletons have row keys which can be deduced for lookups. 
        // things which aren't singletons have row keys that are complex compound keys that fail in round tripping 

        try
        {
            var answer = await _hubServiceMethodsHelperInstance.GetItemAsync(databaseAccount, dataContainer, tablePartition, tableRowKey, CancellationToken.None);

            return answer;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<byte[]> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer)
    {
        try
        {
            var answer = await _hubServiceMethodsHelperInstance.GetArrayOfHubItemAsync(databaseAccount, dataContainer, CancellationToken.None);

            var bytesUtf8 = JghConvert.ToBytesUtf8FromString(answer);

            var answerCompressed = await JghCompression.CompressAsync(bytesUtf8);

            return answerCompressed;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    #endregion
}