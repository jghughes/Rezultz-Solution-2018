using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces03.Apr2022;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.Library01.Mar2024.Repository_algorithms;
using Rezultz.Library01.Mar2024.Repository_interfaces;

// ReSharper disable UnusedMethodReturnValue.Local

namespace Rezultz.Library01.Mar2024.Repositories
{
    // NB For both PointsStandings and TourDurationStandings, the unit of analysis is not simply an individual. an individual who competes in  
    // different race groups during the season is treated as a dual-natured creature with a split personality - Jekyll/Hyde at the outset of the algorithm. 
    // At the end, the creature is reduced to a single person and that person has the finalised seniority that governed her final race of the season
    // we are going to assemble a collection of persons. If the person has traded up, the person takes her points with, if she trades down, she leaves them behind.
    // For TourDurationStandings, the criterion is simpler, a person 
    // who changes seniority is disqualified.
    /// <summary>
    /// </summary>
    public class RepositoryOfSeriesStandings : IRepositoryOfSeriesStandings
    {
        private const string Locus2 = nameof(RepositoryOfSeriesStandings);
        private const string Locus3 = "[Rezultz.Library01.Mar2024]";

        #region constants

        private const int NumberOfEventsDuringKelsoSeasonPlusOne = 15;

        #endregion

        #region global props

        private static ILeaderboardResultsSvcAgent _leaderboardResultsSvcAgent;

        #endregion

        #region ctor

        public RepositoryOfSeriesStandings(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent)
        {
            _leaderboardResultsSvcAgent = leaderboardResultsSvcAgent ?? throw new JghNullObjectInstanceException(nameof(leaderboardResultsSvcAgent));
        }

        #endregion

        #region strings

        //private const string ItWouldSeemThatNoEventsAreFlaggedAsBeingEligibleForSeriesStandingsAndRankingsAsYet =
        //    "It would seem that no events are flagged as being eligible for series standings and rankings as yet.";

        private const string ItSeemsThatNoParticipantsExistOrNoneAreEligibleForSeasonStandings =
            "It seems that no participants exist, or none are eligible for season standings.";

        #endregion

        #region repository backing fields

        private string[] _pointsTableOfRaces = [];
        private string[] _pointsTableOfGenders = [];
        private string[] _pointsTableOfAgeGroups = [];
        private string[] _pointsTableOfCities = [];
        private string[] _pointsTableOfTeams = [];
        private string[] _pointsTableOfUtilityClassifications = [];

        private string[] _tourTableOfRaces = [];
        private string[] _tourTableOfGenders = [];
        private string[] _tourTableOfAgeGroups = [];
        private string[] _tourTableOfCities = [];
        private string[] _tourTableOfTeams = [];
        private string[] _tourTableOfUtilityClassifications = [];

        private SequenceContainerItem[] _pointsStandings = [];
        private SequenceContainerItem[] _tourDurationStandings = [];


        private SearchQueryItem[] _pointsSearchQuerySuggestions = [];
        private SearchQueryItem[] _tourDurationSearchQuerySuggestions = [];

        private bool _repositoryIsInitialised;

        private int _countOfEventsInTheSeriesCompletedToDate;

        private SeriesProfileItem _seriesProfileItemToWhichThisRepositoryBelongs = new();
        private Dictionary<int, string> _dictionaryOfTxxColumnHeaders = new();

        #endregion

        #region methods

