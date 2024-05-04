using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ViewModels01.April2022.Collections;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_algorithms;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.Strings;
using Rezultz.Library02.Mar2024.ValidationViewModels;

// ReSharper disable UnusedMethodReturnValue.Local

namespace Rezultz.Library02.Mar2024.PageViewModels
{
    public class SingleEventAverageSplitTimesPageViewModel : BaseLeaderboardStylePageViewModel
    {
        private const string Locus2 = nameof(SingleEventAverageSplitTimesPageViewModel);
        private const string Locus3 = "[Rezultz.Library02.Mar2024]";

        #region ctor

        public SingleEventAverageSplitTimesPageViewModel(IAzureStorageSvcAgent azureStorageSvcAgent,
            ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
            ISessionState sessionState,
            IThingsPersistedInLocalStorage thingsPersistedInLocalStorage,
            ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationViewModel,
            IRepositoryOfResultsForSingleEvent repositoryOfResultsForThisEvent
            )
            : base(azureStorageSvcAgent, sessionState, thingsPersistedInLocalStorage, globalSeasonProfileAndIdentityValidationViewModel)
        {
            #region assign ctor IOC injections

            _repositoryOfResultsForThisEvent = repositoryOfResultsForThisEvent;

            _leaderboardResultsSvcAgent = leaderboardResultsSvcAgent;

            #endregion

            #region instantiate cbos

            // we need dummies because various methods in base classes operate on them routinely

            CboLookupMoreInfoItemVm = new IndexDrivenCollectionViewModel<MoreInformationItemDisplayObject>(
                Strings2017.Other_leaderboards,
                () => { }, () => true);

            #endregion

            #region layout switches

            LeaderboardListRowGroupingEnum = EnumStrings.RowsAreUnGrouped;
            FavoritesListRowGroupingEnum = EnumStrings.RowsAreUnGrouped;

            #endregion

            #region LeaderboardColumnFormatEnum

            DataGridOfFavoritesVm.ColumnFormatEnum = EnumStrings.AverageSplitTimesColumnFormat;
            DataGridOfLeaderboardVm.ColumnFormatEnum = EnumStrings.AverageSplitTimesColumnFormat;
            LeaderboardColumnFormatEnum = EnumStrings.AverageSplitTimesColumnFormat;
            // options are. EnumStrings.SingleEventResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

            #endregion
        }

        #endregion

        #region OnPageLoaded method

        // nothing. use Base

        #endregion

        #region commands

