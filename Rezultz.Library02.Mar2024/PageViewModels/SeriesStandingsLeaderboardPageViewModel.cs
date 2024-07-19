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
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.Strings;
using Rezultz.Library02.Mar2024.ValidationViewModels;

// ReSharper disable UnusedMethodReturnValue.Local

namespace Rezultz.Library02.Mar2024.PageViewModels
{
    public class SeriesStandingsLeaderboardPageViewModel : BaseLeaderboardStylePageViewModel
    {
        private const string Locus2 = nameof(SeriesStandingsLeaderboardPageViewModel);
        private const string Locus3 = "[Rezultz.Library02.Mar2024]";


        private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 50;

        #region ctor

        public SeriesStandingsLeaderboardPageViewModel(IAzureStorageSvcAgent azureStorageSvcAgent,
            ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
            ISessionState sessionState,
            IThingsPersistedInLocalStorage thingsPersistedInLocalStorage,
            ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationViewModel
            )
            : base(azureStorageSvcAgent, sessionState, thingsPersistedInLocalStorage, globalSeasonProfileAndIdentityValidationViewModel)
        {
            #region assign ctor IOC injections

            _repositoryOfSeriesStandings = new RepositoryOfSeriesStandings(leaderboardResultsSvcAgent);

            #endregion

            #region instantiate cbos

            CboLookupMoreInfoItemVm = new IndexDrivenCollectionViewModel<MoreInformationItemDisplayObject>(
                Strings2017.Other_standings,
                CboListOfMoreInfoOnSelectionChangedExecuteAsync,
                CboListOfMoreInfoOnSelectionChangedCanExecute);

            CboLookupMoreInfoItemVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

            #endregion

            #region layout switches

            LeaderboardListRowGroupingEnum = EnumStrings.RowsAreUnGrouped;
            FavoritesListRowGroupingEnum = EnumStrings.RowsAreUnGrouped;

            #endregion

            #region datagrid and Html column format enum

            // in SingleEventLeaderboardPageViewModel and SingleEventAverageSplitTimesPageViewModel we do this here in the ctor

            // but it's more complicated in SeriesStandingsLeaderboardPageViewModel. standings-format dependent. do it in: -

            // InstallSeriesPointsStandingsForAllProcessedIndividualResults and InstallSeriesTourDurationStandingsForAllProcessedIndividualResults

            #endregion
        }

        #endregion

        #region OnPageLoaded method

        // nothing. use Base

        #endregion

        #region fields

        private MoreInformationItem _lastKnownGoodMoreInfoItem;

        private readonly IRepositoryOfSeriesStandings _repositoryOfSeriesStandings;

        #endregion

        #region Buttons

        //public ButtonControlViewModel PostSeriesPointsStandingsAsHtmlToPublicDataStorageButtonVm { get; }

        //public ButtonControlViewModel PostSeriesTourStandingsAsHtmlToPublicDataStorageButtonVm { get; }

        #endregion

        #region commands

        #region LoadSourceDataButtonOnClickAsync - heap powerful