        /// <summary>
        ///     Because this method fails silently, be sure to verify internet connection and service
        ///     availability before calling this method. if you don't do so, if either of them are problematic
        ///     you'll be oblivious and perceive the equivalent of there being no results for the event, which is a lie.
        /// </summary>
        /// <param name="databaseAccountName"></param>
        /// <param name="dataContainerName"></param>
        /// <param name="seriesProfileItemToWhichThisRepositoryBelongs"></param>
        /// <returns></returns>
        public async Task<bool> LoadRepositoryOfSequenceContainersAsync(string databaseAccountName, string dataContainerName, SeriesProfileItem seriesProfileItemToWhichThisRepositoryBelongs)
        {
            const string failure = "Unable to marshall series totals.";
            const string locus = "[LoadRepositoryOfSequenceContainersAsync]";

            try
            {
                _seriesProfileItemToWhichThisRepositoryBelongs = seriesProfileItemToWhichThisRepositoryBelongs;

                #region figure out which events are eligible for points thus far in the season (only for reporting purposes at the end of this method)

                if (_seriesProfileItemToWhichThisRepositoryBelongs.EventProfileItems is null)
                    throw new JghNullObjectInstanceException(nameof(_seriesProfileItemToWhichThisRepositoryBelongs
                        .EventProfileItems));


                #endregion

                #region MASSIVE HEAVY LIFTING. GENERATE A REPOSITORY OF INDIVIDUAL RESULTS FOR EACH EVENT IN THE SERIES

                var populatedSeriesItem = await _leaderboardResultsSvcAgent.PopulateAllEventsInSingleSeriesWithAllResultsAsync(databaseAccountName, dataContainerName, seriesProfileItemToWhichThisRepositoryBelongs, CancellationToken.None);

                var listOfRepositoriesOfResultsForTheSeries = new List<IRepositoryOfResultsForSingleEvent>();

                var k = 1;

                foreach (var eligibleEvent in populatedSeriesItem.EventProfileItems.Where(z => z.AdvertisedDate <= DateTime.Now).OrderBy(z => z.AdvertisedDate))
                {
                    eligibleEvent.IndexOfEventInSeriesOverallCalc = k;
                    // NB. IndexOfEventInSeriesOverallCalc is used throughout the points calculations. used as the Dictionary key/index for each event

                    var repositoryOfResultsForThisEvent = new RepositoryOfResultsForSingleEvent(null);

                    var loadingDidSucceed = await repositoryOfResultsForThisEvent.LoadPrePopulatedRepositoryOfResultsForEventFailingSilentlyToFalseAsync(databaseAccountName, dataContainerName, eligibleEvent);

                    if (loadingDidSucceed == false) continue;

                    if (eligibleEvent.IsExcludedFromSeriesPointsTotal)
                        continue;

                    listOfRepositoriesOfResultsForTheSeries.Add(repositoryOfResultsForThisEvent);

                    k++;
                }

                #endregion

                #region store the repositories into a dictionary

                var dictionaryOfRepositoriesOfResultsForTheSeries =
                    new Dictionary<int, IRepositoryOfResultsForSingleEvent>();

                foreach (var repositoryOfResults in listOfRepositoriesOfResultsForTheSeries)
                {
                    var index = (await repositoryOfResults.GetEventToWhichThisRepositoryBelongsAsync())
                        .IndexOfEventInSeriesOverallCalc;

                    if (!dictionaryOfRepositoriesOfResultsForTheSeries.ContainsKey(index))
                        dictionaryOfRepositoriesOfResultsForTheSeries.Add(index, repositoryOfResults);
                }

                #endregion

                #region take all the event repositories of individual results and consolidate them into an omnibus repository of series standings

                await PopulateRepositoryWithResultsFromAllEvents(dictionaryOfRepositoriesOfResultsForTheSeries);

                #endregion

                _repositoryIsInitialised = true;

                //if (!eligibleEvents.Any())
                //    throw new JghAlertMessageException(
                //        ItWouldSeemThatNoEventsAreFlaggedAsBeingEligibleForSeriesStandingsAndRankingsAsYet);

                return true;
            }

            #region try catch handling

            catch (Exception ex)
            {
                _repositoryIsInitialised = false;

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        public static double GetValueOfDataPointFromDictionaryOfDataPoints(Dictionary<int, double> dictionaryOfDataPoints, int lineItemIndex)
        {
            if (dictionaryOfDataPoints is null)
                return 0;

            return dictionaryOfDataPoints.ContainsKey(lineItemIndex) ? dictionaryOfDataPoints[lineItemIndex] : 0.0;
        }

        #endregion

        #region methods to access the repository

        public async Task<string[]> GetRacesFoundInPointsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_pointsTableOfRaces);
        }

