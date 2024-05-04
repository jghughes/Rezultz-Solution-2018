using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    /// <summary>
    ///     The ISearchService interface can be implemented by any page, user control or viewmodel
    ///     that provides a search service. It specifies a method for fetching search hints, and a method
    ///     for initiating a response to the submission of a final search
    ///     query.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        ///     The Windows Uwp AutoSearchBox raises the TextChanged event when the text in the TextBox changes.
        ///     (In the codebehind of the AutoSearchBox, it is recommended to check the reason for the text changing
        ///     by checking against args.Reason, and only react if it is the user that has made the change as opposed to the
        ///     program). This interface method is intended to handle the user-entered text by using it to search a universe of
        ///     search suggestions. The method filters the universe down to a small shortlist of search suggestions.
        ///     The intent is to return these to the AutoSearchBox as the list from which the user picks and submits her final
        ///     search query.
        /// </summary>
        /// <param name="searchHint">Search hint.</param>
        /// <returns>
        ///     Shortlist of strings to be displayed as search suggestions in a selector
        /// </returns>
        Task<string[]> OnShortlistOfQuerySuggestionsRequestedFromTheSearchUniverseAsync(string searchHint);

        /// <summary>
        ///     The Windows Uwp AutoSearchBox raises the QuerySubmitted event when :
        ///     - the user presses Enter while focus is in the TextBox
        ///     - the user clicks or tabs to and invokes the query button (defined using the QueryIcon API)
        ///     - the user presses selects (clicks/taps/presses Enter) a suggestion.
        ///     This inteface method is intended as the action to be taken when a final search query is submitted.
        /// </summary>
        /// <param name="finalQuerySubmitted">
        ///     The QueryText, which is the text in the TextBox and also ChosenSuggestion, which is only
        ///     non-null when a user selects an item in the list.
        /// </param>
        /// <returns>true when the action has run to completion</returns>
        Task<bool> OnFinalSearchQuerySubmittedAsTextAsync(string finalQuerySubmitted);
    }
}