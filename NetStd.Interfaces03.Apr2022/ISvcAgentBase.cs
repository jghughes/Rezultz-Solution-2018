using System.Threading;
using System.Threading.Tasks;

namespace NetStd.Interfaces03.Apr2022
{
	public interface ISvcAgentBase
	{
		Task<bool> ThrowIfNoServiceConnectionAsync(CancellationToken ct = default);

		Task<bool> GetIfServiceIsAnsweringAsync(CancellationToken ct = default);

		Task<string[]> GetServiceEndpointsInfoAsync(CancellationToken ct = default);


	}
}