        public async Task<string[]> GetGendersFoundInPointsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_pointsTableOfGenders);
        }

        public async Task<string[]> GetAgeGroupsFoundInPointsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_pointsTableOfAgeGroups);
        }

        public async Task<string[]> GetCitiesFoundInPointsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_pointsTableOfCities);
        }

        public async Task<string[]> GetTeamsFoundInPointsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_pointsTableOfTeams);
        }

        public async Task<string[]> GetUtilityClassificationsFoundInPointsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_pointsTableOfUtilityClassifications);
        }


        public async Task<string[]> GetRacesFoundInTourAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tourTableOfRaces);
        }

        public async Task<string[]> GetGendersFoundInTourAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tourTableOfGenders);
        }

        public async Task<string[]> GetAgeGroupsFoundInTourAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tourTableOfAgeGroups);
        }

        public async Task<string[]> GetCitiesFoundInTourAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tourTableOfCities);
        }

        public async Task<string[]> GetTeamsFoundInTourAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tourTableOfTeams);
        }

        public async Task<string[]> GetUtilityClassificationsFoundInTourAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<string>());

            return await Task.FromResult(_tourTableOfUtilityClassifications);
        }


        public async Task<PopulationCohortItem[]> GetRaceCohortsFoundAsync()
        {
            const string failure = "Populating histogram.";
            const string locus = "[GetRaceCohortsFoundInPointsAsync]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectAllIndividualResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup))
                        .ToLookup(z => z.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup, z => z)
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
            const string locus = "[GetGenderHistogram]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectAllIndividualResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.MostRecentResultItemToWhichThisSequenceApplies.Gender))
                        .ToLookup(z => z.MostRecentResultItemToWhichThisSequenceApplies.Gender, z => z)
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
            const string locus = "[GetCategoryCohortHistogramsAsync]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectAllIndividualResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.MostRecentResultItemToWhichThisSequenceApplies.AgeGroup))
                        .ToLookup(z => z.MostRecentResultItemToWhichThisSequenceApplies.AgeGroup, z => z)
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
            const string locus = "[GetCityCohortsFoundInPointsAsync]";

            try
            {
                if (_repositoryIsInitialised == false)
                    return [];

                return await AlgorithmForPlacings.ComposeTableOfCohortHistogramsGroupedByStringAsync
                (
                    SelectAllIndividualResults()
                        .Where(z => !string.IsNullOrWhiteSpace(z.MostRecentResultItemToWhichThisSequenceApplies.City))
                        .ToLookup(z => JghString.TmLr(z.MostRecentResultItemToWhichThisSequenceApplies.City), z => z)
                        .ToArray()
                );
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        public async Task<bool> GetIsInitialised()
        {
            return await Task.FromResult(_repositoryIsInitialised);
        }

        public async Task<int> GetNumberOfEventsEligibleForPointsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(0);

            var numberOfEligibleEvents =
                _seriesProfileItemToWhichThisRepositoryBelongs.NumOfScoresToCountForSeriesTotalPoints;

            return await Task.FromResult(numberOfEligibleEvents);
        }

        public async Task<SequenceContainerItem[]> GetPointsStandingsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<SequenceContainerItem>());

            return await Task.FromResult(_pointsStandings);
        }

        public async Task<SequenceContainerItem[]> GetTourStandingsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<SequenceContainerItem>());

            return await Task.FromResult(_tourDurationStandings);
        }

        public async Task<SearchQueryItem[]> GetPointsSearchSuggestionsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<SearchQueryItem>());

            return await Task.FromResult(_pointsSearchQuerySuggestions);
        }

        public async Task<SearchQueryItem[]> GetTourSearchSuggestionsAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(Array.Empty<SearchQueryItem>());

            return await Task.FromResult(_tourDurationSearchQuerySuggestions);
        }


        public async Task<Dictionary<int, string>> GetTxxColumnHeadersAsync()
        {
            if (_repositoryIsInitialised == false)
                return await Task.FromResult(new Dictionary<int, string>());

            return await Task.FromResult(_dictionaryOfTxxColumnHeaders);
        }

        #endregion

        #region helpers

        private async Task<bool> PopulateRepositoryWithResultsFromAllEvents(Dictionary<int, IRepositoryOfResultsForSingleEvent> dictionaryOfRepositoriesOfResults)
        {
            const string failure = "Unable to populate repository of series totals from dictionaries of results.";
            const string locus = "[PopulateRepositoryOfSequenceContainers]";

            try
            {
                await PopulateDictionaryOfTxxColumnHeadersLookupTableForSequences(dictionaryOfRepositoriesOfResults);

                await PopulateArrayOfRaceSpecificationsForSeriesItemToWhichThisRepositoryBelongs(
                    dictionaryOfRepositoriesOfResults);

                // convert dictionary of repositories back to dictionary of lists of results for finishers only, and those eligible for series

                _countOfEventsInTheSeriesCompletedToDate = dictionaryOfRepositoriesOfResults.Count;

                var dictionaryOfListsOfThoseEligibleForSeriesAwards = new Dictionary<int, ResultItem[]>();

                foreach (var kvp in dictionaryOfRepositoriesOfResults)
                {
                    if (kvp.Value is null) continue;

                    // for points, be sure to exclude individual results for people not registered for the whole season i.e. without series licenses

                    var xx = kvp.Value;

                    var yy = await xx.GetPlacedResultsAsync();

                    var listOfEligibleFinishers = yy
                        .Where(z => z is not null)
                        .Where(z => z.IsSeries)
                        .ToArray();

                    // give every individual result an index corresponding to the index of the event to which it belongs
                    foreach (var result in listOfEligibleFinishers)
                        result.ScratchPadIndex = (await kvp.Value.GetEventToWhichThisRepositoryBelongsAsync())
                            .IndexOfEventInSeriesOverallCalc;

                    dictionaryOfListsOfThoseEligibleForSeriesAwards[kvp.Key] =
                        listOfEligibleFinishers.Where(individual => individual.IsSeries).ToArray();
                }

                var allResultsInAllEventsInSeries = new List<ResultItem>();

                foreach (var kvp in dictionaryOfListsOfThoseEligibleForSeriesAwards)
                    if (kvp.Value is not null)
                        allResultsInAllEventsInSeries.AddRange(kvp.Value);

                await PopulateRepositoryOfSequenceContainers(allResultsInAllEventsInSeries.ToArray());

                if (!allResultsInAllEventsInSeries.Any())
                    throw new JghAlertMessageException(
                        ItSeemsThatNoParticipantsExistOrNoneAreEligibleForSeasonStandings);

                return true;
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        private async Task<bool> PopulateDictionaryOfTxxColumnHeadersLookupTableForSequences(Dictionary<int, IRepositoryOfResultsForSingleEvent> dictionaryOfRepositoriesOfResults)
        {
            var answer = new Dictionary<int, string>();

            var tuplesOfNumInSeqAndTxxHeader = new List<Tuple<int, string>>();

            var listOfRepositories = dictionaryOfRepositoriesOfResults.Select(z => z.Value).ToList();

            foreach (var repositoryOfResults in listOfRepositories.Where(z => z is not null))
            {
                var eventToWhichRepositoryBelongs =
                    await repositoryOfResults.GetEventToWhichThisRepositoryBelongsAsync();
                var numInSequence = eventToWhichRepositoryBelongs.NumInSequence;
                var txxColumnHeader = eventToWhichRepositoryBelongs.TxxColumnHeader;
                tuplesOfNumInSeqAndTxxHeader.Add(new Tuple<int, string>(numInSequence, txxColumnHeader));
            }

            tuplesOfNumInSeqAndTxxHeader = tuplesOfNumInSeqAndTxxHeader.OrderBy(z => z.Item1).ToList();

            var i = 1;
            foreach (var tuple in tuplesOfNumInSeqAndTxxHeader)
            {
                answer.Add(i, tuple.Item2);

                i++;
            }

            _dictionaryOfTxxColumnHeaders = answer;

            return true;
        }

        private async Task<bool> PopulateArrayOfRaceSpecificationsForSeriesItemToWhichThisRepositoryBelongs(Dictionary<int, IRepositoryOfResultsForSingleEvent> dictionaryOfRepositoriesOfIndividualResults)
        {
            const string failure = "Unable to consolidate all race definition items for all events in series.";
            const string locus = "[PopulateArrayOfRaceSpecificationsForSeriesItemToWhichThisRepositoryBelongs]";

            try
            {
                var repositoriesOfIndividualResults = from kvp in dictionaryOfRepositoriesOfIndividualResults
                    where kvp.Value is not null
                    select kvp.Value;


                var listOfEventItems = new List<EventProfileItem>();

                foreach (var repositoryOfIndividualResult in repositoriesOfIndividualResults)
                    listOfEventItems.Add(await repositoryOfIndividualResult
                        .GetEventToWhichThisRepositoryBelongsAsync());

                var omnibusListOfRaceSpecifications = new List<RaceSpecificationItem>();

                foreach (var eventItem in listOfEventItems)
                    if (eventItem?.EventSettingsItem.RaceSpecificationItems is not null)
                        omnibusListOfRaceSpecifications.AddRange(eventItem.EventSettingsItem.RaceSpecificationItems.ToList());

                var lookupTableUnderGoingCreation = new Dictionary<string, RaceSpecificationItem>();

                foreach (var raceSpecificationItem in omnibusListOfRaceSpecifications.Where(z => z.Label is not null))
                    // for simplicity and speed, first arrivals take precedence
                    if (!lookupTableUnderGoingCreation.ContainsKey(raceSpecificationItem.Label))
                        lookupTableUnderGoingCreation.Add(raceSpecificationItem.Label, raceSpecificationItem);

                _seriesProfileItemToWhichThisRepositoryBelongs.DefaultEventSettingsForAllEvents.RaceSpecificationItems =
                    lookupTableUnderGoingCreation.Select(z => z.Value).ToArray();

                return true;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private void AddDataPointToDictionaryOfDataPoints(Dictionary<int, double> dictionaryOfDataPoints, int dataPointIndex, double dataPointValue)
        {
            if (dictionaryOfDataPoints is null) return;

            if (dictionaryOfDataPoints.ContainsKey(dataPointIndex))
                dictionaryOfDataPoints[dataPointIndex] = dataPointValue;
            else
                dictionaryOfDataPoints.Add(dataPointIndex, dataPointValue);
        }

        private async Task<bool> PopulateRepositoryOfSequenceContainers(ResultItem[] allResultItemsInAllEventsInSeries)
        {
            // make a composite key with an equality comparer that identifies each Jekyll. during the course of a season, jekylls race in more than one 'race' if they are promoted
            // generate a collection of unique jekylls (_dictionaryOfJekylls). number of jekylls is always greater than the number of individuals 
            // if this jekyll is already in the dictionary, add this 
            // data item to his little personal dictionary of data items(DictionaryOfJekyllsSourceDataPerEvent)
            // AddDataPointToDictionaryOfDataPoints method will create DictionaryOfJekyllsSourceDataPerEvent if it doesn't yet exist
            // if this is the first time we've come across Jekyll. instantiate a new entry for him in the dictionary of jekylls.
            // this takes the form of a SequenceContainerItem
            // and while we're about it, add this data item to his new personal dictionary
            // (AddDataPointToDictionaryOfDataPoints method will create a dictionary if it doesn't yet exist)
            // this method presumes that the ScratchPadIndex property of each individual result has been preloaded with the index of the event to which it belongs i.e. 1 thru 14 in the case of Kelso
            // to begin with obtain the event number that this individual result pertains to i.e. 1 through 14 events per season for Kelso

            const string failure = "Unable to populate series totals from list of all results.";
            const string locus = "[PopulateRepositoryOfSequenceContainers]";

            try
            {
                var dictionaryOfJekyllsAndHydes =
                    new Dictionary<Tuple<Tuple<string, string, string, string>, string>, SequenceContainerItem>();

                #region COMPUTE SERIES POINTS STANDINGS

                #region populate collection of all jekyll/hyde pairs and their sequence of points i.e. for each unique jekyll her own sequence of PointsCalculatedRank gathered at each event in her race on the day

                var cloneOfAllResultsItemsInAllEventsInSeriesForPoints = MakeCloneOfResults(allResultItemsInAllEventsInSeries);

                foreach (var resultItem in cloneOfAllResultsItemsInAllEventsInSeriesForPoints.Where(z => z is not null)
                             .Where(z => z.DerivedData is not null))
                {
                    var eventNumber = resultItem.ScratchPadIndex;

                    try
                    {
                        var jekyllKey = Tuple.Create(resultItem.Identifier, JghString.TmLr(resultItem.RaceGroup));

                        if (!dictionaryOfJekyllsAndHydes.ContainsKey(jekyllKey))
                        {
                            //create a first time newSeriesStandingObject entry for this Identifier in this RaceGroup in the dictionary of all Jekylls being assembled

                            var newSeriesStandingObjectForJekyll = new SequenceContainerItem
                            {
                                SequenceTotal = 0,
                                SequenceOfSourceData = new Dictionary<int, double>(),
                                MostRecentResultItemToWhichThisSequenceApplies = resultItem
                            };

                            dictionaryOfJekyllsAndHydes.Add(jekyllKey, newSeriesStandingObjectForJekyll);

                            // enter the DataPoint for this eventNumber in the newSeriesStandingObject

                            AddDataPointToDictionaryOfDataPoints(dictionaryOfJekyllsAndHydes[jekyllKey].SequenceOfSourceData, eventNumber, resultItem.DerivedData.PointsCalculated);
                        }
                        else
                        {
                            // update MostRecentResultItemToWhichThisSequenceApplies

                            dictionaryOfJekyllsAndHydes[jekyllKey].MostRecentResultItemToWhichThisSequenceApplies = resultItem;

                            // enter the DataPoint for this eventNumber in the newSeriesStandingObject

                            AddDataPointToDictionaryOfDataPoints(dictionaryOfJekyllsAndHydes[jekyllKey].SequenceOfSourceData, eventNumber, resultItem.DerivedData.PointsCalculated);
                        }
                    }
                    catch (Exception)
                    {
                        //do nothing - skip the item and carry on as best we can
                    }
                }

                #endregion

                #region collapse each jekyll/hyde pair to a single individual and consolidate points if promoted in seniority but not if demoted

                var allStandingsTableEntriesForTheSeasonV1 = await CollapseAllPairsOfJekyllsAndHydesIntoSingleIndividuals(
                    dictionaryOfJekyllsAndHydes.Select(kvp => kvp.Value).ToArray());

                #endregion

                #region for each individual, compute total points i.e. 10 best out of 14 for Kelso, 6 out of 8 for Kelso2014cx

                foreach (var individualEntry in allStandingsTableEntriesForTheSeasonV1)
                    individualEntry.SequenceTotal =
                        individualEntry.SequenceOfSourceData.OrderByDescending(
                                kvp => kvp.Value)
                            .Take(_seriesProfileItemToWhichThisRepositoryBelongs
                                .NumOfScoresToCountForSeriesTotalPoints).Sum(kvp => kvp.Value);

                #endregion

                #region do generic rankings of totals relative to peer group

                var seriesTotalMustBeRankedByDescending = true;

                // method AlgorithmForSequenceContainerDataRankings.DoRankingsOfTotalsForAllIndividualsRelativeToTheirCompetitorsAsync()
                // requires that all sequence items have a unique key

                var i = 0;

                foreach (var sequenceItem in allStandingsTableEntriesForTheSeasonV1)
                {
                    sequenceItem.ID = i;
                    //sequenceItem.ID = Guid.NewGuid().ToString();
                    i += 1;
                }


                var seriesStandingsWithAllIndividualsDulyRanked =
                    await
                        AlgorithmForSequenceContainerDataRankings
                            .DoRankingsOfTotalsForAllIndividualsRelativeToTheirCompetitorsAsync(
                                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                seriesTotalMustBeRankedByDescending,
                                allStandingsTableEntriesForTheSeasonV1);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                var seriesStandingsItemsWithAllIndividualsDulyRankedBySeasonTotals = seriesTotalMustBeRankedByDescending
                    ? seriesStandingsWithAllIndividualsDulyRanked.OrderByDescending(z => z.SequenceTotal).ToArray()
                    : seriesStandingsWithAllIndividualsDulyRanked.OrderBy(z => z.SequenceTotal).ToArray();

                #endregion

                _pointsStandings =
                    seriesStandingsItemsWithAllIndividualsDulyRankedBySeasonTotals.Where(z => z is not null)
                        .Where(z => z.MostRecentResultItemToWhichThisSequenceApplies is not null).ToArray();

                #region generate unique IDs for all attribute values and cross-index into individual results

                PopulateAllCategoryLookupTablesByInspectionOfPointsStandings(_pointsStandings);

                #endregion

                #region map to autocompletebox list of items

                _pointsSearchQuerySuggestions =
                    await GenerateAutocompleteBoxItemsSource(_pointsStandings);

                #endregion

                #endregion

                #region REPEAT THE WHOLE PROCESS AGAIN FROM THE BEGINNING TO COMPUTE TOUR STANDINGS

                #region populate collection of all jekyll/hydes i.e. for each unique person her own sequence of DurationTimeSpanSec gathered at each event in her race on the day

                var cloneOfAllResultsItemsInAllEventsInSeriesForTourStandings = MakeCloneOfResults(allResultItemsInAllEventsInSeries);

                dictionaryOfJekyllsAndHydes =
                    new Dictionary<Tuple<Tuple<string, string, string, string>, string>, SequenceContainerItem>();

                foreach (var resultItem in cloneOfAllResultsItemsInAllEventsInSeriesForTourStandings)
                {
                    //sidestep everyone who was not a finisher. to be on the tour standings you have to finish, and finish all events
                    if (!resultItem.DerivedData.IsValidDuration)
                        continue;

                    var eventNumber = resultItem.ScratchPadIndex;


                    try
                    {
                        var jekyllKey = Tuple.Create(resultItem.Identifier, JghString.TmLr(resultItem.RaceGroup));

                        if (!dictionaryOfJekyllsAndHydes.ContainsKey(jekyllKey))
                        {
                            //create a first time newSeriesStandingObject entry for this Identifier in this RaceGroup in the dictionary of all Jekylls being assembled

                            var newSeriesStandingObjectForJekyll = new SequenceContainerItem
                            {
                                SequenceTotal = 0,
                                SequenceOfSourceData = new Dictionary<int, double>(),
                                MostRecentResultItemToWhichThisSequenceApplies = resultItem
                            };

                            dictionaryOfJekyllsAndHydes.Add(jekyllKey, newSeriesStandingObjectForJekyll);

                            // enter the DataPoint for this eventNumber in the newSeriesStandingObject

                            AddDataPointToDictionaryOfDataPoints(dictionaryOfJekyllsAndHydes[jekyllKey].SequenceOfSourceData, eventNumber, resultItem.DerivedData.TotalDurationFromAlgorithmInSeconds);

                        }
                        else
                        {
                            // update MostRecentResultItemToWhichThisSequenceApplies

                            dictionaryOfJekyllsAndHydes[jekyllKey].MostRecentResultItemToWhichThisSequenceApplies = resultItem;

                            // enter the DataPoint for this eventNumber in the newSeriesStandingObject

                            AddDataPointToDictionaryOfDataPoints(dictionaryOfJekyllsAndHydes[jekyllKey].SequenceOfSourceData, eventNumber, resultItem.DerivedData.TotalDurationFromAlgorithmInSeconds);

                        }
                    }
                    catch (Exception)
                    {
                        //do nothing - skip the item and carry on as best we can
                    }
                }

                #endregion

                #region eliminate all pairs of jekylls and hydes. you can't be eligible for the tour standings if you've changed seniority

                var scratchpadAmalgamatedStandings =
                    await EliminateAllPairsOfJekyllsAndHydes(dictionaryOfJekyllsAndHydes.Select(kvp => kvp.Value)
                        .ToArray());

                #endregion

                #region only take people who have completed and recorded valid times in all events. all others are disqualified. valid duration must be greater than 1 second (required empirically! don't omit)

                var allStandingsTableEntriesForTheSeasonV2 = new List<SequenceContainerItem>();

                foreach (var typeKelso2013SeriesStandingObject in scratchpadAmalgamatedStandings)
                {
                    var numberOfEventsCompleted =
                        typeKelso2013SeriesStandingObject.SequenceOfSourceData.Count(z => z.Value > 1);

                    if (numberOfEventsCompleted >= _countOfEventsInTheSeriesCompletedToDate)
                        allStandingsTableEntriesForTheSeasonV2.Add(typeKelso2013SeriesStandingObject);
                }

                #endregion

                #region for each individual, compute total duration in sec

                foreach (var individualEntry in allStandingsTableEntriesForTheSeasonV2)
                    individualEntry.SequenceTotal = individualEntry.SequenceOfSourceData.Sum(kvp => kvp.Value);

                #endregion

                #region do generic rankings of season total duration relative to peer group

                seriesTotalMustBeRankedByDescending = false;

                // method AlgorithmForSequenceContainerDataRankings.DoRankingsOfTotalsForAllIndividualsRelativeToTheirCompetitorsAsync()
                // requires that all sequence items have a unique key

                var iV2 = 0;

                foreach (var sequenceItem in allStandingsTableEntriesForTheSeasonV2)
                {
                    sequenceItem.ID = iV2;
                    //sequenceItem.Guid = Guid.NewGuid().ToString();
                    iV2 += 1;
                }


                seriesStandingsWithAllIndividualsDulyRanked =
                    await
                        AlgorithmForSequenceContainerDataRankings
                            .DoRankingsOfTotalsForAllIndividualsRelativeToTheirCompetitorsAsync(
                                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                seriesTotalMustBeRankedByDescending,
                                allStandingsTableEntriesForTheSeasonV2.ToArray());

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                seriesStandingsItemsWithAllIndividualsDulyRankedBySeasonTotals = seriesTotalMustBeRankedByDescending
                    ? seriesStandingsWithAllIndividualsDulyRanked.OrderByDescending(z => z.SequenceTotal).ToArray()
                    : seriesStandingsWithAllIndividualsDulyRanked.OrderBy(z => z.SequenceTotal).ToArray();

                #endregion

                _tourDurationStandings =
                    seriesStandingsItemsWithAllIndividualsDulyRankedBySeasonTotals.Where(z => z is not null)
                        .Where(z => z.MostRecentResultItemToWhichThisSequenceApplies is not null).ToArray();

                #region generate unique IDs for all attribute values and cross-index into individual results

                PopulateAllCategoryLookupTablesByInspectionOfTourDurationStandings(_tourDurationStandings);

                #endregion

                #region map to autocompletebox list of items

                _tourDurationSearchQuerySuggestions =
                    await GenerateAutocompleteBoxItemsSource(_tourDurationStandings);

                #endregion

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        private static ResultItem[] MakeCloneOfResults(IEnumerable<ResultItem> results)
        {
            if (results is null) return null;

            return (from result in results
                where result is not null
                select result.ShallowMemberwiseCloneCopyIncludingDerivedData).ToArray();
        }

        private void PopulateAllCategoryLookupTablesByInspectionOfPointsStandings(SequenceContainerItem[] pointsStandings)
        {
            const string failure = "Unable to populate attribute IDs for series points standings.";
            const string locus = "[PopulateAllCategoryLookupTablesByInspectionForPointsStandings]";

            try
            {
                if (pointsStandings is null)
                    throw new JghNullObjectInstanceException(nameof(pointsStandings));

                var results = pointsStandings
                    .Where(z => z is not null)
                    .Select(z => z.MostRecentResultItemToWhichThisSequenceApplies)
                    .Where(z => z is not null).ToArray();

                _pointsTableOfRaces = results.Select(z => z.RaceGroup).Distinct().OrderBy(z => z).ToArray();
                _pointsTableOfGenders = results.Select(z => z.Gender).Distinct().OrderBy(z => z).ToArray();
                _pointsTableOfAgeGroups = results.Select(z => z.AgeGroup).Distinct().OrderBy(z => z).ToArray();
                _pointsTableOfCities = results.Select(z => z.City).Distinct().OrderBy(z => z).ToArray();
                _pointsTableOfTeams = results.Select(z => z.Team).Distinct().OrderBy(z => z).ToArray();
                _pointsTableOfUtilityClassifications = results.Select(z => z.UtilityClassification).Distinct().OrderBy(z => z).ToArray();
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private void PopulateAllCategoryLookupTablesByInspectionOfTourDurationStandings(SequenceContainerItem[] tourDurationStandings)
        {
            const string failure = "Unable to populate attribute IDs for series tour standings.";
            const string locus = "[PopulateAllCategoryLookupTablesByInspectionOfTourDurationStandings]";

            try
            {
                if (tourDurationStandings is null)
                    throw new JghNullObjectInstanceException(nameof(tourDurationStandings));

                var results = tourDurationStandings
                    .Where(z => z is not null)
                    .Select(z => z.MostRecentResultItemToWhichThisSequenceApplies)
                    .Where(z => z is not null).ToArray();

                _tourTableOfRaces = results.Select(z => z.RaceGroup).Distinct().OrderBy(z => z).ToArray();
                _tourTableOfGenders = results.Select(z => z.Gender).Distinct().OrderBy(z => z).ToArray();
                _tourTableOfAgeGroups = results.Select(z => z.AgeGroup).Distinct().OrderBy(z => z).ToArray();
                _tourTableOfCities = results.Select(z => z.City).Distinct().OrderBy(z => z).ToArray();
                _tourTableOfTeams = results.Select(z => z.Team).Distinct().OrderBy(z => z).ToArray();
                _tourTableOfUtilityClassifications = results.Select(z => z.UtilityClassification).Distinct().OrderBy(z => z).ToArray();
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private static async Task<SearchQueryItem[]> GenerateAutocompleteBoxItemsSource(IEnumerable<SequenceContainerItem> sequenceItems)
        {
            const string failure = "Unable to generate autocompletebox itemsource.";
            const string locus = "[GenerateAutocompleteBoxItemsource]";

            try
            {
                if (sequenceItems is null) return [];

                var scratchpad = sequenceItems
                    .Where(z => z is not null)
                    .Select(z => z.MostRecentResultItemToWhichThisSequenceApplies)
                    .Where(z => z is not null).ToArray();


                return (await JghParallel.SelectAsParallelWorkStealingAsync(
                    scratchpad,
                    ResultItem.ToSearchQuerySuggestionItem,
                    500)).ToArray();
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private Task<SequenceContainerItem[]> CollapseAllPairsOfJekyllsAndHydesIntoSingleIndividuals(IEnumerable<SequenceContainerItem> allJekylls)
        {
            //todo this approach is dangerous. 
            //it presumes a person has only changed race seniority a single time
            // and that ergo that there are max two jekylls for each individual

            const string failure = "Handling promotions and demotions.";
            const string locus = "[CollapseAllPairsOfJekyllsAndHydesIntoSingleIndividuals]";

            var individuals = new List<SequenceContainerItem>();

            var tcs = new TaskCompletionSource<SequenceContainerItem[]>(TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                if (allJekylls is null)
                    throw new JghNullObjectInstanceException(nameof(allJekylls));

                var subGroupsOfJekylls = (from jekyll in allJekylls
                    where jekyll is not null
                    where jekyll.MostRecentResultItemToWhichThisSequenceApplies is not null
                    let primaryKey = jekyll.MostRecentResultItemToWhichThisSequenceApplies.Identifier
                    group jekyll by primaryKey
                    into subGroupsofOneOrTwoJekyllsForEachIndividual
                    orderby subGroupsofOneOrTwoJekyllsForEachIndividual.Key
                    select subGroupsofOneOrTwoJekyllsForEachIndividual).ToArray();


                foreach (var subGroupOfJekyllsForAnIndividual in subGroupsOfJekylls)
                {
                    if (subGroupOfJekyllsForAnIndividual is null)
                        continue;

                    switch (subGroupOfJekyllsForAnIndividual.Count())
                    {
                        case 0:
                            continue; // aberration; do nothing
                        case 1
                            : // this was never a jekyll in the first place. was an individual all along who never changed race groups 
                            individuals.Add(subGroupOfJekyllsForAnIndividual.FirstOrDefault());
                            continue;
                        default: // a jekyll/hyde exists
                        {
                            var mostRecentEvent = subGroupOfJekyllsForAnIndividual
                                .OrderByDescending(item => item.MostRecentResultItemToWhichThisSequenceApplies.ScratchPadIndex).FirstOrDefault();

                            var earliestEvent = subGroupOfJekyllsForAnIndividual
                                .OrderBy(item => item.MostRecentResultItemToWhichThisSequenceApplies.ScratchPadIndex).FirstOrDefault();

                            if ((mostRecentEvent is null) | (earliestEvent is null))
                                continue;

                            Debug.WriteLine($"{mostRecentEvent.MostRecentResultItemToWhichThisSequenceApplies.Identifier} changed from {earliestEvent.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup} to {mostRecentEvent.MostRecentResultItemToWhichThisSequenceApplies.RaceGroup}");

                            try
                            {
                                // try/catch guards against out of bounds exceptions

                                for (var dataPointIndex = 1; dataPointIndex < NumberOfEventsDuringKelsoSeasonPlusOne; dataPointIndex++)

                                    // if seniority has decreased make no changes, do nothing - accept the most recent finishers' points array. his array will by default reflect zero points during the time he was in more senior category which is what we want. otherwise update

                                    if (AlgorithmForPoints.JekyllIsMoreSeniorAtTheEndOfTheKelsoSeriesThanTheBeginning(_seriesProfileItemToWhichThisRepositoryBelongs, mostRecentEvent, earliestEvent))
                                    {
                                        var dictionaryOfDataPoints = mostRecentEvent.SequenceOfSourceData;

                                        var dataPointValue = Math.Max(GetValueOfDataPointFromDictionaryOfDataPoints(dictionaryOfDataPoints, dataPointIndex),
                                            GetValueOfDataPointFromDictionaryOfDataPoints(earliestEvent.SequenceOfSourceData, dataPointIndex));

                                        AddDataPointToDictionaryOfDataPoints(dictionaryOfDataPoints, dataPointIndex, dataPointValue);
                                    }
                            }
                            catch (Exception)
                            {
                                //do nothing. put on a brave face and continue to the next step - add the mostRecentFinisherPointsTableLineItem unaltered
                            }

                            individuals.Add(mostRecentEvent);
                        }
                            break;
                    }
                }

                tcs.TrySetResult(individuals.ToArray());

                return tcs.Task;
            }

            #region trycatch

            catch (Exception ex)
            {
                tcs.TrySetException(JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, ex));
                return tcs.Task;
            }

            #endregion
        }

        private static Task<IEnumerable<SequenceContainerItem>> EliminateAllPairsOfJekyllsAndHydes(IEnumerable<SequenceContainerItem> allJekylls)
        {
            //todo this approach is dangerous. 
            // it presumes <Event> contains the integer number of the event rather than a description of some sort

            const string failure = "Handling promotions and demotions.";
            const string locus = "[EliminateAllPairsOfJekylls]";

            var tcs = new TaskCompletionSource<IEnumerable<SequenceContainerItem>>(TaskCreationOptions.RunContinuationsAsynchronously);

            var individuals = new List<SequenceContainerItem>();

            try
            {
                if (allJekylls is null)
                    throw new JghNullObjectInstanceException(nameof(allJekylls));


                var subGroupsOfJekylls = (from jekyll in allJekylls
                    where jekyll is not null
                    where jekyll.MostRecentResultItemToWhichThisSequenceApplies is not null
                    let primaryKey = jekyll.MostRecentResultItemToWhichThisSequenceApplies.Identifier
                    group jekyll by primaryKey
                    into subGroupsofOneOrTwoJekyllsForEachIndividual
                    orderby subGroupsofOneOrTwoJekyllsForEachIndividual.Key
                    select subGroupsofOneOrTwoJekyllsForEachIndividual).ToArray();


                foreach (var subGroupOfJekyllsForAnIndividual in subGroupsOfJekylls)
                {
                    if (subGroupOfJekyllsForAnIndividual is null)
                        continue;

                    switch (subGroupOfJekyllsForAnIndividual.Count())
                    {
                        case 0:
                            continue; // aberration; do nothing
                        case 1
                            : // this was never a jekyll in the first place. was an individual all along who never changed categories
                            individuals.Add(subGroupOfJekyllsForAnIndividual.FirstOrDefault());
                            continue;
                        default: // more than one jekyll exists
                        {
                            // do nothing. don't add him to the list of tour contenders. TKO. disqualified
                        }
                            break;
                    }
                }

                tcs.TrySetResult(individuals.ToArray());
            }

            #region trycatch

            catch (Exception ex)
            {
                tcs.TrySetException(JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, ex));
            }

            #endregion

            return tcs.Task;
        }

        private SequenceContainerItem[] SelectAllIndividualResults()
        {
            return _pointsStandings.Where(item => item is not null).ToArray();
        }

        #endregion
    }
}