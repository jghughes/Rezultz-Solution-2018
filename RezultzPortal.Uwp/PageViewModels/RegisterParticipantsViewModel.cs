using System;
using System.Collections.Generic;
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
using Rezultz.DataTypes.Nov2023.PortalDisplayObjects;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library01.Mar2024.Repository_interfaces;
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

namespace RezultzPortal.Uwp.PageViewModels;

public class RegisterParticipantsViewModel : HubItemPagesViewModelBase<ParticipantHubItem, ParticipantHubItemEditTemplateViewModel, ParticipantHubItemDisplayObject>
{
    private const string Locus2 = nameof(RegisterParticipantsViewModel);
    private const string Locus3 = "[RezultzPortal.Uwp]";

    #region fields

    private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 5; // virtually nothing because there are no Execute methods wired up to any of the Cbos

    #endregion

    #region ctor

    public RegisterParticipantsViewModel(
        IRegistrationSvcAgent registrationSvcAgent,
        ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
        IRepositoryOfHubStyleEntriesWithStorageBackup<ParticipantHubItem> repositoryOfHubStyleEntries,
        ISessionState sessionState,
        IThingsPersistedInLocalStorage thingsPersistedInLocalStorage, ILocalStorageService localStorageService)
        : base(leaderboardResultsSvcAgent, repositoryOfHubStyleEntries, sessionState, thingsPersistedInLocalStorage, localStorageService)
    {
        const string failure = "Unable to construct object RegistrationPagesViewModel.";
        const string locus = "[ctor]";

        try
        {
            #region assign ctor IOC injections

            _registrationSvcAgent = registrationSvcAgent;

            #endregion

            #region instantiate Button presenters

            CreateParticipantProfileButtonVm =
                new ButtonControlViewModel(
                    CreateParticipantProfileButtonOnClickExecuteAsync,
                    CreateParticipantProfileButtonOnClickCanExecute);

            ExportConsolidatedParticipantDataFromLocalStorageButtonVm =
                new ButtonControlViewModel(() => { }, () => false);

            #endregion

            #region instantiate cbos

            CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid =
                new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                        string.Empty, () => { }, () => false)
                    {IsAuthorisedToOperate = true};

            CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

            #endregion

            SeasonProfileAndIdentityValidationVm.MustHideCboLookupEvent = true;
            SeasonProfileAndIdentityValidationVm.MustHideCboLookupBlobNameToPublishResults = true;
            SeasonProfileAndIdentityValidationVm.CurrentRequiredWorkRole = EnumStringsForTimingSystemWorkRoles.Registration;
            SeasonProfileAndIdentityValidationVm.PropertyChanged += AnyINotifyPropertyChangedEventHandlerWiredToSeasonProfileAndIdentityValidationVm;

            RepositoryOfHubStyleEntries.DesiredHeightOfShortList = AppSettings.DesiredHeightOfShortListOfHubItemsDefault;
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion

    #region global props

    private readonly IRegistrationSvcAgent _registrationSvcAgent;

    private static IParticipantEntriesInMemoryCacheDataGridPresentationService ParticipantEntriesInMemoryCacheDataGridPresentationService
    {
        get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IParticipantEntriesInMemoryCacheDataGridPresentationService>();
            }
            catch (Exception ex)
            {
                var msg =
                    JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                        "[IParticipantEntriesInMemoryCacheDataGridPresentationService]");

                const string locus =
                    "Property getter of [ParticipantEntriesInMemoryCacheDataGridPresentationService]";
                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
    }