        protected override async Task<string> LoadSourceDataButtonOnClickAsync()
        {
            var failure = Strings2017.Unable_to_complete_computations_and_calculations_to_load_page_;
            const string locus = "[GetEventDataButtonOnClickAsync]";

            try
            {
                await ZeroiseAsync();

                await PopulateCboLookUpFileFormatsPresenterForDoingExports(); // arbitrary location for this. ordinarily would do this in ctor but can't because async

                #region null checks

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheSeasonMetadataItem();

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheEventItem();

                #endregion

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

                var currentEvent = EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem);

                #region do page titles, footer, and social media connections

                var organizerTitle = MakeOrganiserTitle();

                HeadersVm.Populate(organizerTitle, MakeSeriesTitle(), MakeEventTitle(), Strings2017.Results__);
                HeadersVm.SaveAsLastKnownGood();
                HeadersVm.IsVisible = true;

                if (currentEvent?.EventSettingsItem != null)
                {
                    var shortHandSettings = currentEvent.EventSettingsItem;

                    FootersVm.Populate(shortHandSettings.OneLinerContactMessage ?? string.Empty);
                    FootersVm.SaveAsLastKnownGood();
                    FootersVm.IsVisible = true;

                    SocialMediaConnectionsVm.PopulateConnections(shortHandSettings.OrganizerEmailAddress,
                        shortHandSettings.OrganizerFacebookUri, shortHandSettings.OrganizerInstagramUri, shortHandSettings.OrganizerTwitterUri);
                }

                #endregion

                #region Txx column headers

                var txxColumnHeadersAsDictionary = new Dictionary<int, string>();

                // insert 15 default headers. all column references beyond the first 15 are commented out in all classes
                // because we don't cater for more than 15 at the time of writing. 

                for (var j = 1; j < 16; j++) txxColumnHeadersAsDictionary.Add(j, string.Concat("T", j.ToString()));

                TxxColumnHeadersLookupTable = txxColumnHeadersAsDictionary;

                #endregion

                #region save last known good - 1st kick

                SaveGenesisOfThisViewModelAsLastKnownGood();

                #endregion

                #region THE MEAT LOAD REPOSITORY OF INDIVIDUAL RESULTS FOR EVENT

                try
                {
                    var mustPullDataFromStagingLocation = await ThingsPersistedInLocalStorage.GetMustUsePreviewDataOnLaunchAsync();

                    var locationOfData = mustPullDataFromStagingLocation
                        ? currentSeries.ContainerForResultsDataFilesPreviewed
                        : currentSeries.ContainerForResultsDataFilesPublished;

                    await _repositoryOfResultsForThisEvent.LoadRepositoryOfResultsFailingNoisilyAsync(
                        locationOfData.AccountName,
                        locationOfData.ContainerName,
                        currentEvent);

                    // Loads the repository of results failing noisily.
                    // if all goes well, there might be some or other JghAlertMessageException
                    // if there is no internet connection, innermost exception will be a JghCommunicationFailureException. a perfectly forseeable happenstance.
                    // throws if no blob posted yet, or event was cancelled. a perfectly legitimate happenstance. innermost exception will be a JghResultsData404Exception
                    // totally normal outcome to come back with zero results
                }

                #region try catch

                catch (Exception ex)
                {
                    if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    {
                        // do nothing. no option but to deem this outcome successful. wish we didn't have to suppress. but we have no other option until a threadsafe, failsafe alert messaging system is invented that doesn't entail exception throwing. which is nowhere on the horizon
                    }
                    else
                    {
                        throw;
                    }
                }

                #endregion

                #endregion

                // SUCCESS! we have a valid repository

                #region populate repositoryOfAverageSplits  - trick.

                var pseudoSingleSplitResults = AlgorithmForAverageSplitTimes.GeneratePseudoAverageSplitTimes(
                    await _repositoryOfResultsForThisEvent.GetPlacedResultsAsync(),
                    await _repositoryOfResultsForThisEvent.GetEventToWhichThisRepositoryBelongsAsync());

                var repositoryOfAverageSplits = new RepositoryOfResultsForSingleEvent(_leaderboardResultsSvcAgent);

                await repositoryOfAverageSplits.LoadRepositoryOfResultsWithDataProvidedAsync(pseudoSingleSplitResults);

                #endregion

                #region datagrid column format enum

                // do this in the ctor

                //DataGridOfFavoritesVm.ColumnFormatEnum = EnumStrings.SingleEventResultsColumnFormat;
                //DataGridOfLeaderboardVm.ColumnFormatEnum = EnumStrings.SingleEventResultsColumnFormat;
                //LeaderboardColumnFormatEnum = EnumStrings.SingleEventResultsColumnFormat;
                // options are. EnumStrings.SingleEventResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

                #endregion

                #region KEY STEP attach pseudoResults to AllDataGridLineItemDisplayObjects in base class so that all the methods in the base class have something to work with


                var averageSplits = await repositoryOfAverageSplits.GetPlacedResultsAsync();

                // reinstate Race and RaceID fields - only for display reasons (races are handled under the umbrella of utility classification)

                foreach (var result in averageSplits.Where(z => z != null).Where(z => z.UtilityClassification != null)) result.RaceGroup = result.UtilityClassification;

                AllDataGridLineItemDisplayObjects = ResultItemDisplayObject.FromModel(averageSplits); // 
                
                #endregion

                ThisViewModelIsInitialised = true;

                #region populate SearchFunctionVm itemssources

                var searchQuerySuggestions = ResultItem.ToSearchQuerySuggestionItem(averageSplits);
                //var searchQuerySuggestions = await repositoryOfAverageSplits.GetSearchQuerySuggestionsAsync();

                //var placedAndNonDnsResults = ResultItemDisplayObject.FromModel(await repositoryOfAverageSplits.GetPlacedAndNonDnsResultsAsync());

                SearchFunctionVm.PopulateItemsSource(searchQuerySuggestions);

                SearchFunctionVm.MakeVisibleIfThereAreThingsToBeSearched();

                #endregion

                #region populate CboRaceCategoryLookup itemsources

                /* In the UWP app, MustSelectAllRacesOnFirstTimeThroughForAnEvent MUST remain FALSE
                 * for the lifetime of the app, and the user must have no access to mess with it. In the
                 * mobile app, it can be TRUE or FALSE and the user can be free to change it back and forth
                 * during each session via user settings. In the user settings service, the implicit
                 * default is FALSE, which is what we keep for the UWP app. In the mobile app, however, we override
                 * the implicit default and change it to TRUE on the landing page upon commencement
                 * of each launch of the app - merely because it is more aesthetically pleasing this way. 
                 */

                //await PopulateCboRaceCategoryLookupsForSingleEventAverageLapTimesAsync(
                //    _repositoryOfResultsForThisEvent,
                //    await ThingsPersistedInLocalStorage.GetMustSelectAllRacesOnFirstTimeThroughForAnEventAsync());

                await CboLookupCategoryFiltersPopulateItemsSourcesForSingleEventLeaderboardAsync(repositoryOfAverageSplits);

                foreach (var lookupCollectionVm in CboLookUpCategoryFiltersMakeListOfAllVms())
                    await lookupCollectionVm.ChangeSelectedIndexAsync(0); // i.e. display all

                #endregion

                #region set indexes of Race CategoryLookups to match specified bib number if required i.e. GetMustSelectCategoryOfResultsForSingleBibNumberOnLaunchAsync = true

                //if (await ThingsPersistedInLocalStorage.GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync())
                //    await CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(
                //        SearchFunctionVm,
                //        await ThingsPersistedInLocalStorage.GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync());

                var mustFocusOnCategoryOfSinglePerson = await ThingsPersistedInLocalStorage.GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync();

                if (mustFocusOnCategoryOfSinglePerson)
                {
                    var idOfPerson = await ThingsPersistedInLocalStorage.GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync();

                    if (PersonWithTargetBibNumberIsSuccessfullyIdentifiedInPopulation(idOfPerson, out var personIdentified))
                    {
                        await CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(personIdentified);

                        CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood();
                    }
                }

                #endregion

                #region save last known good of Race CategoryLookups

                CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood();

                #endregion

                #region save genesis of this vm as last known good

                SaveGenesisOfThisViewModelAsLastKnownGood();

                HeadersVm.SaveAsLastKnownGood();

                FootersVm.SaveAsLastKnownGood();

                #endregion

                #region evaluate Favorites

                await FavoritesDataGridRefilterAsync();

                #endregion

                #region materialise PageImagesInSkyscraperRightVm

                PopulateSourceUriStringOfAllImageUriItems(currentEvent);

                await PageImagesInSkyscraperRightVm.RefillItemsSourceAsync(UriItemDisplayObject.FromModel(currentEvent?.EventSettingsItem?.UriItems));

                PageImagesInSkyscraperRightVm.MakeVisibleIfItemsSourceIsAny();

                #endregion


                var messageOk = await RefreshScreenButtonOnClickAsync();

                //var messageOk = !AllDataGridLineItemDisplayObjects.Any() ? "No results." : $"{AllDataGridLineItemDisplayObjects.Length} results.";

                return messageOk;
            }

