using System.Threading;
using System.Threading.Tasks;
using NetStd.DataTransferObjects.Mar2024;
using Rezultz.DataTransferObjects.Nov2023.PublisherModule;
using RezultzSvc.ClientInterfaces.Mar2024.ClientBase;

namespace RezultzSvc.ClientInterfaces.Mar2024.Clients;

public interface IRaceResultsPublishingServiceClient : IServiceClientBase
{

    public Task<bool> GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(string fileNameFragment, CancellationToken ct);

    public Task<PublisherModuleProfileItemDto> GetPublisherModuleProfileItemAsync(string fileNameFragment, CancellationToken ct);

    public Task<string[]> GetFileNameFragmentsOfAllPublishingProfilesAsync(CancellationToken ct);

    public Task<string> GetIllustrativeExampleOfDatasetExpectedByPublisherAsync(string fileNameWithExtension, CancellationToken ct);

    public Task<PublisherOutputItemDto> ObtainResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsAsync(PublisherInputItemDto publisherInputItemDto, CancellationToken ct);

    public Task<bool> SendFileOfRawDataToBeProcessedSubsequentlyAsync(string identifierOfDataset, EntityLocationDto storageLocation, string datasetAsRawString, CancellationToken ct);
    
    public Task<bool> SendFileOfCompletedResultsForSingleEventAsync(EntityLocationDto storageLocation, string completedResultsAsXml, CancellationToken ct);

}