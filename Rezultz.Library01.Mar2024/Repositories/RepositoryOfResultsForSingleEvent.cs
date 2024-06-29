using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repository_algorithms;
using Rezultz.Library01.Mar2024.Repository_interfaces;

namespace Rezultz.Library01.Mar2024.Repositories
{
    public class RepositoryOfResultsForSingleEvent : IRepositoryOfResultsForSingleEvent
    {
        private const string Locus2 = nameof(RepositoryOfResultsForSingleEvent);
        private const string Locus3 = "[NetStd.Rezultz01.July2018]";

        #region strings

        private const string NoResults = "No results.";

        #endregion

        #region global props

        private readonly ILeaderboardResultsSvcAgent _leaderboardResultsSvcAgent;

        #endregion

        #region ctor

        public RepositoryOfResultsForSingleEvent(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent)
        {
            _leaderboardResultsSvcAgent = leaderboardResultsSvcAgent;

            InitialiseFields();
        }

        private void InitialiseFields()
        {
            _eventProfileToWhichThisBelongs = new();
            _repositoryIsInitialised = false;
            _allProcessedResultsForThisEvent = [];
            //_searchQuerySuggestions = Array.Empty<SearchQueryItem>();
            //_dictionaryOfTxxColumnHeaders = new Dictionary<int, string>();
        }

        #endregion

        #region repository backing fields

        private string[] _tableOfRaces = [];
        private string[] _tableOfGenders = [];
        private string[] _tableOfAgeGroups = [];
        private string[] _tableOfCities = [];
        private string[] _tableOfTeams = [];
        private string[] _tableOfUtilityClassifications = [];

        private bool _repositoryIsInitialised;

        private EventProfileItem _eventProfileToWhichThisBelongs;

        private ResultItem[] _allProcessedResultsForThisEvent; //NB. this is the property that must be populated in the PopulateItemsSource method of derived repositories for everything to work as intended

        //private SearchQueryItem[] _searchQuerySuggestions;

        #endregion

        #region public methods for loading repository

