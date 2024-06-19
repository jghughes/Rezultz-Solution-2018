using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.Collections;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.PageNavigation;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.Strings;
using Rezultz.Library02.Mar2024.ValidationViewModels;

// ReSharper disable UnusedMethodReturnValue.Local

namespace Rezultz.Library02.Mar2024.PageViewModels
{
    public sealed class SingleEventLeaderboardPageViewModel : BaseLeaderboardStylePageViewModel
    {
        private const string Locus2 = nameof(SingleEventLeaderboardPageViewModel);
        private const string Locus3 = "[Rezultz.Library02.Mar2024]";

        private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 50;

        #region fields

        private readonly IRepositoryOfResultsForSingleEvent _repositoryOfResultsForSingleEvent;

        #endregion

        #region ctor

        public SingleEventLeaderboardPageViewModel(IAzureStorageSvcAgent azureStorageSvcAgent,
            ISessionState sessionState,
            IThingsPersistedInLocalStorage thingsPersistedInLocalStorage,
            ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationViewModel,
            IRepositoryOfResultsForSingleEvent repositoryOfResultsForSingleEvent
            )
            : base(azureStorageSvcAgent, sessionState, thingsPersistedInLocalStorage, globalSeasonProfileAndIdentityValidationViewModel)
        {
            #region assign ctor IOC injections

            _repositoryOfResultsForSingleEvent = repositoryOfResultsForSingleEvent;

            #endregion

            #region instantiate cbos

            CboLookupMoreInfoItemVm = new IndexDrivenCollectionViewModel<MoreInformationItemDisplayObject>(
                Strings2017.Other_leaderboards,
                CboMoreInfoLookupOnSelectionChangedExecuteAsync,
                CboMoreInfoLookupOnSelectionChangedCanExecute);

            // taken advantage of only in the portal
            //CboLookupBlobNameToPublishResultsVm = new IndexDrivenCollectionViewModel<DatabaseTargetItemViewModel>(
            //    Strings2017.Select_blob,
            //    CboPreProcessedDataLocationForEventLookUpOnSelectionChangedExecute,
            //    CboPreProcessedDataLocationForEventLookUpOnSelectionChangedCanExecute);

            CboLookupMoreInfoItemVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
            //CboLookupBlobNameToPublishResultsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

            #endregion

            #region layout switches

            LeaderboardListRowGroupingEnum = EnumStrings.RowsAreUnGrouped;
            FavoritesListRowGroupingEnum = EnumStrings.RowsAreUnGrouped;

            #endregion

            #region LeaderboardColumnFormatEnum

            DataGridOfFavoritesVm.ColumnFormatEnum = EnumStrings.SingleEventResultsColumnFormat;

            DataGridOfLeaderboardVm.ColumnFormatEnum = EnumStrings.SingleEventResultsColumnFormat;

            LeaderboardColumnFormatEnum = EnumStrings.SingleEventResultsColumnFormat;
            // options are. EnumStrings.SingleEventResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

            #endregion
        }

        #endregion

        #region global props

        private static INavigationServiceForRezultz NavigationServiceForRezultzInstance
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<INavigationServiceForRezultz>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(Strings2017.Unable_to_retrieve_instance,
                            "<INavigationServiceForRezultz>");

                    const string locus = "Property getter of <NavigationServiceForRezultzInstance]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        #endregion

        #region OnPageLoaded method

        // nothing. use Base - which in turn calls LoadSourceDataButtonOnClickAsync

        #endregion

        #region commands

        #region LoadSourceDataButtonOnClickAsync - heap powerful

