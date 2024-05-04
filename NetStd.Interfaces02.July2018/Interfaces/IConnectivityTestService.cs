using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface IConnectivityTestService
    {
        Task<bool> ThrowIfNoNetworkAsync();

        Task<bool> ThrowIfRezultzStorageAccountUnavailableAsync();

        Task<bool> ThrowIfInternetResourceNotFoundAsync(string url);

    }
}