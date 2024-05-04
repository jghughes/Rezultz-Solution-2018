using System.Text;
using CoreWCF;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using RezultzSvc.Library02.Mar2024.SvcHelpers;
using RezultzSvc.WebApp03.Interfaces;

namespace RezultzSvc.WebApp03.Services;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
internal partial class RaceResultsPublishingSvcWcf : IRaceResultsPublishingSvc
{
    #region ctor

    public RaceResultsPublishingSvcWcf(ILogger<RaceResultsPublishingSvcWcf> logger)
    {
        _raceResultsPublishingServiceMethodsHelper = new RaceResultsPublishingServiceMethodsHelper();

        _logger = logger;
        // logger registered by CoreWcf with DI as an instance in appsettings.json
    }

    #endregion

    #region fields

    private readonly ILogger<RaceResultsPublishingSvcWcf> _logger;

    private readonly RaceResultsPublishingServiceMethodsHelper _raceResultsPublishingServiceMethodsHelper;

    #endregion

    #region svc methods

    public async Task<bool> GetIfServiceIsAnsweringAsync([Injected] HttpRequest httpRequest)
    {
        #region logging

        StringBuilder sb = new();

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
        _logger.LogInformation("GetServiceEndpointsInfoAsync() was called.");

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

    public async Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment)
    {
        _logger.LogInformation("GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment) was called.");

        try
        {
            var isFound = await _raceResultsPublishingServiceMethodsHelper.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(fileNameFragment);

            return isFound;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<string> GetPublishingProfileAsync(string fileNameFragment)
    {
        _logger.LogInformation("GetPublishingProfileAsync(string fileNameFragment) was called.");

        try
        {
            var publisherProfileItem = await _raceResultsPublishingServiceMethodsHelper.GetPublisherModuleProfileItemAsync(fileNameFragment);

            var publisherProfileDto = PublisherModuleProfileItem.ToDataTransferObject(publisherProfileItem);

            var publisherProfileDtoAsJson = JghSerialisation.ToJsonFromObject(publisherProfileDto);

            return publisherProfileDtoAsJson;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync()
    {
        _logger.LogInformation("GetFileNameFragmentsOfAllPublishingProfilesAsync() was called.");

        try
        {
            var fileNamesAsStringArray = await _raceResultsPublishingServiceMethodsHelper.GetFileNameFragmentsOfAllPublishingProfilesAsync();

            return fileNamesAsStringArray;

        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<string> GetIllustrativeExampleOfDatasetExpectedForProcessingAsync(string fileNameWithExtension)
    {
        _logger.LogInformation("GetIllustrativeExampleOfDatasetExpectedForProcessingAsync(string sampleFileName) was called.");

        try
        {
            var fileContentsAsRawString = await _raceResultsPublishingServiceMethodsHelper.GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(fileNameWithExtension);

            return fileContentsAsRawString;

            // Note: don't try the following short cut! the string could be anything and may not be serialisable to JSON - for example if it is XML
            //var fileContentsAsStringAsCompressedBytesBack = await JghCompressionHelper.ConvertObjectToJsonAsCompressedBytesAsync(fileContentsAsRawString);
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<bool> UploadDatasetToBeProcessedSubsequentlyAsync(string identifierOfDataset, string accountName, string containerName, string fileName, string datasetAsRawString)
    {
        _logger.LogInformation("UploadDatasetToBeProcessedSubsequentlyAsync() was called.");

        try
        {
            var datasetAsRawStringAsBytes = JghConvert.ToBytesUtf8FromString(datasetAsRawString);

            var didSucceed = await _raceResultsPublishingServiceMethodsHelper.UploadDatasetAsBytesAsync(accountName, containerName, fileName, datasetAsRawStringAsBytes);

            return didSucceed;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<string> GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(string publisherInputDtoAsJson)
    {
        _logger.LogInformation("GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(byte[] publisherInputDtoAsJsonAsCompressedBytes) was called.");

        try
        {
            var publisherInputItemDto = JghSerialisation.ToObjectFromJson<PublisherInputItemDto>(publisherInputDtoAsJson);

            var publisherInputItem = PublisherInputItem.FromDataTransferObject(publisherInputItemDto);

            var publisherOutputItem = await _raceResultsPublishingServiceMethodsHelper.ConvertPreviouslyUploadedDatasetsToResultsForSingleEventAsync(publisherInputItem);

            var publisherOutputItemDtoAsJson = JghSerialisation.ToJsonFromObject(PublisherOutputItem.ToDataTransferObject(publisherOutputItem));

            return publisherOutputItemDtoAsJson;
        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    public async Task<bool> UploadFileOfCompletedResultsForSingleEventAsync(string accountName, string containerName, string fileName, string completedResultsAsXml)
    {
        _logger.LogInformation("UploadFileOfCompletedResultsForSingleEventAsync() was called.");

        try
        {
            var completedResultsAsXmlAsBytes = JghConvert.ToBytesUtf8FromString(completedResultsAsXml);

            var didSucceed = await _raceResultsPublishingServiceMethodsHelper.UploadDatasetAsBytesAsync(accountName, containerName, fileName, completedResultsAsXmlAsBytes);

            return didSucceed;

        }
        catch (Exception ex)
        {
            var rfc7807 = new JghError(JghExceptionHelpers.FindInnermostException(ex));

            throw new FaultException<JghFault>(new JghFault(rfc7807));
        }
    }

    #endregion
}