    private static IParticipantEntriesInLocalStorageDataGridPresentationService ParticipantEntriesInLocalStorageDataGridPresentationService
    {
        get
        {
            try
            {
                return ServiceLocator.Current.GetInstance<IParticipantEntriesInLocalStorageDataGridPresentationService>();
            }
            catch (Exception ex)
            {
                var msg =
                    JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                        "[IParticipantEntriesInLocalStorageDataGridPresentationService]");

                const string locus =
                    "Property getter of [ParticipantEntriesInLocalStorageDataGridPresentationService]";
                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
    }

    #endregion

    #region props

    #region TextBox presenters

    public TextBoxControlViewModel ForEnteringIndividualIdForParticipantProfileCreateTextVm { get; } = new(() => { }, () => true) {IsAuthorisedToOperate = true};

    #endregion

    #region CheckBoxes etc - bool INPC

    private bool _backingstoreMustDisplayPeopleWithDuplicateIdentifiers;

    public bool MustDisplayPeopleWithDuplicateIdentifiers
    {
        get => _backingstoreMustDisplayPeopleWithDuplicateIdentifiers;
        set
        {
            SetProperty(ref _backingstoreMustDisplayPeopleWithDuplicateIdentifiers, value);

            if (MustDisplayPeopleWithDuplicateIdentifiers)
            {
                MustDisplayMasterList = false;
                //MustDisplayPeopleWithDuplicateIdentifiers = false;
                MustDisplayDuplicatePeople = false;
                MustDisplayDitchedEntries = false;
                //MustIncludeModifiedEntries = false;
                //MustExcludeModifiedEntries = false
                //MustDisplayParticipantsWhoChangedRaceGroupsOnly = false;
            }
        }
    }


    private bool _backingstoreMustDisplayDuplicatePeople;

    public bool MustDisplayDuplicatePeople
    {
        get => _backingstoreMustDisplayDuplicatePeople;
        set
        {
            SetProperty(ref _backingstoreMustDisplayDuplicatePeople, value);

            if (MustDisplayDuplicatePeople)
            {
                MustDisplayMasterList = false;
                MustDisplayPeopleWithDuplicateIdentifiers = false;
                //MustDisplayDuplicatePeople = false;
                MustDisplayDitchedEntries = false;
                //MustIncludeModifiedEntries = false;
                //MustExcludeModifiedEntries = false
                //MustDisplayParticipantsWhoChangedRaceGroupsOnly = false;
            }
        }
    }


    private bool _backingstoreMustDisplayDitchedEntries;

    public bool MustDisplayDitchedEntries
    {
        get => _backingstoreMustDisplayDitchedEntries;
        set
        {
            SetProperty(ref _backingstoreMustDisplayDitchedEntries, value);

            if (MustDisplayDitchedEntries)
            {
                MustDisplayMasterList = false;
                MustDisplayPeopleWithDuplicateIdentifiers = false;
                MustDisplayDuplicatePeople = false;
                //MustDisplayDitchedEntries = false;
                //MustIncludeModifiedEntries = false;
                //MustExcludeModifiedEntries = false
                //MustDisplayParticipantsWhoChangedRaceGroupsOnly = false;
            }
        }
    }


    private bool _backingstoreMustDisplayMasterList;

    public bool MustDisplayMasterList
    {
        get => _backingstoreMustDisplayMasterList;
        set
        {
            SetProperty(ref _backingstoreMustDisplayMasterList, value);

            if (MustDisplayMasterList)
            {
                //MustDisplayMasterList = false;
                MustDisplayPeopleWithDuplicateIdentifiers = false;
                MustDisplayDuplicatePeople = false;
                MustDisplayDitchedEntries = false;
                MustIncludeModifiedEntries = false;
                MustExcludeModifiedEntries = true;
                MustDisplayParticipantsWhoChangedRaceGroupsOnly = false;
            }
        }
    }


    private bool _backingstoreMustIncludeModifiedEntries;

    public bool MustIncludeModifiedEntries
    {
        get => _backingstoreMustIncludeModifiedEntries;
        set
        {
            SetProperty(ref _backingstoreMustIncludeModifiedEntries, value);

            if (MustIncludeModifiedEntries)
                //MustDisplayMasterList = false;
                //MustDisplayPeopleWithDuplicateIdentifiers = false;
                //MustDisplayDuplicatePeople = false;
                //MustDisplayDitchedEntries = false;
                //MustIncludeModifiedEntries = false;
                MustExcludeModifiedEntries = false;
            //MustDisplayParticipantsWhoChangedRaceGroupsOnly = false;
        }
    }


    private bool _backingstoreMustExcludeModifiedEntries;

    public bool MustExcludeModifiedEntries
    {
        get => _backingstoreMustExcludeModifiedEntries;
        set
        {
            SetProperty(ref _backingstoreMustExcludeModifiedEntries, value);

            if (MustExcludeModifiedEntries)
            {
                MustDisplayMasterList = false;
                //MustDisplayPeopleWithDuplicateIdentifiers = false;
                //MustDisplayDuplicatePeople = false;
                //MustDisplayDitchedEntries = false;
                MustIncludeModifiedEntries = false;
                //MustExcludeModifiedEntries = false;
                //MustDisplayParticipantsWhoChangedRaceGroupsOnly = false;
            }
        }
    }


    private bool _backingstoreMustDisplayParticipantsWhoChangedRaceGroupsOnly;

    public bool MustDisplayParticipantsWhoChangedRaceGroupsOnly
    {
        get => _backingstoreMustDisplayParticipantsWhoChangedRaceGroupsOnly;
        set
        {
            SetProperty(ref _backingstoreMustDisplayParticipantsWhoChangedRaceGroupsOnly, value);

            if (MustDisplayParticipantsWhoChangedRaceGroupsOnly) MustDisplayMasterList = false;
            //MustDisplayPeopleWithDuplicateIdentifiers = false;
            //MustDisplayDuplicatePeople = false;
            //MustDisplayDitchedEntries = false;
            //MustIncludeModifiedEntries = false;
            //MustDisplayModifiedEntriesOnly = false;
        }
    }


    private bool _backingstoreMustDisplayEditTemplateForRepositoryItemBeingEdited;

    public bool MustDisplayEditTemplateForRepositoryItemBeingEdited
    {
        get => _backingstoreMustDisplayEditTemplateForRepositoryItemBeingEdited;
        set => SetProperty(ref _backingstoreMustDisplayEditTemplateForRepositoryItemBeingEdited, value);
    }

    #endregion

    #region Button Presenters

    public ButtonControlViewModel CreateParticipantProfileButtonVm { get; }

    public ButtonControlViewModel ExportConsolidatedParticipantDataFromLocalStorageButtonVm { get; }

    #endregion

    #region Cbos

    public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid { get; protected set; }

    #endregion

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
            await _registrationSvcAgent.ThrowIfNoServiceConnectionAsync();
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
            ThrowIfWorkSessionNotReadyForLaunch();

            SaveGenesisOfThisViewModelAsLastKnownGood();

            #region get ready

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);

