using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.ViewModels01.April2022.CollectionBases;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;


// ReSharper disable UnusedMethodReturnValue.Local

namespace Rezultz.Library02.Mar2024.DataGridViewmodels
{
    public class LeaderboardStyleDataGridViewModel : DataGridViewModelBase<ResultItemDisplayObject>
    {
        private const string Locus2 = nameof(LeaderboardStyleDataGridViewModel);
        private const string Locus3 = "[Rezultz.Library02.Mar2024]";

        #region ctor

        public LeaderboardStyleDataGridViewModel(string caption, Action onSelectionChangedExecuteAction,
            Func<bool> onSelectionChangedCanExecuteFunc) : base(caption, onSelectionChangedExecuteAction, onSelectionChangedCanExecuteFunc)
        {

            SeriesProfileToWhichThisPresenterRefers = null; // NB = null not new()

            EventProfileToWhichThisPresenterRefers = null; // NB = null not new()

            ArrayOfSexFilter = [];

            DictionaryOfTxxColumnHeaders = new Dictionary<int, string>();

            PresentationServiceInstanceEnum = string.Empty;
        }


        #endregion

        #region properties

        #region SeriesToWhichThisPresenterRefers

        private SeriesProfileItem _backingstoreSeriesProfileToWhichThisPresenterRefers;

        public SeriesProfileItem SeriesProfileToWhichThisPresenterRefers
        {
            get => _backingstoreSeriesProfileToWhichThisPresenterRefers;
            set => SetProperty(ref _backingstoreSeriesProfileToWhichThisPresenterRefers, value);
        }

        #endregion

        #region EventToWhichThisPresenterRefers

        private EventProfileItem _backingstoreEventProfileToWhichThisPresenterRefers;

        public EventProfileItem EventProfileToWhichThisPresenterRefers
        {
            get => _backingstoreEventProfileToWhichThisPresenterRefers;
            set => SetProperty(ref _backingstoreEventProfileToWhichThisPresenterRefers, value);
        }

        #endregion

        #region ArrayOfSexFilter 

        private CboLookupItem[] _backingstoreArrayOfSexFilter;

        public CboLookupItem[] ArrayOfSexFilter
        {
            get => _backingstoreArrayOfSexFilter ??= [];
            set => SetProperty(ref _backingstoreArrayOfSexFilter, value);
        }

        #endregion

        #region DictionaryOfTxxColumnHeaders

        private Dictionary<int, string> _backingstoreDictionaryOfTxxColumnHeaders;

        public Dictionary<int, string> DictionaryOfTxxColumnHeaders
        {
            get => _backingstoreDictionaryOfTxxColumnHeaders ??= new Dictionary<int, string>();
            set => SetProperty(ref _backingstoreDictionaryOfTxxColumnHeaders, value);
        }

        #endregion

        #region PresentationServiceInstanceEnum

        private string _backingstorePresentationServiceInstanceEnum;

        public string PresentationServiceInstanceEnum
        {
            get => _backingstorePresentationServiceInstanceEnum ??= string.Empty;
            set => SetProperty(ref _backingstorePresentationServiceInstanceEnum, value);
        }

        #endregion

        #endregion

        #region methods

        public new async Task<bool> ZeroiseAsync()
        {
			await base.ZeroiseAsync();

            SeriesProfileToWhichThisPresenterRefers = null; // NB = null not new()

            EventProfileToWhichThisPresenterRefers = null; // NB = null not new()

            ArrayOfSexFilter = [];

            DictionaryOfTxxColumnHeaders = new Dictionary<int, string>();

            PresentationServiceInstanceEnum = string.Empty;

            return true;
        }