        /// <summary>
        ///     Loads the repository of results failing noisily.
        ///     If a specified results data file is not found, throws JghResultsData404Exception as an innermost
        ///     exception.
        /// </summary>
        /// <param name="databaseAccountName">Name of the azure storage account.</param>
        /// <param name="dataContainerName">Name of the azure storage container.</param>
        /// <param name="eventProfileToWhichThisRepositoryBelongs">The event to which this repository belongs.</param>
        /// <returns></returns>
        public async Task<bool> LoadRepositoryOfResultsFailingNoisilyAsync(string databaseAccountName, string dataContainerName, EventProfileItem eventProfileToWhichThisRepositoryBelongs)
        {
            const string failure = "Unable to generate repository of  results.";
            const string locus = "[LoadRepositoryOfResultsFailingNoisilyAsync]";

            try
            {
                #region step 1 null checks

                if (string.IsNullOrWhiteSpace(databaseAccountName))
                    throw new JghNullObjectInstanceException(nameof(databaseAccountName));

                #endregion

                InitialiseFields();

                _eventProfileToWhichThisBelongs = eventProfileToWhichThisRepositoryBelongs ?? throw new JghNullObjectInstanceException(nameof(eventProfileToWhichThisRepositoryBelongs)); // essential

                var populatedEventItem = await _leaderboardResultsSvcAgent.PopulateSingleEventWithResultsAsync(databaseAccountName, dataContainerName, eventProfileToWhichThisRepositoryBelongs, CancellationToken.None);

                if (populatedEventItem?.ResultItemsForEventAsPublished is null)
                    throw new JghResultsData404Exception($"{NoResults} <{eventProfileToWhichThisRepositoryBelongs.Label}>");

                var rawPreprocessedResults = Array.Empty<ResultItem>();

                if (populatedEventItem.ResultItemsForEventAsPublished is not null)
                    rawPreprocessedResults = populatedEventItem.ResultItemsForEventAsPublished;

                await PopulateRepositoryAsync(rawPreprocessedResults);

                _repositoryIsInitialised = true;

                return true;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public async Task<bool> LoadPrePopulatedRepositoryOfResultsForEventFailingSilentlyToFalseAsync(string databaseAccountName, string dataContainerName, EventProfileItem eventProfileToWhichThisRepositoryBelongs)
        {
            try
            {
                await
                    LoadPrePopulatedRepositoryOfResultsFailingNoisilyAsync(databaseAccountName, dataContainerName,
                        eventProfileToWhichThisRepositoryBelongs);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Loads the repository of results failing noisily.
        ///     In this variant of the method, the event is presumed to preloaded with the ResultsPocoArray
        ///     exception.
        /// </summary>
        /// <param name="databaseAccountName">Name of the azure storage account.</param>
        /// <param name="dataContainerName">Name of the azure storage container.</param>
        /// <param name="eventProfileToWhichThisRepositoryBelongs">The event to which this repository belongs.</param>
        /// <returns></returns>
        public async Task<bool> LoadPrePopulatedRepositoryOfResultsFailingNoisilyAsync(string databaseAccountName, string dataContainerName, EventProfileItem eventProfileToWhichThisRepositoryBelongs)
        {
            const string failure = "Unable to generate repository of  results.";
            const string locus = "[LoadRepositoryOfResultsFailingNoisilyAsync]";

            try
            {
                #region step 1 null checks

                if (string.IsNullOrWhiteSpace(databaseAccountName))
                    throw new JghNullObjectInstanceException(nameof(databaseAccountName));

                #endregion

                InitialiseFields();

                _eventProfileToWhichThisBelongs = eventProfileToWhichThisRepositoryBelongs ?? throw new JghNullObjectInstanceException(nameof(eventProfileToWhichThisRepositoryBelongs)); // essential

                var answerAsEventItem = eventProfileToWhichThisRepositoryBelongs;

                if (answerAsEventItem.ResultItemsForEventAsPublished is null || !answerAsEventItem.ResultItemsForEventAsPublished.Any())
                    throw new JghResultsData404Exception($"{NoResults} ({eventProfileToWhichThisRepositoryBelongs.Label})");

                var rawPreprocessedResults = Array.Empty<ResultItem>();

                if (answerAsEventItem.ResultItemsForEventAsPublished is not null)
                    rawPreprocessedResults = answerAsEventItem.ResultItemsForEventAsPublished;

                await PopulateRepositoryAsync(rawPreprocessedResults);

                _repositoryIsInitialised = true;

                return true;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Used only in SingleEventAverageSplitTimesVm
        /// </summary>
        /// <param name="resultsData"></param>
        /// <returns></returns>
        public async Task<bool> LoadRepositoryOfResultsWithDataProvidedAsync(ResultItem[] resultsData)
        {
            const string failure = "Unable to generate repository of individual results.";
            const string locus = "[LoadRepositoryOfResultsFailingNoisilyAsync]";

            try
            {
                if (resultsData is null)
                    throw new JghNullObjectInstanceException(nameof(resultsData));

                await PopulateRepositoryAsync(resultsData);

                return true;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region public methods for accessing the repository

        public async Task<string[]> GetRacesFoundAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tableOfRaces);
        }

        public async Task<string[]> GetGendersFoundAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tableOfGenders);
        }

        public async Task<string[]> GetAgeGroupsFoundAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tableOfAgeGroups);
        }

        public async Task<string[]> GetCitiesFoundAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tableOfCities);
        }