        /// <summary>
        ///     Throws three exceptions that happen all the time in perfectly ordinary happenstances. The calling
        ///     method must filter these and treat them as benign however it so chooses. If there is no internet
        ///     connection, innermost exception will be a JghCommunicationFailureException.
        ///     If no blob posted yet, or event was cancelled, innermost exception will be a JghResultsData404Exception.
        ///     To convey a harmless informative message, a message that must be deemed a mere footnote
        ///     to a successful outcome, innermost exception will be a JghAlertMessageException.
        ///     All other exceptions are totally unanticipated and should be deemed showstoppers.
        /// </summary>
        /// <returns></returns>
        protected override async Task<string> LoadSourceDataButtonOnClickAsync()
        {
            const string failure = "Unable to complete initialisation of viewmodel for Leaderboard page.";
            const string locus = "[LoadSourceDataButtonOnClickAsync]";

            try
            {
                #region do work

                await ZeroiseAsync();

                await PopulateCboLookUpFileFormatsPresenterForDoingExports(); // arbitrary location for this. ordinarily would do this in ctor but can't because async

                #region null checks

                ThrowExceptionIfNoConnection();

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheSeasonMetadataItem();

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheEventItem();

                #endregion

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);

                var currentEvent = EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem);

                #region populate CboLookUpMoreInfoItemPresenter 

                var kindsOfMoreInfo =
                    currentEvent.EventSettingsItem.MoreInformationItems ?? [];

                kindsOfMoreInfo = kindsOfMoreInfo
                    .Where(z => z is not null)
                    .OrderBy(z => z.DisplayRank)
                    .ThenBy(z => z.Label)
                    .ToArray();

                await CboLookupMoreInfoItemVm.RefillItemsSourceAsync(MoreInformationItemDisplayObject.FromModel(kindsOfMoreInfo));

                CboLookupMoreInfoItemVm.MakeVisibleIfItemsSourceIsAny();

                CboLookupMoreInfoItemVm.IsDropDownOpen = false;
                // on this page the default is false

                CboLookupMoreInfoItemVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

                await CboLookupMoreInfoItemVm.ChangeSelectedIndexToNullAsync();

                CboLookupMoreInfoItemVm.SaveSelectedIndexAsLastKnownGood();

                #endregion

                #region save last known good - 1st kick 

                SaveGenesisOfThisViewModelAsLastKnownGood();

                CboLookupMoreInfoItemVm.SaveSelectedIndexAsLastKnownGood();

                SessionState.CurrentAmbientPageFooterMessage = currentEvent.EventSettingsItem.OneLinerContactMessage ?? string.Empty;

                #endregion

                #region page headings/footer and social media connections 

                HeadersVm.Populate(MakeOrganiserTitle(), MakeSeriesTitle(), MakeEventTitle(), Strings2017.Results__);
                HeadersVm.SaveAsLastKnownGood();

                FootersVm.Populate(currentEvent.EventSettingsItem.OneLinerContactMessage ?? string.Empty);
                FootersVm.SaveAsLastKnownGood();

                SocialMediaConnectionsVm.PopulateConnections(currentEvent.EventSettingsItem.OrganizerEmailAddress,
                    currentEvent.EventSettingsItem.OrganizerFacebookUri, currentEvent.EventSettingsItem.OrganizerInstagramUri,
                    currentEvent.EventSettingsItem.OrganizerTwitterUri);

                #endregion

                #region materialise PageImagesInSkyscraperRightVm 

                PopulateSourceUriStringOfAllUriItems(currentEvent);

                await PageImagesInSkyscraperRightVm.RefillItemsSourceAsync(UriItemDisplayObject.FromModel(currentEvent.EventSettingsItem.UriItems));

                PageImagesInSkyscraperRightVm.MakeVisibleIfItemsSourceIsAny();

                #endregion

                #region PopulateNavigateToPostedLeaderboardHyperlinkButtonVm 

                var targetToPost = new EntityLocationItem();

                var storageLocationForAllPostsInThisSeries = GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.LocationOfDocumentsPosted;

                if (storageLocationForAllPostsInThisSeries is not null)
                {
                    targetToPost.AccountName = storageLocationForAllPostsInThisSeries.DatabaseAccountName;
                    targetToPost.ContainerName = storageLocationForAllPostsInThisSeries.DataContainerName;
                    targetToPost.EntityName = GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem?.HtmlDocumentNameForPostedResults;

                    PopulateNavigateToPostedLeaderboardHyperlinkButtonVm(targetToPost); // LeaderboardColumnFormatEnum is assigned in ctor
                }

                #endregion

                ThisViewModelIsInitialised = true; // Yaaay! //to do - almost certainly delete

