using System.Threading;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

namespace NetStd.Interfaces03.Apr2022
{
    public interface IRegistrationSvcAgent : ISvcAgentBase
	{
        Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct = default);

		Task<string> PostParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, ParticipantHubItem participantItem, CancellationToken ct = default);

		Task<string> PostParticipantItemArrayAsync(string databaseAccount, string dataContainer, ParticipantHubItem[] itemArray, CancellationToken ct = default);

		Task<ParticipantHubItem> GetParticipantItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct = default);

		Task<ParticipantHubItem[]> GetParticipantItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct = default);



	}
}