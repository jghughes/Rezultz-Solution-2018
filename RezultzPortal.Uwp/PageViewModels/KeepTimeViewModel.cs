using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.PortalDisplayObjects;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.DataGridDesigners;
using Rezultz.Library02.Mar2024.DataGridInterfaces;
using RezultzPortal.Uwp.EditTemplateViewModels;
using RezultzPortal.Uwp.PageViewModelBases;
using RezultzPortal.Uwp.Strings;
using RezultzSvc.Agents.Mar2024.SvcAgents;

/* simple viewmodels i.e. ones that inherit from GenericViewViewModel
    (only) drive their BusyIndicator off IsBusy, not off
    the ProgressIndicatorViewModel in LeaderboardStylePageViewModelBase. NB this governs the choice of FullScreenLoadingTemplate
    as opposed to RezultzContentPageLoadingTemplate for page controls in Xamarin depending on which vm they have as their datacontext
*/

namespace RezultzPortal.Uwp.PageViewModels
{
    public class KeepTimeViewModel : HubItemPagesViewModelBase<TimeStampHubItem, TimeStampHubItemEditTemplateViewModel, TimeStampHubItemDisplayObject>
    {
        private const string Locus2 = nameof(KeepTimeViewModel);
        private const string Locus3 = "[RezultzPortal.Uwp]";

        #region ctor

        public KeepTimeViewModel(
            ITimeKeepingSvcAgent timeKeepingSvcAgent,
            IRegistrationSvcAgent registrationSvcAgent,
            ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
            IRepositoryOfHubStyleEntriesWithStorageBackup<TimeStampHubItem> repositoryOfTimeStamps,
            ISessionState sessionState,
            IThingsPersistedInLocalStorage thingsPersistedInLocalStorage, ILocalStorageService localStorageService)
            : base(leaderboardResultsSvcAgent, repositoryOfTimeStamps, sessionState, thingsPersistedInLocalStorage, localStorageService)
        {
            const string failure = "Unable to construct object KeepTimeViewModel.";
            const string locus = "[ctor]";

            try
            {
                #region assign ctor IOC injections

                _timeKeepingSvcAgent = timeKeepingSvcAgent;

                _registrationSvcAgent = registrationSvcAgent;

                #endregion

                LocalRepositoryOfParticipants = new RepositoryOfHubStyleEntries<ParticipantHubItem>();

                #region instantiate button presenters

                CreateTimeStampForGunStartButtonVm =
                    new ButtonControlViewModel(CreateTimeStampForGunStartButtonOnClickExecuteAsync,
                        CreateTimeStampForGunStartButtonOnClickCanExecute);

                CreateTimeStampForTimingMatSignalButtonVm =
                    new ButtonControlViewModel(
                        CreateTimeStampForTimingMatSignalButtonOnClickExecuteAsync,
                        CreateTimeStampForTimingMatSignalButtonOnClickCanExecute);

                CreateTimeStampCloneButtonVm =
                    new ButtonControlViewModel(
                        CreateTimeStampCloneButtonOnClickExecuteAsync,
                        CreateTimeCloneStampButtonOnClickCanExecute);

                RejectTimeStampCloneButtonVm =
                    new ButtonControlViewModel(
                        RejectTimeStampCloneButtonOnClickExecuteAsync,
                        RejectTimeStampCloneButtonOnClickCanExecute);

                PullParticipantProfilesFromHubButtonVm =
                    new ButtonControlViewModel(
                        PullParticipantProfilesFromHubButtonOnClickExecuteAsync,
                        PullParticipantProfilesFromHubButtonOnClickCanExecute);

                PullParticipantProfilesFromHubCancelButtonVm =
                    new ButtonControlViewModel(
                        PullParticipantProfilesFromHubCancelButtonOnClickExecuteAsync,
                        PullParticipantProfilesFromHubCancelButtonOnClickCanExecute);

                RefreshDataGridOfSplitIntervalsPerPersonButtonVm =
                    new ButtonControlViewModel(
                        RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickExecuteAsync,
                        RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickCanExecute);

                // the following buttons used in page code behind for file-related operations - hence no execute methods in this vm

                ExportAllTimeStampsButtonVm =
                    new ButtonControlViewModel(() => { }, () => false);

                ExportAllSplitIntervalsPerPersonButtonVm =
                    new ButtonControlViewModel(() => { }, () => false);

                ExportResultsLeaderboardForPreprocessingButtonVm =
                    new ButtonControlViewModel(() => { }, () => false);

                #endregion

                #region instantiate cbos

                CboLookupKindOfGunStartVm =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            StringsPortal.Clock_mode,
                            CboLookupKindOfGunStartOnSelectionChangedExecuteAsync,
                            CboLookupKindOfGunStartOnSelectionChangedCanExecute)
                        {IsAuthorisedToOperate = true};

                CboLookupKindOfGunStartVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupGroupLabelForGroupStartVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                        string.Empty,
                        CboLookupGroupLabelForGroupStartOnSelectionChangedExecuteAsync,
                        CboLookupGroupLabelForGroupStartOnSelectionChangedCanExecute)
                    {IsVisible = false, IsAuthorisedToOperate = false};

                CboLookupGroupLabelForGroupStartVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupDnxVm =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            StringsPortal.Dnx_status, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupDnxVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupAnomalousThresholdForTooManySplitsVm =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            string.Empty, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupAnomalousThresholdForTooManySplitsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupAnomalousThresholdForTooFewSplitsVm =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            string.Empty, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupAnomalousThresholdForTooFewSplitsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupAnomalousThresholdForTooBriefSplitsVm =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            string.Empty, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupAnomalousThresholdForTooBriefSplitsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            string.Empty, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            string.Empty, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);


                CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            string.Empty, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid =
                    new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                            string.Empty, () => { }, () => false)
                        {IsAuthorisedToOperate = true};

                CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

                #endregion

                #region instantiate Textboxes

                TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm =
                    new TextBoxControlViewModel(TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedExecuteAsync, TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedCanExecute);

                TextBoxForEnteringIndividualIdForTimeStampCloneVm =
                    new TextBoxControlViewModel(TextBoxForEnteringIndividualIdForTimeStampCloneCreateTextChangedExecute, TextBoxForEnteringIndividualIdForTimeStampCloneCreateOnTextChangedCanExecute);

                #endregion

                SeasonProfileAndIdentityValidationVm.MustHideCboLookupBlobNameToPublishResults = true;
                SeasonProfileAndIdentityValidationVm.CurrentRequiredWorkRole = EnumStringsForTimingSystemWorkRoles.TimeKeeping;
                SeasonProfileAndIdentityValidationVm.PropertyChanged += AnyINotifyPropertyChangedEventHandlerWiredToSeasonProfileAndIdentityValidationVm;

                RepositoryOfHubStyleEntries.DesiredHeightOfShortList = AppSettings.DesiredHeightOfShortListOfHubItemsDefault;
            }
            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
        }

        #endregion

        #region fields

        private readonly ParticipantDatabase _participantDatabase = new();


        private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 5; // virtually nothing because there are no Execute methods wired up to any of the Cbos

        #endregion

        #region global props

        private readonly ITimeKeepingSvcAgent _timeKeepingSvcAgent;

        private readonly IRegistrationSvcAgent _registrationSvcAgent;

        private static ITimeStampEntriesInMemoryCacheDataGridPresentationService TimeStampEntriesInMemoryCacheDataGridPresentationService
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<ITimeStampEntriesInMemoryCacheDataGridPresentationService>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[ITimeStampEntriesInMemoryCacheDataGridPresentationService]");

                    const string locus = "Property getter of [TimeStampEntriesInMemoryCacheDataGridPresentationService]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        private static ITimeStampEntriesInLocalStorageDataGridPresentationService TimeStampEntriesInLocalLocalStorageDataGridPresentationService
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<ITimeStampEntriesInLocalStorageDataGridPresentationService>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[ITimeStampEntriesInLocalStorageDataGridPresentationService]");

                    const string locus = "Property getter of [TimeStampEntriesInLocalLocalStorageDataGridPresentationService]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        private static ISplitIntervalsPerPersonDataGridPresentationService SplitIntervalsPerPersonDataGridPresentationService
        {
            get
            {
                try
                {
                    return ServiceLocator.Current.GetInstance<ISplitIntervalsPerPersonDataGridPresentationService>();
                }
                catch (Exception ex)
                {
                    var msg =
                        JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                            "[ISplitIntervalsPerPersonDataGridPresentationService]");

                    const string locus =
                        "Property getter of [SplitIntervalsPerPersonDataGridPresentationService]";
                    throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
                }
            }
        }

        #endregion

        #region props

        public CancellationTokenSource PullParticipantProfilesFromHubCts { get; set; }

        #region TextBox presenters

        public TextBoxControlViewModel TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm { get; }

        public TextBoxControlViewModel TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm01 { get; } = new(() => { }, () => true) {IsAuthorisedToOperate = true};
        public TextBoxControlViewModel TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm02 { get; } = new(() => { }, () => true) {IsAuthorisedToOperate = true};
        public TextBoxControlViewModel TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm03 { get; } = new(() => { }, () => true) {IsAuthorisedToOperate = true};

        public TextBoxControlViewModel TextBoxForEnteringIndividualIdForTimeStampCloneVm { get; }

        public TextBoxControlViewModel TextBoxForEnteringMultipleIndividualIds { get; } = new(() => { }, () => true) {IsAuthorisedToOperate = true};

        #endregion

        #region CheckBoxes etc - bool INPC

        private bool _backingstoreMustDisplayEarliestFirst;

        public bool MustDisplayEarliestFirst
        {
            get => _backingstoreMustDisplayEarliestFirst;
            set => SetProperty(ref _backingstoreMustDisplayEarliestFirst, value);
        }

        private bool _backingstoreMustDisplayParticipantProfileInfoInTimeStamps;

        public bool MustDisplayParticipantProfileInfoInTimeStamps
        {
            get => _backingstoreMustDisplayParticipantProfileInfoInTimeStamps;
            set => SetProperty(ref _backingstoreMustDisplayParticipantProfileInfoInTimeStamps, value);
        }

        private bool _backingstoreMustDisplayGunStartsOnly;

        public bool MustDisplayGunStartsOnly
        {
            get => _backingstoreMustDisplayGunStartsOnly;
            set => SetProperty(ref _backingstoreMustDisplayGunStartsOnly, value);
        }

        private bool _backingstoreMustHighlightTimingMatEntriesThatOccurInClusters;

        public bool MustHighlightTimingMatEntriesThatOccurInClusters
        {
            get => _backingstoreMustHighlightTimingMatEntriesThatOccurInClusters;
            set => SetProperty(ref _backingstoreMustHighlightTimingMatEntriesThatOccurInClusters, value);
        }

        private bool _backingstoreMustOnlyShowTimingMatEntriesThatOccurInClusters;

        public bool MustOnlyShowTimingMatEntriesThatOccurInClusters
        {
            get => _backingstoreMustOnlyShowTimingMatEntriesThatOccurInClusters;
            set => SetProperty(ref _backingstoreMustOnlyShowTimingMatEntriesThatOccurInClusters, value);
        }

        private bool _backingstoreMustIncludePhotos;

        public bool MustIncludePhotos
        {
            get => _backingstoreMustIncludePhotos;
            set => SetProperty(ref _backingstoreMustIncludePhotos, value);
        }


        private bool _backingstoreMustIncludePhotoIdentification;

        public bool MustIncludePhotoIdentification
        {
            get => _backingstoreMustIncludePhotoIdentification;
            set => SetProperty(ref _backingstoreMustIncludePhotoIdentification, value);
        }

        // display options for SplitIntervalsPerPerson data grid

        private bool _backingstoreMustDisplayAbridgedColumnsInSplitIntervalsPerPersonPerPerson;

        public bool MustDisplayAbridgedColumnsInSplitIntervalsPerPerson
        {
            get => _backingstoreMustDisplayAbridgedColumnsInSplitIntervalsPerPersonPerPerson;
            set => SetProperty(ref _backingstoreMustDisplayAbridgedColumnsInSplitIntervalsPerPersonPerPerson, value);
        }

        private bool _backingstoreMustOnlyShowCommentedRowsInSplitIntervalsPerPersonDataGrid;

        public bool MustOnlyShowCommentedRowsInSplitIntervalsPerPersonDataGrid
        {
            get => _backingstoreMustOnlyShowCommentedRowsInSplitIntervalsPerPersonDataGrid;
            set => SetProperty(ref _backingstoreMustOnlyShowCommentedRowsInSplitIntervalsPerPersonDataGrid, value);
        }

        private bool _backingstoreMustDisplayTimeStampsNotIntervalsForTxxInSplitIntervalsPerPersonItems;

        public bool MustDisplayTimeStampsNotIntervalsForTxxInSplitIntervalsPerPersonItems
        {
            get => _backingstoreMustDisplayTimeStampsNotIntervalsForTxxInSplitIntervalsPerPersonItems;
            set => SetProperty(ref _backingstoreMustDisplayTimeStampsNotIntervalsForTxxInSplitIntervalsPerPersonItems, value);
        }

        private bool _backingstoreMustDisplayModifiedEntriesAsWell;

        public bool MustDisplayModifiedEntriesAsWell
        {
            get => _backingstoreMustDisplayModifiedEntriesAsWell;
            set => SetProperty(ref _backingstoreMustDisplayModifiedEntriesAsWell, value);
        }

        private bool _backingstoreMustDisplayEditTemplateForRepositoryItemBeingEdited;

        public bool MustDisplayEditTemplateForRepositoryItemBeingEdited
        {
            get => _backingstoreMustDisplayEditTemplateForRepositoryItemBeingEdited;
            set => SetProperty(ref _backingstoreMustDisplayEditTemplateForRepositoryItemBeingEdited, value);
        }

        #endregion

        #region ButtonVms

        public ButtonControlViewModel CreateTimeStampForGunStartButtonVm { get; }

        public ButtonControlViewModel CreateTimeStampForTimingMatSignalButtonVm { get; }

        public ButtonControlViewModel CreateTimeStampCloneButtonVm { get; }
        public ButtonControlViewModel RejectTimeStampCloneButtonVm { get; }

        public ButtonControlViewModel PullParticipantProfilesFromHubButtonVm { get; }
        public ButtonControlViewModel PullParticipantProfilesFromHubCancelButtonVm { get; }

        public ButtonControlViewModel RefreshDataGridOfSplitIntervalsPerPersonButtonVm { get; }

        // the buttons here below are used in page code behind for file-related operations - hence no execute methods in this vm

        public ButtonControlViewModel ExportAllTimeStampsButtonVm { get; }

        public ButtonControlViewModel ExportAllSplitIntervalsPerPersonButtonVm { get; }

        public ButtonControlViewModel ExportResultsLeaderboardForPreprocessingButtonVm { get; }

        #endregion

        #region SearchFunction

        public SearchViewModel SearchFunctionForSplitIntervalsPerPersonVm { get; } =
            new("search", 2, 9, null, null);

        #endregion

        #region DataGridDesigners

        public DataGridDesigner DataGridDesignerForSplitIntervalsPerPerson { get; } = new();

        #endregion

        #region DataGridViewModels

        public DataGridViewModel<SplitIntervalConsolidationForParticipantDisplayObject> DataGridOfSplitIntervalsPerPerson { get; } =
            new(string.Empty, () => { }, () => false) {IsVisible = false};

        #endregion

        #region Cbos

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupDnxVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupKindOfGunStartVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupGroupLabelForGroupStartVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupAnomalousThresholdForTooManySplitsVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupAnomalousThresholdForTooFewSplitsVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupAnomalousThresholdForTooBriefSplitsVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid { get; protected set; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid { get; protected set; }

        #endregion

        public readonly RepositoryOfHubStyleEntries<ParticipantHubItem> LocalRepositoryOfParticipants;

        #endregion

        #region commands

        #region CheckConnectionToRezultzHubButtonOnClickAsync

        protected override async Task<string> CheckConnectionToRezultzHubButtonOnClickAsync()
        {
            //const string failure = "Unable to do what this method does.";
            //const string locus = "[CheckConnectionToRezultzHubButtonOnClickAsync]";

            if (!NetworkInterface.GetIsNetworkAvailable())
                return StringsPortal.NoConnection;

            try
            {
                await _timeKeepingSvcAgent.ThrowIfNoServiceConnectionAsync(); // throws if no connection, timeout, svc doesn't answer etc
            }
            catch (Exception e)
            {
                return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
            }

            return StringsPortal.ServiceUpAndRunning;
        }

        #endregion

        #region LaunchWorkSession

        protected override async Task<string> LaunchWorkSessionButtonOnClickAsync()
        {
            const string failure = "Unable to launch or re-launch work session from data preserved in local storage.";
            const string locus = "[LaunchWorkSessionButtonOnClickAsync]";

            try
            {
                WorkSessionIsLaunched = false;

                ThrowIfWorkSessionNotReadyForLaunch();

                SaveGenesisOfThisViewModelAsLastKnownGood();

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                #region restore timestamp data from storage

                var sb = new StringBuilder();

                try
                {
                    var initialisedCount = await RepositoryOfHubStyleEntries.RestoreMemoryCacheFromLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                    sb.AppendLine(StringsPortal.Launch_succeeded);
                    sb.AppendLine("");

                    sb = initialisedCount switch
                    {
                        0 => sb.AppendLine(
                            "Note:  no timekeeping data happens to be resident on this machine for this event at the moment.  If you know that data exists, and are wondering where it is, it will be on the remote hub that you and others share."),
                        1 => sb.AppendLine("Note:  a single item of timekeeping data happens to be resident on this machine for this event at the moment."),
                        _ => sb.AppendLine($"Note:  {initialisedCount} timekeeping data entries happen to be resident on this machine for this event at the moment.")
                    };
                }
                catch (Exception)
                {
                    // Note: an exception will be thrown if there is a JSON format discontinuity in local storage, typically caused when a new version of the app is released and the storage format evolves
                    await RepositoryOfHubStyleEntries.ClearLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                    sb.AppendLine(
                        "Sorry. Had trouble restoring timekeeping data resident on this machine from the previous work session. This can happen on extremely rare occasions, for example when a new version of the app is released. If you believe that there is data relating to the previous session, your best bet is to pull down the data for the event from the remote hub and hope it was saved there.");
                }

                RepositoryOfHubStyleEntries.FlagAllEntriesAsSaved(); // by definition this is true because we just got them from storage

                #endregion

                #region refresh GUI

                await RefreshAllDataGridsAndListViewsAsync();

                HeadersVm.Populate(StringsPortal.Target_event, SeriesItemUponLaunchOfWorkSession.Label, EventItemUponLaunchOfWorkSession.Label, $"Work session launched at {DateTime.Now:HH:mm}");

                HeadersVm.SaveAsLastKnownGood();

                if (SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm != null)
                    SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.Label = $"{SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem?.Label}   Target initialised at {DateTime.Now:HH:mm}";

                if (SeasonProfileAndIdentityValidationVm.CboLookupEventVm != null)
                    SeasonProfileAndIdentityValidationVm.CboLookupEventVm.Label = $"{SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem?.Label}   Target initialised at {DateTime.Now:HH:mm}";

                await PopulateCboLookupGroupLabelForGroupStartAsync(SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem);

                #endregion

                WorkSessionIsLaunched = true;

                sb.AppendLine();
                sb.AppendLine(
                    "Where there is data on the hub (generated and uploaded previously by you or someone else), you can optionally pull down a copy during your work session any time you like.  The hub data won't overwrite your local data.  It will be merged with it.  Look for 'Pull timestamps from hub'.  You can do work regardless of whether or not you pull down previous data to this machine. ");

                var answer = sb.ToString();

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                WorkSessionIsLaunched = false;

                HeadersVm.Populate(StringsPortal.Target_event, StringsPortal.Not_yet_launched);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region PullAllItemsFromHubButtonOnClickAsync

        protected override async Task<string> PullAllItemsFromHubButtonOnClickAsync()
        {
            const string failure = "Unable to pull all timestamps from hub.";
            const string locus = "[PullAllItemsFromHubButtonOnClickAsync]";

            try
            {
                #region check connection

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghAlertMessageException(StringsPortal.NoConnection);

                PullAllItemsFromHubCts = new CancellationTokenSource();

                await _timeKeepingSvcAgent.ThrowIfNoServiceConnectionAsync(PullAllItemsFromHubCts.Token); // throws if no connection, timeout, svc doesn't answer etc

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var cloudDataLocation =
                    TimeKeepingSvcAgent.MakeDataLocationForStorageOfClockDataOnRemoteHub(currentSeries, currentEvent);

                #endregion

                #region pull timestamps

                var downLoadedItems = Array.Empty<TimeStampHubItem>(); // default

                var containerDoesExist = await _timeKeepingSvcAgent.GetIfContainerExistsAsync(cloudDataLocation.Item1, cloudDataLocation.Item2, PullAllItemsFromHubCts.Token);

                if (containerDoesExist)
                    downLoadedItems = await _timeKeepingSvcAgent.GetTimeStampItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2, PullAllItemsFromHubCts.Token);

                #endregion

                #region download complete (which may or may not have found any items) - reinitialise RepositoryOfHubStyleEntries etc

                var didRunToCompletion =
                    RepositoryOfHubStyleEntries.TryAddRangeNoDuplicates(downLoadedItems, out var errorMessage);

                var localDataLocation = MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                if (!didRunToCompletion)
                {
                    PullAllItemsFromHubProtectionButtonVm.IsChecked = false;

                    throw new JghAlertMessageException(errorMessage);
                }

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshAllDataGridsAndListViewsAsync();

                var messageOk = downLoadedItems.Length switch
                {
                    0 => "Nothing received. Hub was clean.",
                    1 => "Success. A copy of a single data item was received from the hub and merged with the data on this machine.",
                    _ => $"Success. Copies of {downLoadedItems.Length} data items were received from the hub and merged with the data on this machine."
                };

                return messageOk;

                #endregion
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                PullAllItemsFromHubCts.Dispose();
            }

            #endregion
        }

        #endregion

        #region PullParticipantProfilesFromHubButtonOnClickAsync

        private bool PullParticipantProfilesFromHubButtonOnClickCanExecute()
        {
            return PullParticipantProfilesFromHubButtonVm.IsAuthorisedToOperate;
        }

        private async void PullParticipantProfilesFromHubButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[PullParticipantProfilesFromHubButtonOnClickExecuteAsync]";

            try
            {
                if (!PullParticipantProfilesFromHubButtonOnClickCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____looking_for_data);

                DeadenGui();

                var messageOk = await PullParticipantProfilesFromHubButtonOnClickAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowOkAsync(messageOk);
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        protected async Task<string> PullParticipantProfilesFromHubButtonOnClickAsync()
        {
            const string failure = "Unable to pull all participant items from hub.";
            const string locus = "[PullParticipantProfilesFromHubButtonOnClickAsync]";

            try
            {
                #region check connection

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghAlertMessageException(StringsPortal.NoConnection);

                PullParticipantProfilesFromHubCts = new CancellationTokenSource();

                await _registrationSvcAgent.ThrowIfNoServiceConnectionAsync(PullParticipantProfilesFromHubCts.Token); // throws if no connection, timeout, svc doesn't answer etc

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem);

                var cloudDataLocation =
                    ParticipantRegistrationSvcAgent.MakeDataLocationForStorageOfParticipantDataOnRemoteHub(currentSeries, currentEvent);

                #endregion

                #region pull profiles

                var downLoadedItems = Array.Empty<ParticipantHubItem>(); // default

                var containerDoesExist = await _registrationSvcAgent.GetIfContainerExistsAsync(cloudDataLocation.Item1, cloudDataLocation.Item2, PullParticipantProfilesFromHubCts.Token);

                if (containerDoesExist) downLoadedItems = await _registrationSvcAgent.GetParticipantItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2, PullParticipantProfilesFromHubCts.Token);

                #endregion

                #region download complete (which may or may not have found any items) - refresh _repositoryOfParticipants

                var didRunToCompletion =
                    LocalRepositoryOfParticipants.TryAddRangeNoDuplicates(downLoadedItems, out var errorMessage);


                if (!didRunToCompletion) throw new JghAlertMessageException(errorMessage);


                _participantDatabase.LoadDatabaseV2(LocalRepositoryOfParticipants);

                MustDisplayParticipantProfileInfoInTimeStamps = true;

                await RefreshAllDataGridsAndListViewsAsync();

                var messageOk = downLoadedItems.Length == 0
                    ? JghString.ConcatAsParagraphs("Nothing received. Hub was clean.")
                    : JghString.ConcatAsParagraphs($"{downLoadedItems.Length} copies received from the hub and merged.");

                return messageOk;

                #endregion
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                PullParticipantProfilesFromHubCts.Dispose();
            }

            #endregion
        }

        #endregion

        #region PullParticipantProfilesFromHubCancelButtonOnClickAsync

        private bool PullParticipantProfilesFromHubCancelButtonOnClickCanExecute()
        {
            return true;
        }

        private void PullParticipantProfilesFromHubCancelButtonOnClickExecuteAsync()
        {
            try
            {
                PullParticipantProfilesFromHubCts?.Cancel();
            }
            catch (Exception)
            {
                //do nothing. this is where we end up if the cts has been disposed of.
            }
        }

        #endregion

        #region CboLookupKindOfGunStartOnSelectionChangedAsync

        protected virtual bool CboLookupKindOfGunStartOnSelectionChangedCanExecute()
        {
            return CboLookupKindOfGunStartVm.IsAuthorisedToOperate;
        }

        private async void CboLookupKindOfGunStartOnSelectionChangedExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[CboLookupKindOfGunStartOnSelectionChangedExecuteAsync]";

            try
            {
                if (!CboLookupKindOfGunStartOnSelectionChangedCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator("");

                DeadenGui();

                await CboLookupKindOfGunStartOnSelectionChangedAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        private async Task CboLookupKindOfGunStartOnSelectionChangedAsync()
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            // put a label on the button

            CreateTimeStampForGunStartButtonVm.Label = CboLookupKindOfGunStartVm.CurrentItem?.EnumString switch
            {
                EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody => "Click to start everyone now!",
                EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual => "Click to start person now!",
                _ => "Click to start now!"
            };

            // zeroise the irrelevant controls

            await CboLookupGroupLabelForGroupStartVm.ChangeSelectedIndexAsync(-1);

            await TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.ChangeTextAsync("");

            await Task.FromResult(true);
        }

        #endregion

        #region CboLookupGroupLabelForGroupStartOnSelectionChangedAsync

        protected virtual bool CboLookupGroupLabelForGroupStartOnSelectionChangedCanExecute()
        {
            return CboLookupGroupLabelForGroupStartVm.IsAuthorisedToOperate;
        }

        private async void CboLookupGroupLabelForGroupStartOnSelectionChangedExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[CboLookupGroupLabelForGroupStartOnSelectionChangedExecuteAsync]";

            try
            {
                if (!CboLookupGroupLabelForGroupStartOnSelectionChangedCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator("");

                DeadenGui();

                await CboLookupGroupLabelForGroupStartOnSelectionChangedAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        private async Task CboLookupGroupLabelForGroupStartOnSelectionChangedAsync()
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion


            CreateTimeStampForGunStartButtonVm.Label = $"Click to start {CboLookupGroupLabelForGroupStartVm.CurrentItem?.Label} now!";

            await TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.ChangeTextAsync("");
        }

        #endregion

        #region TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedAsync

        protected virtual bool TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedCanExecute()
        {
            return TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsAuthorisedToOperate;
        }

        private async void TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedExecuteAsync]";

            try
            {
                if (!TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator("");

                DeadenGui();

                TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        private void TextBoxForEnteringIndividualIdForTimeStampOfGunStartTextChangedAsync()
        {
            // do nothing for know. The functionality is implemented in the code-behind. I couldn't get it to work properly here...
        }

        #endregion

        #region TextBoxForEnteringIndividualIdForTimeStampCloneCreateTextChangedAsync

        protected virtual bool TextBoxForEnteringIndividualIdForTimeStampCloneCreateOnTextChangedCanExecute()
        {
            return TextBoxForEnteringIndividualIdForTimeStampCloneVm.IsAuthorisedToOperate;
        }

        private void TextBoxForEnteringIndividualIdForTimeStampCloneCreateTextChangedExecute()
        {
            CreateTimeStampCloneButtonVm.IsAuthorisedToOperate = true;
        }

        #endregion


        #region CreateTimeStampForGunStartButtonOnClickAsync

        private bool CreateTimeStampForGunStartButtonOnClickCanExecute()
        {
            return CreateTimeStampForGunStartButtonVm.IsAuthorisedToOperate;
        }

        private async void CreateTimeStampForGunStartButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[CreateTimeStampForGunStartButtonOnClickExecuteAsync]";

            try
            {
                if (!CreateTimeStampForGunStartButtonOnClickCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator("Creating new item...");

                DeadenGui();

                await CreateTimeStampForGunStartButtonOnClickAsync();

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

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        protected async Task<bool> CreateTimeStampForGunStartButtonOnClickAsync()
        {
            const string failure = "Unable to add a new item of timestamp data.";
            const string locus = "[CreateTimeStampForGunStartButtonOnClickAsync]";

            // we have arrived here because the user clicked the clock timer button.
            // she might or might not have entered a bib in the case of an individual gun start but a Symbols.SymbolUnspecified timestamp is automatically entered nevertheless

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                #region add item to RepositoryOfHubStyleEntries

                if (CboLookupKindOfGunStartVm.SelectedIndex == -1)
                    throw new JghAlertMessageException("Choose type of gun start!");

                var kindOfGunStartEntryEnum = CboLookupKindOfGunStartVm?.CurrentItem?.EnumString;

                var labelOfStartingGroupOrIdOfSingleIndividual = string.Empty; // ... as the case may be - if relevant!

                switch (kindOfGunStartEntryEnum)
                {
                    case EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody:

                        labelOfStartingGroupOrIdOfSingleIndividual = kindOfGunStartEntryEnum;

                        break;

                    case EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup:

                        if (CboLookupGroupLabelForGroupStartVm.SelectedIndex == -1)
                            throw new JghAlertMessageException("Choose which race to start!");

                        labelOfStartingGroupOrIdOfSingleIndividual = CboLookupGroupLabelForGroupStartVm.CurrentItem?.Label;

                        break;

                    case EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual:


                        if (string.IsNullOrWhiteSpace(TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.Text))
                        {
                            labelOfStartingGroupOrIdOfSingleIndividual = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());
                            // NB. we use Symbols.SymbolUnspecified enum string in many places to decide courses of action and shortcuts when IDs are missing, whether for individual Id or gun start enum
                        }
                        else
                        {
                            if (!JghString.IsOnlyLettersOrDigitsOrHyphen(TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.Text))
                                throw new JghAlertMessageException("ID can only consist of letters, digits, or hyphens (or be initially blank).");

                            labelOfStartingGroupOrIdOfSingleIndividual = TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.Text;
                        }

                        break;
                }

                ButtonClickCounter += 1;

                var touchedBy = SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem?.UserName;

                var thisTimeStampBinaryFormat = JghDateTime.RoundedToTenthOfSecond(DateTime.Now).ToBinary();

                var item = TimeStampHubItem.Create(ButtonClickCounter, labelOfStartingGroupOrIdOfSingleIndividual, string.Empty, kindOfGunStartEntryEnum, thisTimeStampBinaryFormat, touchedBy);

                var didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(item, out var errorMessage);

                if (didRunToCompletion == false)
                    throw new JghAlertMessageException(errorMessage);

                #endregion

                #region move forward

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshRepositoryDataGridAsync();

                HeadlineItem = item; // this only place HeadlineItem ever gets changed, except ... BeInitialisedFromPageCodeBehindAsync()

                switch (kindOfGunStartEntryEnum)
                {
                    case EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual:
                        await TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.ChangeTextAsync("");
                        break;
                    case EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup:
                        await CboLookupGroupLabelForGroupStartVm.ChangeSelectedIndexAsync(-1);
                        break;
                }

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

        #region CreateTimeStampForTimingMatSignalButtonOnClickAsync

        private bool CreateTimeStampForTimingMatSignalButtonOnClickCanExecute(string xamlCommandParameter)
        {
            return CreateTimeStampForTimingMatSignalButtonVm.IsAuthorisedToOperate;
        }

        private async void CreateTimeStampForTimingMatSignalButtonOnClickExecuteAsync(string xamlCommandParameter)
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[CreateTimeStampForTimingMatSignalButtonOnClickExecuteAsync]";

            try
            {
                if (!CreateTimeStampForTimingMatSignalButtonOnClickCanExecute(xamlCommandParameter))
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator("Creating new item...");

                DeadenGui();

                await CreateTimeStampForTimingMatSignalButtonOnClickAsyncV2(xamlCommandParameter);

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

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        protected async Task<bool> CreateTimeStampForTimingMatSignalButtonOnClickAsyncV2(string xamlCommandParameter)
        {
            const string failure = "Unable to add a new item of timestamp data.";
            const string locus = "[CreateTimeStampForTimingMatSignalButtonOnClickAsync]";

            // we have arrived here because the user clicked one of several clock timer buttons. she might or might not have entered a bib but a timestamp is automatically entered

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                #region add item to RepositoryOfHubStyleEntries

                var participantId = string.Empty;

                if (JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(xamlCommandParameter, "ForEnteringIndividualIdForTimeStampOfTimingMatBeepCreateTextVm01"))
                    participantId = TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm01.Text;
                else if (JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(xamlCommandParameter, "ForEnteringIndividualIdForTimeStampOfTimingMatBeepCreateTextVm02"))
                    participantId = TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm02.Text;

                if (string.IsNullOrWhiteSpace(participantId)) participantId = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());
                // NB. we use Symbols.SymbolUnspecified enum string in many places to decide courses of action and shortcuts when IDs are missing

                if (!JghString.IsOnlyLettersOrDigitsOrHyphen(participantId))
                    throw new JghAlertMessageException("participant ID must consist of letters, digits, or hyphens (or be initially blank).");

                var kindOfEntryEnum = EnumStrings.KindOfEntryIsTimeStampForTimingMatSignal;

                ButtonClickCounter += 1;

                var touchedBy = SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem?.UserName;

                var thisTimeStampBinaryFormat = JghDateTime.RoundedToTenthOfSecond(DateTime.Now).ToBinary();

                var item = TimeStampHubItem.Create(ButtonClickCounter, participantId, string.Empty, kindOfEntryEnum, thisTimeStampBinaryFormat, touchedBy);

                var didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(item, out var errorMessage);

                if (didRunToCompletion == false) throw new JghAlertMessageException(errorMessage);

                #endregion

                #region move forward

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshRepositoryDataGridAsync();

                HeadlineItem = item; // this only place HeadlineItem ever gets changed, except ... BeInitialisedFromPageCodeBehindAsync()

                if (JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(xamlCommandParameter, "ForEnteringIndividualIdForTimeStampOfTimingMatBeepCreateTextVm01"))
                    await TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm01.ChangeTextAsync(string.Empty);
                else if (JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(xamlCommandParameter, "ForEnteringIndividualIdForTimeStampOfTimingMatBeepCreateTextVm02"))
                    await TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm02.ChangeTextAsync(string.Empty);

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

        #region CreateClonedTimeStampButtonOnClickAsync

        private bool CreateTimeCloneStampButtonOnClickCanExecute()
        {
            return CreateTimeStampCloneButtonVm.IsAuthorisedToOperate;
        }

        private async void CreateTimeStampCloneButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[CreateClonedTimeStampButtonOnClickExecuteAsync]";

            try
            {
                if (!CreateTimeCloneStampButtonOnClickCanExecute())
                    return;

                GlobalProgressIndicatorVm.OpenProgressIndicator("Creating new item...");

                DeadenGui();

                var messageOk = await CreateClonedTimeStampButtonOnClickAsync();

                EnlivenGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowOkAsync(messageOk);
            }

            #region try catch

            catch (Exception ex)
            {
                RestoreGui();

                if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                    EvaluateGui();

                GlobalProgressIndicatorVm.FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                GlobalProgressIndicatorVm.CloseProgressIndicator();
            }

            #endregion
        }

        protected async Task<string> CreateClonedTimeStampButtonOnClickAsync()
        {
            const string failure = "Unable to clone a new item of timestamp data.";
            const string locus = "[CreateClonedTimeStampButtonOnClickAsync]";

            // we have arrived here because the user clicked one of several clock timer buttons. she might or might not have entered a bib but a timestamp is automatically entered

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                #region add item to RepositoryOfHubStyleEntries

                var participantId = TextBoxForEnteringIndividualIdForTimeStampCloneVm.Text;

                if (string.IsNullOrWhiteSpace(participantId)) participantId = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());
                // NB. we use Symbols.SymbolUnspecified enum string in many places to decide courses of action and shortcuts when IDs are missing

                if (!JghString.IsOnlyLettersOrDigitsOrHyphen(participantId))
                    throw new JghAlertMessageException("participant ID must consist of letters, digits, or hyphens (or be initially blank).");

                var kindOfEntryEnum = EnumStrings.KindOfEntryIsTimeStampForTimingMatSignal;

                ButtonClickCounter += 1;

                var touchedBy = SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem?.UserName;


                if (EditTemplateForRepositoryItemBeingEdited == null || EditTemplateForRepositoryItemBeingEdited.IsEmpty())
                    throw new JghAlertMessageException("No timestamp info available to use. Please select a timestamp before proceeding.");

                var timeStampBinaryFormat = EditTemplateForRepositoryItemBeingEdited.GetBinaryTimeStamp();

                var item = TimeStampHubItem.Create(ButtonClickCounter, participantId, string.Empty, kindOfEntryEnum, timeStampBinaryFormat, touchedBy);


                var didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(item, out var errorMessage);

                if (didRunToCompletion == false) throw new JghAlertMessageException(errorMessage);

                #endregion

                #region move forward

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshRepositoryDataGridAsync();

                HeadlineItem = item;

                await TextBoxForEnteringIndividualIdForTimeStampCloneVm.ChangeTextAsync(string.Empty);

                #endregion

                return $"{JghDateTime.ToTimeLocalhhmmssf(item.TimeStampBinaryFormat)} for {item.Bib} confirmed.";
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region RejectClonedTimeStampButtonOnClickAsync

        private bool RejectTimeStampCloneButtonOnClickCanExecute()
        {
            return RejectTimeStampCloneButtonVm.IsAuthorisedToOperate;
        }

        private async void RejectTimeStampCloneButtonOnClickExecuteAsync()
        {
            await TextBoxForEnteringIndividualIdForTimeStampCloneVm.ChangeTextAsync("");

            CreateTimeStampCloneButtonVm.IsAuthorisedToOperate = false;
        }

        #endregion

        #region CommitDataInMemoryIntoLocalStorageButtonOnClickAsync

        protected override async Task<string> CommitDataInMemoryIntoLocalStorageButtonOnClickAsync()
        {
            const string failure = "Unable to commit clock data into local storage.";
            const string locus = "[CommitDataInMemoryIntoLocalStorageButtonOnClickAsync]";

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                if (RepositoryOfHubStyleEntries.Count == 0)
                    throw new JghAlertMessageException(
                        "Nothing to backup. Memory is empty.");

                var commitCount = await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshAllDataGridsAndListViewsAsync(); // display effect of backup on contents of cache

                if (commitCount.Item2 == 0)
                    return JghString.ConcatAsParagraphs(
                        $"Backup complete. Memory cache backed up in local storage. Backup contains {RepositoryOfHubStyleEntries.Count} data entries.");

                return JghString.ConcatAsParagraphs(
                    $"Full backup complete. All data in memory saved on this machine. Backup contains {RepositoryOfHubStyleEntries.Count} data entries, including {commitCount.Item2} new saves.");
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickAsync

        protected override async Task<string> PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickAsync()
        {
            const string failure = "Unable to push data item to cloud (and commit locally).";
            const string locus = "[PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickAsync]";

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region check connection

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghAlertMessageException(StringsPortal.NoConnection);

                PushDataIncrementallyFromMemoryToRemoteHubCts = new CancellationTokenSource();

                #endregion

                #region get parameters ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var cloudDataLocation =
                    TimeKeepingSvcAgent.MakeDataLocationForStorageOfClockDataOnRemoteHub(currentSeries, currentEvent);


                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                #region prepare data

                var entriesInRepositoryNotPushedPreviously = RepositoryOfHubStyleEntries.GetAllEntriesAsRawData()
                    .Where(z => z.IsStillToBePushed)
                    .ToArray();

                if (!entriesInRepositoryNotPushedPreviously.Any())
                    throw new JghAlertMessageException(JghString.ConcatAsLines(
                        "Nothing new to push to remote hub."));

                var numberOfEntriesNotPushedPreviously = entriesInRepositoryNotPushedPreviously.Count(z => z.IsStillToBePushed);

                var whenPushed = DateTime.Now.ToBinary();

                #endregion

                #region push

                var scratchPadOfClonesToBePushed = new List<TimeStampHubItem>();

                foreach (var item in entriesInRepositoryNotPushedPreviously)
                {
                    var clone = item.ToShallowMemberwiseClone();

                    clone.IsStillToBePushed = false;

                    clone.WhenPushedBinaryFormat = whenPushed;

                    scratchPadOfClonesToBePushed.Add(clone);
                }

                var uploadReportOfItemsPushedForTheFirstTimeEver = await
                    _timeKeepingSvcAgent.PostTimeStampItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2,
                        scratchPadOfClonesToBePushed.ToArray(),
                        PushDataIncrementallyFromMemoryToRemoteHubCts.Token);

                #endregion

                #region success - move on

                foreach (var entry in entriesInRepositoryNotPushedPreviously)
                {
                    entry.IsStillToBePushed = false;
                    entry.WhenPushedBinaryFormat = whenPushed;
                }

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshAllDataGridsAndListViewsAsync(); // display effect of Push on contents of cache

                var messageOk = numberOfEntriesNotPushedPreviously switch
                {
                    0 => "Nothing pushed. Cache was empty.",
                    1 => "Success. A copy of a single entry was pushed to the hub.",
                    _ => "Success. Copies of multiple entries were pushed to the hub."
                };

                var reportRegardingItemsNeverPushedBefore = JghString.ConcatAsParagraphs(messageOk,
                    "The upload service confirmed the name of the destination on the hub and the number of items saved : -",
                    $"{uploadReportOfItemsPushedForTheFirstTimeEver}");

                return reportRegardingItemsNeverPushedBefore;

                #endregion
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                PushDataIncrementallyFromMemoryToRemoteHubCts.Dispose();
            }

            #endregion
        }

        #endregion

        #region ForcePushAllDataInMemoryToRemoteHubButtonOnClickAsync

        protected override async Task<string> ForcePushAllDataInMemoryToRemoteHubButtonOnClickAsync()
        {
            const string failure = "Unable to push data items to cloud";
            const string locus = "[ForcePushAllDataInMemoryToRemoteHubButtonOnClickAsync]";

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region doublecheck

                if (!NetworkInterface.GetIsNetworkAvailable())
                    throw new JghAlertMessageException(StringsPortal.NoConnection);

                ForcePushAllDataInMemoryToRemoteHubCts = new CancellationTokenSource();

                #endregion

                #region get parameters ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var cloudDataLocation =
                    TimeKeepingSvcAgent.MakeDataLocationForStorageOfClockDataOnRemoteHub(currentSeries, currentEvent);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                #region prepare data

                var allEntriesInRepository = RepositoryOfHubStyleEntries.GetAllEntriesAsRawData();

                if (!allEntriesInRepository.Any())
                    throw new JghAlertMessageException(JghString.ConcatAsLines(
                        "Nothing to push to remote hub. Cache is empty."));

                var numberOfEntriesNotPushedPreviously = allEntriesInRepository.Count(z => z.IsStillToBePushed);

                var whenPushed = DateTime.Now.ToBinary();

                #endregion

                #region push

                var scratchPadOfClonesToBePushed = new List<TimeStampHubItem>();

                foreach (var item in allEntriesInRepository)
                {
                    var clone = item.ToShallowMemberwiseClone();

                    clone.IsStillToBePushed = false;

                    clone.WhenPushedBinaryFormat = whenPushed;

                    scratchPadOfClonesToBePushed.Add(clone);
                }

                var uploadReport = await
                    _timeKeepingSvcAgent.PostTimeStampItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2,
                        scratchPadOfClonesToBePushed.ToArray(),
                        ForcePushAllDataInMemoryToRemoteHubCts.Token);

                #endregion

                #region success - move on

                foreach (var entry in allEntriesInRepository)
                {
                    entry.IsStillToBePushed = false;
                    entry.WhenPushedBinaryFormat = whenPushed;
                }

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshAllDataGridsAndListViewsAsync();

                return JghString.ConcatAsParagraphs("Everything on this machine was pushed to the remote hub.",
                    $"{allEntriesInRepository.Length} timestamp-related entries were on this machine.",
                    $"{numberOfEntriesNotPushedPreviously} of these entries had not been pushed previously.",
                    "The upload service confirmed the name of the destination on the hub and the number of items saved : -",
                    $"{uploadReport}");

                #endregion
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }
            finally
            {
                ForcePushAllDataInMemoryToRemoteHubCts.Dispose();
            }

            #endregion
        }

        #endregion

        #region DeleteAllDataButtonOnClickAsync

        protected override async Task<string> DeleteAllDataButtonOnClickAsync()
        {
            const string failure = "Unable to clear all data.";
            const string locus = "[DeleteAllDataButtonOnClickAsync]";

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                if (!DeleteAllDataInMemoryProtectionButtonVm.IsChecked)
                    throw new JghAlertMessageException(
                        "Are you sure you wish to delete all data? Unlock if you wish to proceed.");

                var clearedCount = RepositoryOfHubStyleEntries.ClearCache();

                //await ZeroiseDisplayOfSimpleElementsOfGuiAsync();

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshAllDataGridsAndListViewsAsync();

                DeleteAllDataInMemoryProtectionButtonVm.IsChecked = false;

                return clearedCount == 0 ? "No data to delete. Cache is empty." : $"{clearedCount} items deleted. Cache is empty.";
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region DeleteAllDataInMemoryCacheButtonOnClickAsync

        protected override async Task<string> DeleteAllDataInMemoryCacheButtonOnClickAsync()
        {
            const string failure = "Unable to clear all data.";
            const string locus = "[DeleteAllDataInMemoryCacheButtonOnClickAsync]";

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                if (!DeleteAllDataInMemoryProtectionButtonVm.IsChecked)
                    throw new JghAlertMessageException(
                        "Are you sure you wish to delete all data in memory? Unlock if you wish to proceed.");

                var clearedCount = RepositoryOfHubStyleEntries.ClearCache();

                await RepositoryOfHubStyleEntries.ClearLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await ZeroiseDisplayOfSimpleElementsOfGuiAsync();

                await RefreshAllDataGridsAndListViewsAsync();

                DeleteAllDataInMemoryProtectionButtonVm.IsChecked = false;

                return clearedCount == 0 ? "Cache and local storage are empty." : $"{clearedCount} entries cleared out. Cache and local storage are empty.";
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region DeleteAllDataInLocalStorageButtonOnClickAsync

        public override async Task<string> DeleteAllDataInLocalStorageButtonOnClickAsync()
        {
            const string failure = "Unable to clear all data.";
            const string locus = "[DeleteAllDataInLocalStorageButtonOnClickAsync]";

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                await RepositoryOfHubStyleEntries.ClearLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshAllDataGridsAndListViewsAsync();

                return "Local storage is empty.";
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region RefreshRepositoryDataGridAsync - heap powerful

        public override async Task RefreshRepositoryDataGridAsync()
        {
            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
            var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

            HeadlineItem = RepositoryOfHubStyleEntries.GetBestGuessHeadlineEntry();

            #region prepare data - i.e. entries

            TimeStampHubItem[] entries;

            if (MustDisplayModifiedEntriesAsWell)
            {
                entries = RepositoryOfHubStyleEntries.GetAllEntriesAsRawData()
                    .Where(z => z != null).ToArray();

                foreach (var timeStampHubItem in entries) timeStampHubItem.Comment = string.Empty;
            }
            else
            {
                entries = RepositoryOfHubStyleEntries.GetYoungestDescendentOfEachOriginatingItemGuidIncludingDitches()
                    .Where(z => z != null).ToArray();

                foreach (var timeStampHubItem in entries) timeStampHubItem.Comment = string.Empty;

                // it's eccentric, and ostensibly premature to do the following here, however unfortunately we can exercise this user option only iff we are dealing with GetSurfaceViewOfUnderlyingHubItemRepository()

                if (MustHighlightTimingMatEntriesThatOccurInClusters)
                {
                    var tightlyClusteredEntriesForEditing =
                        FilterAndInsertACommentInTimingMatEntriesThatOccurInTightClusters(entries.ToArray(),
                            JghConvert.ToDouble(CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm.CurrentItem?.Label),
                            JghConvert.ToInt32(CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm.CurrentItem?.Label));

                    if (MustOnlyShowTimingMatEntriesThatOccurInClusters)
                    {
                        entries = tightlyClusteredEntriesForEditing;
                    }
                    else
                    {
                        // laboriously merge the tightly clustered entries back into the full list

                        var dictionaryOfEntriesForEditing = entries.ToDictionary(timeStampHubItem => timeStampHubItem.GetBothGuids());

                        var dictionaryOfTightlyClusteredEntriesForEditing = tightlyClusteredEntriesForEditing.ToDictionary(timeStampHubItem => timeStampHubItem.GetBothGuids());

                        foreach (var clusteredTimeStampHubItemKvp in dictionaryOfTightlyClusteredEntriesForEditing) dictionaryOfEntriesForEditing[clusteredTimeStampHubItemKvp.Key] = clusteredTimeStampHubItemKvp.Value; // i.e. overwrite

                        entries = dictionaryOfEntriesForEditing.Select(kvp => kvp.Value).ToArray();
                    }
                }
            }

            if (MustDisplayGunStartsOnly)
                entries = entries
                    .Where(z => z.RecordingModeEnum
                        is EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody
                        or EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup
                        or EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual).ToArray();

            entries = entries
                .OrderByDescending(z => z.WhenTouchedBinaryFormat).ToArray();

            var displayObjects = entries
                .Select(TimeStampHubItemDisplayObject.FromModel)
                .ToArray();

            // allow user to not do dictionary/database lookup if things are slow. O(n). Empirically determined this is too slow for rapid successive clock clicks for dead heats.
            if (MustDisplayParticipantProfileInfoInTimeStamps)
                GetParticipantInfoAndInsertInEntriesAndComments(displayObjects, currentEvent);

            await InsertACommentToHighlightAllModifiedEntriesAsync(displayObjects);

            if (!string.IsNullOrWhiteSpace(ForEnteringMultipleIdentifiersForDataGridRowFilter.Text))
            {
                var multipleIdentifiers = ForEnteringMultipleIdentifiersForDataGridRowFilter.Text.Split(',');

                multipleIdentifiers = JghString.ToTrimmedLowerCaseStrings(multipleIdentifiers);

                displayObjects = displayObjects
                    .Where(displayObject => multipleIdentifiers.Contains(JghString.TmLr(displayObject.Bib)))
                    .ToArray();
            }

            if (!string.IsNullOrWhiteSpace(ForEnteringMultipleUserNamesOfPeopleWhoDidTheDataEntriesForDataGridRowFilter.Text))
            {
                var multipleIdentifiers = ForEnteringMultipleUserNamesOfPeopleWhoDidTheDataEntriesForDataGridRowFilter.Text.Split(',');

                multipleIdentifiers = JghString.ToTrimmedLowerCaseStrings(multipleIdentifiers);

                displayObjects = displayObjects
                    .Where(displayObject => multipleIdentifiers.Contains(JghString.TmLr(displayObject.TouchedBy)))
                    .ToArray();
            }

            var desiredRaceGroup = CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.CurrentItem?.Label;

            if (!string.IsNullOrWhiteSpace(desiredRaceGroup) && desiredRaceGroup != Symbols.SymbolNotApplicable)
                displayObjects = displayObjects
                    .Where(z => JghString.AreEqualIgnoreOrdinalCase(z.RaceGroup, desiredRaceGroup))
                    .ToArray();

            if (MustDisplayCommentedRowsOnly) displayObjects = displayObjects.Where(z => !string.IsNullOrWhiteSpace(z.Comment)).ToArray();

            #endregion

            #region populate the DataGridDesigner to obtain array of column specification items for RadDataGrid control in a PresentationService

            displayObjects = MustDisplayEarliestFirst ? displayObjects.OrderBy(z => z.TimeStamp).ToArray() : displayObjects.OrderByDescending(z => z.WhenTouched).ToArray();


            DataGridDesignerForItemsInRepository.InitialiseDesigner(currentSeries, currentEvent,
                EnumStrings.TimeStampEntryAsRawDataEntryColumnFormat, displayObjects);

            var columnSpecificationItemsForDisplayObjects = MustDisplayAbridgedColumnsOnly
                ? DataGridDesignerForItemsInRepository.GetNonEmptyColumnSpecificationItemsForAbridgedTimeStampHubItemDisplayObjects()
                : DataGridDesignerForItemsInRepository.GetNonEmptyColumnSpecificationItemsForTimeStampHubItemDisplayObjects();

            #endregion

            #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

            await
                TimeStampEntriesInMemoryCacheDataGridPresentationService
                    .GenerateDataGridColumnCollectionManuallyAsync(columnSpecificationItemsForDisplayObjects); // essential if using a datagrid, empty method if uploading a pretty-printed html document to be viewed in a browser 

            #endregion

            #region populate the Presenter to provide a datacontext and hence row collection for the PresentationService

            await DataGridOfItemsInRepository.PopulatePresenterAsync(currentSeries, currentEvent,
                null, null, EnumStrings.RowsAreUnGrouped, EnumStrings.TimeStampEntryAsRawDataEntryColumnFormat, null, displayObjects);

            #endregion

            #region Populate SearchFunctionVm

            var searchQuerySuggestions = TimeStampHubItemDisplayObject.ToSearchQuerySuggestionItem(displayObjects);

            SearchFunctionVm.PopulateItemsSource(searchQuerySuggestions);

            #endregion
        }

        #endregion

        #region RefreshLocalStorageDataGridAsync

        public override async Task RefreshLocalStorageDataGridAsync()
        {
            #region get ready

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
            var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

            var localDataLocation =
                MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

            #endregion

            #region Refresh DataGrid for Local storage

            #region prepare data

            var rows = (await RepositoryOfHubStyleEntries.GetFromLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2))
                .Where(z => z != null)
                .Select(TimeStampHubItemDisplayObject.FromModel)
                .ToArray();

            // allow user to not do dictionary/database lookup if things are slow. O(n). Empirically determined this is too slow for rapid successive clock clicks for dead heats.
            if (MustDisplayParticipantProfileInfoInTimeStamps)
                GetParticipantInfoAndInsertInEntriesAndComments(rows, currentEvent);

            #endregion

            #region populate the DataGridDesigner to obtain array of column specification items for RadDataGrid control

            DataGridDesignerForItemsInLocalStorage.InitialiseDesigner(currentSeries, currentEvent,
                EnumStrings.TimeStampEntryAsRawDataEntryColumnFormat, rows);

            var nonEmptyColumnSpecificationItemsForEntriesInLocalStorage = DataGridDesignerForItemsInLocalStorage
                .GetNonEmptyColumnSpecificationItemsForTimeStampHubItemDisplayObjects(); // NB. we use a standard format for raw data

            #endregion

            #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

            await TimeStampEntriesInLocalLocalStorageDataGridPresentationService
                .GenerateTableColumnCollectionManuallyAsync(nonEmptyColumnSpecificationItemsForEntriesInLocalStorage); // essential if using a datagrid, empty method if uploading a pretty-printed html document to be viewed in a browser 

            #endregion

            #region populate the Presenter to provide a datacontext and hence row collection for the PresentationService

            await DataGridOfItemsInLocalStorage.PopulatePresenterAsync(currentSeries, currentEvent,
                null, null, EnumStrings.RowsAreUnGrouped, EnumStrings.TimeStampEntryAsRawDataEntryColumnFormat, null, rows);

            #endregion

            #endregion
        }

        #endregion

        #region RefreshRemoteHubDataGridAsync

        public override Task RefreshRemoteHubDataGridAsync()
        {
            throw new NotImplementedException("RefreshRemoteHubDataGridAsync() not implemented");
        }

        #endregion

        #region RefreshAllDataGridsAndListViewsAsync

        public override async Task RefreshAllDataGridsAndListViewsAsync()
        {
            //slow method unfortunately: O(n) if GetShortListOfMostRecentEntries() or GetAllEntriesAsRawData() triggers a re-sort
            HeadlineItem = RepositoryOfHubStyleEntries.GetBestGuessHeadlineEntry();

            //to do: comment this out - we only use it for debugging
            //await RefreshLocalStorageDataGridAsync();

            await RefreshRepositoryDataGridAsync();

            // Note: if we want to speed up this method we could omit this, and only do it when we navigate to the page, or press refresh on that page. but on balance I prefer to keep it here so that we know what's happening where and when
            await RefreshDataGridOfSplitIntervalsPerPersonAsync(LocalRepositoryOfParticipants);
        }

        #endregion

        #region RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickAsync

        private bool RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickCanExecute()
        {
            return RefreshDataGridOfSplitIntervalsPerPersonButtonVm.IsAuthorisedToOperate;
        }

        private async void RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickExecuteAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickExecuteAsync]";

            try
            {
                GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____processing);

                var messageOk = await RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickOrchestrateAsync();

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

        public async Task<string> RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickOrchestrateAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickOrchestrateAsync]";

            try
            {
                if (!RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickCanExecute())
                    return string.Empty;

                DeadenGui();

                var messageOk = await RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickAsync();

                EnlivenGui();

                return messageOk;
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

        protected async Task<string> RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickAsync()
        {
            const string failure = "Unable to clean and refresh all data.";
            const string locus = "[RefreshDataGridOfSplitIntervalsPerPersonButtonOnClickAsync]";

            try
            {
                await RefreshDataGridOfSplitIntervalsPerPersonAsync(LocalRepositoryOfParticipants);

                return string.Empty;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        private async Task RefreshDataGridOfSplitIntervalsPerPersonAsync(IRepositoryOfHubStyleEntries<ParticipantHubItem> repositoryOfParticipantHubItems)
        {
            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
            var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

            #region prepare data

            var repositoryOfSplitIntervals = new RepositoryOfSplitIntervalsPerParticipant();

            repositoryOfSplitIntervals.LoadRepository(currentEvent, RepositoryOfHubStyleEntries, repositoryOfParticipantHubItems);

            var splitIntervalsForAllPeople =
                repositoryOfSplitIntervals.GetTimeStampsAsSplitIntervalsPerPersonInRankOrder(
                    JghConvert.ToInt32(CboLookupAnomalousThresholdForTooManySplitsVm.CurrentItem?.Label),
                    JghConvert.ToInt32(CboLookupAnomalousThresholdForTooFewSplitsVm.CurrentItem?.Label),
                    JghConvert.ToDouble(CboLookupAnomalousThresholdForTooBriefSplitsVm.CurrentItem?.Label));


            SplitIntervalConsolidationForParticipantDisplayObject[] displayObjects;

            if (MustDisplayTimeStampsNotIntervalsForTxxInSplitIntervalsPerPersonItems)
                displayObjects = splitIntervalsForAllPeople
                    .Where(z => z != null)
                    .Select(SplitIntervalConsolidationForParticipantDisplayObject.FromModelShowingTimeStampTxx)
                    .ToArray();
            else
                displayObjects = splitIntervalsForAllPeople
                    .Where(z => z != null)
                    .Select(SplitIntervalConsolidationForParticipantDisplayObject.FromModel)
                    .ToArray();


            if (!string.IsNullOrWhiteSpace(TextBoxForEnteringMultipleIndividualIds.Text))
            {
                var multipleIds = TextBoxForEnteringMultipleIndividualIds.Text.Split(',');

                var cleanIds = JghString.ToTrimmedLowerCaseStrings(multipleIds);

                displayObjects = displayObjects
                    .Where(displayObject => cleanIds.Contains(JghString.TmLr(displayObject.Bib)))
                    .ToArray();
            }

            var desiredRaceGroup = CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.CurrentItem?.Label;

            if (!string.IsNullOrWhiteSpace(desiredRaceGroup) && desiredRaceGroup != Symbols.SymbolNotApplicable)
                displayObjects = displayObjects
                    .Where(z => JghString.AreEqualIgnoreOrdinalCase(z.RaceGroup, desiredRaceGroup))
                    .ToArray();

            if (MustOnlyShowCommentedRowsInSplitIntervalsPerPersonDataGrid)
                displayObjects = displayObjects
                    .Where(z => !string.IsNullOrWhiteSpace(z.Comment))
                    .ToArray();

            #endregion

            #region populate the DataGridDesigner to obtain array of column specification items for RadDataGrid control

            DataGridDesignerForSplitIntervalsPerPerson.InitialiseDesigner(currentSeries, currentEvent,
                EnumStrings.SplitIntervalItemAsSimpleResultColumnFormat, displayObjects);

            var columnSpecificationItemsForSplits = MustDisplayAbridgedColumnsInSplitIntervalsPerPerson
                ? DataGridDesignerForSplitIntervalsPerPerson.GetNonEmptyColumnSpecificationItemsForAbridgedSplitIntervalsPerPersonDisplayObjects()
                : DataGridDesignerForSplitIntervalsPerPerson.GetNonEmptyColumnSpecificationItemsForSplitIntervalsPerPersonDisplayObjects();

            #endregion

            #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

            await
                SplitIntervalsPerPersonDataGridPresentationService.GenerateDataGridColumnCollectionManuallyAsync(
                    columnSpecificationItemsForSplits); // essential if using a datagrid, empty method if uploading a pretty-printed html document to be viewed in a browser 

            #endregion

            #region populate the Presenter to provide a datacontext and hence row collection for the PresentationService

            await DataGridOfSplitIntervalsPerPerson.PopulatePresenterAsync(currentSeries, currentEvent,
                null, null, EnumStrings.RowsAreUnGrouped, EnumStrings.SplitIntervalItemAsSimpleResultColumnFormat, null, displayObjects);

            #endregion

            #region populate SearchFunctionForSplitIntervalsPerPersonVm for TimeStampsAsResults

            var searchQuerySuggestions = SplitIntervalConsolidationForParticipantDisplayObject.ToSearchQuerySuggestionItem(displayObjects);

            SearchFunctionForSplitIntervalsPerPersonVm.PopulateItemsSource(searchQuerySuggestions);

            #endregion
        }

        #endregion

        #region DataGridOfItemsInRepositoryOnSelectionChangedAsync - this is where we launch template editing

        protected override async Task<bool> DataGridOfItemsInRepositoryOnSelectionChangedAsync()
        {
            const string failure = "Unable to execute ICommand Execute method.";
            const string locus = "[DataGridOfItemsInRepositoryOnSelectionChangedAsync]";

            try
            {
                // get ready

                DataGridOfItemsInRepository.SaveSelectedItemAsLastKnownGood();

                if (DataGridOfItemsInRepository.SelectedItem == null)
                {
                    await RejectItemBeingEditedButtonOnClickAsync();

                    return true;
                }

                // bale if selectedItem is not the most recent item

                var key = DataGridOfItemsInRepository.SelectedItem.GetSourceItemBothGuids();

                var sourceItem = RepositoryOfHubStyleEntries.GetEntryByBothGuidsAsKey(key);

                if (!RepositoryOfHubStyleEntries.IsMostRecentEntryWithSameOriginatingItemGuid(sourceItem))
                {
                    await RejectItemBeingEditedButtonOnClickAsync();

                    var mostRecent = RepositoryOfHubStyleEntries.GetYoungestDescendentWithSameOriginatingItemGuid(
                        sourceItem.OriginatingItemGuid);

                    var mostRecentAsDisplayObject = TimeStampHubItemDisplayObject.FromModel(mostRecent);

                    var label = TimeStampHubItemDisplayObject.ToLabel(mostRecentAsDisplayObject);

                    var errorMessage = JghString.ConcatAsLines(
                        "This item has been modified. To edit, search for : -",
                        label);

                    throw new JghAlertMessageException(errorMessage);
                }

                // success. all is well. on we go

                await EditTemplateForRepositoryItemBeingEdited.PopulateWithItemBeingModifiedAsync(sourceItem);

                EditTemplateForRepositoryItemBeingEdited.IsAuthorisedToOperate = true;

                LastKnownGoodEditTemplateForRepositoryItemBeingEdited = EditTemplateForRepositoryItemBeingEdited;

                RejectRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = true;

                MustDisplayEditTemplateForRepositoryItemBeingEdited = true;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

            return await Task.FromResult(true);
        }

        #endregion

        #region AcceptItemBeingEditedButtonOnClickAsync

        protected override async Task<string> AcceptItemBeingEditedButtonOnClickAsync()
        {
            const string failure = "Unable to accept a modification into cache.";
            const string locus = "[AcceptItemBeingEditedButtonOnClickAsync]";

            async Task Reset()
            {
                await DataGridOfItemsInRepository.ChangeSelectedItemToNullAsync();

                DataGridOfItemsInRepository.SaveSelectedItemAsLastKnownGood();

                await EditTemplateForRepositoryItemBeingEdited.ZeroiseAsync();

                AcceptRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = false;

                RejectRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = false;
            }

            try
            {
                #region bale if something is out of whack

                ThrowIfWorkSessionNotProperlyInitialised();

                ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

                #endregion

                #region null checks

                if (DataGridOfItemsInRepository.SelectedItem == null)
                    throw new JghAlertMessageException("Nothing selected. Select an item.");

                if (EditTemplateForRepositoryItemBeingEdited.OneOrMoreEntriesAreInvalid(out var dirtyErrorMessage))
                    throw new JghInvalidParticularsException(dirtyErrorMessage);

                if (!EditTemplateForRepositoryItemBeingEdited.WasTouched)
                    throw new JghAlertMessageException("Nothing was touched.");

                if (EditTemplateForRepositoryItemBeingEdited.AllEntriesAreUnchangedSinceInitiallyPopulated())
                    throw new JghAlertMessageException("Nothing was changed.");

                #endregion

                #region get ready

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
                var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

                var localDataLocation =
                    MakeDataLocationForClockDataInLocalStorage(currentSeries, currentEvent);

                #endregion

                #region add item

                var key = DataGridOfItemsInRepository.SelectedItem.GetSourceItemBothGuids();

                var sourceItem = RepositoryOfHubStyleEntries.GetEntryByBothGuidsAsKey(key);

                var touchedBy = SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem?.UserName;

                var editedItem = EditTemplateForRepositoryItemBeingEdited.MergeEditsBackIntoItemBeingModified(sourceItem, touchedBy);

                if (RepositoryOfHubStyleEntries.ContainsEntryWithMatchingBothGuids(editedItem))
                    throw new JghAlertMessageException("Already exists. Duplicates not allowed.");

                var initialCount = RepositoryOfHubStyleEntries.Count;

                // click counter is only changed in three places. CreateTimeStampForGunStartButtonOnClickAsync(), AcceptItemBeingEditedButtonOnClickAsync(), and BeInitialisedFromPageCodeBehindAsync()
                ButtonClickCounter += 1;

                editedItem.ClickCounter = ButtonClickCounter;

                var didRunToCompletion =
                    RepositoryOfHubStyleEntries.TryAddNoDuplicate(editedItem, out var errorMessage);

                if (!didRunToCompletion)
                    throw new JghAlertMessageException(errorMessage);

                #endregion

                #region move forward

                var subsequentCount = RepositoryOfHubStyleEntries.Count;

                await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                await RefreshRepositoryDataGridAsync();

                await Reset();

                return
                    $"Added. Memory contained {initialCount} data entries before and {subsequentCount} entries after.";

                #endregion
            }

            #region try catch

            catch (JghInvalidParticularsException e)
            {
                var exx = new JghAlertMessageException(e.Message);

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);
            }
            catch (Exception ex)
            {
                // NB. if anything at all out of the ordinary happens, including if an JghAlertMessageException is intentionally thrown, always reset
                await Reset();

                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region RejectItemBeingEditedButtonOnClick

        protected override async Task<string> RejectItemBeingEditedButtonOnClickAsync()
        {
            const string failure = "Unable to reject an edited template of a data item.";
            const string locus = "[RejectItemBeingEditedButtonOnClick]";

            async Task Reset()
            {
                await DataGridOfItemsInRepository.ChangeSelectedItemToNullAsync();

                DataGridOfItemsInRepository.SaveSelectedItemAsLastKnownGood();

                await EditTemplateForRepositoryItemBeingEdited.ZeroiseAsync();

                AcceptRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = false;

                RejectRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = false;
            } // always keep this defn identical to the same in AcceptItemBeingEditedButtonOnClickAsync()

            try
            {
                await Reset();

                return string.Empty;
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

        #region search

        public virtual async Task<string[]> OnShortlistOfQuerySuggestionsRequestedFromTheSearchUniverseForSplitIntervalItemsAsync(
            string queryText)
        {
            return await SearchFunctionForSplitIntervalsPerPersonVm.GetQueriesThatSatisfyUserEnteredHint(queryText);
        }

        public virtual async Task<bool> OnFinalSearchQuerySubmittedAsTextForSplitIntervalsAsync(string finalQuerySubmitted)
        {
            const string failure = "Unable to complete search operation.";
            const string locus = "[OnFinalSearchQuerySubmittedAsTextForSplitIntervalsAsync]";

            try
            {
                var searchResults = SearchFunctionForSplitIntervalsPerPersonVm.GetSubsetOfSearchQueryItemsThatEquateToSelectedSearchQuery(finalQuerySubmitted);

                await OrchestrateActionsToBeTakenWhenSearchOutcomeForSplitIntervalsIsToHandAsync(searchResults.Where(z => z != null).ToArray());
            }

            #region try catch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion

            return await Task.FromResult(true);
        }

        private async Task OrchestrateActionsToBeTakenWhenSearchOutcomeForSplitIntervalsIsToHandAsync(SearchQueryItem[] discoveredQueryItems)
        {
            const string failure = "Unable to take action in response to selection of item.";
            const string locus = "[OrchestrateActionsToBeTakenWhenSearchOutcomeForSplitIntervalsIsToHandAsync]";

            var prequels = 1; // arbitrary number of prequels to show in hte search results on the grid

            const string nothingFound = "Currently offscreen. Refresh screen. Expand the onscreen population. Do whatever is needed to surface the search target on the datagrid.";

            try
            {
                #region null checks

                if (DataGridOfSplitIntervalsPerPerson == null) return;

                if (discoveredQueryItems == null)
                    throw new JghNullObjectInstanceException(nameof(discoveredQueryItems));

                if (!discoveredQueryItems.Any() || discoveredQueryItems.FirstOrDefault() == null)
                    throw new JghAlertMessageException(nothingFound);

                #endregion

                var discoveredTag = discoveredQueryItems.FirstOrDefault()?.TagAsString; // the Tag is meant to contain the Guid of the item

                if (string.IsNullOrWhiteSpace(discoveredTag))
                    throw new JghAlertMessageException(nothingFound);

                var firstItemWithMatchingGuid = DataGridOfSplitIntervalsPerPerson.ItemsSource.FirstOrDefault(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(z.Guid, discoveredTag));

                if (firstItemWithMatchingGuid == null)
                    throw new JghAlertMessageException(nothingFound);

                var skip = Math.Max(DataGridOfSplitIntervalsPerPerson.ItemsSource.IndexOf(firstItemWithMatchingGuid) - prequels, 0);

                var truncatedItemsSource = new ObservableCollection<SplitIntervalConsolidationForParticipantDisplayObject>(DataGridOfSplitIntervalsPerPerson.ItemsSource.Skip(skip));

                await DataGridOfSplitIntervalsPerPerson.RefillItemsSourceAsync(truncatedItemsSource);
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region helpers

        protected override async Task PopulateCboLookupPrepopulatedCbosAsync()
        {
            await PopulateCboLookUpFileFormatsCboAsync(); // arbitrary location for this. ordinarily would do this in ctor but can't because async

            var dnxSymbols = new[]
            {
                new CboLookupItem {Label = Symbols.NullDnx},
                new CboLookupItem {Label = Symbols.SymbolDnf},
                new CboLookupItem {Label = Symbols.SymbolDns},
                new CboLookupItem {Label = Symbols.SymbolDq},
                new CboLookupItem {Label = Symbols.SymbolTbd}
            };

            await CboLookupDnxVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromModel(dnxSymbols));

            await CboLookupDnxVm.ChangeSelectedIndexToMatchItemLabelAsync(Symbols.NullDnx); // default

            var kindsOfGunStart = new[]
            {
                new CboLookupItem
                {
                    EnumString = EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody,
                    Label = "Start everyone (mass start of one or more races)"
                },
                new CboLookupItem
                {
                    EnumString = EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup,
                    Label = "Start a single race"
                },
                new CboLookupItem
                {
                    EnumString = EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual,
                    Label = "Start an individual"
                }
            };

            await CboLookupKindOfGunStartVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromModel(kindsOfGunStart));

            await CboLookupKindOfGunStartVm.ChangeSelectedIndexAsync(-1);

            var splitCount = new[]
            {
                new CboLookupItem {Label = Symbols.SymbolNotApplicable},
                new CboLookupItem {Label = "2"},
                new CboLookupItem {Label = "3"},
                new CboLookupItem {Label = "4"},
                new CboLookupItem {Label = "5"},
                new CboLookupItem {Label = "6"},
                new CboLookupItem {Label = "7"},
                new CboLookupItem {Label = "8"},
                new CboLookupItem {Label = "9"},
                new CboLookupItem {Label = "10"}
            };

            await CboLookupAnomalousThresholdForTooManySplitsVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromModel(splitCount));

            await CboLookupAnomalousThresholdForTooManySplitsVm.ChangeSelectedIndexAsync(0);


            await CboLookupAnomalousThresholdForTooFewSplitsVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromModel(splitCount));

            await CboLookupAnomalousThresholdForTooFewSplitsVm.ChangeSelectedIndexAsync(0);

            var minutesList = new List<CboLookupItem> {new() {Label = Symbols.SymbolNotApplicable}};

            for (var i = 1; i < 121; i++) minutesList.Add(new CboLookupItem {Label = i.ToString()});

            await CboLookupAnomalousThresholdForTooBriefSplitsVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromModel(minutesList.ToArray()));

            await CboLookupAnomalousThresholdForTooBriefSplitsVm.ChangeSelectedIndexAsync(0);

            var deltaSecondsList = new List<CboLookupItem> {new() {Label = Symbols.SymbolNotApplicable}};

            for (var i = 1; i < 31; i++) deltaSecondsList.Add(new CboLookupItem {Label = i.ToString()});

            await CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromModel(deltaSecondsList.ToArray()));

            await CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm.ChangeSelectedIndexAsync(2); // arbitrary default

            var clusterSizeThresholdList = new List<CboLookupItem>();

            for (var i = 2; i < 11; i++) clusterSizeThresholdList.Add(new CboLookupItem {Label = i.ToString()});

            await CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm.RefillItemsSourceAsync(CboLookupItemDisplayObject.FromModel(clusterSizeThresholdList.ToArray()));

            await CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm.ChangeSelectedIndexAsync(0); // arbitrary default
        }

        private async Task PopulateCboLookupGroupLabelForGroupStartAsync(EventItemDisplayObject eventItemAtMomentOfInitialisation)
        {
            if (eventItemAtMomentOfInitialisation == null)
            {
                await CboLookupGroupLabelForGroupStartVm.ZeroiseAsync();
                await CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.ZeroiseAsync();
                await CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.ZeroiseAsync();

                return;
            }


            var currentEvent = EventItemDisplayObject.ObtainSourceModel(eventItemAtMomentOfInitialisation);

            var raceSpecificationItems = currentEvent?.EventSettingsItem?.RaceSpecificationItems;

            if (raceSpecificationItems == null || !raceSpecificationItems.Any()) return;

            var raceLookUpItems = raceSpecificationItems.Where(raceSpecificationItemVm => raceSpecificationItemVm != null)
                .Where(raceSpecificationItemVm => !string.IsNullOrWhiteSpace(raceSpecificationItemVm.Label))
                .Select(raceSpecificationItemVm => new CboLookupItemDisplayObject {Label = raceSpecificationItemVm.Label}).ToList();


            await CboLookupGroupLabelForGroupStartVm.RefillItemsSourceAsync(raceLookUpItems);

            await CboLookupGroupLabelForGroupStartVm.ChangeSelectedIndexAsync(-1);

            raceLookUpItems.Reverse();

            raceLookUpItems.Add(new CboLookupItemDisplayObject {Label = Symbols.SymbolNotApplicable});

            raceLookUpItems.Reverse();

            await CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.RefillItemsSourceAsync(raceLookUpItems);

            await CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.ChangeSelectedIndexAsync(0);

            await CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.RefillItemsSourceAsync(raceLookUpItems);

            await CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.ChangeSelectedIndexAsync(0);
        }

        protected override async Task ZeroiseDisplayOfSimpleElementsOfGuiAsync()
        {
            await base.ZeroiseDisplayOfSimpleElementsOfGuiAsync();

            await TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.ChangeTextAsync(string.Empty);

            await TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm01.ChangeTextAsync(string.Empty);
            await TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm02.ChangeTextAsync(string.Empty);
            await TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm03.ChangeTextAsync(string.Empty);
        }

        private static TimeStampHubItem[] FilterAndInsertACommentInTimingMatEntriesThatOccurInTightClusters(TimeStampHubItem[] entries, double deltaSeconds, int clusterSizeThreshold)
        {
            if (entries == null)
                return Array.Empty<TimeStampHubItem>();

            var timestampsFromFirstToLast = entries
                .Where(z => z != null)
                .Where(z => z.RecordingModeEnum == EnumStrings.KindOfEntryIsTimeStampForTimingMatSignal)
                .OrderBy(z => z.TimeStampBinaryFormat)
                .ToArray();

            var allEntriesInBunches = Array.Empty<TimeStampHubItem>();

            var editedEntriesCounter = 1;

            if (timestampsFromFirstToLast.Length > 1)
            {
                var sequentialClusterId = 1;

                Dictionary<string, TimeStampHubItem> dictionaryOfBunchedTimeStampHubItems = new();

                JghListDictionary<int, TimeStampHubItem> listDictionaryOfBunches = new();

                List<TimeStampHubItem> flattenedListOfBunchedTimeStampHubItems = new();

                TimeSpan scratchPadIntervalDuration;

                for (var i = 0; i < timestampsFromFirstToLast.Length - 1; i++)
                {
                    // step 1: a cunningly simple brute force kludge. throw all the delta-busting timestamps into a dictionary. why a dictionary? to forestall all the duplicates the brute force blindly generates

                    var thisTimeStampHubItem = timestampsFromFirstToLast[i];
                    var followingStampHubItem = timestampsFromFirstToLast[i + 1];

                    scratchPadIntervalDuration = DateTime.FromBinary(followingStampHubItem.TimeStampBinaryFormat) -
                                                 DateTime.FromBinary(thisTimeStampHubItem.TimeStampBinaryFormat);

                    if (scratchPadIntervalDuration.TotalSeconds > deltaSeconds) continue;

                    if (!dictionaryOfBunchedTimeStampHubItems.ContainsKey(thisTimeStampHubItem.GetBothGuids()))
                        dictionaryOfBunchedTimeStampHubItems.Add(thisTimeStampHubItem.GetBothGuids(), thisTimeStampHubItem);

                    if (!dictionaryOfBunchedTimeStampHubItems.ContainsKey(followingStampHubItem.GetBothGuids()))
                        dictionaryOfBunchedTimeStampHubItems.Add(followingStampHubItem.GetBothGuids(), followingStampHubItem);
                }

                var bunchedTimeStampHubItems = dictionaryOfBunchedTimeStampHubItems.Values
                    .OrderBy(z => z.TimeStampBinaryFormat)
                    .ToArray();

                if (bunchedTimeStampHubItems.Length > 1)
                {
                    // step 2: use a list dictionary to accumulate the splits for each cluster

                    for (var i = 0; i < bunchedTimeStampHubItems.Length - 1; i++)
                    {
                        var thisTimeStampHubItem = bunchedTimeStampHubItems[i];
                        var followingStampHubItem = bunchedTimeStampHubItems[i + 1];
                        var indexOfLastTimeThrough = bunchedTimeStampHubItems.Length - 2;

                        scratchPadIntervalDuration = DateTime.FromBinary(followingStampHubItem.TimeStampBinaryFormat) -
                                                     DateTime.FromBinary(thisTimeStampHubItem.TimeStampBinaryFormat);

                        listDictionaryOfBunches.Add(sequentialClusterId, thisTimeStampHubItem);

                        if (i == indexOfLastTimeThrough)
                            listDictionaryOfBunches.Add(sequentialClusterId, followingStampHubItem); // sui generis for the final item

                        if (scratchPadIntervalDuration.TotalSeconds > deltaSeconds) sequentialClusterId++;
                    }

                    // step 3: add an informative comment to all the items in all clusters

                    foreach (var bunch in listDictionaryOfBunches)
                    {
                        var sizeOfCluster = bunch.Value.Count;

                        if (sizeOfCluster < clusterSizeThreshold)
                            continue;

                        var sizeGlyph = new string('*', sizeOfCluster);

                        foreach (var timeStampHubItem in bunch.Value)
                        {
                            var editAsterisk = string.Empty;

                            if (timeStampHubItem.DatabaseActionEnum == EnumStrings.DatabaseModify)
                            {
                                editAsterisk = $"edit : {editedEntriesCounter}";
                                editedEntriesCounter++;
                            }

                            timeStampHubItem.Comment = $"cluster : {JghString.RightAlign(bunch.Key.ToString(), 3, ' ')}  {sizeGlyph}    {editAsterisk}";

                            //timeStampHubItem.Comment = JghString.ConcatAsSentences(timeStampHubItem.Comment, $"cluster : {JghString.RightAlign(bunch.Key.ToString(), 3, ' ')}  {sizeGlyph}    {editAsterisk}");

                            flattenedListOfBunchedTimeStampHubItems.Add(timeStampHubItem);
                        }
                    }

                    allEntriesInBunches = flattenedListOfBunchedTimeStampHubItems.OrderByDescending(z => z.TimeStampBinaryFormat).ToArray();
                }
            }

            return allEntriesInBunches;
        }

        private void GetParticipantInfoAndInsertInEntriesAndComments(IEnumerable<TimeStampHubItemDisplayObject> displayObjects, EventProfileItem eventProfileItem)
        {
            foreach (var displayObject in displayObjects)
                InsertParticipantInfo(displayObject, eventProfileItem);

            void InsertParticipantInfo(HubItemDisplayObjectBase displayObject, EventProfileItem eventItem1)
            {
                if (displayObject.RecordingModeEnum != EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual && displayObject.RecordingModeEnum != EnumStrings.KindOfEntryIsTimeStampForTimingMatSignal) return;

                var participantHubItem = _participantDatabase.GetParticipantFromMasterList(JghString.TmLr(displayObject.Bib));

                if (participantHubItem == null)
                {
                    displayObject.Comment = JghString.ConcatAsSentences(displayObject.Comment, "Missing participant. No known participant has this ID.");

                    return;
                }

                displayObject.FirstName = participantHubItem.FirstName;

                displayObject.MiddleInitial = participantHubItem.MiddleInitial;

                displayObject.LastName = participantHubItem.LastName;

                var eventIsBeforeRaceGroupTransition = eventItem1.AdvertisedDate.Date < participantHubItem.DateOfRaceGroupTransition.Date;

                displayObject.RaceGroup = eventIsBeforeRaceGroupTransition ? participantHubItem.RaceGroupBeforeTransition : participantHubItem.RaceGroupAfterTransition;


                if (string.IsNullOrWhiteSpace(displayObject.RaceGroup))
                {
                    displayObject.RaceGroup = Symbols.SymbolUncategorised;
                    displayObject.Comment = JghString.ConcatAsSentences(displayObject.Comment, "Race not specified.");
                }

                if (string.IsNullOrWhiteSpace(participantHubItem.FirstName))
                    displayObject.Comment = JghString.ConcatAsSentences(displayObject.Comment, "First name not specified.");

                if (string.IsNullOrWhiteSpace(participantHubItem.LastName))
                    displayObject.Comment = JghString.ConcatAsSentences(displayObject.Comment, "Last name not specified.");


                if (string.IsNullOrWhiteSpace(participantHubItem.Gender))
                    displayObject.Comment = JghString.ConcatAsSentences(displayObject.Comment, "Gender not specified.");
            }
        }

        private async Task InsertACommentToHighlightAllModifiedEntriesAsync(IList<TimeStampHubItemDisplayObject> displayObjects)
        {
            TimeStampHubItemDisplayObject InsertCommentIfSuperseded(TimeStampHubItemDisplayObject displayObject)
            {
                var mostRecentlyEnteredHubItem = RepositoryOfHubStyleEntries.GetYoungestDescendentWithSameOriginatingItemGuid(displayObject.GetSourceItemOriginatingItemGuid());

                if (mostRecentlyEnteredHubItem == null) return displayObject;

                if (mostRecentlyEnteredHubItem.GetBothGuids() == displayObject.GetSourceItemBothGuids())
                    return displayObject;

                // if we arrive here this is a superseded item. insert a comment saying so

                var label = TimeStampHubItemDisplayObject.ToLabel(TimeStampHubItemDisplayObject.FromModel(mostRecentlyEnteredHubItem));

                displayObject.Comment = JghString.ConcatAsSentences(displayObject.Comment, $"Modified to :   {label}");

                return displayObject;
            }

            await JghParallel.SelectAsParallelWorkStealingAsync(displayObjects, InsertCommentIfSuperseded, 10);
        }

        private static Tuple<string, string> MakeDataLocationForClockDataInLocalStorage(SeriesProfileItem thisSeriesProfileItem, EventProfileItem thisEventProfileItem)
        {
            #region null checks

            var answer = new Tuple<string, string>(string.Empty, string.Empty);

            if (thisSeriesProfileItem == null) return answer;

            if (thisSeriesProfileItem
                    .ContainerForTimestampHubItemData == null)
                return new Tuple<string, string>(string.Empty, string.Empty);

            if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForTimestampHubItemData.AccountName)) return answer;

            if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForTimestampHubItemData.ContainerName)) return answer;

            if (thisEventProfileItem == null) return answer;

            #endregion

            var databaseAccount = thisSeriesProfileItem.ContainerForTimestampHubItemData.AccountName;

            var dataContainer = string.Concat(thisSeriesProfileItem.ContainerForTimestampHubItemData.ContainerName, JghString.ToStringMin2(thisEventProfileItem.NumInSequence)); // a neat little naming convention

            answer = new Tuple<string, string>(databaseAccount, dataContainer);

            return answer;
        }

        #endregion

        #region Gui stuff

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
        {
            base.EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

            CboLookupDnxVm.IsAuthorisedToOperate = true;

            CboLookupAnomalousThresholdForTooManySplitsVm.IsAuthorisedToOperate = true;
            CboLookupAnomalousThresholdForTooFewSplitsVm.IsAuthorisedToOperate = true;
            CboLookupAnomalousThresholdForTooBriefSplitsVm.IsAuthorisedToOperate = true;
            CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm.IsAuthorisedToOperate = true;
            CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm.IsAuthorisedToOperate = true;
            CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.IsAuthorisedToOperate = true;

            CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.IsAuthorisedToOperate = true;

            #region timing mat beep stuff

            TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm01.IsAuthorisedToOperate = true;
            TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm02.IsAuthorisedToOperate = true;
            TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm03.IsAuthorisedToOperate = true;


            TextBoxForEnteringMultipleIndividualIds.IsAuthorisedToOperate = true;

            CreateTimeStampForTimingMatSignalButtonVm.IsAuthorisedToOperate = true;

            #endregion

            #region timestamp cloning stuff

            TextBoxForEnteringIndividualIdForTimeStampCloneVm.IsAuthorisedToOperate = true;

            CreateTimeStampCloneButtonVm.IsAuthorisedToOperate = false;

            RejectTimeStampCloneButtonVm.IsAuthorisedToOperate = true;

            #endregion

            #region gun start stuff

            CboLookupKindOfGunStartVm.IsAuthorisedToOperate = true;

            switch (CboLookupKindOfGunStartVm.CurrentItem?.EnumString)
            {
                case EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody:
                    CboLookupGroupLabelForGroupStartVm.IsAuthorisedToOperate = false;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsAuthorisedToOperate = false;
                    CreateTimeStampForGunStartButtonVm.IsAuthorisedToOperate = true;
                    break;
                case EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup:
                    CboLookupGroupLabelForGroupStartVm.IsAuthorisedToOperate = true;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsAuthorisedToOperate = false;
                    CreateTimeStampForGunStartButtonVm.IsAuthorisedToOperate = CboLookupGroupLabelForGroupStartVm.SelectedIndex != -1;
                    break;
                case EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual:
                    CboLookupGroupLabelForGroupStartVm.IsAuthorisedToOperate = false;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsAuthorisedToOperate = true;
                    CreateTimeStampForGunStartButtonVm.IsAuthorisedToOperate = true;
                    break;
                default:
                    CboLookupGroupLabelForGroupStartVm.IsAuthorisedToOperate = false;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsAuthorisedToOperate = false;
                    CreateTimeStampForGunStartButtonVm.IsAuthorisedToOperate = false;
                    break;
            }

            #endregion

            SearchFunctionForSplitIntervalsPerPersonVm.IsAuthorisedToOperate = true;
            //SearchFunctionForSplitIntervalsPerPersonVm.IsAuthorisedToOperate = SearchFunctionForSplitIntervalsPerPersonVm.PopulationOfThingsToBeSearched.Any();
        }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
        {
            base.EvaluateVisibilityOfAllGuiControlsThatTouchData(makeVisible);

            CboLookupGroupLabelForGroupStartVm.IsVisible = CboLookupGroupLabelForGroupStartVm.ItemsSource.Any();

            CboLookupAnomalousThresholdForTooManySplitsVm.IsVisible = CboLookupAnomalousThresholdForTooManySplitsVm.ItemsSource.Any();

            CboLookupAnomalousThresholdForTooFewSplitsVm.IsVisible = CboLookupAnomalousThresholdForTooFewSplitsVm.ItemsSource.Any();

            CboLookupAnomalousThresholdForTooBriefSplitsVm.IsVisible = CboLookupAnomalousThresholdForTooBriefSplitsVm.ItemsSource.Any();

            CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm.IsVisible = CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm.ItemsSource.Any();

            CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm.IsVisible = CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm.ItemsSource.Any();

            CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.IsVisible = CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid.ItemsSource.Any();

            CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.IsVisible = CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid.ItemsSource.Any();

            SearchFunctionForSplitIntervalsPerPersonVm.IsVisible = true;

            switch (CboLookupKindOfGunStartVm.CurrentItem?.EnumString)
            {
                case EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody:
                    CboLookupGroupLabelForGroupStartVm.IsVisible = false;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsVisible = false;
                    CreateTimeStampForGunStartButtonVm.IsVisible = true;
                    break;
                case EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup:
                    CboLookupGroupLabelForGroupStartVm.IsVisible = true;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsVisible = false;
                    CreateTimeStampForGunStartButtonVm.IsVisible = CboLookupGroupLabelForGroupStartVm.SelectedIndex != -1;
                    break;
                case EnumStrings.KindOfEntryIsTimeStampForGunStartForSingleIndividual:
                    CboLookupGroupLabelForGroupStartVm.IsVisible = false;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsVisible = true;
                    CreateTimeStampForGunStartButtonVm.IsVisible = true;
                    break;
                default:
                    CboLookupGroupLabelForGroupStartVm.IsVisible = false;
                    TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm.IsVisible = false;
                    CreateTimeStampForGunStartButtonVm.IsVisible = false;
                    break;
            }
        }

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
        {
            var answer = base.MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate();

            AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm01);
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm02);
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm03);

            AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringMultipleIndividualIds);

            AddToCollectionIfIHasIsAuthorisedToOperate(answer, EditTemplateForRepositoryItemBeingEdited);

            return answer;
        }

        protected override ButtonControlViewModel[] MakeListOfAllGuiButtonsThatTouchData()
        {
            var temp = base.MakeListOfAllGuiButtonsThatTouchData().ToList();

            temp.Add(CreateTimeStampForGunStartButtonVm);
            temp.Add(CreateTimeStampForTimingMatSignalButtonVm);
            temp.Add(CreateTimeStampCloneButtonVm);
            temp.Add(RejectTimeStampCloneButtonVm);

            temp.Add(PullParticipantProfilesFromHubButtonVm);
            temp.Add(RefreshDataGridOfSplitIntervalsPerPersonButtonVm);

            temp.Add(ExportAllTimeStampsButtonVm);
            temp.Add(ExportAllSplitIntervalsPerPersonButtonVm);
            temp.Add(ExportResultsLeaderboardForPreprocessingButtonVm);

            return temp.ToArray();
        }

        protected override IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>[] MakeListOfCboViewModels()
        {
            var temp = base.MakeListOfCboViewModels().ToList();

            temp.Add(CboLookupDnxVm);
            temp.Add(CboLookupKindOfGunStartVm);
            temp.Add(CboLookupGroupLabelForGroupStartVm);
            temp.Add(CboLookupAnomalousThresholdForTooManySplitsVm);
            temp.Add(CboLookupAnomalousThresholdForTooFewSplitsVm);
            temp.Add(CboLookupAnomalousThresholdForTooBriefSplitsVm);
            temp.Add(CboLookupDeltaForCalculatingClusteredTimingMatEntriesVm);
            temp.Add(CboLookupClusterSizeThresholdForCalculatingClusteredTimingMatEntriesVm);
            temp.Add(CboLookupOnlySubGroupOfSingleRaceInSplitIntervalsPerPersonDataGrid);
            temp.Add(CboLookupOnlySubGroupOfSingleRaceInTimeStampsDataGrid);


            return temp.ToArray();
        }

        protected override TextBoxControlViewModel[] MakeListOfTextBoxControlViewModels()
        {
            var temp = base.MakeListOfTextBoxControlViewModels().ToList();

            temp.Add(TextBoxForEnteringIndividualIdForTimeStampOfGunStartVm);
            temp.Add(TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm01);
            temp.Add(TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm02);
            temp.Add(TextBoxForEnteringIndividualIdForTimeStampOfTimingMatVm03);
            temp.Add(TextBoxForEnteringIndividualIdForTimeStampCloneVm);
            temp.Add(TextBoxForEnteringMultipleIndividualIds);

            return temp.ToArray();
        }

        #endregion
    }
}