        public async Task<string[]> GetTeamsFoundAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tableOfTeams);
        }

        public async Task<string[]> GetUtilityClassificationsFoundAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tableOfUtilityClassifications);
        }


        public async Task<PopulationCohortItem[]> GetRaceCohortsFoundAsync()
        {
            const string failure = "Populating histogram.";
            const string locus = "[GetRaceCohortsFoundAsync]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectAllIndividualResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.RaceGroup))
                        .ToLookup(z => z.RaceGroup, z => z)
                        .ToArray()
                );
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        public async Task<PopulationCohortItem[]> GetGenderCohortsFoundAsync()
        {
            const string failure = "Populating histogram.";
            const string locus = "[GetGenderCohortsFoundAsync]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectAllIndividualResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.Gender))
                        .ToLookup(z => z.Gender, z => z)
                        .ToArray()
                );
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        public async Task<PopulationCohortItem[]> GetAgeGroupCohortsFoundAsync()
        {
            const string failure = "Populating histogram.";
            const string locus = "[GetAgeGroupCohortsFoundAsync]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectPlacedResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.AgeGroup))
                        .ToLookup(z => z.AgeGroup, z => z)
                        .ToArray()
                );
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        public async Task<PopulationCohortItem[]> GetCityCohortsFoundAsync()
        {
            const string failure = "Populating histogram.";
            const string locus = "[GetCityCohortsFoundAsync]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectPlacedResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.City))
                        .ToLookup(z => JghString.TmLr(z.City), z => z)
                        .ToArray()
                );
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }


        public async Task<ResultItem[]> GetPlacedResultsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<ResultItem>());

            var xx = SelectPlacedResults();

            return await Task.FromResult(xx);
        }

        public async Task<ResultItem[]> GetPlacedAndNonDnsResultsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<ResultItem>());

            var aa = SelectPlacedResults() ?? [];

            var bb = SelectValidDnxResults()
                .Where(item => item.DerivedData is not null)
                .Where(item => JghString.AreNotEqualIgnoreOrdinalCase(item.DerivedData.DnxStringFromAlgorithm, Symbols.SymbolDns))
                .OrderBy(item => item.ToString())
                .ToArray();

            var answer = new List<ResultItem>();

            answer.AddRange(aa.ToList());

            answer.AddRange(bb.ToList());

            var xx = answer.ToArray();

            return await Task.FromResult(xx);
        }

        //public async Task<SearchQueryItem[]> GetSearchQuerySuggestionsAsync()
        //{
        //    if (_repositoryIsInitialised == false)
        //        return await Task.FromResult(Array.Empty<SearchQueryItem>());

        //    return await Task.FromResult(_searchQuerySuggestions);
        //}

        public async Task<EventProfileItem> GetEventToWhichThisRepositoryBelongsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(new EventProfileItem());

            return await Task.FromResult(_eventProfileToWhichThisBelongs);
        }

        #endregion

        #region helpers to populate _allProcessedresultsForThisEvent

        private async Task PopulateRepositoryAsync(ResultItem[] rawPreprocessedResults)
        {
            const string failure = "Assembling repository of results for event.";
            const string locus = "[PopulateRepositoryAsync]";


            try
            {
                #region null checks

                if (rawPreprocessedResults is null)
                    throw new JghNullObjectInstanceException(nameof(rawPreprocessedResults));

                if (_eventProfileToWhichThisBelongs is null)
                    throw new JghNullObjectInstanceException(
                        nameof(_eventProfileToWhichThisBelongs));

                #endregion

                var resultsUndergoingConversion = rawPreprocessedResults;

                #region calculate derived data such as  TotalDurationFromAlgorithmInSeconds, CalculatedNumOfSplitsCompleted, Dnx

                resultsUndergoingConversion = PopulateBasicDerivedData(resultsUndergoingConversion);

                #endregion

                #region generate tables of categories

                PopulateAllCategoryLookupFilterTablesByInspectionOfPopulation(resultsUndergoingConversion);

                #endregion

                #region populate placings


                resultsUndergoingConversion = await AlgorithmForPlacings.PopulateAllPlacingsForThisEventAsync(resultsUndergoingConversion.ToArray());


                #endregion

                _allProcessedResultsForThisEvent = resultsUndergoingConversion;
                //N.B. trick. important shift at this precise juncture. enables us to be smart and use GetPlacedIndividualResults and GetAllIndividualResults

                #region populate points (on a series-specific basis) but iff an algorithm has been specified

                if (!string.IsNullOrWhiteSpace(_eventProfileToWhichThisBelongs.EventSettingsItem.AlgorithmForCalculatingPointsEnumString))
                    _allProcessedResultsForThisEvent = AlgorithmForPoints.InsertPoints(_eventProfileToWhichThisBelongs, SelectPlacedResults(), SelectAllIndividualResults());

                #endregion

                _repositoryIsInitialised = true;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private static ResultItem[] PopulateBasicDerivedData(ResultItem[] resultsUndergoingConversion)
        {
            foreach (var thisItem in resultsUndergoingConversion)
            {
                if (thisItem is null) continue;

                thisItem.DerivedData = new()
                {
                    IsValidDnx = !string.IsNullOrWhiteSpace(thisItem.DnxString),
                    DnxStringFromAlgorithm = thisItem.DnxString
                };

                if (thisItem.DerivedData.IsValidDnx) continue;

                thisItem.DerivedData.IsValidDuration = true;

                List<double> listOfIntervalsInSeconds =
                [
                    JghTimeSpan.ToTotalSeconds(thisItem.T01),
                    JghTimeSpan.ToTotalSeconds(thisItem.T02),
                    JghTimeSpan.ToTotalSeconds(thisItem.T03),
                    JghTimeSpan.ToTotalSeconds(thisItem.T04),
                    JghTimeSpan.ToTotalSeconds(thisItem.T05),
                    JghTimeSpan.ToTotalSeconds(thisItem.T06),
                    JghTimeSpan.ToTotalSeconds(thisItem.T07),
                    JghTimeSpan.ToTotalSeconds(thisItem.T08),
                    JghTimeSpan.ToTotalSeconds(thisItem.T09),
                    JghTimeSpan.ToTotalSeconds(thisItem.T10),
                    JghTimeSpan.ToTotalSeconds(thisItem.T11),
                    JghTimeSpan.ToTotalSeconds(thisItem.T12),
                    JghTimeSpan.ToTotalSeconds(thisItem.T13),
                    JghTimeSpan.ToTotalSeconds(thisItem.T14),
                    JghTimeSpan.ToTotalSeconds(thisItem.T15)
                ];

                thisItem.DerivedData.TotalDurationFromAlgorithmInSeconds = listOfIntervalsInSeconds.Sum();

                thisItem.DerivedData.CalculatedNumOfSplitsCompleted = listOfIntervalsInSeconds.Count(z => z != 0);
            }

            return resultsUndergoingConversion;
        }

        private void PopulateAllCategoryLookupFilterTablesByInspectionOfPopulation(ResultItem[] results)
        {
            var failure = "Unable to populate categories for CboFilters.";
            const string locus = "[PopulateAllCategoryLookupFilterTablesByInspectionOfPopulation]";

            try
            {
                if (results is null)
                    throw new JghNullObjectInstanceException(nameof(results));

                results = results.Where(z => z is not null).ToArray();

                _tableOfRaces = results.Select(z => z.RaceGroup).Distinct().OrderBy(z => z).ToArray();
                _tableOfGenders = results.Select(z => z.Gender).Distinct().OrderBy(z => z).ToArray();
                _tableOfAgeGroups = results.Select(z => z.AgeGroup).Distinct().OrderBy(z => z).ToArray();
                _tableOfCities = results.Select(z => z.City).Where(z=> !string.IsNullOrWhiteSpace(z)).Distinct().OrderBy(z => z).ToArray();
                _tableOfTeams = results.Select(z => z.Team).Where(z => !string.IsNullOrWhiteSpace(z)).Distinct().OrderBy(z => z).ToArray();
                _tableOfUtilityClassifications = results.Select(z => z.UtilityClassification).Where(z => !string.IsNullOrWhiteSpace(z)).Distinct().OrderBy(z => z).ToArray();
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private ResultItem[] SelectAllIndividualResults()
        {
            return _allProcessedResultsForThisEvent.Where(item => item is not null).ToArray();
        }

        private ResultItem[] SelectPlacedResults()
        {
            return _allProcessedResultsForThisEvent
                .Where(item => item is not null)
                .Where(item => item.DerivedData is not null)
                .Where(item => !item.DerivedData.IsValidDnx)
                .Where(item => item.DerivedData.IsValidDuration)
                .OrderBy(item => item.DerivedData.PlaceCalculatedOverallInt).ToArray();
        }

        private ResultItem[] SelectValidDnxResults()
        {
            return _allProcessedResultsForThisEvent
                .Where(item => item is not null)
                .Where(item => item.DerivedData is not null)
                .Where(item => item.DerivedData.IsValidDnx)
                .OrderBy(item => item.DerivedData.DnxStringFromAlgorithm)
                .ThenBy(ResultItem.FormatFullName)
                .ToArray();
        }

        #endregion
    }
}

