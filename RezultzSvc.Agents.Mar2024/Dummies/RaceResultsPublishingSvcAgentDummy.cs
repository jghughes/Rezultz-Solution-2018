using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.DataTypes.Mar2024;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

namespace RezultzSvc.Agents.Mar2024.Dummies
{
    public class RaceResultsPublishingSvcAgentDummy : IRaceResultsPublishingSvcAgent
    {
        public Task<bool> ThrowIfNoServiceConnectionAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PublisherModuleProfileItem> GetPublishingProfileAsync(string fileNameFragment, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(string entityFileName, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadDatasetFileToBeProcessedSubsequentlyAsync(string identifierOfDataset, EntityLocationItem storageLocation, string datasetAsRawString, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PublisherOutputItem> GetResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(string fileNameFragmentOfAssociatedPublishingProfile, string seriesLabelAsEventIdentifier, string eventLabelAsEventIdentifier,
            SeriesProfileItem seriesProfile, PublisherImportFileTargetItem[] filesToBeFetchedForProcessing,
            CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadFileOfCompletedResultsForSingleEventAsync(EntityLocationItem storageLocation, string datasetAsRawString, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}