                #region THE MEAT load _repositoryOfResultsForSingleEvent

                try
                {
                    var mustPullDataFromStagingLocation = await ThingsPersistedInLocalStorage.GetMustUsePreviewDataOnLaunchAsync();

                    var locationOfData = mustPullDataFromStagingLocation
                        ? currentSeries.ContainerForResultsDataFilesPreviewed
                        : currentSeries.ContainerForResultsDataFilesPublished;

                    await _repositoryOfResultsForSingleEvent.LoadRepositoryOfResultsFailingNoisilyAsync(
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

                #region KEY STEP attach pseudoResults to AllDataGridLineItemDisplayObjects in base class so that all the methods in the base class have something to work with

                AllDataGridLineItemDisplayObjects = ResultItemDisplayObject.FromModel(await _repositoryOfResultsForSingleEvent.GetPlacedAndNonDnsResultsAsync());

                #endregion

                #region instantiate TxxColumnHeadersLookupTable

                var txxColumnHeadersAsDictionary = new Dictionary<int, string>();

                // insert 15 default headers. all column references beyond the first 15 are commented out in all classes
                // because we don't cater for more than 15 at the time of writing. 

                for (var j = 1; j < 16; j++) txxColumnHeadersAsDictionary.Add(j, string.Concat("T", j.ToString()));

                TxxColumnHeadersLookupTable = txxColumnHeadersAsDictionary;

                #endregion

                #region populate SearchFunctionVm itemssources from _repositoryOfResultsForSingleEvent

                var searchQuerySuggestions = ResultItem.ToSearchQuerySuggestionItem(await _repositoryOfResultsForSingleEvent.GetPlacedAndNonDnsResultsAsync());

                SearchFunctionVm.PopulateItemsSource(searchQuerySuggestions);

                //var searchQuerySuggestions = await _repositoryOfResultsForSingleEvent.GetSearchQuerySuggestionsAsync();


                #endregion

                #region populate CboRaceCategoryLookup filter itemsources from _repositoryOfResultsForSingleEvent

                /* In the UWP app, MustSelectAllRacesOnFirstTimeThroughForAnEvent MUST remain FALSE
                 * for the lifetime of the app, and the user must have no access to mess with it. In the
                 * mobile app, it can be TRUE or FALSE and the user can be free to change it back and forth
                 * during each session via user settings. In the user settings service, the implicit
                 * default is FALSE, which is what we keep for the UWP app. In the mobile app, however, we override
                 * the implicit default and change it to TRUE on the landing page upon commencement
                 * of each launch of the app - merely because it is more aesthetically pleasing this way. 
                 */

                await CboLookupCategoryFiltersPopulateItemsSourcesForSingleEventLeaderboardAsync(_repositoryOfResultsForSingleEvent);

                #endregion

                #region evaluate Favorites among AllDataGridLineItemDisplayObjects

                await FavoritesDataGridRefilterAsync();

                #endregion

                #endregion

                var messageOk = await RefreshScreenButtonOnClickAsync(); // drill down

                return messageOk;
            }

            #region try catch handling

            catch (Exception ex)
            {
                //ThisViewModelIsInitialised = JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region CboMoreInfoLookupOnSelectionChangedAsync

        // the page navigation (at time of writing in July 2017, our Navigation system contains an irresolvable frailty
        // when navigating to pages where the Onloaded handler is an async method - which they universally are in the modern Task era)

        private bool CboMoreInfoLookupOnSelectionChangedCanExecute()
        {
            return CboLookupMoreInfoItemVm.IsAuthorisedToOperate;
        }

        private async void CboMoreInfoLookupOnSelectionChangedExecuteAsync()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[CboMoreInfoLookupOnSelectionChangedExecuteAsync]";

            try
            {
                if (!CboMoreInfoLookupOnSelectionChangedCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

                DeadenGui();

                CboMoreInfoLookupOnSelectionChanged();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        private bool CboMoreInfoLookupOnSelectionChanged()
        {
            const string failure = "Unable to fully load dataset associated with specified item.";
            const string locus = "[CboMoreInfoLookupOnSelectionChanged]";

            /*
             * Once upon a time this was an async method on a non-UI thread, and therein lay the problem. Page navigation fails
             * if it is on a random thread. Check out the navigation tests in the TestHarnessUWP for enlightenment. Navigation MUST be done on the UI thread, or 
             * else all kinds of things go awry. So I changed this method to a sync method.
             */

            try
            {
                #region legitimately null checks

                // this could legitimately have been set to index = -1 i.e. GetCurrentItem() returns null

                if (CboLookupMoreInfoItemVm?.SelectedIndex == -1)
                    return true;

                if (CboLookupMoreInfoItemVm?.CurrentItem?.EnumString is null)
                    return true;

                #endregion

                #region do work

                #region belt and braces. always be sure to save leaderboardpage variables before navigating away

                //SessionState.CurrentCboSeriesItemLookupOnLeaderboardPage = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileJsonFilenameValidationVm.CboLookupSeriesVm.CurrentItem);

                //SessionState.CurrentCboEventItemLookupOnLeaderboardPage = EventItemDisplayObject.ObtainSourceModel(SeasonProfileJsonFilenameValidationVm.CboLookupEventVm.CurrentItem);

                //SessionState.CurrentCboMoreInfoItemLookupOnLeaderboardPage = MoreInformationItemDisplayObject.ObtainSourceModel(CboLookupMoreInfoItemVm.CurrentItem);

                #endregion

                switch (CboLookupMoreInfoItemVm.CurrentItem.EnumString)
                {
                    case EnumStringsForSeriesProfile.AverageSplitTimes:
                    {
                        NavigationServiceForRezultzInstance.NavigateToAverageSplitTimesPage(ThingsPersistedInLocalStorage.GetNavigationContextQueryString());
                    }
                        break;

                    case EnumStringsForSeriesProfile.EventPopulationCohorts:
                    {
                        NavigationServiceForRezultzInstance.NavigateToSingleEventPopulationCohortsPage(ThingsPersistedInLocalStorage.GetNavigationContextQueryString());
                    }
                        break;
                    case EnumStringsForSeriesProfile.SeriesStandings:
                    {
                        NavigationServiceForRezultzInstance.NavigateToSeriesStandingsPage(ThingsPersistedInLocalStorage.GetNavigationContextQueryString());
                    }
                        break;

                    case EnumStringsForSeriesProfile.SeriesPopulationCohorts:
                    {
                        NavigationServiceForRezultzInstance.NavigateToSeriesPopulationCohortsPage(ThingsPersistedInLocalStorage.GetNavigationContextQueryString());
                    }
                        break;
                    case null:
                    {
                        // do nothing
                    }
                        break;
                    default:
                    {
                        var culprit = $"The EnumString '{CboLookupMoreInfoItemVm?.CurrentItem?.EnumString}' is not a recognised type of series or event analysis in the relevant section of the data profile for this series/event.";

                        throw new JghInvalidValueException(culprit); //bale
                    }
                }

                #endregion

                // SUCCESS! on we go

                CboLookupMoreInfoItemVm.SelectedIndex = -1;
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

            return true;
        }

        #endregion

        #endregion

        #region helpers

        private bool PopulateSourceUriStringOfAllUriItems(EventProfileItem currentEventProfile)
        {
            if (!JghFilePathValidator.IsValidContainerLocationSpecification(
                    GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DatabaseAccountName,
                    GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DataContainerName,
                    out var _))
                return true;

            if (currentEventProfile?.EventSettingsItem?.UriItems is null) return true;

            foreach (var uriItem in currentEventProfile.EventSettingsItem.UriItems.Where(z => z is not null))
            {
                if (!JghFilePathValidator.IsValidBlobName(uriItem.BlobName, out var _))
                    continue;

                uriItem.SourceUriString = JghFilePathValidator
                    .MakeAzureAndRezultzCompliantBlobUriHttps(
                        GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DatabaseAccountName,
                        GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.LocationOfMedia?.DataContainerName,
                        uriItem.BlobName).ToString();
            }

            return true;
        }

        public async Task<bool> PopulateGuiWithSeriesLookupItemInfoAsync()
        {
            const string failure = "Unable to fully load dataset associated with specified series.";
            const string locus = "[PopulateGuiWithSeriesLookupItemInfoAsync]";

            // when we arrive here we already have an established CboSeriesLookup.ItemsSource and CboLookupSeriesVm.SelectedIndex, but we may or may not have a CboEventLookup

            // this method does nothing but update the GUI. only in the most inconceivable circumstances can it ever throw

            try
            {
                #region do work

                #region null checks

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheSeasonMetadataItem();

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheEventItem();

                #endregion

                #region do page titles in accordance with new series

                HeadersVm.Populate(MakeOrganiserTitle(), MakeSeriesTitle());

                HeadersVm.SaveAsLastKnownGood();

                HeadersVm.IsVisible = true;

                #endregion

                #region do cbo header i.e. Caption

                GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.Label = GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.Label;

                #endregion

                #region save last known good - 1st kick

                GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.SaveSelectedIndexAsLastKnownGood();

                //SessionState.CurrentCboSeriesItemLookupOnLeaderboardPage = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileJsonFilenameValidationVm.CboLookupSeriesVm.CurrentItem);

                #endregion

                #region save hyperlinks to webpages - 1st kick

                //PopulateHyperlinksToWebDocumentsForSeriesAndEvent();

                #endregion

                #region populate CboEventLookup

                if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem is null)
                    return true;

                //await PopulateCboEventLookupAndRelatedInformationForTheFirstTimeAsync();

                var listOfEventItems = GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem?.ArrayOfEventItems ?? [];

                listOfEventItems = listOfEventItems
                    .Where(z => z is not null)
                    .OrderBy(z => z.DisplayRank)
                    .ThenByDescending(z => z.AdvertisedDateTime)
                    .ThenBy(z => z.Label)
                    .ToArray();

                await GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.RefillItemsSourceAsync(listOfEventItems);

                GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.IsDropDownOpen = false;

                var bestGuessChoiceOfEventItemToKickOffWith =
                    JghArrayHelpers.SelectMostRecentItemBeforeDateTimeNowInArrayOfItemsOrFailingThatPickTheEarliest(EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.ItemsSource.ToArray()));

                if (bestGuessChoiceOfEventItemToKickOffWith is null)
                    throw new Jgh404Exception(
                        "No Event particulars found. bestGuessChoiceOfEventItemToKickOffWith is null. Unable to proceed.");

                await GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.ChangeSelectedIndexToMatchItemIdAsync(bestGuessChoiceOfEventItemToKickOffWith.ID);

                GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.SaveSelectedIndexAsLastKnownGood();

                #endregion

                #endregion
            }

            #region try catch

            catch (Exception ex)
            {
                // FAILURE - cleanup and/or corrective action if any
                HeadersVm.RestoreToLastKnownGood();
                FootersVm.RestoreToLastKnownGood();

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

            return true;
        }

        protected override string[] FinaliseTableHeadings()
        {
            const string failure = "Unable to format leaderboard headings.";
            const string locus = "[FinaliseTableHeadings]";

            try
            {
                var answer2 = new[]
                {
                    MakeSeriesLabel(), MakeEventLabel(), Strings2017.Results__, MakeRaceLabel(),
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
                    MakeSeriesLabel(), MakeEventLabel(), Strings2017.Results__,
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
                Title = "Results",
                Blurb = string.Empty
            };


            //var currentCboListOfMoreInfoSelectedItem = LeaderboardColumnFormatEnum switch
            //{
            //    EnumStrings.SingleEventResultsColumnFormat => SessionState.CurrentCboMoreInfoItemLookupOnLeaderboardPage,
            //    EnumStrings.AverageSplitTimesColumnFormat => SessionState.CurrentCboMoreInfoItemLookupOnLeaderboardPage,
            //    EnumStrings.SeriesTotalPointsColumnFormat => SessionState.CurrentCboMoreInfoItemLookupOnSeriesStandingsPage,
            //    EnumStrings.SeriesTourDurationColumnFormat => SessionState.CurrentCboMoreInfoItemLookupOnSeriesStandingsPage,
            //    _ => new MoreInformationItem()
            //};

            return answer;
        }


        #endregion
    }
}