            var localDataLocation = MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

            #endregion

            #region restore participant data from storage

            var sb = new StringBuilder();

            try
            {
                var initialisedCount = await RepositoryOfHubStyleEntries.RestoreMemoryCacheFromLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

                sb.AppendLine(StringsPortal.Launch_succeeded);
                sb.AppendLine("");

                sb = initialisedCount switch
                {
                    0 => sb.AppendLine(
                        "Note:  no registration data happens to be resident on this machine for this series at the moment.  If you know that data exists, and are wondering where it is, it will be on the remote system hub that you and others share."),
                    1 => sb.AppendLine("Note:  a single item of registration data happens to resident on this machine at the moment."),
                    _ => sb.AppendLine($"Note:  {initialisedCount} registration data entries happen to be resident on this machine at the moment.")
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

            HeadersVm.Populate(StringsPortal.Target_event, SeriesItemUponLaunchOfWorkSession.Label, $"Work session launched at {DateTime.Now:HH:mm}");

            HeadersVm.SaveAsLastKnownGood();

            if (SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm != null)
                SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.Label = $"{SeriesItemUponLaunchOfWorkSession.Label}   Target initialised at {DateTime.Now:HH:mm}";

            if (SeasonProfileAndIdentityValidationVm.CboLookupEventVm != null)
                SeasonProfileAndIdentityValidationVm.CboLookupEventVm.Label = $"{EventItemUponLaunchOfWorkSession.Label}   Target initialised at {DateTime.Now:HH:mm}"; // placeholder. superfluous

            await PopulateCboLookupGroupLabelForGroupStartAsync(EventItemUponLaunchOfWorkSession);

            #endregion

            WorkSessionIsLaunched = true;

            sb.AppendLine();
            sb.AppendLine(
                "Where there is data on the hub (generated and uploaded previously by you or someone else), you can optionally pull down a copy during your work session any time you like.  The hub data won't overwrite your local data.  It will be merged with it.  Look for 'Pull participants from hub'.  You can do work regardless of whether or not you pull down previous data to this machine. ");

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
        const string failure = "Unable to pull all participant items from hub.";
        const string locus = "[PullAllItemsFromHubButtonOnClickAsync]";

        try
        {
            #region check connection

            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghAlertMessageException(StringsPortal.NoConnection);

            PullAllItemsFromHubCts = new CancellationTokenSource();

            await _registrationSvcAgent.ThrowIfNoServiceConnectionAsync(PullAllItemsFromHubCts.Token); // throws if no connection, timeout, svc doesn't answer etc

            #endregion

            #region get ready

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);
            var currentEvent = EventItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem);

            var cloudDataLocation =
                ParticipantRegistrationSvcAgent.MakeDataLocationForStorageOfParticipantDataOnRemoteHub(currentSeries, currentEvent);

            #endregion

            #region pull profiles

            var downLoadedItems = Array.Empty<ParticipantHubItem>(); // default

            var containerDoesExist = await _registrationSvcAgent.GetIfContainerExistsAsync(cloudDataLocation.Item1, cloudDataLocation.Item2, PullAllItemsFromHubCts.Token);

            if (containerDoesExist) downLoadedItems = await _registrationSvcAgent.GetParticipantItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2, PullAllItemsFromHubCts.Token);

