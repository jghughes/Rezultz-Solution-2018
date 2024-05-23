using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using RezultzSvc.Agents.Mar2024.Bases;
using RezultzSvc.ClientInterfaces.Mar2024.Clients;

namespace RezultzSvc.Agents.Mar2024.SvcAgents;

public class RaceResultsPublishingSvcAgent : SvcAgentBase, IRaceResultsPublishingSvcAgent
{
    private const string Locus2 = "[RaceResultsPublishingSvcAgent]";
    private const string Locus3 = "[RezultzSvc.Agents.Mar2024]";

    #region fields

    private readonly IRaceResultsPublishingServiceClient _myServiceClient;

    #endregion

    #region ctor stuff

    public RaceResultsPublishingSvcAgent(IRaceResultsPublishingServiceClient serviceClientInstance)
    {
        const string failure = "Unable to instantiate service agent.";
        const string locus = "[RaceResultsPublishingSvcAgent]";

        try
        {
            ClientBase = serviceClientInstance;

            _myServiceClient = serviceClientInstance;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion

    #region methods

    public async Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment, CancellationToken ct = default)
    {
        const string failure = "Unable to determine if filename of file containing profile of publishing module is recognised.";
        const string locus = "[GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync]";

        try
        {
            var answer = await _myServiceClient.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(fileNameFragment, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<PublisherModuleProfileItem> GetPublishingProfileAsync(string fileNameFragment, CancellationToken ct = default)
    {
        const string failure = "Unable to obtain XML file containing profile of publisher module.";
        const string locus = "[GetPublisherModuleProfileItemAsync]";

        try
        {
            var profileDto = await _myServiceClient.GetPublisherModuleProfileItemAsync(fileNameFragment, ct);

            var answer = PublisherModuleProfileItem.FromDataTransferObject(profileDto);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync(CancellationToken ct = default)
    {
        const string failure = "Unable to get file name fragments.";
        const string locus = "[GetFileNameFragmentsOfAllPublishingProfilesAsync]";

        try
        {
            var answer = await _myServiceClient.GetFileNameFragmentsOfAllPublishingProfilesAsync(ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<string> GetIllustrativeExampleOfSourceDatasetExpectedByPublishingServiceAsync(string entityFileName, CancellationToken ct = default)
    {
        const string failure = "Unable to get example of input dataset.";
        const string locus = "[GetIllustrativeExampleOfDatasetExpectedByPublisherAsync]";

        try
        {
            var answer = await _myServiceClient.GetIllustrativeExampleOfSourceDatasetExpectedByPublishingServiceAsync(entityFileName, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<bool> UploadSourceDatasetToBeProcessedSubsequentlyAsync(string identifierOfDataset, EntityLocationItem storageLocation, string datasetAsRawString, CancellationToken ct = default)
    {
        const string failure = "Unable to upload input dataset.";
        const string locus = "[SendFileOfRawDataToBeProcessedSubsequentlyAsync]";

        try
        {
            var storageLocationDto = EntityLocationItem.ToDataTransferObject(storageLocation);

            var answer = await _myServiceClient.UploadSourceDatasetToBeProcessedSubsequentlyAsync(identifierOfDataset, storageLocationDto, datasetAsRawString, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<PublisherOutputItem> ProcessPreviouslyUploadedSourceDataIntoPublishableResultsForSingleEventAsync(string fileNameFragmentOfAssociatedPublishingProfile, string seriesLabelAsEventIdentifier, 
        string eventLabelAsEventIdentifier, SeriesProfileItem seriesProfile, PublisherImportFileTargetItem[] filesToBeFetchedForProcessing, CancellationToken ct = default)
    {
        const string failure = "Unable to process datasets.";
        const string locus = "[ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync]";

        try
        {
            var publisherInput = new PublisherInputItem
            {
                FileNameFragmentOfAssociatedPublishingProfile = fileNameFragmentOfAssociatedPublishingProfile,
                SeriesLabelAsEventIdentifier = seriesLabelAsEventIdentifier,
                EventLabelAsEventIdentifier = eventLabelAsEventIdentifier,
                SeriesProfile = seriesProfile,
                DatasetTargetsToBeProcessed = filesToBeFetchedForProcessing
            };

            var publisherInputDto = PublisherInputItem.ToDataTransferObject(publisherInput);

            var publisherOutputDto = await _myServiceClient.ProcessPreviouslyUploadedSourceDataIntoPublishableResultsForSingleEventAsync(publisherInputDto, ct);

            var answer = PublisherOutputItem.FromDataTransferObject(publisherOutputDto);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    public async Task<bool> UploadPublishableResultsForSingleEventAsync(EntityLocationItem storageLocation, string datasetAsRawString, CancellationToken ct = default)
    {
        const string failure = "Unable to upload completed results.";
        const string locus = "[SendFileOfCompletedResultsForSingleEventAsync]";

        try
        {
            var storageLocationDto = EntityLocationItem.ToDataTransferObject(storageLocation);

            var answer = await _myServiceClient.UploadPublishableResultsForSingleEventAsync(storageLocationDto, datasetAsRawString, ct);

            return answer;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion

}