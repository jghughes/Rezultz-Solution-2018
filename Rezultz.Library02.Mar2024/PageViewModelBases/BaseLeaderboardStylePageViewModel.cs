using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.RezultzDisplayObjects;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.DataGridDesigners;
using Rezultz.Library02.Mar2024.DataGridInterfaces;
using Rezultz.Library02.Mar2024.DataGridViewmodels;
using Rezultz.Library02.Mar2024.Strings;
using Rezultz.Library02.Mar2024.ValidationViewModels;

namespace Rezultz.Library02.Mar2024.PageViewModelBases
{
    public abstract class BaseLeaderboardStylePageViewModel : BaseViewViewModel, ISearchService
    {
        private const string Locus2 = nameof(BaseLeaderboardStylePageViewModel);
        private const string Locus3 = "[Rezultz.Library02.Mar2024]";

        private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 50;

        #region ctor

        protected BaseLeaderboardStylePageViewModel(IAzureStorageSvcAgent azureStorageSvcAgent,
            ISessionState sessionState,
            IThingsPersistedInLocalStorage thingsPersistedInLocalStorage,
            ISeasonProfileAndIdentityValidationViewModel globalSeasonProfileAndIdentityValidationViewModel
        )
    {
        #region assign ctor IOC injections

        SessionState = sessionState;

        GlobalSeasonProfileAndIdentityValidationVm = globalSeasonProfileAndIdentityValidationViewModel;

        GlobalSeasonProfileAndIdentityValidationVm.CurrentRequiredWorkRole = EnumStringsForTimingSystemWorkRoles.Publishing;

        ThingsPersistedInLocalStorage = thingsPersistedInLocalStorage;

        _azureStorageSvcAgent = azureStorageSvcAgent;

        #endregion

        #region instantiate ButtonVms

        LoadSourceDataButtonVm = new ButtonControlViewModel(LoadSourceDataButtonOnClickExecuteAsync, LoadSourceDataButtonOnClickCanExecute);

        RefreshScreenButtonVm = new ButtonControlViewModel(RefreshScreenButtonOnClickExecuteAsync, RefreshScreenButtonOnClickCanExecute);

        DisplayPodiumResultsOnlyToggleButtonVm = new ButtonControlViewModel(() => { }, () => false)
            ;

        AddPersonToFavoritesButtonVm = new ButtonControlViewModel(AddPersonToFavoritesButtonOnClickExecuteAsync, AddPersonToFavoritesButtonOnClickCanExecute)
            ;

        DeletePersonFromFavoritesButtonVm = new ButtonControlViewModel(DeletePersonFromFavoritesButtonOnClickExecuteAsync, DeletePersonFromFavoritesButtonOnClickCanExecute);

        DeleteAllFavoritesButtonVm = new ButtonControlViewModel(DeleteAllFavoritesButtonOnClickExecuteAsync, DeleteAllFavoritesButtonOnClickCanExecute);

        ToggleFavoritesGridVisibilityButtonVm = new ButtonControlViewModel(ToggleFavoritesGridVisibilityButtonOnClickExecute, ToggleFavoritesGridVisibilityButtonOnClickCanExecute);

        ExportFavoritesButtonVm = new ButtonControlViewModel(() => { }, () => false); // no commands. click event in code behind

        ExportSingleEventLeaderboardButtonVm = new ButtonControlViewModel(() => { }, () => false); // no commands. click event in code behind

        PostLeaderboardAsHtmlToPublicDataStorageButtonVm = new ButtonControlViewModel(PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickExecuteAsync, PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickCanExecute);

        #endregion

        #region instantiate Cbos

        CboLookupRaceCategoryFilterVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(Strings2017.Race__, () => { }, () => true);
        CboLookupGenderCategoryFilterVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(Strings2017.Gender_, () => { }, () => true);
        CboLookupAgeGroupCategoryFilterVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(Strings2017.AgeGroup_, () => { }, () => true);
        CboLookupCityCategoryFilterVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(Strings2017.City_, () => { }, () => true);
        CboLookupTeamCategoryFilterVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(Strings2017.Team__, () => { }, () => true);
        CboLookupUtilityClassificationCategoryFilterVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(Strings2017.Other__, () => { }, () => true);

        CboLookupRaceCategoryFilterVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
        CboLookupGenderCategoryFilterVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
        CboLookupAgeGroupCategoryFilterVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
        CboLookupCityCategoryFilterVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
        CboLookupTeamCategoryFilterVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
        CboLookupUtilityClassificationCategoryFilterVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

        CboLookUpFileFormatsVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>("", () => { }, () => true);
        CboLookUpFileFormatsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

        // Note: I empirically proved in Aug 2022 that even on my super-fast machine a 20ms discombobulates the binding engine during debug. 
        // So, I choose an arbitrary safety margin of 50ms to accomodate slower machines. This is 4x faster than the default 200ms setting.
        // Noticeably faster. This will need to be tested in real life on the machines that organisers actually use.
        // Potentially very slow tablets. Test it by seeing what happens to the Calculate changed category button.
        // If it fails to light properly when enabled, increase the safety margin. even if it looks OK, you are not out of the woods.
        // You may still get mysterious and subtle inconsistencies between selections as they appear on the GUI and as they actually are in the
        // associated vm. These can be very difficult to debug.

        #endregion

        #region instantiate AllDataGridLineItemDisplayObjects

        AllDataGridLineItemDisplayObjects = [];

        #endregion

        #region instantiate DataGridVms

        DataGridOfLeaderboardVm = new LeaderboardStyleDataGridViewModel(string.Empty, LeaderboardDataGridPresenterOnSelectionChangedExecute, LeaderboardDataGridPresenterOnSelectionChangedCanExecute)
        {
            PresentationServiceInstanceEnum = EnumStrings.LeaderboardList
        };

        DataGridOfFavoritesVm = new LeaderboardStyleDataGridViewModel(string.Empty, FavoritesDataGridPresenterOnSelectionChangedExecute, FavoritesDataGridPresenterOnSelectionChangedCanExecute)
        {
            PresentationServiceInstanceEnum = EnumStrings.FavoritesList
        };

        #endregion
    }

        #endregion

        #region methods called on arrival to page to which this vm is the data context each time by page-loaded event