            #region trycatch

            catch (Exception ex)
            {
                ThisViewModelIsInitialised = JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        protected override async Task<string> PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickAsync()
        {
            return await Task.FromResult("Feature not available.");
        }

        #endregion

        #region global props

        private static IRepositoryOfResultsForSingleEvent _repositoryOfResultsForThisEvent;

        private readonly ILeaderboardResultsSvcAgent _leaderboardResultsSvcAgent;

        #endregion

        #region helpers

        protected override string MakeMoreInfoLabel()
        {
            return "Average split times";
        }

        protected override string[] FinaliseTableHeadings()
        {
            const string failure = "Unable to format leaderboard headings.";
            const string locus = "[PopulateTableHeadings]";

            try
            {
                var answer2 = new[]
                {
                    MakeSeriesLabel(), MakeEventLabel(), MakeMoreInfoLabel(), MakeUtilityClassificationLabel(),
                    MakeGenderLabel(), MakeCategoryLabel(), MakeCityLabel()
                };

                return answer2;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        protected override string[] FinaliseFavoritesDataGridHeadings()
        {
            const string failure = "Unable to format favorites headings.";
            const string locus = "[FinaliseFavoritesDataGridHeadings]";

            try
            {
                var answer2 = new[]
                {
                    MakeSeriesLabel(), MakeEventLabel(), MakeMoreInfoLabel(),
                    Strings2017.Header_Favorites_List
                };

                return answer2;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        protected override MoreInformationItem PopulateDataGridTitleAndBlurb()
        {
            var answer = new MoreInformationItem
            {
                Title = "Average split times",
                Blurb = string.Empty
            };

            return answer;
        }

        protected override async Task<bool> CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(ResultItemDisplayObject personWithBibNumberSpecifiedForOpenOnLaunch)
        {
            if (personWithBibNumberSpecifiedForOpenOnLaunch == null) return true;

            await CboLookupRaceCategoryFilterVm.ChangeSelectedIndexToMatchItemLabelAsync(personWithBibNumberSpecifiedForOpenOnLaunch.RaceGroup);

            await CboLookupGenderCategoryFilterVm.ChangeSelectedIndexToMatchItemLabelAsync(personWithBibNumberSpecifiedForOpenOnLaunch.Gender);

            await CboLookupAgeGroupCategoryFilterVm.ChangeSelectedIndexToMatchItemLabelAsync(personWithBibNumberSpecifiedForOpenOnLaunch.AgeGroup);

            await CboLookupCityCategoryFilterVm.ChangeSelectedIndexAsync(0); // n/a

            await CboLookupTeamCategoryFilterVm.ChangeSelectedIndexAsync(0); // n/a

            // NB. the following line is what distinguishes this method from the virtual base method
            await CboLookupUtilityClassificationCategoryFilterVm.ChangeSelectedIndexToMatchItemLabelAsync(CboLookupUtilityClassificationCategoryFilterVm.GetItemByItemLabel(personWithBibNumberSpecifiedForOpenOnLaunch.RaceGroup).Label);

            return true;
        }

        #endregion

        #region Gui stuff

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
        {
            base.EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

            CboLookupMoreInfoItemVm.IsAuthorisedToOperate = false;

            ExportFavoritesButtonVm.IsAuthorisedToOperate = false;

            NavigateToPostedLeaderboardHyperlinkButtonVm.IsAuthorisedToOperate = false;

            PostLeaderboardAsHtmlToPublicDataStorageButtonVm.IsAuthorisedToOperate = false;
        }

        protected sealed override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
        {
            base.EvaluateVisibilityOfAllGuiControlsThatTouchData(makeVisible);

            CboLookupMoreInfoItemVm.IsVisible = false;

            ExportFavoritesButtonVm.IsVisible = false;

            NavigateToPostedLeaderboardHyperlinkButtonVm.IsVisible = false;

            PostLeaderboardAsHtmlToPublicDataStorageButtonVm.IsVisible = false;
        }

        #endregion
    }
}