        public async Task<bool> PopulatePresenterAsync(string[] titlesAndSubtitles,
	        SeriesProfileItem seriesProfileToWhichThisPresenterRefers,
	        EventProfileItem eventProfileToWhichThisPresenterRefers,
	        CboLookupItem[] arrayOfSexFilter,
	        Dictionary<int, string> txxColumnHeaders,
	        string rowGroupingEnum,
	        string columnFormatEnum,
	        string leaderboardStyleOrFavoritesStyleEnum,
	        IEnumerable<ResultItemDisplayObject> rowCollection)
        {
            const string failure = "Unable to populate presenter.";
            const string locus = "[PopulatePresenterAsync]";

            try
            {
                IsAuthorisedToOperate = false;
                IsVisible = false;
				// this presenter is used for both the Leaderboard and Favorites. Because the Favorites datagrid is positioned above the Leaderboard 
				// and steals an varying amount of space on the page there depending on its size, it must not be simultaneously visible with
				// the Leaderboard during Layout or else a Windows.UI.Xaml.LayoutCycleException will be thrown. you have no alternative other than
				// to ensure IsVisible = false during layout. unfortunately this causes screen flicker when you return to previously rendered page. Oh well.

				HeadersVm.Populate(titlesAndSubtitles);

                SeriesProfileToWhichThisPresenterRefers = seriesProfileToWhichThisPresenterRefers ?? new SeriesProfileItem();
                EventProfileToWhichThisPresenterRefers = eventProfileToWhichThisPresenterRefers ?? new EventProfileItem();

                ArrayOfSexFilter = arrayOfSexFilter ?? [];
                DictionaryOfTxxColumnHeaders = txxColumnHeaders ?? new Dictionary<int, string>();
                // Note. for a single event ArrayOfTxxColumnHeaders = IRepositoryOfIndividualResults.ArrayOfTxxColumnHeaders. for a series of events = RepositoryOfSeriesTotalStandings.TxxColumnHeadersLookupTableForSequenceOfEvents

                RowGroupingFormatEnum = rowGroupingEnum ?? string.Empty;
                ColumnFormatEnum = columnFormatEnum ?? string.Empty;

                PresentationServiceInstanceEnum = leaderboardStyleOrFavoritesStyleEnum ?? string.Empty;

                var lineItemViewModels = rowCollection.ToArray();

                await RefillItemsSourceAsync(lineItemViewModels);

				await UpdateHeadingRhsGenderTotalsAsync();

				await UpdateDataPagerVisibilityAsync();

                HeadingRhsTextVm.IsVisible = true;

                IsVisible = true;
                IsAuthorisedToOperate = true;
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            return true;
        }


        private async Task<bool> UpdateHeadingRhsGenderTotalsAsync()
        {
            HeadingRhsTextVm.IsVisible = true;
            
            if (ItemsSource is null || ArrayOfSexFilter is null)
            {
                HeadingRhsTextVm.Text = string.Empty;

                return await Task.FromResult(true);
            }

            var theItemsList = ItemsSource;

            var kindsOfGender = ArrayOfSexFilter;

            var itemsListArray = theItemsList.ToArray();

            lock (theItemsList)
            {
                HeadingRhsTextVm.Text = GetGenderTotalsInFormatOfSingleLineReport(itemsListArray, kindsOfGender);
            }

            return await Task.FromResult(true);
        }

        private async Task<bool> UpdateDataPagerVisibilityAsync()
        {

            if (ItemsSource is null)
            {
                DataPagerIsVisible = false;

                return await Task.FromResult(true);
            }

            DataPagerIsVisible =
                ItemsSource.Count >= DataPagerPageSize; // 

            return await Task.FromResult(true);
        }

        private static string GetGenderTotalsInFormatOfSingleLineReport<T1, T2>(T1[] itemsHavingSexIdProperty, T2[] itemsHavingIdsAndLabels)
            where T1 : class, IHasGender
            where T2 : class, IHasItemID, IHasLabel, new()
        {

            static Dictionary<string, int> CalculateGenderFieldTotals<TU>(TU[] itemsHavingSexIdProperty) where TU : class, IHasGender
            {
                const string failure = "Unable to calculate totals of male and female participants.";
                const string locus = "[CalculateGenderFieldTotals]";

                var answer = new Dictionary<string, int>();

                try
                {
                    if (itemsHavingSexIdProperty is null || !itemsHavingSexIdProperty.Any())
                        return answer;

                    foreach (var individualResult in itemsHavingSexIdProperty.Where(z => z is not null))
                        if (answer.ContainsKey(individualResult.Gender))
                            answer[individualResult.Gender]++;
                        else
                            answer.Add(individualResult.Gender, 1);
                    return answer;
                }

                #region try catch handling

                catch (Exception ex)
                {
                    throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
                }

                #endregion
            }

            const string failure =
                "Unable to calculate totals of male and female participants and format in a one-liner summary.";
            const string locus = "[GetGenderTotalsInFormatOfSingleLineReport]";

            if (itemsHavingSexIdProperty is null || !itemsHavingSexIdProperty.Any() ||
                itemsHavingIdsAndLabels is null || !itemsHavingIdsAndLabels.Any())
                return string.Empty;

            const string spacer = " /  ";

            try
            {
                var genderSummaries = new List<string>();

                Dictionary<string, int> dictionaryOfTalliesByGender = CalculateGenderFieldTotals(itemsHavingSexIdProperty);

                Dictionary<string, string> dictionaryOfGenderDisplayLabels = itemsHavingIdsAndLabels.Where(z => z is not null)
                    .ToDictionary(sex => sex.Label, sex => sex.Label);

                foreach (var genderTallyKvp in dictionaryOfTalliesByGender)
                    if (genderTallyKvp.Value > 0)
                    {
                        var thisGenderDisplayLabel = string.Empty;

                        if (dictionaryOfGenderDisplayLabels.ContainsKey(genderTallyKvp.Key))
                            thisGenderDisplayLabel = dictionaryOfGenderDisplayLabels[genderTallyKvp.Key];

                        var thisGenderTally = genderTallyKvp.Value;

                        var thisGenderSummary = JghString.ConcatAsSentences(
                            thisGenderTally.ToString(JghFormatSpecifiers.DecimalFormat0Dp), thisGenderDisplayLabel, spacer);

                        genderSummaries.Add(thisGenderSummary);
                    }

                var totalParticipantsSummary = JghString.ConcatAsSentences("total",
                    dictionaryOfTalliesByGender.Sum(z => z.Value).ToString(JghFormatSpecifiers.DecimalFormat0Dp));

                var sb = new StringBuilder();

                foreach (var summary in genderSummaries.Where(z => z is not null))
                    sb.Append(summary);

                sb.Append(totalParticipantsSummary);

                return sb.ToString();
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

    }
}