        public async Task<string> BeInitialisedFromPageCodeBehindOrchestrateAsync()
    {
        var failure = Strings2017.Unable_to_complete_computations_and_calculations_to_load_page_;
        const string locus = nameof(BeInitialisedFromPageCodeBehindOrchestrateAsync);

        try
        {
            if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                return string.Empty;

            DeadenGui();

            var messageOk = await LoadSourceDataButtonOnClickAsync();

            EnlivenGui();

            SaveGenesisOfThisViewModelAsLastKnownGood();

            ThisViewModelIsInitialised = true;

            return messageOk;
        }

        #region try catch handling

        catch (Exception ex)
        {
            var isHarmless = JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex)
                             || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghResultsData404Exception>(ex);

            ThisViewModelIsInitialised = isHarmless;

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region type

        public class RaceCategorySelectorFilterMask : IHasRaceCategoryFilterMaskProperties

        {
            public string RaceGroup { get; set; }
            public string Gender { get; set; }
            public string AgeGroup { get; set; }
            public string City { get; set; }
            public string Team { get; set; }
            public string UtilityClassification { get; set; }
        }

        #endregion

        #region constants

        private static readonly Uri DefaultUri = new("http://undefined");

        private const string DefaultTargetName = @"_blank";

        #endregion

        #region fields

        protected bool MustDisplayFavoritesDatagrid;

        private static IFavoritesDataGridPresentationService FavoritesDataGridPresentationServiceInstance
        {
            get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IFavoritesDataGridPresentationService>();
            }
            catch (Exception ex)
            {
                var msg =
                    JghString.ConcatAsSentences(Strings2017.Unable_to_retrieve_instance,
                        "<IFavoritesDataGridPresentationService>");

                const string locus = "Property getter of <FavoritesDataGridPresentationServiceInstance]";
                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
        }

        private static ILeaderboardDataGridPresentationService LeaderboardDataGridPresentationServiceInstance
        {
            get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<ILeaderboardDataGridPresentationService>();
            }
            catch (Exception ex)
            {
                var msg =
                    JghString.ConcatAsSentences(Strings2017.Unable_to_retrieve_instance,
                        "<ILeaderboardDataGridPresentationService>");

                const string locus = "Property getter of <LeaderboardDataGridPresentationServiceInstance]";
                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
        }

        #endregion

        #region global props

        protected readonly ISessionState SessionState;

        protected readonly ISeasonProfileAndIdentityValidationViewModel GlobalSeasonProfileAndIdentityValidationVm;

        protected static IAlertMessageService AlertMessageService
        {
            get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IAlertMessageService>();
            }
            catch (Exception ex)
            {
                var msg =
                    JghString.ConcatAsSentences(Strings2017.Unable_to_retrieve_instance,
                        "[IAlertMessageService]");

                const string locus = "Property getter of [AlertMessageServiceInstance]";
                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
        }

        protected readonly IThingsPersistedInLocalStorage ThingsPersistedInLocalStorage;

        protected static IProgressIndicatorViewModel GlobalProgressIndicatorVm
        {
            get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IProgressIndicatorViewModel>();
            }
            catch (Exception ex)
            {
                var msg = JghString.ConcatAsSentences(StringsForXamlPages.UnableToRetrieveInstance, $"[{nameof(IProgressIndicatorViewModel)}]");

                const string locus = StringsForXamlPages.PropertyGetterOf + $"[{nameof(GlobalProgressIndicatorVm)}]";

                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
        }

        private readonly IAzureStorageSvcAgent _azureStorageSvcAgent;

        #endregion

        #region props

        #region Miscellaneous

        public bool ThisViewModelIsInitialised { get; protected set; }

        public HeaderOrFooterViewModel HeadersVm { get; } = new();

        public HeaderOrFooterViewModel FootersVm { get; } = new();

        protected ResultItemDisplayObject[] AllDataGridLineItemDisplayObjects { get; set; }

        #endregion

        #region formatting enums

        // set in derived class ctor for flexibilty for the future, but invariably EnumStrings.RowsAreUnGrouped
        protected string LeaderboardListRowGroupingEnum { get; set; }

        // set in derived class ctor for flexibilty for the future, but invariably EnumStrings.RowsAreUnGrouped
        protected string FavoritesListRowGroupingEnum { private get; set; }

        // set in derived class ctor
        public string LeaderboardColumnFormatEnum { get; set; }
        // set somewhere in derived class. EnumStrings.OrdinaryResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

        #endregion

        #region Buttons

        public ButtonControlViewModel LoadSourceDataButtonVm { get; }

        public ButtonControlViewModel RefreshScreenButtonVm { get; }

        public ButtonControlViewModel DisplayPodiumResultsOnlyToggleButtonVm { get; }

        public ButtonControlViewModel AddPersonToFavoritesButtonVm { get; }

        public ButtonControlViewModel DeletePersonFromFavoritesButtonVm { get; }

        public ButtonControlViewModel DeleteAllFavoritesButtonVm { get; }

        public ButtonControlViewModel ToggleFavoritesGridVisibilityButtonVm { get; }

        public ButtonControlViewModel ExportFavoritesButtonVm { get; }

        public ButtonControlViewModel ExportSingleEventLeaderboardButtonVm { get; }

        public ButtonControlViewModel PostLeaderboardAsHtmlToPublicDataStorageButtonVm { get; }

        public ButtonWithHyperlinkControlViewModel NavigateToPostedLeaderboardHyperlinkButtonVm { get; } = new(() => { }, () => false) {NavigateUri = DefaultUri, TargetName = DefaultTargetName};

        #endregion

        #region Cbos

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupRaceCategoryFilterVm { get; }
        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupGenderCategoryFilterVm { get; }
        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupAgeGroupCategoryFilterVm { get; }
        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupCityCategoryFilterVm { get; }
        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupTeamCategoryFilterVm { get; }
        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupUtilityClassificationCategoryFilterVm { get; }

        public IndexDrivenCollectionViewModel<MoreInformationItemDisplayObject> CboLookupMoreInfoItemVm { get; protected set; }
        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookUpFileFormatsVm { get; protected set; }

        //public IndexDrivenCollectionViewModel<DatabaseTargetItemViewModel> CboLookupBlobNameToPublishResultsVm { get; protected set; }

        #endregion

        #region SearchFunction

        public SearchViewModel SearchFunctionVm { get; } = new("search", 2, 9, null, null);

        #endregion

        #region DataGridVms

        public LeaderboardStyleDataGridViewModel DataGridOfFavoritesVm { get; }

        public LeaderboardStyleDataGridViewModel DataGridOfLeaderboardVm { get; }

        #endregion

        #region DataGridDesigners

        public DataGridDesigner DataGridDesignerForFavorites { get; } = new();

        public DataGridDesigner DataGridDesignerForLeaderboard { get; } = new();

        #endregion

        #region TxxColumnHeadersLookupTable

        private Dictionary<int, string> _backingstoreTxxColumnHeadersLookupTable = new();

        public Dictionary<int, string> TxxColumnHeadersLookupTable
        {
            get => _backingstoreTxxColumnHeadersLookupTable ??= new Dictionary<int, string>();
            set => SetProperty(ref _backingstoreTxxColumnHeadersLookupTable, value);
        }

        #endregion

        #region ImageUriVms

        public IndexDrivenCollectionViewModel<UriItemDisplayObject> PageImagesInSkyscraperRightVm { get; } = new("Skyscraper right image album", () => { }, () => true);

        #endregion

        #region SocialMediaConnectionsVm

        public SocialMediaConnectionsViewModel SocialMediaConnectionsVm { get; } = new();

        #endregion

        #endregion

        #region commands

        #region LoadSourceDataButtonOnClickAsync - heap powerful

        private bool LoadSourceDataButtonOnClickCanExecute()
    {
        return LoadSourceDataButtonVm.IsAuthorisedToOperate;
    }

        private async void LoadSourceDataButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[LoadSourceDataButtonOnClickExecuteAsync]";

        try
        {
            if (!LoadSourceDataButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

            DeadenGui();

            var messageOk = await LoadSourceDataButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex)
                || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<Jgh404Exception>(ex)
                || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghCommunicationFailureException>(ex))
            {
                EvaluateGui();
            }
            else
            {
                LoadSourceDataButtonVm.IsAuthorisedToOperate = true;

                EvaluateVisibilityOfAllGuiControlsThatTouchData(false);
            }

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

        protected abstract Task<string> LoadSourceDataButtonOnClickAsync();

        #endregion

        #region RefreshScreenButtonOnClickAsync - heap powerful

        private bool RefreshScreenButtonOnClickCanExecute()
    {
        return RefreshScreenButtonVm.IsAuthorisedToOperate;
    }

        private async void RefreshScreenButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[RefreshScreenButtonOnClickExecuteAsync]";

        try
        {
            GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

            var messageOk = await RefreshScreenButtonOnClickOrchestrateAsync();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
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

        public async Task<string> RefreshScreenButtonOnClickOrchestrateAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[RefreshScreenButtonOnClickOrchestrateAsync]";

        try
        {
            if (!RefreshScreenButtonOnClickCanExecute())
                return string.Empty;

            DeadenGui();

            var outcome = await RefreshScreenButtonOnClickAsync();

            EnlivenGui();

            return outcome;
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                EvaluateGui();

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        protected async Task<string> RefreshScreenButtonOnClickAsync()
    {
        // end of the line in the drill down sequence from series to event to race to filter results by selected category

        const string failure = "Unable to filter and display or redisplay list of results fully.";
        const string locus = "[RefreshScreenButtonOnClickAsync]";

        try
        {
            #region do work

            #region null checks

            if (AllDataGridLineItemDisplayObjects == null)
                throw new JghNullObjectInstanceException(nameof(AllDataGridLineItemDisplayObjects));

            #endregion

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem);

            var currentEvent = EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem);

            #region save last known good filter settings

            CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood();

            #endregion

            #region update all filter captions

            CboLookUpCategoryFiltersUpdateCaptionsOfAllVms();

            #endregion

            #region update page headers - 4th and final kick

            HeadersVm.Populate(FinaliseTableHeadings());

            HeadersVm.SaveAsLastKnownGood();

            #endregion

            #region filter AllDataGridLineItemDisplayObjects to obtain DataGridOfLeaderboardVm itemssource

            var allFilteredResultDisplayObjects =
                CboLookUpCategoryFiltersSelectResultsInAccordanceWithCboSelections(AllDataGridLineItemDisplayObjects);

            #endregion

            #region populate DataGridDesigner to obtain array of column specification items for RadDataGrid control in a PresentationService

            DataGridDesignerForLeaderboard.InitialiseDesigner(currentSeries, currentEvent, LeaderboardColumnFormatEnum, allFilteredResultDisplayObjects, PopulateDataGridTitleAndBlurb(), TxxColumnHeadersLookupTable);

            var columnSpecificationItemsForDisplayObjects = await ThingsPersistedInLocalStorage.GetMustDisplayConciseLeaderboardColumnsOnlyAsync()
                ? DataGridDesignerForLeaderboard.GetNonEmptyColumnSpecificationItemsForConciseLeaderboardOfResultItemDisplayObjects()
                : DataGridDesignerForLeaderboard.GetNonEmptyColumnSpecificationItemsForLeaderboardOfResultItemDisplayObjects();

            #endregion

            #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

            await LeaderboardDataGridPresentationServiceInstance
                .GenerateDataGridColumnCollectionManuallyAsync(columnSpecificationItemsForDisplayObjects); // essential if using a datagrid, empty method if uploading a pretty-printed html document to be viewed in a browser 

            #endregion

            #region create a Presenter to provide a datacontext and hence row collection and datacontext for the PresentationService

            var genders = CboLookupItemDisplayObject.ObtainSourceModel(CboLookupGenderCategoryFilterVm?.ItemsSource);

            await DataGridOfLeaderboardVm.PopulatePresenterAsync(FinaliseTableHeadings(), currentSeries, currentEvent,
                genders, TxxColumnHeadersLookupTable, LeaderboardListRowGroupingEnum, LeaderboardColumnFormatEnum, EnumStrings.LeaderboardList, allFilteredResultDisplayObjects);

            #endregion

            #endregion

            #region messageOk

            string messageOk;

            if (GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem == null)
            {
                messageOk = StringsRezultz02.SeasonDataNotInitialised;

                return messageOk;
            }

            if (!AllDataGridLineItemDisplayObjects.Any())
            {
                messageOk = "No results in this category.  No results overall in this event.";

                return messageOk;
            }

            if (AllDataGridLineItemDisplayObjects.Length == 1)
            {
                if (allFilteredResultDisplayObjects.Length == 1)
                {
                    messageOk = "1 result in this category. 1 result overall in this event.";

                    return messageOk;
                }

                messageOk = allFilteredResultDisplayObjects.Any() ? $"{allFilteredResultDisplayObjects.Length} results in this category. 1 result overall in this event." : "No results in this category. 1 result overall in this event.";

                return messageOk;
            }

            messageOk = allFilteredResultDisplayObjects.Any()
                ? $"{allFilteredResultDisplayObjects.Length} results in this category. {AllDataGridLineItemDisplayObjects.Length} results overall in this event."
                : $"No results in this category. {AllDataGridLineItemDisplayObjects.Length} results overall in this event.";

            return messageOk;

            #endregion
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region LeaderboardDataGridPresenterOnSelectionChanged - mickey mouse

        protected bool LeaderboardDataGridPresenterOnSelectionChangedCanExecute()
    {
        return true;
    }

        private void LeaderboardDataGridPresenterOnSelectionChangedExecute()
    {
        EvaluateGui();
    }

        #endregion

        #region FavoritesDatagridPresenterOnSelectionChanged - mickey mouse

        protected bool FavoritesDataGridPresenterOnSelectionChangedCanExecute()
    {
        return true;
    }

        private void FavoritesDataGridPresenterOnSelectionChangedExecute()
    {
        EvaluateGui();
    }

        #endregion

        #region AddPersonToFavoritesButtonOnClickAsync

        private bool AddPersonToFavoritesButtonOnClickCanExecute()
    {
        return AddPersonToFavoritesButtonVm.IsAuthorisedToOperate;
    }

        private async void AddPersonToFavoritesButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[AddPersonToFavoritesButtonOnClickExecuteAsync]";

        try
        {
            if (!AddPersonToFavoritesButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

            DeadenGui();

            await AddPersonToFavoritesButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();
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

        private async Task AddPersonToFavoritesButtonOnClickAsync()
    {
        const string failure = "Unable to add person to favorites.";
        const string locus = "[AddPersonToFavoritesButtonOnClickAsync]";

        try
        {
            #region do work

            if (DataGridOfLeaderboardVm?.SelectedItem == null) return;

            await AddPersonToFavoritesAsync(DataGridOfLeaderboardVm?.SelectedItem);

            await FavoritesDataGridRefilterAsync();

            MustDisplayFavoritesDatagrid = true;

            if (DataGridOfLeaderboardVm != null) await DataGridOfLeaderboardVm.ChangeSelectedItemToNullAsync();

            #endregion
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region DeletePersonFromFavoritesButtonOnClickAsync

        private bool DeletePersonFromFavoritesButtonOnClickCanExecute()
    {
        return DeletePersonFromFavoritesButtonVm.IsAuthorisedToOperate;
    }

        private async void DeletePersonFromFavoritesButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[DeletePersonFromFavoritesButtonOnClickExecuteAsync]";

        try
        {
            if (!DeletePersonFromFavoritesButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

            DeadenGui();

            await DeletePersonFromFavoritesButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();
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

        private async Task DeletePersonFromFavoritesButtonOnClickAsync()
    {
        const string failure = "Unable to delete.";
        const string locus = "[DeletePersonFromFavoritesButtonOnClickAsync]";

        try
        {
            #region do work

            if (DataGridOfFavoritesVm?.SelectedItem == null) return;

            var favoritesToBeUpdated =
                await ThingsPersistedInLocalStorage.GetFavoritesListIdentitiesAsync() ?? [];

            var pseudoItem = DataGridOfFavoritesVm?.SelectedItem;

            favoritesToBeUpdated = ThingWithNamesItem.RemoveItemByName(pseudoItem, favoritesToBeUpdated);

            await ThingsPersistedInLocalStorage.SaveFavoritesListIdentitiesAsync(favoritesToBeUpdated);

            await FavoritesDataGridRefilterAsync();

            MustDisplayFavoritesDatagrid = true;

            if (DataGridOfFavoritesVm != null) await DataGridOfFavoritesVm.ChangeSelectedItemToNullAsync();

            #endregion
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region DeleteAllFavoritesButtonOnClickAsync

        private bool DeleteAllFavoritesButtonOnClickCanExecute()
    {
        return DeleteAllFavoritesButtonVm.IsAuthorisedToOperate;
    }

        private async void DeleteAllFavoritesButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[DeleteAllFavoritesButtonOnClickExecuteAsync]";

        try
        {
            if (!DeleteAllFavoritesButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2017.Working_____looking_for_information);

            DeadenGui();


            await DeleteAllFavoritesButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();
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

        private async Task DeleteAllFavoritesButtonOnClickAsync()
    {
        const string failure = "Unable to delete all favorites.";
        const string locus = "[DeleteAllFavoritesButtonOnClickAsync]";

        try
        {
            #region Dowork

            if (DataGridOfFavoritesVm?.ItemsSource == null) return;

            await ThingsPersistedInLocalStorage.SaveFavoritesListIdentitiesAsync([]);

            await FavoritesDataGridRefilterAsync();

            #endregion
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region ToggleFavoritesGridVisibilityButtonOnClickAsync - mickey mouse

        private bool ToggleFavoritesGridVisibilityButtonOnClickCanExecute()
    {
        return ToggleFavoritesGridVisibilityButtonVm.IsAuthorisedToOperate;
    }

        private void ToggleFavoritesGridVisibilityButtonOnClickExecute()
    {
        MustDisplayFavoritesDatagrid = !MustDisplayFavoritesDatagrid;

        EvaluateGui();
    }

        #endregion

        #region PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickAsync

        protected virtual bool PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickCanExecute()
    {
        var outComeOfCanExecuteTest =
            PostLeaderboardAsHtmlToPublicDataStorageButtonVm.IsAuthorisedToOperate;

        return outComeOfCanExecuteTest;
    }

        private async void PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[PostSingleEventLeaderboardAsHtmlToPublicDataStorageButtonOnClickExecute]";

        try
        {
            if (!PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(Strings2016.Working______uploading);

            DeadenGui();

            var messageOk = await PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                EvaluateGui(); // a JghAlertMessageException is deemed a successful outcome

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        protected virtual async Task<string> PostDisplayedLeaderboardAsHtmlToPublicDataStorageButtonOnClickAsync()
    {
        const string failure = "Unable to do what this method does ....";
        const string locus = "[PostSingleEventLeaderboardAsHtmlToPublicDataStorageButtonOnClickAsync]";

        var targetToPost = new EntityLocationItem();

        try
        {
            #region preflight checks

            if (GlobalSeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem == null)
                throw new JghUserAuthenticationMessageException("Only administrators can publish/post web pages. They must authenticate themselves in Preferences.");

            if (!GlobalSeasonProfileAndIdentityValidationVm.GetIfCurrentlyAuthenticatedIdentityUserIsAuthorisedForRequiredWorkRole())
                throw new JghUserAuthenticationMessageException($"{Strings2017.Sorry_not_authorised_for_workrole}");

            #endregion

            #region load "all races" into DataGridOfLeaderboardVm

            await CboLookupRaceCategoryFilterVm.ChangeSelectedIndexAsync(0);
            // Races are unique: we commence with the index = 1 for the "first" item on the list, rather index = 0 meaning "all" , so we need to reset it here to 0

            await RefreshScreenButtonOnClickAsync();

            #endregion

            #region upload hardcopy of leaderboard

            var (blobName, htmlToUpload) = await TabulateResultsAsHtmlWebpageAsync(DataGridOfLeaderboardVm);

            var messageOk = string.Empty;

            if (GlobalSeasonProfileAndIdentityValidationVm?.CboLookupSeriesVm?.CurrentItem?.LocationOfDocumentsPosted != null)
            {
                var storageLocationForAllPostsInThisSeries = GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfDocumentsPosted;

                targetToPost.AccountName = storageLocationForAllPostsInThisSeries.DatabaseAccountName;
                targetToPost.ContainerName = storageLocationForAllPostsInThisSeries.DataContainerName;
                targetToPost.EntityName = blobName;

                messageOk = await UploadWorkToAzureAsync(targetToPost, htmlToUpload);
            }
            //PopulateNavigateToPostedLeaderboardHyperlinkButtonVm(targetToPost, DataGridOfLeaderboardVm.ColumnFormatEnum); // 2nd and final kick
            // set somewhere in derived class. EnumStrings.OrdinaryResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

            #endregion

            return messageOk;
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #endregion

        #region CboLookUpCategoryFilters

        protected async Task<bool> CboLookupCategoryFiltersPopulateItemsSourcesForSingleEventLeaderboardAsync(IHasSingleEventCategories repository)
    {
        var races = AddLabelForSelectingAll(StringsRezultz02.Races___all, await repository.GetRacesFoundAsync());
        var genders = AddLabelForSelectingAll(StringsRezultz02.Genders___all, await repository.GetGendersFoundAsync());
        var ageGroups = AddLabelForSelectingAll(StringsRezultz02.AgeGroups___all, await repository.GetAgeGroupsFoundAsync());
        var cities = AddLabelForSelectingAll(StringsRezultz02.Cities___all, await repository.GetCitiesFoundAsync());
        var teams = AddLabelForSelectingAll(StringsRezultz02.Teams___all, await repository.GetTeamsFoundAsync());
        var utilityClassifications = AddLabelForSelectingAll(StringsRezultz02.Other___all, await repository.GetUtilityClassificationsFoundAsync());

        await CboLookupRaceCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(races));
        await CboLookupGenderCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(genders));
        await CboLookupAgeGroupCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(ageGroups));
        await CboLookupCityCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(cities));
        await CboLookupTeamCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(teams));
        await CboLookupUtilityClassificationCategoryFilterVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromLabel(utilityClassifications));

        /*  Note. the default is to initialise the selected index of all lookups to zero, which means Display all.
          This is what we want for all the lookups except one. CboLookupRaceCategoryFilterVm is slightly different. Where results
          are displayed on a web page, it is groovy for for 'Races - all' to be shown at the outset. Where results are
          shown in a datagrid, it is senseless to show 'Races - all'. In a grid, each Race must be shown singly and so the index
          must be manually initialised to '1' i.e. the first Race on the list. The mobile app falls into the first category.
          The UWP app falls into the second. To specify which of the two behaviours we want, I have created a user setting,
          MustSelectAllRacesOnFirstTimeThroughForAnEventAsync(), which is essentially platform/app specific, and that's
          what gets pulled into this method as the variable 'mustShowAllRacesOnFirstTimeThru'. In the mobile
          app, 'mustShowAllRacesOnFirstTimeThru' is true. in the UWP app it is false. The intent is to save this setting in App.Settings
          and assign it in the code-behind of the landing page
         */

        var mustFocusOnCategoryOfSinglePerson = await ThingsPersistedInLocalStorage.GetMustSelectCategoryOfResultsForSingleParticipantIdOnLaunchAsync();

        if (mustFocusOnCategoryOfSinglePerson)
        {
            var idOfPerson = await ThingsPersistedInLocalStorage.GetTargetParticipantIdForSelectedCategoryOfResultsOnLaunchAsync();

            if (PersonWithTargetBibNumberIsSuccessfullyIdentifiedInPopulation(idOfPerson, out var personIdentified))
            {
                await CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(personIdentified);

                CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood();

                return true;
            }
        }

        var mustShowAllRacesOnFirstTimeThru = await ThingsPersistedInLocalStorage.GetMustSelectAllRacesOnFirstTimeThroughForAnEventAsync();

        if (mustShowAllRacesOnFirstTimeThru || CboLookupRaceCategoryFilterVm.ItemsSource.Length <= 1)
            await CboLookupRaceCategoryFilterVm.ChangeSelectedIndexAsync(0);
        else
            await CboLookupRaceCategoryFilterVm.ChangeSelectedIndexAsync(1);

        await CboLookupGenderCategoryFilterVm.ChangeSelectedIndexAsync(0);
        await CboLookupAgeGroupCategoryFilterVm.ChangeSelectedIndexAsync(0);
        await CboLookupCityCategoryFilterVm.ChangeSelectedIndexAsync(0);
        await CboLookupTeamCategoryFilterVm.ChangeSelectedIndexAsync(0);
        await CboLookupUtilityClassificationCategoryFilterVm.ChangeSelectedIndexAsync(0);

        CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood();

        return true;
    }

        // Note: this here implementation does the job for the leaderboard for a single event and for tour winners. 
        private ResultItemDisplayObject[] CboLookUpCategoryFiltersSelectResultsInAccordanceWithCboSelections(ResultItemDisplayObject[] population)
    {
        RaceCategorySelectorFilterMask CboLookUpCategoryFiltersObtainFilterMask()
        {
            var multiIdFilterMask = new RaceCategorySelectorFilterMask
            {
                RaceGroup = CboLookupRaceCategoryFilterVm.CurrentItem?.Label ?? string.Empty,
                Gender = CboLookupGenderCategoryFilterVm.CurrentItem?.Label ?? string.Empty,
                AgeGroup = CboLookupAgeGroupCategoryFilterVm.CurrentItem?.Label ?? string.Empty,
                City = CboLookupCityCategoryFilterVm.CurrentItem?.Label ?? string.Empty,
                Team = CboLookupTeamCategoryFilterVm.CurrentItem?.Label ?? string.Empty,
                UtilityClassification = CboLookupUtilityClassificationCategoryFilterVm.CurrentItem?.Label ?? string.Empty
            };
            return multiIdFilterMask;
        }

        if (population == null)
            throw new JghNullObjectInstanceException(nameof(population));

        if (!population.Any()) return [];

        var filterMask = CboLookUpCategoryFiltersObtainFilterMask();

        //ResultItemDisplayObject[] filteredResults = AlgorithmForFilteringResults.FilterPopulationUsingFilterCriteriaMask(population, filterMask);


        IEnumerable<ResultItemDisplayObject> populationUndergoingFiltering = population.Where(z => z != null).ToList();

        if (CboLookupRaceCategoryFilterVm.CurrentItem.Label != StringsRezultz02.Races___all)
            populationUndergoingFiltering = populationUndergoingFiltering.Where(z => z.RaceGroup == filterMask.RaceGroup);

        if (CboLookupGenderCategoryFilterVm.CurrentItem.Label != StringsRezultz02.Genders___all)
            populationUndergoingFiltering = populationUndergoingFiltering.Where(z => z.Gender == filterMask.Gender);

        if (CboLookupAgeGroupCategoryFilterVm.CurrentItem.Label != StringsRezultz02.AgeGroups___all)
            populationUndergoingFiltering = populationUndergoingFiltering.Where(z => z.AgeGroup == filterMask.AgeGroup);

        if (CboLookupTeamCategoryFilterVm.CurrentItem.Label != StringsRezultz02.Teams___all)
            populationUndergoingFiltering = populationUndergoingFiltering.Where(z => z.Team == filterMask.Team);

        if (CboLookupCityCategoryFilterVm.CurrentItem.Label != StringsRezultz02.Cities___all)
            populationUndergoingFiltering = populationUndergoingFiltering.Where(z => z.City == filterMask.City);

        if (CboLookupUtilityClassificationCategoryFilterVm.CurrentItem.Label != StringsRezultz02.Other___all)
            populationUndergoingFiltering = populationUndergoingFiltering.Where(z => z.UtilityClassification == filterMask.UtilityClassification);

        var provisionalAnswer = populationUndergoingFiltering.ToArray();

        if (DisplayPodiumResultsOnlyToggleButtonVm.IsChecked)
        {
            List<ResultItemDisplayObject> podiumOnly = [];

            foreach (var raceCboLookupItemVm in CboLookupRaceCategoryFilterVm.ItemsSource)
            foreach (var ageGroupCboLookupItemVm in CboLookupAgeGroupCategoryFilterVm.ItemsSource)
            foreach (var genderCboLookupItemVm in CboLookupGenderCategoryFilterVm.ItemsSource)
            {
                // filtering out FractionalPlaceInRaceInNumeratorOverDenominatorFormat is our crude way of eliminating DNFs
                var partOfTheAnswer = provisionalAnswer
                    .Where(z => !string.IsNullOrWhiteSpace(z.FractionalPlaceInRaceInNumeratorOverDenominatorFormat))
                    .Where(z => z.RaceGroup == raceCboLookupItemVm.Label)
                    .Where(z => z.AgeGroup == ageGroupCboLookupItemVm.Label)
                    .Where(z => z.Gender == genderCboLookupItemVm.Label)
                    .OrderBy(z => z.PlaceCalculatedOverallAsString)
                    .Take(3)
                    .ToList();

                podiumOnly.AddRange(partOfTheAnswer);
            }

            provisionalAnswer = podiumOnly.ToArray();
        }

        // todo - in here do the placings overall. we are going to ditch subplacings

        return provisionalAnswer;
    }


        public bool PersonWithTargetBibNumberIsSuccessfullyIdentifiedInPopulation(string targetBibNumber, out ResultItemDisplayObject personWithBibNumberIdentified)
    {
        personWithBibNumberIdentified = AllDataGridLineItemDisplayObjects
            .Where(z => z != null)
            .Where(z => JghString.AreEqualIgnoreOrdinalCase(z.Bib, targetBibNumber))
            .Select(z => z)
            .FirstOrDefault();

        return personWithBibNumberIdentified != null;
    }

        protected virtual async Task<bool> CboLookUpCategoryFiltersChangeToMatchPersonWithTargetBibNumberAsync(ResultItemDisplayObject personWithBibNumberSpecifiedForOpenOnLaunch)
    {
        if (personWithBibNumberSpecifiedForOpenOnLaunch == null)
            return false;

        await CboLookupRaceCategoryFilterVm.ChangeSelectedIndexToMatchItemLabelAsync(personWithBibNumberSpecifiedForOpenOnLaunch.RaceGroup);

        await CboLookupGenderCategoryFilterVm.ChangeSelectedIndexToMatchItemLabelAsync(personWithBibNumberSpecifiedForOpenOnLaunch.Gender);

        await CboLookupAgeGroupCategoryFilterVm.ChangeSelectedIndexToMatchItemLabelAsync(personWithBibNumberSpecifiedForOpenOnLaunch.AgeGroup);

        await CboLookupTeamCategoryFilterVm.ChangeSelectedIndexAsync(0); // i.e. all

        await CboLookupCityCategoryFilterVm.ChangeSelectedIndexAsync(0); // i.e. all 

        await CboLookupUtilityClassificationCategoryFilterVm.ChangeSelectedIndexAsync(0); // i.e. all

        return true;
    }

        public static string[] AddLabelForSelectingAll(string labelForSelectingAll, string[] lookupTable)
    {
        if (lookupTable == null)
            return [];

        // sort table

        var amendedTable = lookupTable.Where(z => z != null)
            .OrderByDescending(z => z)
            .ToList();

        // add label for selecting all

        labelForSelectingAll ??= StringsRezultz02.Select___all; // fallback

        //amendedTable.Reverse();

        amendedTable.Add(labelForSelectingAll);

        amendedTable.Reverse();

        // move blank to the bottom if any

        if (amendedTable.Contains(string.Empty))
        {
            amendedTable = amendedTable.Where(z => z != string.Empty).ToList();
            amendedTable.Add(string.Empty);
        }

        return amendedTable.ToArray();
    }

        public void CboLookUpCategoryFiltersUpdateCaptionsOfAllVms()
    {
        foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms())
            if (lookupVm?.CurrentItem != null)
                lookupVm.Label = lookupVm.CurrentItem.Label;
    }

        public void CboLookUpCategoryFiltersSaveAllSelectionsAsLastKnownGood()
    {
        foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms())
            lookupVm.SaveSelectedIndexAsLastKnownGood();
    }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>[] CboLookUpCategoryFiltersMakeListOfAllVms()
    {
        // this array is merely a lazy labor saving device that we use many times

        var answer = new[]
        {
            CboLookupRaceCategoryFilterVm,
            CboLookupGenderCategoryFilterVm,
            CboLookupAgeGroupCategoryFilterVm,
            CboLookupCityCategoryFilterVm,
            CboLookupTeamCategoryFilterVm,
            CboLookupUtilityClassificationCategoryFilterVm
        };

        return answer.ToArray();
    }

        #endregion

        #region CboLookupMoreInfoItem

        protected void CboLookUpCategoryFilterThrowExceptionIfCboMoreInfoItemLookupVmIsEmpty()
    {
        if (CboLookupMoreInfoItemVm == null)
            throw new JghNullObjectInstanceException(nameof(CboLookupMoreInfoItemVm));

        if (CboLookupMoreInfoItemVm.ItemsSource == null)
            throw new JghNullObjectInstanceException(nameof(CboLookupMoreInfoItemVm.ItemsSource));
    }

        protected abstract MoreInformationItem PopulateDataGridTitleAndBlurb();

        #endregion

        #region season data

        protected void ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheSeasonMetadataItem()
    {
        if (!GlobalSeasonProfileAndIdentityValidationVm.ThisViewModelIsInitialised)
            throw new JghAlertMessageException("Season profile not initialised.  Please submit a valid profile ID.");

        if (GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem == null)
            throw new JghAlertMessageException("Season profile not initialised.  Please submit a valid profile ID.");

        if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm == null)
            throw new JghAlertMessageException(
                $"Season profile successfully loaded using profile ID = {GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem.FragmentInFileNameOfAssociatedProfileFile}.  Warning : there are no constituent series identified in the season data. The list is null. This is a system error. Sorry.");

        if (!GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.ItemsSource.Any())
            throw new JghAlertMessageException(
                $"Season profile successfully loaded using profile ID = {GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem.FragmentInFileNameOfAssociatedProfileFile}.  Warning : there are no constituent series available via this ID. Either the list of series in the corresponding profile is empty, or the particulars regarding the listed series are not found on the server.  Either of these possibilities might or might not be intentional on the part of the author of the season profile.");


        if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem == null)
            throw new JghAlertMessageException(
                $"Season profile successfully loaded using profile ID = {GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem.FragmentInFileNameOfAssociatedProfileFile}.  A series has not yet been selected.  A series needs to be chosen.  Please select a series.");
    }

        protected void ThrowExceptionIfThereAreAnyFundamentalDeficienciesInTheEventItem()
    {
        if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm == null)
            throw new JghAlertMessageException($"Season profile successfully loaded for profile ID = {GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem.FragmentInFileNameOfAssociatedProfileFile} but the list of events is null. This is a system error. Sorry");


        if (!GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.ItemsSource.Any())
            throw new JghAlertMessageException(
                $"Season profile successfully loaded for ID={GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem.FragmentInFileNameOfAssociatedProfileFile}. The number of series listed is {GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.ItemsSource.Length}. The series chosen for analysis is {GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.Label}. There are no events listed for this series. The data profile might be incomplete.");

        if (GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem == null)
            throw new JghAlertMessageException(
                $"Season profile successfully loaded for ID={GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem.FragmentInFileNameOfAssociatedProfileFile}. The series selected for analysis is {GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.Label}. The number of events listed for it is {GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.ArrayOfEventItems.Length}. An event has not yet been selected by the user. An event must be selected in order to proceed.");
    }

        #endregion

        #region Search

        public virtual async Task<string[]> OnShortlistOfQuerySuggestionsRequestedFromTheSearchUniverseAsync(string queryText)
    {
        return await SearchFunctionVm.GetQueriesThatSatisfyUserEnteredHint(queryText);
    }

        public virtual async Task<bool> OnFinalSearchQuerySubmittedAsTextAsync(string finalQuerySubmitted)
    {
        const string failure = "Unable to complete search operation.";
        const string locus = "[OnFinalSearchQuerySubmittedAsTextAsync]";

        try
        {
            #region do work

            var searchResults =
                SearchFunctionVm.GetSubsetOfSearchQueryItemsThatEquateToSelectedSearchQuery(
                    finalQuerySubmitted);

            await OrchestrateActionsToBeTakenWhenSearchOutcomeIsToHandAsync(
                searchResults.Where(z => z != null).ToArray());

            #endregion
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion

        return true;
    }

        /// <summary>
        ///     Action taken as the final step of DoSearchOperationButtonOnClickAsync.
        ///     Since this LeaderboardStylePageViewModelBase is the base class
        ///     for classes that have access to the universe of all individual results and
        ///     a favorites list in storage in which search findings are saved, this implementation takes the
        ///     first of the discovered result items, adds it to the list of favorites and displays
        ///     the new list of favorites in FavoritesDataGridPresentation.
        /// </summary>
        /// <param name="discoveredQueryItems">The discovered result items.</param>
        /// <returns>true</returns>
        public async Task OrchestrateActionsToBeTakenWhenSearchOutcomeIsToHandAsync(SearchQueryItem[] discoveredQueryItems)
    {
        const string failure = "Unable to take action in response to selection of item.";
        const string locus = "[OrchestrateActionsToBeTakenWhenSearchOutcomeIsToHandAsync]";


        try
        {
            #region null checks

            if (DataGridOfFavoritesVm == null) return;

            if (discoveredQueryItems == null)
                throw new JghNullObjectInstanceException(nameof(discoveredQueryItems));

            if (!discoveredQueryItems.Any() || discoveredQueryItems.FirstOrDefault() == null)
                throw new JghAlertMessageException(Strings2017.No_items_found_in_this_subset);

            #endregion

            var discoveredTag = discoveredQueryItems.FirstOrDefault()?.TagAsString;

            var discovery = AllDataGridLineItemDisplayObjects.FirstOrDefault(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(z.Guid, discoveredTag));

            await AddPersonToFavoritesAsync(discovery);

            await FavoritesDataGridRefilterAsync();

            MustDisplayFavoritesDatagrid = true;

            EvaluateGui();


            #region old code

            //var prequels = 1; // arbitrary number of prequels to show in the search results on the grid

            //const string nothingFound = "No scroll. To scroll automatically, expand the onscreen population, refilter rows, and search again. Do whatever is necessary to bring the population containing the search target to the surface.";

            #region null checks

            //if (DataGridOfLeaderboardVm == null) return;

            //if (discoveredQueryItems == null)
            //    throw new JghNullObjectInstanceException(nameof(discoveredQueryItems));

            //if (!discoveredQueryItems.Any() || discoveredQueryItems.FirstOrDefault() == null)
            //    throw new JghAlertMessageException(nothingFound);

            #endregion

            #region do work - this works 100% but after experiencing it i decided to revert to the way we have always done it before

            //var discoveredTag = discoveredQueryItems.FirstOrDefault()?.TagAsString; // the Tag is meant to contain the Guid of the item

            //if (string.IsNullOrWhiteSpace(discoveredTag))
            //    throw new JghAlertMessageException(nothingFound);

            //var firstItemWithMatchingGuid = DataGridOfLeaderboardVm.ItemsSource.FirstOrDefault(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(z.Guid, discoveredTag));

            //if (firstItemWithMatchingGuid == null)
            //    throw new JghAlertMessageException(nothingFound);

            //var skip = Math.Max(DataGridOfLeaderboardVm.ItemsSource.IndexOf(firstItemWithMatchingGuid) - prequels, 0);

            //var truncatedItemsSource = new ObservableCollection<ResultItemDisplayObject>(DataGridOfLeaderboardVm.ItemsSource.Skip(skip));

            //await DataGridOfLeaderboardVm.RefillItemsSourceAsync(truncatedItemsSource);

            #endregion

            #endregion
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region Favorites

        protected async Task<bool> FavoritesDataGridRefilterAsync()
    {
        const string failure = "Unable to display list of favorites.";
        const string locus = "[FavoritesDataGridRefilterAsync]";

        try
        {
            #region do work

            #region null checks

            if (AllDataGridLineItemDisplayObjects == null)
                throw new JghNullObjectInstanceException(nameof(AllDataGridLineItemDisplayObjects));

            #endregion

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm?.CboLookupSeriesVm?.CurrentItem);

            var currentEvent = EventItemDisplayObject.ObtainSourceModel(GlobalSeasonProfileAndIdentityValidationVm?.CboLookupEventVm?.CurrentItem);

            #region obtain column format enum

            //  nothing required. already have it. use LeaderboardAndFavoritesListColumnFormatEnum property, which 
            // is set somewhere in derived class, most likely the ctor
            // can be EnumStrings.OrdinaryResultsColumnFormat, EnumStrings.SeriesTotalPointsColumnFormat, 
            // EnumStrings.SeriesTourDurationColumnFormat or EnumStrings.AverageSplitTimesColumnFormat

            #endregion

            #region PopulateDataGridTitleAndBlurb according to columnFormatEnum

            //var currentCboListOfMoreInfoSelectedItem = PopulateDataGridTitleAndBlurb();

            #endregion

            #region obtain row collection destined to become the datagrid itemssource by getting the favorites from storage and repopulating them

            var favoritesToBeSearchedFor = await ThingsPersistedInLocalStorage.GetFavoritesListIdentitiesAsync();

            var discoveredFavoriteThings = ThingWithNamesItem.SearchByName(favoritesToBeSearchedFor, AllDataGridLineItemDisplayObjects)
                .Where(z => z != null)
                .Distinct()
                .ToArray();
            // NB use .Distinct() to hide duplicates that tend to proliferate in general in Favorites

            var favorites =
                SortResultsInDesiredOrder(discoveredFavoriteThings, LeaderboardColumnFormatEnum);

            #endregion

            #region create DataGridDesigner to obtain array of column specification items for RadDataGrid control in a PresentationService

            DataGridDesignerForFavorites.InitialiseDesigner(
                currentSeries,
                currentEvent,
                LeaderboardColumnFormatEnum, favorites, PopulateDataGridTitleAndBlurb(), TxxColumnHeadersLookupTable);

            var columnSpecificationItemsForDisplayObjects = await ThingsPersistedInLocalStorage.GetMustDisplayConciseLeaderboardColumnsOnlyAsync()
                ? DataGridDesignerForFavorites.GetNonEmptyColumnSpecificationItemsForConciseLeaderboardOfResultItemDisplayObjects()
                : DataGridDesignerForFavorites.GetNonEmptyColumnSpecificationItemsForLeaderboardOfResultItemDisplayObjects();

            #endregion

            #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

            await
                FavoritesDataGridPresentationServiceInstance.GenerateDataGridColumnCollectionManuallyAsync(
                    columnSpecificationItemsForDisplayObjects);
            // unfortunately the TelerikRadDataColumnCollection is an internal property and cannot be set (or data bound to) externally. therefore we have to use this roundabout route

            #endregion

            #region create a Presenter to provide a datacontext and hence row collection and datacontext for the PresentationService

            var genders = CboLookupItemDisplayObject.ObtainSourceModel(CboLookupGenderCategoryFilterVm?.ItemsSource);

            await DataGridOfFavoritesVm.PopulatePresenterAsync(
                FinaliseFavoritesDataGridHeadings(),
                currentSeries,
                currentEvent,
                genders,
                TxxColumnHeadersLookupTable,
                FavoritesListRowGroupingEnum,
                LeaderboardColumnFormatEnum,
                EnumStrings.FavoritesList,
                favorites);

            #endregion

            #endregion
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion

        return true;
    }

        protected async Task AddPersonToFavoritesAsync(ResultItemDisplayObject personToBeAddedVm)
    {
        const string failure = "Unable to add person to favorites.";
        const string locus = "[AddPersonToFavoritesAsync]";

        try
        {
            if (personToBeAddedVm == null) return;

            var favoritesToBeUpdated = (await ThingsPersistedInLocalStorage.GetFavoritesListIdentitiesAsync())?.ToList() ?? [];

            // add to favorites - note to self. don't make the mistake of preempting duplicates. there is no sure-fire way of knowing what is a duplicate and what is not between one event and the next. let the user delete.
            // for now i'm ignoring this advice, which hails from the Silverlight era. if it proves to still be valid, it's easy to comment out the numberOfPossibleMatches test

            //var personToBeAdded = ResultItemViewModel.ObtainSourceItem(personToBeAddedVm);

            //personToBeAdded.FullName = ResultItem.FormatFullName(personToBeAdded); // need this further down

            var numberOfPossibleMatches = !favoritesToBeUpdated.Any()
                ? 0
                : favoritesToBeUpdated.Count(z => ThingWithNamesItem.AreMatchingNamesInAllProbability(z, personToBeAddedVm));

            if (numberOfPossibleMatches > 0) return;

            var newThing = ThingWithNamesItem.FromThing(personToBeAddedVm);

            favoritesToBeUpdated.Add(newThing);

            await ThingsPersistedInLocalStorage.SaveFavoritesListIdentitiesAsync(favoritesToBeUpdated.ToArray());
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        private static ResultItemDisplayObject[] SortResultsInDesiredOrder(ResultItemDisplayObject[] rawInputItems, string columnFormatEnum)
    {
        if (rawInputItems == null || !rawInputItems.Any()) return [];

        ResultItemDisplayObject[] rowCollection;

        if (columnFormatEnum == EnumStrings.SeriesTotalPointsColumnFormat)
            rowCollection = rawInputItems
                .Where(z => z != null)
                .OrderByDescending(z => z.PointsCalculatedRank)
                .ToArray();
        else
            rowCollection = rawInputItems
                .Where(z => z != null)
                .Where(z => ResultItemDisplayObject.ObtainSourceItem(z) != null)
                .Where(z => ResultItemDisplayObject.ObtainSourceItem(z).DerivedData != null)
                .OrderByDescending(z => z.CalculatedNumOfSplitsCompleted)
                .ThenBy(z => ResultItemDisplayObject.ObtainSourceItem(z).DerivedData.TotalDurationFromAlgorithmInSeconds)
                .ToArray();

        return rowCollection;
    }

        #endregion

        #region helpers

        #region make titles

        protected string MakeOrganiserTitle()
    {
        return GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem?.Organizer?.Title ?? string.Empty;
    }

        protected virtual string MakeSeriesTitle()
    {
        return GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.Title ?? string.Empty;
    }

        protected virtual string MakeEventTitle()
    {
        return GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem?.Title ?? string.Empty;
    }

        #endregion

        #region make labels

        protected virtual string MakeSeriesLabel()
    {
        return GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.Label ?? string.Empty;
    }

        protected virtual string MakeEventLabel()
    {
        return GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem?.Label ?? string.Empty;
    }

        protected string MakeUtilityClassificationLabel()
    {
        return CboLookupUtilityClassificationCategoryFilterVm?.CurrentItem?.Label ?? string.Empty;
    }

        protected abstract string[] FinaliseTableHeadings();

        protected abstract string[] FinaliseFavoritesDataGridHeadings();

        protected virtual string MakeMoreInfoLabel()
    {
        return CboLookupMoreInfoItemVm?.CurrentItem?.Label ?? string.Empty;
    }

        protected string MakeRaceLabel()
    {
        return CboLookupRaceCategoryFilterVm?.CurrentItem?.Label ?? string.Empty;
        //return CategorisationOfResultsVm?.CboLookupRaceCategoryFilterVm?.CurrentItem?.Label ?? string.Empty;
    }

        protected string MakeGenderLabel()
    {
        return CboLookupGenderCategoryFilterVm?.CurrentItem?.Label ?? string.Empty;
        //return CategorisationOfResultsVm?.CboLookupGenderCategoryFilterVm?.CurrentItem?.Label ?? string.Empty;
    }

        protected string MakeCategoryLabel()
    {
        return CboLookupAgeGroupCategoryFilterVm?.CurrentItem?.Label ??
               string.Empty;
        //return CategorisationOfResultsVm?.CboLookupAgeGroupCategoryFilterVm?.CurrentItem?.Label ??
        //       string.Empty;
    }

        protected string MakeCityLabel()
    {
        return CboLookupCityCategoryFilterVm?.CurrentItem?.Label ?? string.Empty;
    }

        #endregion

        #region uploading/publishing leaderboard as web document to server

        private async Task<Tuple<string, string>> TabulateResultsAsHtmlWebpageAsync(LeaderboardStyleDataGridViewModel individualResultsDataGrid)
    {
        const string failure = "Unable to author webpage for upload.";
        const string locus = "[TabulateResultsAsHtmlWebpage]";

        try
        {
            if (individualResultsDataGrid == null)
                throw new JghNullObjectInstanceException(nameof(individualResultsDataGrid));

            var printerOfHardCopy = CreateDataGridDesignerToWriteTableOfResults(individualResultsDataGrid);

            if (printerOfHardCopy == null)
                throw new JghNullObjectInstanceException(nameof(printerOfHardCopy));

            var htmWebPageAsString =
                await printerOfHardCopy.GetLeaderboardStyleResultsArrayAsPrettyPrintedWebPageOrTextFileAsync(false);

            var blobName = printerOfHardCopy.GetBlobNameToBeUsedForExportingResultsAsWebpage();

            var answer = new Tuple<string, string>(blobName, htmWebPageAsString);

            return answer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public DataGridDesigner CreateDataGridDesignerToWriteTableOfResults(LeaderboardStyleDataGridViewModel individualResultsDataGrid)
    {
        const string failure = "Unable to create a printer to tabulate the results.";
        const string locus = "[CreateDataGridDesignerToWriteTableOfResults]";


        try
        {
            #region load results printer

            #region populate variables for MenuItem_OnClick_UploadRezultzAsWebPage

            var currentCboListOfMoreInfoSelectedItem = CboLookupMoreInfoItemVm.CurrentItem;


            //switch (individualResultsDataGrid.ColumnFormatEnum)
            //{
            //    case EnumStrings.OrdinaryResultsColumnFormat:
            //    case EnumStrings.AverageSplitTimesColumnFormat:
            //        currentCboListOfMoreInfoSelectedItem = CboLookupMoreInfoItemVm.CurrentItem;
            //        break;
            //    case EnumStrings.SeriesTotalPointsColumnFormat:
            //    case EnumStrings.SeriesTourDurationColumnFormat:
            //        currentCboListOfMoreInfoSelectedItem = _seriesStandingsPageVm.CboLookupMoreInfoItemVm.CurrentItem;
            //        break;
            //    default:
            //        throw new Exception(Strings2016.The_desired_column_format_for_the_table_of_results_is_unspecified_or_unrecognised__Sorry__Coding_error);
            //}

            #endregion

            var resultItemsVm = individualResultsDataGrid.ItemsSource.ToArray();

            var printer = new DataGridDesigner();

            printer.InitialiseDesigner(
                individualResultsDataGrid.SeriesProfileToWhichThisPresenterRefers,
                individualResultsDataGrid.EventProfileToWhichThisPresenterRefers,
                individualResultsDataGrid.ColumnFormatEnum, resultItemsVm, MoreInformationItemDisplayObject.ObtainSourceModel(currentCboListOfMoreInfoSelectedItem), individualResultsDataGrid.DictionaryOfTxxColumnHeaders);

            //printer.LoadRowItemsForLeaderboardStyleFormat(individualResultsDataGrid.ItemsSource);

            #endregion

            return printer;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        protected async Task<string> UploadWorkToAzureAsync(EntityLocationItem destination, string stringToBeUploaded)
    {
        const string failure = "Unable to upload work to Azure.";
        const string locus = "[UploadWorkToAzureAsync]";

        try
        {
            #region null checks

            if (destination == null || string.IsNullOrWhiteSpace(destination.AccountName) || string.IsNullOrWhiteSpace(destination.ContainerName) || string.IsNullOrWhiteSpace(destination.EntityName))
                throw new JghAlertMessageException("Not posted. One or more of particulars for the target destination are null or blank.");

            #endregion

            var bytesToBeUploaded = JghConvert.ToBytesUtf8FromString(stringToBeUploaded);

            destination.EntityName = JghFilePathValidator.EliminateSpaces('-', destination.EntityName);
            // just in case contains spaces or is longer than 255 characters

            var uploadSucceeded = await _azureStorageSvcAgent.UploadBytesAsync(destination.AccountName, destination.ContainerName, destination.EntityName, bytesToBeUploaded, CancellationToken.None);

            var descriptionOfSize = JghConvert.SizeOfBytesInHighestUnitOfMeasure(JghConvert.ToBytesUtf8FromString(stringToBeUploaded).Length);

            var outcome = uploadSucceeded
                ? $"Upload succeeded. <{destination.EntityName}> was uploaded to <{destination.ContainerName}>  ({descriptionOfSize})"
                : $"Warning. Upload operation returned false, indicating lack of success. <Data item: {destination.EntityName}> <DataContainer: {destination.ContainerName}>";

            return outcome;
        }

        #region trycatch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        protected void PopulateNavigateToPostedLeaderboardHyperlinkButtonVm(EntityLocationItem destination)
    {
        Uri hardCopyUri;

        if (destination == null || string.IsNullOrWhiteSpace(destination.AccountName) || string.IsNullOrWhiteSpace(destination.ContainerName) || string.IsNullOrWhiteSpace(destination.EntityName))
        {
            hardCopyUri = DefaultUri;
        }
        else
        {
            destination.EntityName ??= "unknown";

            hardCopyUri = JghFilePathValidator.MakeAzureAndRezultzCompliantBlobUriHttps(destination.AccountName,
                destination.ContainerName, destination.EntityName);
        }

        NavigateToPostedLeaderboardHyperlinkButtonVm.NavigateUri = hardCopyUri;

        NavigateToPostedLeaderboardHyperlinkButtonVm.TargetName = DefaultTargetName;
    }

        #endregion

        protected void ThrowExceptionIfNoConnection()
    {
        if (!NetworkInterface.GetIsNetworkAvailable()) throw new JghCommunicationFailureException(StringsRezultz02.NoConnection);
    }

        protected async Task<bool> PopulateCboLookUpFileFormatsPresenterForDoingExports()
    {
        var kindsOfMoreInfoVm = new CboLookupItemDisplayObject[]
        {
            new() {Label = "HTML document (.htm)", EnumString = EnumStrings.AsHtmlDocument},
            new() {Label = "text document (.txt)", EnumString = EnumStrings.AsTextDocument},
            new() {Label = "text data (.csv)", EnumString = EnumStrings.AsCsvFile},
            new() {Label = "XML data (.xml)", EnumString = EnumStrings.AsFlatFileXml}
        };

        await CboLookUpFileFormatsVm.RefillItemsSourceAsync(kindsOfMoreInfoVm);


        CboLookUpFileFormatsVm.IsDropDownOpen = false; // on this page the default is false

        await CboLookUpFileFormatsVm.ChangeSelectedIndexToMatchItemEnumStringAsync(EnumStrings.AsHtmlDocument);

        CboLookUpFileFormatsVm.SaveSelectedIndexAsLastKnownGood();

        return true;
    }

        protected bool PopulateSourceUriStringOfAllImageUriItems(EventProfileItem currentEventProfile)
    {
        if (!JghFilePathValidator.IsValidContainerLocationSpecification(
                GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.LocationOfMedia?.DatabaseAccountName,
                GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.LocationOfMedia?.DataContainerName,
                out var _))
            return true;

        if (currentEventProfile?.EventSettingsItem?.UriItems == null) return true;

        foreach (var uriItem in currentEventProfile.EventSettingsItem.UriItems.Where(z => z != null))
        {
            if (!JghFilePathValidator.IsValidBlobName(uriItem.BlobName, out var _))
                continue;

            uriItem.SourceUriString = JghFilePathValidator
                .MakeAzureAndRezultzCompliantBlobUriHttps(
                    GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.LocationOfMedia?.DatabaseAccountName,
                    GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.LocationOfMedia?.DataContainerName,
                    uriItem.BlobName).ToString();
        }

        return true;
    }

        #endregion

        #region zeroisation stuff

        public async Task<bool> ZeroiseAsync()
    {
        AllDataGridLineItemDisplayObjects = [];

        HeadersVm.Zeroise();
        FootersVm.Zeroise();
        SearchFunctionVm.Zeroise();

        await ZeroiseDataGridVmsAndDesignersAsync();

        await ZeroiseItemsSourcesOfCboPresentersAsync();

        AllGuiControlsThatTouchDataAreAuthorisedToOperate(false);

        return true;
    }

        protected async Task<bool> ZeroiseDataGridVmsAndDesignersAsync()
    {
        await DataGridOfFavoritesVm.ZeroiseAsync();
        DataGridDesignerForFavorites.ZeroiseDesigner();

        await DataGridOfLeaderboardVm.ZeroiseAsync();
        DataGridDesignerForLeaderboard.ZeroiseDesigner();

        return true;
    }

        protected async Task<bool> ZeroiseItemsSourcesOfCboPresentersAsync()
    {
        foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms())
            await lookupVm.ZeroiseItemsSourceAsync();

        await CboLookupMoreInfoItemVm.ZeroiseItemsSourceAsync();

        return true;
    }

        #endregion

        #region Gui stuff

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
    {
        var answer = new List<object>();

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, LoadSourceDataButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, RefreshScreenButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, AddPersonToFavoritesButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, DeletePersonFromFavoritesButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, DeleteAllFavoritesButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, ToggleFavoritesGridVisibilityButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, ExportFavoritesButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, ExportSingleEventLeaderboardButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, PostLeaderboardAsHtmlToPublicDataStorageButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, NavigateToPostedLeaderboardHyperlinkButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, DataGridOfFavoritesVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, DataGridOfLeaderboardVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupRaceCategoryFilterVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupGenderCategoryFilterVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupAgeGroupCategoryFilterVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupCityCategoryFilterVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupTeamCategoryFilterVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupUtilityClassificationCategoryFilterVm);
        //AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupBlobNameToPublishResultsVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupMoreInfoItemVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, SearchFunctionVm);

        return answer;
    }

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
    {
        LoadSourceDataButtonVm.IsAuthorisedToOperate = true;
        RefreshScreenButtonVm.IsAuthorisedToOperate = true;

        //CboLookupBlobNameToPublishResultsVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
        CboLookupMoreInfoItemVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

        CboLookUpFileFormatsVm.IsAuthorisedToOperate = DataGridOfLeaderboardVm.ItemsSource.Any() || DataGridOfFavoritesVm.ItemsSource.Any();

        foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms())
            lookupVm.MakeAuthorisedToOperateIfItemsSourceIsAny(); // 1st kick

        var weHaveResults = AllDataGridLineItemDisplayObjects.Any();

        DataGridOfFavoritesVm.IsAuthorisedToOperate = DataGridOfFavoritesVm.ItemsSource.Any();
        DataGridOfLeaderboardVm.IsAuthorisedToOperate = weHaveResults;

        DisplayPodiumResultsOnlyToggleButtonVm.IsAuthorisedToOperate = weHaveResults;
        AddPersonToFavoritesButtonVm.IsAuthorisedToOperate = DataGridOfLeaderboardVm.SelectedItem != null;
        DeletePersonFromFavoritesButtonVm.IsAuthorisedToOperate = DataGridOfFavoritesVm.SelectedItem != null;
        DeleteAllFavoritesButtonVm.IsAuthorisedToOperate = DataGridOfFavoritesVm.ItemsSource.Any();
        ToggleFavoritesGridVisibilityButtonVm.IsAuthorisedToOperate = weHaveResults;

        ExportFavoritesButtonVm.IsAuthorisedToOperate = DataGridOfFavoritesVm.ItemsSource.Any(); // NB
        ExportSingleEventLeaderboardButtonVm.IsAuthorisedToOperate = DataGridOfLeaderboardVm.ItemsSource.Any(); // NB

        PostLeaderboardAsHtmlToPublicDataStorageButtonVm.IsAuthorisedToOperate = weHaveResults;
        NavigateToPostedLeaderboardHyperlinkButtonVm.IsAuthorisedToOperate = NavigateToPostedLeaderboardHyperlinkButtonVm.NavigateUri != DefaultUri;

        SearchFunctionVm.IsAuthorisedToOperate = weHaveResults;

        if (!weHaveResults)
        {
            foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms())
                lookupVm.IsAuthorisedToOperate = false; // 2nd kick

            CboLookupMoreInfoItemVm.IsAuthorisedToOperate = false;
            CboLookUpFileFormatsVm.IsAuthorisedToOperate = false;
        }
    }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
    {
        DataGridOfFavoritesVm.IsVisible = MustDisplayFavoritesDatagrid;

        DataGridOfLeaderboardVm.IsVisible = DataGridOfLeaderboardVm.ItemsSource.Any();

        SearchFunctionVm.IsVisible = true;

        HeadersVm.IsVisible = true;

        FootersVm.IsVisible = true;

        LoadSourceDataButtonVm.IsVisible = true;

        RefreshScreenButtonVm.IsVisible = true;

        ExportFavoritesButtonVm.IsVisible = DataGridOfFavoritesVm.ItemsSource.Any(); // NB

        ExportSingleEventLeaderboardButtonVm.IsVisible = DataGridOfLeaderboardVm.ItemsSource.Any(); // NB

        NavigateToPostedLeaderboardHyperlinkButtonVm.IsVisible = true;
        PostLeaderboardAsHtmlToPublicDataStorageButtonVm.IsVisible = true;

        DisplayPodiumResultsOnlyToggleButtonVm.IsVisible = AllDataGridLineItemDisplayObjects.Any();

        ToggleFavoritesGridVisibilityButtonVm.IsVisible = true;

        DeleteAllFavoritesButtonVm.IsVisible = true;

        AddPersonToFavoritesButtonVm.IsVisible = true;

        DeletePersonFromFavoritesButtonVm.IsVisible = true;

        CboLookupMoreInfoItemVm.MakeVisibleIfItemsSourceIsAny();

        CboLookUpFileFormatsVm.IsVisible = DataGridOfLeaderboardVm.ItemsSource.Any() || DataGridOfFavoritesVm.ItemsSource.Any();

        foreach (var lookupVm in CboLookUpCategoryFiltersMakeListOfAllVms())
            lookupVm.MakeVisibleIfItemsSourceIsGreaterThanTwo(); // because we added a(spurious) labeled item to the top of the list signifying 'all' and display is superfluous if there is only one item
    }

        #endregion

        #region GenesisAsLastKnownGood

        private SeasonProfileItem _lastKnownGoodSeasonProfileItem;

        private SeriesItemDisplayObject _lastKnownGoodSeriesItemVm;

        private EventItemDisplayObject _lastKnownGoodEventItemVm;


        protected void SaveGenesisOfThisViewModelAsLastKnownGood()
    {
        _lastKnownGoodSeasonProfileItem = GlobalSeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem;
        _lastKnownGoodSeriesItemVm = GlobalSeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem;
        _lastKnownGoodEventItemVm = GlobalSeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem;
    }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        if (_lastKnownGoodSeasonProfileItem == null)
            return true;

        if (GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem == null)
            return true;

        if (_lastKnownGoodSeasonProfileItem != GlobalSeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem)
            return true;

        if (_lastKnownGoodSeriesItemVm != GlobalSeasonProfileAndIdentityValidationVm?.CboLookupSeriesVm?.CurrentItem)
            return true;

        if (_lastKnownGoodEventItemVm != GlobalSeasonProfileAndIdentityValidationVm?.CboLookupEventVm?.CurrentItem)
            return true;

        return false;
    }

        #endregion
    }
}