        protected override async Task<string> LoadSourceDataButtonOnClickAsync()
        {
            var failure = Strings2017.Unable_to_complete_computations_and_calculations_to_load_page_;
            const string locus = "[LoadSourceDataButtonOnClickAsync]";

            try
            {
                #region do work

                #region begin

                await ZeroiseAsync();

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheSeasonMetadataItem();

                await PopulateCboLookUpFileFormatsPresenterForDoingExports(); // arbitrary location for this. ordinarily would do this in ctor but can't because async

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

                var currentEvent = EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem);

                #endregion

                #region load CboListOfMoreInfo a.k.a sundry kinds of season standings

                var twoKindsOfSeasonStandings = currentSeries.MoreSeriesInformationItems ?? [];

                twoKindsOfSeasonStandings = twoKindsOfSeasonStandings
                    .Where(z => z is not null)
                    .OrderBy(z => z.DisplayRank)
                    .ThenBy(z => z.Label)
                    .ToArray();

                await CboLookupMoreInfoItemVm.RefillItemsSourceAsync(MoreInformationItemDisplayObject.FromModel(twoKindsOfSeasonStandings));

                CboLookupMoreInfoItemVm.MakeVisibleIfItemsSourceIsGreaterThanOne();

                CboLookupMoreInfoItemVm.IsDropDownOpen = false;
                // on this page, if we want the drop down initialised to visible, set this to true

                CboLookupMoreInfoItemVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

                await CboLookupMoreInfoItemVm.ChangeSelectedIndexAsync(0); // default - first item on the list

                CboLookupMoreInfoItemVm.SaveSelectedIndexAsLastKnownGood();

                #endregion

                #region bale if CboListOfMoreInfo is empty or unselected

                if (CboLookupMoreInfoItemVm.ItemsSource is null)
                {
                    await ZeroiseAsync();

                    throw new JghNullObjectInstanceException(nameof(CboLookupMoreInfoItemVm.ItemsSource));
                }

                if (!CboLookupMoreInfoItemVm.ItemsSource.Any())
                {
                    await ZeroiseAsync();

                    throw new JghAlertMessageException(Strings2017.More_information_not_selected);
                }

                #endregion

                #region page headings/footer and social media connections

                // todo - make MakeMoreInfoTitle
                HeadersVm.Populate(MakeOrganiserTitle(), MakeSeriesTitle(), MakeMoreInfoTitle());
                HeadersVm.SaveAsLastKnownGood();

                HeadersVm.IsVisible = true;

                FootersVm.Populate( SessionState.CurrentAmbientPageFooterMessage);
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

                #region load dummy CboListOfSeries

                await GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.RefillItemsSourceAsync(SeriesItemDisplayObject.FromModel([currentSeries]));
                await GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.ChangeSelectedIndexAsync(0); // default - only item on the list
                // dummy required for the DisplayLeaderboardDataGridPresentation method in LeaderboardStylePageViewModelBase

                #endregion

                #region load dummy CboListOfEvent

                //await GlobalSeasonProfileJsonFilenameValidationVm.CboLookupEventVm.RefillItemsSourceAsync(Array.Empty<EventItemDisplayObject>());
                //await GlobalSeasonProfileJsonFilenameValidationVm.CboLookupSeriesVm.ChangeSelectedIndexAsync(0); // default - only item on the list
                // dummy required for the DisplayLeaderboardDataGridPresentation method in LeaderboardStylePageViewModelBase

                #endregion

                #region HEAP POWERFUL. LOAD REPOSITORY OF SERIES TOTALS

                try
                {
                    var mustPullDataFromStagingLocation = await ThingsPersistedInLocalStorage.GetMustUsePreviewDataOnLaunchAsync();

                    var locationOfData = mustPullDataFromStagingLocation
                        ? GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfResultsForPreview
                        : GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfResultsPublished;

                    await _repositoryOfSeriesStandings.LoadRepositoryOfSequenceContainersAsync(
                        locationOfData.DatabaseAccountName,
                        locationOfData.DataContainerName,
                        currentSeries);
                    // Loads the repository of series totals failing SILENTLY.be sure to verify internet connection and service availability beforehand
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

                #region populate ArrayOfTxxColumnHeaders for series

                TxxColumnHeadersLookupTable = await _repositoryOfSeriesStandings.GetTxxColumnHeadersAsync() ??
                                              new Dictionary<int, string>();

                #endregion

                #region save genesis as last known good

                CboLookupMoreInfoItemVm.SaveSelectedIndexAsLastKnownGood();

                SaveGenesisOfThisViewModelAsLastKnownGood();

                HeadersVm.SaveAsLastKnownGood();

                FootersVm.SaveAsLastKnownGood();

                #endregion

                #endregion

                var messageOk = await CboListOfMoreInfoOnSelectionChangedAsync(); // install either points or tour standings depending on CboLookupMoreInfoItemVm?.CurrentItem?.EnumString

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

        #endregion

        #region CboListOfMoreInfoOnSelectionChangedAsync - relatively mickey mouse

        protected virtual bool CboListOfMoreInfoOnSelectionChangedCanExecute()
        {
            return CboLookupMoreInfoItemVm.IsAuthorisedToOperate;
        }

        private async void CboListOfMoreInfoOnSelectionChangedExecuteAsync()
        {
            const string failure = "Unable to complete ICommand Execute action.";
            const string locus = "[CboListOfMoreInfoOnSelectionChangedExecuteAsync]";

            try
            {
                if (!CboListOfMoreInfoOnSelectionChangedCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____processing);

                DeadenGui();

                await CboListOfMoreInfoOnSelectionChangedAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                if (!DataGridOfLeaderboardVm.ItemsSource.Any())
                    await AlertMessageService.ShowOkAsync("No results.");
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                    Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        public async Task<string> CboListOfMoreInfoOnSelectionChangedAsync()
        {
            const string failure = "Unable to execute selection changed command.";
            const string locus = "[CboListOfMoreInfoOnSelectionChangedAsync]";

            try
            {
                #region do work

                #region null checks - throw if any of them fail

                ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheSeasonMetadataItem();

                CboLookUpCategoryFilterThrowExceptionIfCboMoreInfoItemLookupVmIsEmpty();

                #endregion

                #region Key step. Assign AllDataGridLineItemDisplayObjects

                await ThrowExceptionIfRepositoryOfSeriesTotalsNotFound(); // bale

                if (CboLookupMoreInfoItemVm is null)
                    throw new Exception($"The {nameof(CboLookupMoreInfoItemVm)} object is null.");

                if (CboLookupMoreInfoItemVm.CurrentItem is null)
                    throw new JghInvalidValueException($"The {nameof(CboLookupMoreInfoItemVm)} GetCurrentItem() method returned null.");

                switch (CboLookupMoreInfoItemVm?.CurrentItem?.EnumString)
                {
                    case EnumStringsForSeriesProfile.SeriesTotalPointsStandings:
                    {
                        await InstallSeriesPointsStandingsForAllProcessedIndividualResults();
                    }
                        break;

                    case EnumStringsForSeriesProfile.SeriesTourDurationStandings:
                    {
                        await InstallSeriesTourDurationStandingsForAllProcessedIndividualResults();
                    }
                        break;


                    default:
                    {
                        var culprit =
                            $"The string '{CboLookupMoreInfoItemVm?.CurrentItem?.EnumString}' is not a valid Enum name for a known type of Series Standings. First of all, check the relevant item your Settings data file.";

                        throw new JghInvalidValueException(culprit); //bale
                    }
                }

                #endregion

                ThisViewModelIsInitialised = true;

                #region save  genesis of current more info item

                SaveCboMoreInfoItemLookupCurrentItemAsLastKnownGood();

                #endregion

                #region save session state

                SessionState
                    .CurrentCboMoreInfoItemLookupOnSeriesStandingsPage = MoreInformationItemDisplayObject.ObtainSourceModel(CboLookupMoreInfoItemVm.CurrentItem);

                #endregion

                #region evaluate Favorites among AllDataGridLineItemDisplayObjects

                await FavoritesDataGridRefilterAsync();

                #endregion

                #endregion

                var messageOk = await ApplyFiltersButtonOnClickAsync();

                return messageOk;
            }

            #region try catch

            catch (Exception ex)
            {
                ThisViewModelIsInitialised = JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
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


        private async Task<bool> ThrowExceptionIfRepositoryOfSeriesTotalsNotFound()
        {
            if (_repositoryOfSeriesStandings is null)
            {
                await ZeroiseDataGridVmsAndDesignersAsync();

                throw new JghResultsData404Exception(Strings2017.Results_not_found);
            }

            if (!await _repositoryOfSeriesStandings.GetIsInitialised())
            {
                await ZeroiseDataGridVmsAndDesignersAsync();

                throw new JghResultsData404Exception(Strings2017.Results_not_found);
            }

            return true;
        }

        private string MakeMoreInfoTitle()
        {
            // todo

            return string.Empty;
            //return
            //    SessionState.CurrentCboMoreInfoItemLookupOnLeaderboardPage?
            //        .Title ?? string.Empty;
        }

        protected override string[] FinaliseTableHeadings()
        {
            const string failure = "Unable to format leaderboard headings.";
            const string locus = "[PopulateTableHeadings]";

            try
            {
                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

                #region supporting narrative on the underpinnings of the standings table

                var currentMoreInfoItem = CboLookupMoreInfoItemVm.CurrentItem ?? new MoreInformationItemDisplayObject();

                var supportingNarrative = currentMoreInfoItem.EnumString switch
                {
                    EnumStringsForSeriesProfile.SeriesTourDurationStandings => string.Format(currentMoreInfoItem.Blurb),
                    EnumStringsForSeriesProfile.SeriesTotalPointsStandings => string.Format(currentMoreInfoItem.Blurb,
                        currentSeries.NumOfScoresToCountForSeriesTotalPoints),
                    _ => string.Empty
                };

                #endregion

                var answer2 = new[]
                {
                    MakeSeriesLabel(), MakeMoreInfoLabel(), MakeRaceLabel(), MakeGenderLabel(), MakeCategoryLabel(),
                    MakeCityLabel(), $"({supportingNarrative})"
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
                #region supporting narrative on the underpinnings of the standings table

                var currentMoreInfoItem = CboLookupMoreInfoItemVm.CurrentItem ?? new MoreInformationItemDisplayObject();

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

                var supportingNarrative = currentMoreInfoItem.EnumString switch
                {
                    EnumStringsForSeriesProfile.SeriesTourDurationStandings => string.Format(currentMoreInfoItem.Blurb),
                    EnumStringsForSeriesProfile.SeriesTotalPointsStandings => string.Format(currentMoreInfoItem.Blurb, currentSeries.NumOfScoresToCountForSeriesTotalPoints),
                    _ => string.Empty
                };

                #endregion

                var answer2 = new[]
                {
                    MakeSeriesLabel(), MakeMoreInfoLabel(), Strings2017.Header_Favorites_List,
                    $"({supportingNarrative})"
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
                Title = "Series standings",
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

        private async Task<bool> InstallSeriesPointsStandingsForAllProcessedIndividualResults()
        {
            #region LeaderboardColumnFormatEnum

            DataGridOfFavoritesVm.ColumnFormatEnum = EnumStrings.SeriesTotalPointsColumnFormat;
            DataGridOfLeaderboardVm.ColumnFormatEnum = EnumStrings.SeriesTotalPointsColumnFormat;
            LeaderboardColumnFormatEnum = EnumStrings.SeriesTotalPointsColumnFormat;
            // options are. EnumStrings.OrdinaryResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

            #endregion

            #region populate AllDataGridLineItemDisplayObjects

            var pointsTable = (await _repositoryOfSeriesStandings.GetPointsStandingsAsync())
                .OrderByDescending(z => z.SequenceTotal).ToArray();

            var numberOfEventsEligibleForSeries = await _repositoryOfSeriesStandings.GetNumberOfEventsEligibleForPointsAsync();


            var standingsTable = ConvertSeriesPointsTableToDisplayFormat(pointsTable, numberOfEventsEligibleForSeries);

            AllDataGridLineItemDisplayObjects = standingsTable ?? throw new JghAlertMessageException(Strings2017.Table_of_points_standings_not_found);
            //AllDataGridLineItemDisplayObjects = ResultItemViewModel.ObtainSourceItem(standingsTable) ?? throw new JghAlertMessageException(Strings2017.Table_of_points_standings_not_found);
            // KEY STEP attach standings table as AllDataGridLineItemDisplayObjects to base class so that all the methods in the base class have something to work with

            #endregion


            #region populate SearchFunctionVm itemssources

            var searchSuggestions = await _repositoryOfSeriesStandings.GetPointsSearchSuggestionsAsync();

            SearchFunctionVm.PopulateItemsSource(searchSuggestions);

            SearchFunctionVm.MakeVisibleIfThereAreThingsToBeSearched();

            #endregion

            #region PopulateCboRaceCategoryLookupsForSeriesTotalPoints

            /* In the UWP app, MustSelectAllRacesOnFirstTimeThroughForAnEvent MUST remain FALSE
             * for the lifetime of the app, and the user must have no access to mess with it. In the
             * mobile app, it can be TRUE or FALSE and the user can be free to change it back and forth
             * during each session via user settings. In the user settings service, the implicit
             * default is FALSE, which is what we keep for the UWP app. In the mobile app, however, we override
             * the implicit default and change it to TRUE on the landing page upon commencement
             * of each launch of the app - merely because it is more aesthetically pleasing this way. 
             */

            await PopulateCboRaceCategoryLookupsForSeriesTotalPointsAsync(_repositoryOfSeriesStandings, await ThingsPersistedInLocalStorage.GetMustSelectAllRacesOnFirstTimeThroughForAnEventAsync());

            //foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms())
            //{
            //    lookupVm.IsVisible = true; // because we added a (spurious) labeled item to the top of the list signifying 'all' 
            //}

            #endregion

            #region reset indexes of CbolistOfRace and CategorisationOfResultsVm to match specified bib number

            //if (await ThingsPersistedInLocalStorage.GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync())
            //    await CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(
            //        SearchFunctionVm,
            //        await ThingsPersistedInLocalStorage
            //            .GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync());

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

            #region PopulateNavigateToPostedLeaderboardHyperlinkButtonVm

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

            var targetToPost = new EntityLocationItem();

            var storageLocationForAllPostsInThisSeries = currentSeries.ContainerForDocumentsPosted;

            targetToPost.AccountName = storageLocationForAllPostsInThisSeries.AccountName;
            targetToPost.ContainerName = storageLocationForAllPostsInThisSeries.ContainerName;
            targetToPost.EntityName = currentSeries.HtmlDocumentNameForSeriesTotalPointsStandings;

            PopulateNavigateToPostedLeaderboardHyperlinkButtonVm(targetToPost); // 2nd and final kick

            #endregion

            #region save last known good

            HeadersVm.SaveAsLastKnownGood();

            FootersVm.SaveAsLastKnownGood();

            CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood();

            #endregion

            return true;
        }

        private async Task<bool> InstallSeriesTourDurationStandingsForAllProcessedIndividualResults()
        {
            #region LeaderboardColumnFormatEnum

            DataGridOfFavoritesVm.ColumnFormatEnum = EnumStrings.SeriesTourDurationColumnFormat;
            DataGridOfLeaderboardVm.ColumnFormatEnum = EnumStrings.SeriesTourDurationColumnFormat;
            LeaderboardColumnFormatEnum = EnumStrings.SeriesTourDurationColumnFormat;
            // options are. EnumStrings.OrdinaryResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

            #endregion

            #region populate AllDataGridLineItemDisplayObjects

            var tourTable = (await _repositoryOfSeriesStandings.GetTourStandingsAsync()).OrderBy(z => z.SequenceTotal)
                .ToArray();

            var standingsTable = ConvertSeriesTourDurationTableToDisplayFormat(tourTable);

            AllDataGridLineItemDisplayObjects = standingsTable ?? throw new Exception(Strings2017.Table_of_tour_duration_standings_not_found);
            // KEY STEP attach standings table as AllDataGridLineItemDisplayObjects to base class so that all the methods in the base class have something to work with

            #endregion

            #region populate SearchFunctionVm itemssources

            SearchFunctionVm.PopulateItemsSource(await _repositoryOfSeriesStandings.GetTourSearchSuggestionsAsync());

            SearchFunctionVm.MakeVisibleIfThereAreThingsToBeSearched();

            #endregion

            #region populate MultiFactoralFilter itemssources

            /* In the UWP app, MustSelectAllRacesOnFirstTimeThroughForAnEvent MUST remain FALSE
             * for the lifetime of the app, and the user must have no access to mess with it. In the
             * mobile app, it can be TRUE or FALSE and the user can be free to change it back and forth
             * during each session via user settings. In the user settings service, the implicit
             * default is FALSE, which is what we keep for the UWP app. In the mobile app, however, we override
             * the implicit default and change it to TRUE on the landing page upon commencement
             * of each launch of the app - merely because it is more aesthetically pleasing this way. 
             */

            await PopulateCboRaceCategoryLookupsForSeriesTourDurationAsync(_repositoryOfSeriesStandings, await ThingsPersistedInLocalStorage.GetMustSelectAllRacesOnFirstTimeThroughForAnEventAsync());

            foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms()) lookupVm.IsVisible = true; // because we added a (spurious) labelled item to the top of the list signifying 'all' 

            #endregion

            #region reset indexes of CbolistOfRace and CategorisationOfResultsVm to match specified bib number

            //if (await ThingsPersistedInLocalStorage.GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync())
            //    await CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(SearchFunctionVm, await ThingsPersistedInLocalStorage.GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync());

            var mustFocusOnCategoryOfSinglePerson = await ThingsPersistedInLocalStorage.GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync();

            if (mustFocusOnCategoryOfSinglePerson)
            {
                var idOfPerson = await ThingsPersistedInLocalStorage.GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync();

                if (PersonWithTargetBibNumberIsSuccessfullyIdentifiedInPopulation(idOfPerson, out var personIdentified)) await CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(personIdentified);
            }

            #endregion

            #region PopulateNavigateToPostedLeaderboardHyperlinkButtonVm

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

            var targetToPost = new EntityLocationItem();

            var storageLocationForAllPostsInThisSeries = currentSeries.ContainerForDocumentsPosted;

            targetToPost.AccountName = storageLocationForAllPostsInThisSeries.AccountName;
            targetToPost.ContainerName = storageLocationForAllPostsInThisSeries.ContainerName;
            targetToPost.EntityName = currentSeries.HtmlDocumentNameForSeriesTourDurationStandings;

            PopulateNavigateToPostedLeaderboardHyperlinkButtonVm(targetToPost);

            #endregion

            #region save last known good

            HeadersVm.SaveAsLastKnownGood();

            FootersVm.SaveAsLastKnownGood();

            CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood();

            #endregion

            return true;
        }

        private async Task<bool> PopulateCboRaceCategoryLookupsForSeriesTotalPointsAsync(IRepositoryOfSeriesStandings repository, bool mustShowAllRacesOnFirstTimeThru)
        {
            /*  Upon instantiation, the selected index of all presenters initialise to zero by default, which means 'All selections'.
             * This is what we want for all the presenters except one. CboLookupRaceCategoryFilterVm is slightly different. Where results
             * are displayed on a web page, it is groovy for for 'Races - all' to be shown at the outset. Where results are
             * shown in a datagrid, it is senseless to show 'Races -all'. In a grid, each Race must be shown singly and so the index
             * must be manually initialised to '1' i.e. the first Race on the list. The mobile app falls into the first category.
             * The UWP app falls into the second. To specify which of the two behaviours we want, I have created a user setting,
             * MustSelectAllRacesOnFirstTimeThroughForAnEventAsync(), which is essentially platform/app specific, and that's
             * what gets pulled into this method as the variable 'mustShowAllRacesOnFirstTimeThru'. In the mobile
             * app, 'mustShowAllRacesOnFirstTimeThru' is true. in the UWP app it is false.
             */

            var races = AddLabelForSelectingAll(StringsRezultz02.Races___all, await repository.GetRacesFoundInPointsAsync());
            var genders = AddLabelForSelectingAll(StringsRezultz02.Genders___all, await repository.GetGendersFoundInPointsAsync());
            var ageGroups = AddLabelForSelectingAll(StringsRezultz02.AgeGroups___all, await repository.GetAgeGroupsFoundInPointsAsync());
            var cities = AddLabelForSelectingAll(StringsRezultz02.Cities___all, await repository.GetCitiesFoundInPointsAsync());
            var teams = AddLabelForSelectingAll(StringsRezultz02.Teams___all, await repository.GetTeamsFoundInPointsAsync());
            var utilityClassifications = AddLabelForSelectingAll(StringsRezultz02.Other___all, await repository.GetUtilityClassificationsFoundInPointsAsync());

            await CboLookupRaceCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(races));
            await CboLookupGenderCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(genders));
            await CboLookupAgeGroupCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(ageGroups));
            await CboLookupCityCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(cities));
            await CboLookupTeamCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(teams));
            await CboLookupUtilityClassificationCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(utilityClassifications));

            foreach (var lookupCollectionVm in CboLookUpCategoryFiltersMakeListOfAllVms())
                await lookupCollectionVm.ChangeSelectedIndexAsync(0); // i.e. display all

            if (CboLookupRaceCategoryFilterVm.ItemsSource.Length <= 1)
                return true; // only one race. nothing to fuss about. exit.

            if (!mustShowAllRacesOnFirstTimeThru)
                await CboLookupRaceCategoryFilterVm.ChangeSelectedIndexAsync(1);

            return true;
        }

        private async Task<bool> PopulateCboRaceCategoryLookupsForSeriesTourDurationAsync(IRepositoryOfSeriesStandings repository, bool mustShowAllRacesOnFirstTimeThru)
        {
            // populate the Lookup Cbos

            var races = AddLabelForSelectingAll(StringsRezultz02.Races___all, await repository.GetRacesFoundInTourAsync());
            var genders = AddLabelForSelectingAll(StringsRezultz02.Genders___all, await repository.GetGendersFoundInTourAsync());
            var ageGroups = AddLabelForSelectingAll(StringsRezultz02.AgeGroups___all, await repository.GetAgeGroupsFoundInTourAsync());
            var cities = AddLabelForSelectingAll(StringsRezultz02.Cities___all, await repository.GetCitiesFoundInTourAsync());
            var teams = AddLabelForSelectingAll(StringsRezultz02.Teams___all, await repository.GetTeamsFoundInTourAsync());
            var utilityClassifications = AddLabelForSelectingAll(StringsRezultz02.Other___all, await repository.GetUtilityClassificationsFoundInTourAsync());

            await CboLookupRaceCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(races));
            await CboLookupGenderCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(genders));
            await CboLookupAgeGroupCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(ageGroups));
            await CboLookupCityCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(cities));
            await CboLookupTeamCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(teams));
            await CboLookupUtilityClassificationCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(utilityClassifications));

            // initialise them

            foreach (var lookupCollectionVm in CboLookUpCategoryFiltersMakeListOfAllVms())
            {
                lookupCollectionVm.MakeVisibleIfItemsSourceIsGreaterThanTwo(); // because we added a (spurious) labelled item to the top of the list signifying 'all' 
                lookupCollectionVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
                await lookupCollectionVm.ChangeSelectedIndexAsync(0); // i.e. display all
            }

            /*  Upon instantiation, the selected index of all presenters initialise to zero by default, which means 'All selections'.
             * This is what we want for all the presenters except one. CboLookupRaceCategoryFilterVm is slightly different. Where results
             * are displayed on a web page, it is groovy for for 'Races - all' to be shown at the outset. Where results are
             * shown in a datagrid, it is senseless to show 'Races -all'. In a grid, each Race must be shown singly and so the index
             * must be manually initialised to '1' i.e. the first Race on the list. The mobile app falls into the first category.
             * The UWP app falls into the second. To specify which of the two behaviours we want, I have created a user setting,
             * MustSelectAllRacesOnFirstTimeThroughForAnEventAsync(), which is essentially platform/app specific, and that's
             * what gets pulled into this method as the variable 'mustShowAllRacesOnFirstTimeThru'. In the mobile
             * app, 'mustShowAllRacesOnFirstTimeThru' is true. in the UWP app it is false.
             */

            if (CboLookupRaceCategoryFilterVm.ItemsSource.Length <= 1)
                return true; // only one race. nothing to fuss about. exit.

            if (!mustShowAllRacesOnFirstTimeThru)
                await CboLookupRaceCategoryFilterVm.ChangeSelectedIndexAsync(1);

            return true;
        }

        public static ResultItemDisplayObject[] ConvertSeriesPointsTableToDisplayFormat(SequenceContainerItem[] theTable, int scoresToCountForPoints)
        {
            var answer = ResultItemDisplayObject.FromItemToSeriesPointsFormat(theTable, scoresToCountForPoints);

            return answer;
        }

        public static ResultItemDisplayObject[] ConvertSeriesTourDurationTableToDisplayFormat(SequenceContainerItem[] theTable)
        {
            var answer = ResultItemDisplayObject.FromItemToSeriesTourDurationFormat(theTable);

            return answer;
        }

        #endregion

        #region Gui stuff

        #endregion

        #region GenesisAsLastKnownGood

        //protected override void SaveGenesisOfThisViewModelAsLastKnownGood()
        //{
        //    _lastKnownGoodSeriesToWhichThisVmBelongs = SessionState.CurrentCboSeriesItemLookupOnLeaderboardPage;

        //}

        //public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
        //{
        //    if (_lastKnownGoodSeriesToWhichThisVmBelongs != SessionState.CurrentCboSeriesItemLookupOnLeaderboardPage)
        //        return true;

        //    return false;
        //}

        private void SaveCboMoreInfoItemLookupCurrentItemAsLastKnownGood()
        {
            _lastKnownGoodMoreInfoItem = MoreInformationItemDisplayObject.ObtainSourceModel(CboLookupMoreInfoItemVm.CurrentItem);
        }

        // ReSharper disable once UnusedMember.Local
        private bool LastKnownGoodCboMoreInfoItemLookupCurrentItemHasChanged()
        {
            return _lastKnownGoodMoreInfoItem != MoreInformationItemDisplayObject.ObtainSourceModel(CboLookupMoreInfoItemVm.CurrentItem);
        }

        #endregion
    }
}