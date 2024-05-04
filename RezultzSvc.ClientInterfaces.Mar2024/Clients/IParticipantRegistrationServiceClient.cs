using System.Threading;
using System.Threading.Tasks;
using Rezultz.DataTransferObjects.Nov2023.TimekeepingSystem;
using RezultzSvc.ClientInterfaces.Mar2024.ClientBase;

namespace RezultzSvc.ClientInterfaces.Mar2024.Clients
{
    public interface IParticipantRegistrationServiceClient : IServiceClientBase
    {
        Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct);

        Task<string> PostParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, ParticipantHubItemDto dataTransferObject, CancellationToken ct);

        Task<string> PostParticipantItemArrayAsync(string databaseAccount, string dataContainer, ParticipantHubItemDto[] dataTransferObject, CancellationToken ct);

        Task<ParticipantHubItemDto> GetParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct);

        Task<ParticipantHubItemDto[]> GetParticipantItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct);



    }
}