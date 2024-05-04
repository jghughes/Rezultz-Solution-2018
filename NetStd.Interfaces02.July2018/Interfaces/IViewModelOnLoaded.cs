using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
	public interface IViewModelOnLoaded
	{
       Task<bool> LoadThisViewModelButtonOnClickOrchestrateAsync(bool mustForceRefresh);

        //Task<bool> LoadThisViewModelAsync(bool mustForceRefresh);

    }
}