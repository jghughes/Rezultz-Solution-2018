using System.Threading;
using System.Threading.Tasks;
using Rezultz.DataTypes.Nov2023.PortalHubItems;


namespace NetStd.Interfaces03.Apr2022
{
    public interface ITimeKeepingSvcAgent : ISvcAgentBase
	{
        Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct = default);

		Task<string> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, TimeStampHubItem timeStampItem, CancellationToken ct = default);

		Task<string> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, TimeStampHubItem[] itemArray, CancellationToken ct = default);

		Task<TimeStampHubItem> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct = default);

		Task<TimeStampHubItem[]> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct = default);
	}
}