using System.Threading;
using System.Threading.Tasks;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using RezultzSvc.ClientInterfaces.Mar2024.ClientBase;

namespace RezultzSvc.ClientInterfaces.Mar2024.Clients
{
    public interface ITimeKeepingServiceClient : IServiceClientBase
    {
        Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct);

        Task<string> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, TimeStampHubItemDto dataTransferObject, CancellationToken ct);

        Task<string> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, TimeStampHubItemDto[] dataTransferObject, CancellationToken ct);

        Task<TimeStampHubItemDto> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct);

        Task<TimeStampHubItemDto[]> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct);
    }
}