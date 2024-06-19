using System;
using System.Linq;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Prism.July2018;
using Rezultz.DataTypes.Nov2023;


// ReSharper disable UnusedMethodReturnValue.Local

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NetStd.ViewModels01.April2022.UserControls
{
    /// <summary>
    ///     Facilitates search functionality for searching a collection of individual results.
    ///     Designed to serve as a property of any parent viewmodel that provides the datacontext
    ///     to a parent view that contains a SearchUserControl.xaml or equivalent. It provides a bindable
    ///     surface for most of the properties and all but one event/command of the SearchUserControl.
    ///     The parent viewmodel must call PopulateItemsSource to prime the presenter.
    ///     The SearchUserControl for Silverlight has a SearchOnBasisOfSilverlightAutoCompleteBoxSelectedItemButton
    ///     whose click event invokes ExecuteSearchOperationButtonOnClickCommand. This command
    ///     needs to be available on the parent viewmodel and the associated command method needs to call
    ///     DoSearchOnBasisOfIDOfSelectedItemAsync to obtain the results of a search
    ///     done by this presenter the associated command method then orchestrates any resultant actions it pleases.
    ///     A parent view can include any number of copycat SearchUserControls and the parent viewmodel
    ///     can include a matching multiplicity of SearchFunctionalityPresenters if desired. Each copycat SearchUserControl
    ///     must have a click event that calls a uniquely named equivalent of ExecuteSearchOperationButtonOnClickCommand
    ///     tied to a matching command on the parent viewmodel.
    ///     This presenter provides dual functionality for Silverlight and Windows respectively.
    /// </summary>
    public class SearchViewModel : BindableBase
    {
        private const string Locus2 = nameof(SearchViewModel);
        private const string Locus3 = "[NetStd.ViewModels01.April2022]";

        #region ctor

        public SearchViewModel(string defaultSearchBoxPlaceholderText, uint minLengthOfQueryText,
            uint maxNumberOfSearchBoxSuggestions, Action doSearchButtonOnClickExecuteAction,
            Func<bool> doSearchButtonOnClickCanExecuteFunc)
        {
            _defaultSearchBoxPlaceHolderText = defaultSearchBoxPlaceholderText ?? string.Empty;

            _defaultMinimumLengthOfQuery = minLengthOfQueryText;

            _defaultMaxNumberOfSearchBoxSuggestions = maxNumberOfSearchBoxSuggestions;

            DoSearchButtonVm =
                new ButtonControlViewModel(
                    doSearchButtonOnClickExecuteAction,
                    doSearchButtonOnClickCanExecuteFunc)
                {
                    IsVisible = true,
                    IsAuthorisedToOperate = true
                };

            ClearSearchQueryButtonVm =
                new ButtonControlViewModel(
                    ClearSearchQueryButtonOnClickExecute,
                    ClearSearchQueryButtonOnClickCanExecute)
                {
                    IsVisible = true,
                    IsAuthorisedToOperate = true
                };


            //PopulationOfThingsToBeSearched = Array.Empty<T>();

            AllSearchQueryItems = [];

            SelectedSearchQueryItem = null;

            SearchBoxText = _defaultSearchBoxPlaceHolderText;

            IsVisible = false;
        }

        #endregion

        #region fields

        private readonly string _defaultSearchBoxPlaceHolderText;

        private readonly uint _defaultMaxNumberOfSearchBoxSuggestions;

        private readonly uint _defaultMinimumLengthOfQuery;

        private bool _capturedIsAuthorisedToOperateValue;

        #endregion

        #region props

        #region buttons

        public ButtonControlViewModel DoSearchButtonVm { get; }

        public ButtonControlViewModel ClearSearchQueryButtonVm { get; }

        #endregion

        #region AllSearchQueryItems

        private SearchQueryItem[] _backingstoreAllSearchQueryItems;

        public SearchQueryItem[] AllSearchQueryItems
        {
            get => _backingstoreAllSearchQueryItems;
            private set => SetProperty(ref _backingstoreAllSearchQueryItems, value);
        }

        #endregion

        #region SelectedSearchQueryItem

        private SearchQueryItem _backingstoreSelectedSearchQueryItem;
        // this is intentionally null (as opposed to new) - don't do different

        public SearchQueryItem SelectedSearchQueryItem
        {
            get => _backingstoreSelectedSearchQueryItem;
            // NB don't screen for null here. we do want nulls to be bound to the autocompletebox
            set => SetProperty(ref _backingstoreSelectedSearchQueryItem, value);
        }

        #endregion

        #region SearchBoxText

        private string _backingstoreSearchBoxText;

        public string SearchBoxText
        {
            get => _backingstoreSearchBoxText;
            set => SetProperty(ref _backingstoreSearchBoxText, value);
        }

        #endregion

        #region IsVisible

        private bool _backingstoreIsVisible;

        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        #endregion

        #region IsAuthorisedToOperate

        // this functionality is a carbon copy of a Button style Command with its intrinsic RaiseCanExecuteChanged() 
        // functionality, except that this is not a button, hence the copying out of inapplicable RaiseCanExecuteChanged() stuff

        private bool _backingstoreIsAuthorisedToOperate;

        public bool IsAuthorisedToOperate
        {
            get => _backingstoreIsAuthorisedToOperate;
            set => SetProperty(ref _backingstoreIsAuthorisedToOperate, value);
        }

        #endregion

        #region MinimumLengthOfQuery

        // ReSharper disable once ConvertToAutoProperty
        public uint MinimumLengthOfQuery => _defaultMinimumLengthOfQuery;

        #endregion

        #endregion

        #region button command implementations

        #region ClearSearchQueryButton

        protected virtual bool ClearSearchQueryButtonOnClickCanExecute()
        {
            return true;
        }

        private async void ClearSearchQueryButtonOnClickExecute()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[ClearSearchQueryButtonOnClickExecute]";

            try
            {
                await ClearSearchQueryButtonOnClickAsync();
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private async Task<bool> ClearSearchQueryButtonOnClickAsync()
        {
            const string failure = "Unable to do what this method does ....";
            const string locus = "[ClearSearchQueryButtonOnClickAsync]";

            try
            {
                #region do work

                ClearSearchBox();

                #endregion
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

            return await Task.FromResult(true);
        }

        #endregion

        #region DoSearchButton

        public async Task<string[]> GetQueriesThatSatisfyUserEnteredHint(string searchBoxTextFragmentEnteredByUser)
        {

            SearchQueryItem[] filteredSearchQueryItems;

            if (AllSearchQueryItems is null)
            {
                filteredSearchQueryItems = [];
            }
            else
            {
                if (searchBoxTextFragmentEnteredByUser is null)
                {
                    filteredSearchQueryItems = (from item in AllSearchQueryItems
                        where item is not null
                        select item).ToArray();
                }
                else
                {
                    if (searchBoxTextFragmentEnteredByUser == string.Empty)
                        filteredSearchQueryItems = [];
                    else if (searchBoxTextFragmentEnteredByUser.Length < MinimumLengthOfQuery)
                        filteredSearchQueryItems = [];
                    else
                        filteredSearchQueryItems = (from item in AllSearchQueryItems
                            where item is not null
                            where JghString.TmLr(item.SearchQueryAsString).Contains(JghString.TmLr(searchBoxTextFragmentEnteredByUser))
                            select item).ToArray();
                }
            }

            var truncatedListOfFilteredSearchQueryItems = filteredSearchQueryItems.Take((int)_defaultMaxNumberOfSearchBoxSuggestions).ToArray();

            var answer = truncatedListOfFilteredSearchQueryItems
                .Where(z => z is not null)
                .Select(z => z.SearchQueryAsString)
                .Where(z => !string.IsNullOrWhiteSpace(z))
                .ToArray();

            return await Task.FromResult(answer);
        }

        public SearchQueryItem[] GetSubsetOfSearchQueryItemsThatEquateToSelectedSearchQuery(string selectedSearchQueryString)
        {
            const string failure = "Unable to do search.";
            const string locus = "[GetSubsetOfSearchQueryItemsThatEquateToSelectedSearchQuery]";

            try
            {
                #region null checks

                if (string.IsNullOrEmpty(selectedSearchQueryString))
                {
                    // to do: make sure this doesn't trigger text changed otherwise we'll go into an infinite loop

                    NullifyWindowsSearchQuerySelectedSuggestionItem();

                    return [];
                }

                if (AllSearchQueryItems is null)
                    throw new ArgumentNullException(nameof(AllSearchQueryItems));

                #endregion

                var subsetOfQualifyingSearchQueryItems = (from item in AllSearchQueryItems
                    where item is not null
                    where JghString.TmLr(item.SearchQueryAsString) == JghString.TmLr(selectedSearchQueryString)
                    select item).ToArray();

                return subsetOfQualifyingSearchQueryItems;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public void NullifyWindowsSearchQuerySelectedSuggestionItem()
        {
            SearchBoxText = _defaultSearchBoxPlaceHolderText;
        }

        #endregion

        #endregion


        #region methods

        public void PopulateItemsSource(SearchQueryItem[] searchQuerySuggestionItems)
        {
            AllSearchQueryItems = searchQuerySuggestionItems ?? [];

            //PopulationOfThingsToBeSearched = populationOfThingsToBeSearched?.ToArray() ?? Array.Empty<T>();
        }

        public bool Zeroise()
        {
            DoSearchButtonVm.Zeroise();

            ClearSearchQueryButtonVm.Zeroise();

            AllSearchQueryItems = [];

            SelectedSearchQueryItem = null;

            SearchBoxText = _defaultSearchBoxPlaceHolderText;

            IsVisible = false;

            return true;
        }

        public void ZeroiseItemsSources()
        {
            AllSearchQueryItems = [];
        }

        public void MakeVisibleIfThereAreThingsToBeSearched()
        {
            IsVisible = true;
            //IsVisible = PopulationOfThingsToBeSearched.Any(z => z is not null);
        }

        public void CaptureIsAuthorisedToOperateValue()
        {
            _capturedIsAuthorisedToOperateValue = _backingstoreIsAuthorisedToOperate;
        }

        public void RestoreCapturedIsAuthorisedToOperateValue()
        {
            IsAuthorisedToOperate = _capturedIsAuthorisedToOperateValue;
        }

        #endregion

        #region helpers

        private void ClearSearchBox()
        {
            SelectedSearchQueryItem = null;

            SearchBoxText = string.Empty;
        }

        #endregion


    }
}