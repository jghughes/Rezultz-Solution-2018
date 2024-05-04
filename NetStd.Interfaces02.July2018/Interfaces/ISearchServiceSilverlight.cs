using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    public interface ISearchServiceSilverlight
    {
        /// <summary>
        ///     Searches the list of all results using current searchbox selecteditem for the search criterion.
        ///     Implementation should then call a virtual method OrchestrateSearchOutcome or some such and on we go.
        /// </summary>
        /// <returns></returns>
        Task<bool> DoSearchOperationButtonOnClickAsync();
    }
}