            #endregion

            #region download complete (which may or may not have found any items) - reinitialise RepositoryOfHubStyleEntries etc

            var didRunToCompletion =
                RepositoryOfHubStyleEntries.TryAddRangeNoDuplicates(downLoadedItems, out var errorMessage);

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);


            if (!didRunToCompletion)
            {
                PullAllItemsFromHubProtectionButtonVm.IsChecked = false;

                throw new JghAlertMessageException(errorMessage);
            }

            await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

            await RefreshAllDataGridsAndListViewsAsync();

            PullAllItemsFromHubProtectionButtonVm.IsChecked = false;

            var messageOk = downLoadedItems.Length switch
            {
                0 => JghString.ConcatAsParagraphs("Nothing received. Hub was clean."),
                1 => JghString.ConcatAsParagraphs("A copy of a single data item was received from the hub and merged with the data on this machine."),
                _ => JghString.ConcatAsParagraphs($"Copies of {downLoadedItems.Length} data items were received from the hub and merged with the data on this machine.")
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

    #region CreateParticipantProfileButtonOnClickAsync

    private bool CreateParticipantProfileButtonOnClickCanExecute()
    {
        return CreateParticipantProfileButtonVm.IsAuthorisedToOperate;
    }

    private async void CreateParticipantProfileButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[CreateParticipantProfileButtonOnClickExecuteAsync]";

        try
        {
            if (!CreateParticipantProfileButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator("Creating new item...");

            DeadenGui();

            await CreateParticipantProfileButtonOnClickAsync();

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

    protected async Task<bool> CreateParticipantProfileButtonOnClickAsync()
    {
        // NB. for creating Participants, the parameter is irrelevant. not used

        const string failure = "Unable to add a new item of participant data.";
        const string locus = "[CreateParticipantProfileButtonOnClickAsync]";

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            #region get ready

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

            #endregion

            #region add item to repository

            var participantIdentifier = ForEnteringIndividualIdForParticipantProfileCreateTextVm.Text;

            if (string.IsNullOrWhiteSpace(participantIdentifier))
                participantIdentifier = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, Guid.NewGuid().ToString());

            if (!JghString.IsOnlyLettersOrDigitsOrHyphen(participantIdentifier))
                throw new JghAlertMessageException("ID must consist of letters, digits, or hyphens (or be initially blank).");

            // click counter is only changed in three places. CreateTimeStampForGunStartButtonOnClickAsync(), AcceptItemBeingEditedButtonOnClickAsync(), and BeInitialisedFromPageCodeBehindAsync()
            ButtonClickCounter += 1;

            var kindOfEntryEnum = EnumStrings.KindOfEntryIsParticipantEntry;

            var touchedBy = SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem?.UserName;

            var item = ParticipantHubItem.Create(ButtonClickCounter, participantIdentifier, kindOfEntryEnum, touchedBy);

            var didRunToCompletion = RepositoryOfHubStyleEntries.TryAddNoDuplicate(item, out var errorMessage);

            if (didRunToCompletion == false) throw new JghAlertMessageException(errorMessage);

            HeadlineItem = item;

            #endregion

            #region move forward

            await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

            await RefreshRepositoryDataGridAsync();

            await ForEnteringIndividualIdForParticipantProfileCreateTextVm.ChangeTextAsync(string.Empty);

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

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

            #endregion


            if (RepositoryOfHubStyleEntries.Count == 0)
                throw new JghAlertMessageException(
                    "Nothing to backup. Memory is empty.");

            var commitCount = await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

            await RefreshAllDataGridsAndListViewsAsync();

            if (commitCount.Item2 == 0)
                return JghString.ConcatAsParagraphs(
                    $"Backup complete. Memory cache backed up in local storage. Backup contains {RepositoryOfHubStyleEntries.Count} data entries.");

            return JghString.ConcatAsParagraphs(
                $"Full backup complete. All data entries in memory saved in local storage. Backup contains {RepositoryOfHubStyleEntries.Count} data entries, including {commitCount.Item2} new saves.");
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

            var cloudDataLocation = ParticipantRegistrationSvcAgent.MakeDataLocationForStorageOfParticipantDataOnRemoteHub(currentSeries, currentEvent);

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

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

            var scratchPadOfClonesToBePushed = new List<ParticipantHubItem>();

            foreach (var item in entriesInRepositoryNotPushedPreviously)
            {
                var clone = item.ToShallowMemberwiseClone();
                clone.IsStillToBePushed = false;
                clone.WhenPushedBinaryFormat = whenPushed;
                scratchPadOfClonesToBePushed.Add(clone);
            }

            var uploadReportOfItemsPushedForTheFirstTimeEver = await
                _registrationSvcAgent.PostParticipantItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2,
                    scratchPadOfClonesToBePushed.ToArray(),
                    PushDataIncrementallyFromMemoryToRemoteHubCts.Token);

            #region success - move on

            foreach (var item in entriesInRepositoryNotPushedPreviously)
            {
                item.IsStillToBePushed = false;
                item.WhenPushedBinaryFormat = whenPushed;
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

            #region check connection

            if (!NetworkInterface.GetIsNetworkAvailable())
                throw new JghAlertMessageException(StringsPortal.NoConnection);

            ForcePushAllDataInMemoryToRemoteHubCts = new CancellationTokenSource();

            #endregion

            #region get parameters ready

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeriesItemUponLaunchOfWorkSession);
            var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

            var cloudDataLocation = ParticipantRegistrationSvcAgent.MakeDataLocationForStorageOfParticipantDataOnRemoteHub(currentSeries, currentEvent);

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

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

            var scratchPadOfClonesToBePushed = new List<ParticipantHubItem>();

            foreach (var item in allEntriesInRepository)
            {
                var clone = item.ToShallowMemberwiseClone();

                clone.IsStillToBePushed = false;

                clone.WhenPushedBinaryFormat = whenPushed;

                scratchPadOfClonesToBePushed.Add(clone);
            }


            var uploadReport = await
                _registrationSvcAgent.PostParticipantItemArrayAsync(cloudDataLocation.Item1, cloudDataLocation.Item2,
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
                $"{allEntriesInRepository.Length} participant-related entries were pushed.",
                $"{numberOfEntriesNotPushedPreviously} of these were pushed for the first time.",
                "The upload service confirmed the name of the destination on the hub and the number of items merged : -",
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

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

            #endregion

            if (!DeleteAllDataInMemoryProtectionButtonVm.IsChecked)
                throw new JghAlertMessageException(
                    "Are you sure you wish to delete all data? Unlock if you wish to proceed.");


            //await ZeroiseDisplayOfSimpleElementsOfGuiAsync();

            var clearedCount = RepositoryOfHubStyleEntries.ClearCache();

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

            if (!DeleteAllDataInMemoryProtectionButtonVm.IsChecked)
                throw new JghAlertMessageException(
                    "Are you sure you wish to delete all data in memory? Unlock if you wish to proceed.");

            var clearedCount = RepositoryOfHubStyleEntries.ClearCache();


            await ZeroiseDisplayOfSimpleElementsOfGuiAsync();

            await RefreshRepositoryDataGridAsync();

            DeleteAllDataInMemoryProtectionButtonVm.IsChecked = false;

            return clearedCount == 0 ? "Cache is empty." : $"{clearedCount} items cleared out of memory. Cache is empty.";
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

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

            #endregion

            await RepositoryOfHubStyleEntries.ClearLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

            await DataGridOfItemsInLocalStorage.ZeroiseItemsSourceAsync();

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

        ParticipantHubItem[] entries;

        var database = new ParticipantDatabase();

        database.LoadDatabaseV2(RepositoryOfHubStyleEntries);

        #region prepare data i.e. entries -> DisplayObjects

        if (MustDisplayPeopleWithDuplicateIdentifiers)
            entries = database.GetDuplicateIdentifiers();
        else if (MustDisplayDuplicatePeople)
            entries = database.GetDuplicatePeople();
        else if (MustDisplayDitchedEntries)
            entries = database.GetDitches();
        //else if (MustDisplayMasterList)
        //    entries = database.GetSurfaceViewOfUnderlyingHubItemRepository();
        else if (MustIncludeModifiedEntries)
            entries = RepositoryOfHubStyleEntries.GetAllEntriesAsRawData();
        else if (MustDisplayParticipantsWhoChangedRaceGroupsOnly)
            entries = database.GetParticipantsWhoOstensiblyTransitionedFromOneRaceGroupToAnother();
        else
            entries = database.GetMasterList(); //default

        var displayObjects = entries
            .Select(ParticipantHubItemDisplayObject.FromModel)
            .ToArray();

        await InsertACommentToHighlightAllModifiedEntriesAsync(displayObjects);

        if (!string.IsNullOrWhiteSpace(ForEnteringMultipleIdentifiersForDataGridRowFilter.Text))
        {
            var multipleIdentifiers = ForEnteringMultipleIdentifiersForDataGridRowFilter.Text.Split(',');

            multipleIdentifiers = JghString.ToTrimmedLowerCaseStrings(multipleIdentifiers);

            displayObjects = displayObjects
                .Where(displayObject => multipleIdentifiers.Contains(JghString.TmLr(displayObject.Identifier)))
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


        var desiredRaceGroup = CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid.CurrentItem?.Label;

        if (!string.IsNullOrWhiteSpace(desiredRaceGroup) && desiredRaceGroup != Symbols.SymbolNotApplicable)
            displayObjects = displayObjects
                .Where(z => JghString.AreEqualIgnoreOrdinalCase(z.RaceGroup, desiredRaceGroup))
                .ToArray();

        if (MustDisplayCommentedRowsOnly)
            displayObjects = displayObjects.Where(z => !string.IsNullOrWhiteSpace(z.Comment)).ToArray();

        if (MustExcludeModifiedEntries)
            displayObjects = displayObjects.Where(z => !z.Comment.Contains("Modified")).ToArray();

        #endregion

        #region populate the DataGridDesigner to obtain array of column specification items for RadDataGrid control in a PresentationService

        displayObjects = displayObjects
            .OrderBy(z => z.LastName)
            .ThenBy(z => z.FirstName)
            .ThenBy(z => z.MiddleInitial)
            .ThenBy(z => z.Identifier)
            .ThenByDescending(z => z.WhenTouched)
            .ToArray();

        DataGridDesignerForItemsInRepository.InitialiseDesigner(currentSeries, currentEvent,
            EnumStrings.ParticipantEntryAsRawDataEntryColumnFormat, displayObjects);

        ColumnSpecificationItem[] columnSpecificationItemsForDisplayObjects;

        if (MustDisplayMasterList)
            columnSpecificationItemsForDisplayObjects = DataGridDesignerForItemsInRepository.GetNonEmptyColumnSpecificationItemsForParticipantHubItemMasterList();
        else
            columnSpecificationItemsForDisplayObjects = MustDisplayAbridgedColumnsOnly
                ? DataGridDesignerForItemsInRepository.GetNonEmptyColumnSpecificationItemsForAbridgedParticipantHubItemDisplayObjects()
                : DataGridDesignerForItemsInRepository.GetNonEmptyColumnSpecificationItemsForParticipantHubItemDisplayObjects();

        #endregion

        #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

        await ParticipantEntriesInMemoryCacheDataGridPresentationService
            .GenerateTableColumnCollectionManuallyAsync(columnSpecificationItemsForDisplayObjects); // essential if using a datagrid, empty method if uploading a pretty-printed html document to be viewed in a browser 

        #endregion

        #region populate the Presenter to provide a datacontext and hence row collection for the PresentationService

        await DataGridOfItemsInRepository.PopulatePresenterAsync(currentSeries, currentEvent,
            null, null, EnumStrings.RowsAreUnGrouped, EnumStrings.ParticipantEntryAsRawDataEntryColumnFormat, null, displayObjects);

        #endregion

        #region Populate SearchFunctionVm

        var searchQuerySuggestions = ParticipantHubItemDisplayObject.ToSearchQuerySuggestionItem(displayObjects);

        SearchFunctionVm.PopulateItemsSource(searchQuerySuggestions);

        //SearchFunctionVm.MakeVisibleIfThereAreThingsToBeSearched();

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
            MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

        #endregion

        #region prepare data - rawEntriesInLocalStorageAsDisplayObjects

        var rawEntriesInLocalStorage = await RepositoryOfHubStyleEntries.GetFromLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2) ?? Array.Empty<ParticipantHubItem>();

        var rawEntriesInLocalStorageAsDisplayObjects = rawEntriesInLocalStorage
            .Where(z => z != null)
            .Select(ParticipantHubItemDisplayObject.FromModel)
            .ToArray();

        #endregion

        #region populate the DataGridDesigner to obtain array of column specification items for RadDataGrid control in a PresentationService

        DataGridDesignerForItemsInLocalStorage.InitialiseDesigner(currentSeries, currentEvent,
            EnumStrings.ParticipantEntryAsRawDataEntryColumnFormat, rawEntriesInLocalStorageAsDisplayObjects);

        var nonEmptyColumnSpecificationsForRawEntriesInLocalStorage = DataGridDesignerForItemsInLocalStorage
            .GetNonEmptyColumnSpecificationItemsForParticipantHubItemDisplayObjects(); // NB. we use a standard format for raw data

        #endregion

        #region inside the PresentationService which houses the RadDataGrid control, attach the column collection

        await ParticipantEntriesInLocalStorageDataGridPresentationService
            .GenerateDataGridColumnCollectionManuallyAsync(nonEmptyColumnSpecificationsForRawEntriesInLocalStorage); // essential if using a datagrid, empty method if uploading a pretty-printed html document to be viewed in a browser 

        #endregion

        #region populate the Presenter to provide a datacontext and hence row collection for the RadDataGrid control

        await DataGridOfItemsInLocalStorage.PopulatePresenterAsync(currentSeries, currentEvent,
            null, null, EnumStrings.RowsAreUnGrouped, EnumStrings.ParticipantEntryAsRawDataEntryColumnFormat, null, rawEntriesInLocalStorageAsDisplayObjects);

        #endregion
    }

    #endregion

    #region RefreshRemoteHubDataGridAsync

    public override Task RefreshRemoteHubDataGridAsync()
    {
        throw new NotImplementedException("RefreshRemoteHubDataGridAsync() not implemented");
    }

    #endregion

    #region RefreshAllDataGridsAndListViewsButtonOnClickAsync

    public override async Task RefreshAllDataGridsAndListViewsAsync()
    {
        await RefreshLocalStorageDataGridAsync(); // to be deleted? in the 2023 version of this app this is irrelevant.

        await RefreshRepositoryDataGridAsync();

        HeadlineItem = RepositoryOfHubStyleEntries.GetBestGuessHeadlineEntry();
    }

    #endregion

    #region DataGridOfItemsInRepositoryOnSelectionChangedAsync - this is where we do template editing EditPage

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

                var mostRecentAsDisplayObject = ParticipantHubItemDisplayObject.FromModel(mostRecent);

                var label = ParticipantHubItemDisplayObject.ToLabel(mostRecentAsDisplayObject);

                var errorMessage = JghString.ConcatAsLines(
                    "This item has been modified. To edit, search for : -",
                    label);

                throw new JghAlertMessageException(errorMessage);
            }

            // success. all is well. on we go

            var currentEvent = EventItemDisplayObject.ObtainSourceModel(EventItemUponLaunchOfWorkSession);

            var raceSpecificationItems = currentEvent?.EventSettingsItem?.RaceSpecificationItems;

            await EditTemplateForRepositoryItemBeingEdited.PopulateWithItemBeingModifiedAsync(sourceItem, raceSpecificationItems);

            LastKnownGoodEditTemplateForRepositoryItemBeingEdited = EditTemplateForRepositoryItemBeingEdited;

            MustDisplayEditTemplateForRepositoryItemBeingEdited = true;

            return true;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
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

            var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem);

            var localDataLocation =
                MakeDataLocationForParticipantDataInLocalStorage(currentSeries);

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

            EditTemplateForRepositoryItemBeingEdited.EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

            var subsequentCount = RepositoryOfHubStyleEntries.Count;

            await RepositoryOfHubStyleEntries.SaveMemoryCacheToLocalStorageBackupAsync(localDataLocation.Item1, localDataLocation.Item2);

            await RefreshRepositoryDataGridAsync();

            await Reset();

            return
                $"Added. Memory contained {initialCount} data entries before and {subsequentCount} data entries after.";

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
            await Reset();
            // NB. if anything at all out of the ordinary happens, including if an JghAlertMessageException is intentionally thrown, always reset

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

            AcceptRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = false;

            RejectRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = false;
        } // always keep this definition identical to the same in AcceptItemBeingEditedButtonOnClickAsync()

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

    #region helpers

    private async Task PopulateCboLookupGroupLabelForGroupStartAsync(EventItemDisplayObject eventItemAtMomentOfInitialisation)
    {
        var currentEvent = EventItemDisplayObject.ObtainSourceModel(eventItemAtMomentOfInitialisation);

        var raceSpecificationItems = currentEvent?.EventSettingsItem?.RaceSpecificationItems;

        if (raceSpecificationItems == null || !raceSpecificationItems.Any()) return;

        var raceLookUpItems = raceSpecificationItems.Where(raceSpecificationItemVm => raceSpecificationItemVm != null)
            .Where(raceSpecificationItemVm => !string.IsNullOrWhiteSpace(raceSpecificationItemVm.Label))
            .Select(raceSpecificationItemVm => new CboLookupItemDisplayObject {Label = raceSpecificationItemVm.Label}).ToList();

        raceLookUpItems.Reverse();

        raceLookUpItems.Add(new CboLookupItemDisplayObject {Label = Symbols.SymbolNotApplicable});

        raceLookUpItems.Reverse();

        await CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid.RefillItemsSourceAsync(raceLookUpItems);

        await CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid.ChangeSelectedIndexAsync(0);
    }

    protected override async Task ZeroiseDisplayOfSimpleElementsOfGuiAsync()
    {
        await base.ZeroiseDisplayOfSimpleElementsOfGuiAsync();

        await ForEnteringIndividualIdForParticipantProfileCreateTextVm.ChangeTextAsync(string.Empty);
    }

    protected override async Task PopulateCboLookupPrePopulatedCbosAsync()
    {
        await PopulateCboLookUpFileFormatsPresenter(); // arbitrary location for this. ordinarily would do this in ctor but can't because async
    }

    private async Task InsertACommentToHighlightAllModifiedEntriesAsync(ParticipantHubItemDisplayObject[] rawEntriesInRepositoryInMemoryAsDisplayObjects)
    {
        ParticipantHubItemDisplayObject InsertCommentIfSuperseded(ParticipantHubItemDisplayObject thisDisplayObject)
        {
            var mostRecentlyEnteredHubItem = RepositoryOfHubStyleEntries.GetYoungestDescendentWithSameOriginatingItemGuid(thisDisplayObject.GetSourceItemOriginatingItemGuid());

            if (mostRecentlyEnteredHubItem == null) return thisDisplayObject;

            if (mostRecentlyEnteredHubItem.GetBothGuids() == thisDisplayObject.GetSourceItemBothGuids())
                return thisDisplayObject;

            // if we arrive here this is a superseded item. insert a comment saying so

            var label = ParticipantHubItemDisplayObject.ToLabel(ParticipantHubItemDisplayObject.FromModel(mostRecentlyEnteredHubItem));

            thisDisplayObject.Comment = $"Modified to :   {label}";

            return thisDisplayObject;
        }

        await JghParallel.SelectAsParallelWorkStealingAsync(rawEntriesInRepositoryInMemoryAsDisplayObjects, InsertCommentIfSuperseded, 10);
    }

    public static Tuple<string, string> MakeDataLocationForParticipantDataInLocalStorage(SeriesProfileItem thisSeriesProfileItem)
    {
        #region null checks

        var answer = new Tuple<string, string>(string.Empty, string.Empty);

        if (thisSeriesProfileItem == null) return answer;

        if (thisSeriesProfileItem.ContainerForParticipantHubItemData == null) return answer;

        if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForParticipantHubItemData.AccountName)) return answer;

        if (string.IsNullOrWhiteSpace(thisSeriesProfileItem.ContainerForParticipantHubItemData.ContainerName)) return answer;

        #endregion

        var databaseAccount = thisSeriesProfileItem.ContainerForParticipantHubItemData.AccountName;

        var dataContainer = thisSeriesProfileItem.ContainerForParticipantHubItemData.ContainerName;

        answer = new Tuple<string, string>(databaseAccount, dataContainer);

        return answer;
    }

    #endregion

    #region Gui stuff

    protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
    {
        var answer = base.MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate();

        return answer;
    }

    protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
    {
        base.EvaluateVisibilityOfAllGuiControlsThatTouchData(makeVisible);

        SeasonProfileAndIdentityValidationVm.CboLookupEventVm.IsVisible = false; // always hidden
        SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm.IsVisible = false; // always hidden

        CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid.IsVisible = CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid.ItemsSource.Any();
    }

    protected override ButtonControlViewModel[] MakeListOfAllGuiButtonsThatTouchData()
    {
        var temp = base.MakeListOfAllGuiButtonsThatTouchData().ToList();

        temp.Add(CreateParticipantProfileButtonVm);
        temp.Add(ExportConsolidatedParticipantDataFromLocalStorageButtonVm);

        return temp.ToArray();
    }

    protected override TextBoxControlViewModel[] MakeListOfTextBoxControlViewModels()
    {
        var temp = base.MakeListOfTextBoxControlViewModels().ToList();

        temp.Add(ForEnteringIndividualIdForParticipantProfileCreateTextVm);

        return temp.ToArray();
    }

    protected override IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>[] MakeListOfCboViewModels()
    {
        var temp = base.MakeListOfCboViewModels().ToList();

        temp.Add(CboLookupOnlySubGroupOfSingleRaceInParticipantProfileDataGrid);

        return temp.ToArray();
    }

    #endregion
}
