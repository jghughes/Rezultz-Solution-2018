using System;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

namespace RezultzSvc.Agents.Mar2024.Dummies
{
    public class TimeKeepingSvcAgentDummy : ITimeKeepingSvcAgent
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

        public Task<bool> GetIfContainerExistsAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> PostTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, TimeStampHubItem timeStampItem, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> PostTimeStampItemArrayAsync(string databaseAccount, string dataContainer, TimeStampHubItem[] itemArray, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<TimeStampHubItem> GetTimeStampItemAsync(string databaseAccount, string dataContainer, string tablePartition, string tableRowKey, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<TimeStampHubItem[]> GetTimeStampItemArrayAsync(string databaseAccount, string dataContainer, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}