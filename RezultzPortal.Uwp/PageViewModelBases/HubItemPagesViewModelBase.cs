using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Interfaces01.July2018.Objects;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repository_interfaces;
using Rezultz.Library02.Mar2024.DataGridDesigners;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.ValidationViewModels;

using RezultzPortal.Uwp.Strings;

namespace RezultzPortal.Uwp.PageViewModelBases
{
    public abstract class HubItemPagesViewModelBase<THubItem, THubItemEditTemplateViewModel, THubItemDisplayObject> : BaseViewViewModel, ISearchService
        where THubItem : class, IHubItem, IHasCollectionLineItemPropertiesV2, new()
        where THubItemEditTemplateViewModel : class, INotifyPropertyChanged, IHasGuid, IHasWasTouched, IHasAnyEditTemplateEntryChangedExecuteAction, IHasIsAuthorisedToOperate, IHasZeroiseAsync, new()
        where THubItemDisplayObject : class, INotifyPropertyChanged, IHasCollectionLineItemPropertiesV2, new()
    {
        private const string Locus2 = nameof(HubItemPagesViewModelBase<THubItem, THubItemEditTemplateViewModel, THubItemDisplayObject>);
        private const string Locus3 = "[RezultzPortal.Uwp]";

        private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 50;


        #region ctor

        protected HubItemPagesViewModelBase(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
            IRepositoryOfHubStyleEntriesWithStorageBackup<THubItem> repositoryOfHubStyleEntries,
            ISessionState sessionState,
            IThingsPersistedInLocalStorage thingsPersistedInLocalStorage, ILocalStorageService localStorageService)
    {
        const string failure = "Unable to construct object HubItemPagesViewModelBase.";
        const string locus = "[ctor]";

        try
        {
            #region assign ctor IOC injections

            SessionState = sessionState;
            ThingsPersistedInLocalStorage = thingsPersistedInLocalStorage;
            SeasonProfileAndIdentityValidationVm = new SeasonProfileAndIdentityValidationViewModel(leaderboardResultsSvcAgent, thingsPersistedInLocalStorage, localStorageService);
            RepositoryOfHubStyleEntries = repositoryOfHubStyleEntries;
            SeasonProfileAndIdentityValidationVm.OwnerOfThisServiceIsPortalNotRezultz = true;

            #endregion

            #region instantiate Button ViewModels

            CheckConnectionToRezultzHubButtonVm = new ButtonControlViewModel(
                CheckConnectionToRezultzHubButtonOnClickExecuteAsync,
                CheckConnectionToRezultzHubButtonOnClickCanExecute);

            LaunchWorkSessionButtonVm = new ButtonControlViewModel(
                LaunchWorkSessionButtonOnClickExecuteAsync,
                InitialiseWorkSessionButtonOnClickCanExecute);

            PullAllItemsFromHubButtonVm = new ButtonControlViewModel(
                PullAllItemsFromHubButtonOnClickExecuteAsync,
                PullAllItemsFromHubButtonOnClickCanExecute);
            PullAllItemsFromHubCancelButtonVm = new ButtonControlViewModel(
                PullAllItemsFromHubCancelButtonOnClickExecuteAsync,
                PullAllItemsFromHubCancelButtonOnClickCanExecute);

            PushDataIncrementallyFromMemoryToRemoteHubButtonVm = new ButtonControlViewModel(
                PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickExecuteAsync,
                PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickCanExecute);
            PushDataIncrementallyFromMemoryToRemoteHubCancelButtonVm = new ButtonControlViewModel(
                PushDataIncrementallyFromMemoryToRemoteHubCancelButtonOnClickExecuteAsync,
                PushDataIncrementallyFromMemoryToRemoteHubCancelButtonOnClickCanExecute);

            ForcePushAllDataInMemoryToRemoteHubButtonVm = new ButtonControlViewModel(
                ForcePushAllDataInMemoryToRemoteHubButtonOnClickExecuteAsync,
                ForcePushAllDataInMemoryToRemoteHubButtonOnClickCanExecute);
            ForcePushAllDataInMemoryToRemoteHubCancelButtonVm = new ButtonControlViewModel(
                ForcePushAllDataInMemoryToRemoteHubCancelButtonOnClickExecuteAsync,
                ForcePushAllDataInMemoryToRemoteHubCancelButtonOnClickCanExecute);

            CommitDataInMemoryIntoLocalStorageButtonVm = new ButtonControlViewModel(
                CommitDataInMemoryIntoLocalStorageButtonOnClickExecuteAsync,
                CommitDataInMemoryIntoLocalStorageButtonOnClickCanExecute);

            AcceptRepositoryItemBeingEditedButtonVm = new ButtonControlViewModel(
                AcceptItemBeingEditedButtonOnClickExecuteAsync,
                AcceptItemBeingEditedButtonOnClickCanExecute);

            RejectRepositoryItemBeingEditedButtonVm = new ButtonControlViewModel(
                RejectItemBeingEditedButtonOnClickExecuteAsync,
                RejectItemBeingEditedButtonOnClickCanExecute);

            DeleteAllDataButtonVm = new ButtonControlViewModel(
                DeleteAllDataButtonOnClickExecuteAsync,
                DeleteAllDataButtonOnClickCanExecute);

            DeleteAllDataInMemoryProtectionButtonVm = new ButtonControlViewModel(
                () => { }, () => false);

            DeleteAllDataInMemoryButtonVm = new ButtonControlViewModel(
                DeleteAllDataInMemoryCacheButtonOnClickExecuteAsync,
                DeleteAllDataInMemoryCacheButtonOnClickCanExecute);

            DeleteAllDataInLocalStorageButtonVm = new ButtonControlViewModel(
                DeleteAllDataInLocalStorageButtonOnClickExecuteAsync,
                DeleteAllDataInLocalStorageButtonOnClickCanExecute);

            PullAllItemsFromHubProtectionButtonVm = new ButtonControlViewModel(
                () => { }, () => false);

            RefreshRepositoryDataGridButtonVm = new ButtonControlViewModel(
                RefreshRepositoryDataGridButtonOnClickExecuteAsync,
                RefreshRepositoryDataGridButtonOnClickCanExecute);

            RefreshLocalStorageDataGridButtonVm = new ButtonControlViewModel(
                RefreshLocalStorageDataGridButtonOnClickExecuteAsync,
                RefreshLocalStorageDataGridButtonOnClickCanExecute);

            RefreshRemoteHubDataGridButtonVm = new ButtonControlViewModel(
                RefreshRemoteHubDataGridButtonOnClickExecuteAsync,
                RefreshRemoteHubDataGridButtonOnClickCanExecute);

            RefreshAllDataGridsAndListViewsButtonVm = new ButtonControlViewModel(
                RefreshAllDataGridsAndListViewsButtonOnClickExecuteAsync,
                RefreshAllDataGridsAndListViewsButtonOnClickCanExecute);

            #endregion

            #region instantiate Cbos

            CboLookUpFileFormatsVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>("", () => { }, () => true);
            CboLookUpFileFormatsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

            #endregion

            #region instantiate EditTemplateVm

            EditTemplateForRepositoryItemBeingEdited.AnyEditTemplateEntryChangedExecuteAction = MakeAcceptItemBeingEditedButtonVmAuthorisedToOperate;

            #endregion

            #region instantiate DataGridViewModels

            DataGridOfItemsInRepository = new DataGridViewModel<THubItemDisplayObject>(string.Empty,
                    DataGridOfItemsInRepositoryOnSelectionChangedExecuteAsync,
                    DataGridOfItemsInRepositoryOnSelectionChangedCanExecute)
                {IsAuthorisedToOperate = true, IsVisible = false};

            DataGridOfItemsInLocalStorage = new DataGridViewModel<THubItemDisplayObject>(string.Empty,
                    () => { },
                    () => false)
                {IsAuthorisedToOperate = true, IsVisible = false};

            #endregion
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        #endregion

        #region PropertyChangedEvent event handler

        public void AnyINotifyPropertyChangedEventHandlerWiredToSeasonProfileAndIdentityValidationVm(object o, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem) or nameof(SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem))
            EvaluateVisibilityOfAllGuiControlsThatTouchData(true); // makeVisible is arbitrary in this method because at the time of writing it makes no diff if it's true or false
    }

        #endregion

        #region methods called directly or indirectly on arrival to page to which this vm is the data context each time by page-loaded event

        public async Task<string> BeInitialisedFromPageCodeBehindOrchestrateAsync()
    {
        var failure = StringsForXamlPages.UnableToInitialiseViewmodel;
        const string locus = nameof(BeInitialisedFromPageCodeBehindOrchestrateAsync);

        try
        {
            if (ThisViewModelIsInitialised)
                return string.Empty;

            DeadenGui();

            await BeInitialisedFromPageCodeBehindAsync();

            EnlivenGui();

            SaveGenesisOfThisViewModelAsLastKnownGood();

            return string.Empty;
        }

        #region try catch handling

        catch (Exception ex)
        {
            EvaluateGui();

            var isHarmless = JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex)
                             || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghResultsData404Exception>(ex);

            ThisViewModelIsInitialised = isHarmless;

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        private async Task BeInitialisedFromPageCodeBehindAsync()
    {
        await ZeroiseDisplayOfSimpleElementsOfGuiAsync();

        FootersVm.Populate(StringsPortal.Welcome__);

        await PopulateCboLookupPrepopulatedCbosAsync();

        await SeasonProfileAndIdentityValidationVm.BeInitialisedForRezultzPortalOrchestrateAsync(); // Note. This has additional internal first time thru logic.

        SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.Label = StringsPortal.Not_yet_launched;
        SeasonProfileAndIdentityValidationVm.CboLookupEventVm.Label = StringsPortal.Not_yet_launched;
        SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm.Label = StringsPortal.Not_yet_launched; // dummy. not used in hub pages

        ThisViewModelIsInitialised = true;
    }

        #endregion

        #region global props

        protected readonly ISessionState SessionState;
        protected readonly IThingsPersistedInLocalStorage ThingsPersistedInLocalStorage;

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
                    JghString.ConcatAsSentences(StringsPortal.Unable_to_retrieve_instance,
                        $"'{nameof(IAlertMessageService)}");

                var locus = $"Property getter of '{nameof(AlertMessageService)}]";
                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
        }

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

        public readonly IRepositoryOfHubStyleEntriesWithStorageBackup<THubItem> RepositoryOfHubStyleEntries;

        #endregion

        #region props

        public CancellationTokenSource PullAllItemsFromHubCts { get; set; }
        public CancellationTokenSource PushDataIncrementallyFromMemoryToRemoteHubCts { get; set; }
        public CancellationTokenSource ForcePushAllDataInMemoryToRemoteHubCts { get; set; }

        #region simple props

        public bool ThisViewModelIsInitialised;

        public bool WorkSessionIsLaunched;

        protected int ButtonClickCounter;

        protected readonly StringBuilder SbFriendlyLogOfActivity = new();

        protected readonly StringBuilder SbFriendlyLogOfBlobAndFileTransfers = new();

        #endregion

        #region SeasonProfileAndIdentityValidationVm

        public SeasonProfileAndIdentityValidationViewModel SeasonProfileAndIdentityValidationVm { get; }

        #endregion

        #region headers and footers

        public HeaderOrFooterViewModel FootersVm { get; } = new();

        public HeaderOrFooterViewModel HeadersVm { get; } = new();

        #endregion

        #region HeadlineItem

        private THubItem _backingstoreHeadlineItem = new();

        public THubItem HeadlineItem
        {
            get => _backingstoreHeadlineItem;
            set => SetProperty(ref _backingstoreHeadlineItem, value);
        }

        #endregion

        #region EditTemplateVm

        public THubItemEditTemplateViewModel EditTemplateForRepositoryItemBeingEdited { get; } = new();

        protected THubItemEditTemplateViewModel LastKnownGoodEditTemplateForRepositoryItemBeingEdited = new();

        #endregion

        #region TextBox presenters

        public TextBoxControlViewModel ForEnteringMultipleIdentifiersForDataGridRowFilter { get; } = new(() => { }, () => true) {IsAuthorisedToOperate = true};

        public TextBoxControlViewModel ForEnteringMultipleUserNamesOfPeopleWhoDidTheDataEntriesForDataGridRowFilter { get; } = new(() => { }, () => true) {IsAuthorisedToOperate = true};

        #endregion

        #region TextBlock presenters

        public TextBlockControlViewModel ForDisplayingLogOfFilesThatWereTransferredTextVm { get; } = new();

        public TextBlockControlViewModel ForDisplayingLogOfActivityTextVm { get; } = new();

        #endregion

        #region CheckBoxes - bool INPC

        private bool _backingstoreMustDisplayAbridgedColumnsOnly;

        public bool MustDisplayAbridgedColumnsOnly
        {
            get => _backingstoreMustDisplayAbridgedColumnsOnly;
            set => SetProperty(ref _backingstoreMustDisplayAbridgedColumnsOnly, value);
        }

        private bool _backingstoreMustDisplayCommentedRowsOnly;

        public bool MustDisplayCommentedRowsOnly
        {
            get => _backingstoreMustDisplayCommentedRowsOnly;
            set => SetProperty(ref _backingstoreMustDisplayCommentedRowsOnly, value);
        }

        #endregion

        #region Button presenters

        public ButtonControlViewModel CheckConnectionToRezultzHubButtonVm { get; }

        public ButtonControlViewModel LaunchWorkSessionButtonVm { get; }

        public ButtonControlViewModel PullAllItemsFromHubButtonVm { get; } // todo in xaml
        public ButtonControlViewModel PullAllItemsFromHubCancelButtonVm { get; }
        public ButtonControlViewModel PullAllItemsFromHubProtectionButtonVm { get; }

        public ButtonControlViewModel PushDataIncrementallyFromMemoryToRemoteHubButtonVm { get; }
        public ButtonControlViewModel PushDataIncrementallyFromMemoryToRemoteHubCancelButtonVm { get; }

        public ButtonControlViewModel ForcePushAllDataInMemoryToRemoteHubButtonVm { get; }
        public ButtonControlViewModel ForcePushAllDataInMemoryToRemoteHubCancelButtonVm { get; }

        public ButtonControlViewModel CommitDataInMemoryIntoLocalStorageButtonVm { get; }

        public ButtonControlViewModel DeleteAllDataButtonVm { get; }

        public ButtonControlViewModel DeleteAllDataInMemoryButtonVm { get; }

        public ButtonControlViewModel DeleteAllDataInLocalStorageButtonVm { get; }

        public ButtonControlViewModel DeleteAllDataInMemoryProtectionButtonVm { get; }

        public ButtonControlViewModel AcceptRepositoryItemBeingEditedButtonVm { get; }

        public ButtonControlViewModel RejectRepositoryItemBeingEditedButtonVm { get; }

        public ButtonControlViewModel RefreshRepositoryDataGridButtonVm { get; }

        public ButtonControlViewModel RefreshLocalStorageDataGridButtonVm { get; }

        public ButtonControlViewModel RefreshRemoteHubDataGridButtonVm { get; }

        public ButtonControlViewModel RefreshAllDataGridsAndListViewsButtonVm { get; }

        #endregion

        #region Cbo presenters

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookUpFileFormatsVm { get; protected set; }

        #endregion

        #region SearchFunction

        public SearchViewModel SearchFunctionVm { get; } = new("search", 2, 9, null, null);

        #endregion

        #region DataGridDesigners

        public DataGridDesigner DataGridDesignerForItemsInRepository { get; } = new();

        public DataGridDesigner DataGridDesignerForItemsInLocalStorage { get; } = new();

        #endregion

        #region DataGridViewModels

        public DataGridViewModel<THubItemDisplayObject> DataGridOfItemsInRepository { get; }

        public DataGridViewModel<THubItemDisplayObject> DataGridOfItemsInLocalStorage { get; }

        #endregion

        #endregion

        #region commands

        #region CheckConnectionToRezultzHubButtonOnClickAsync

        protected virtual bool CheckConnectionToRezultzHubButtonOnClickCanExecute()
    {
        return CheckConnectionToRezultzHubButtonVm.IsAuthorisedToOperate;
    }

        protected async void CheckConnectionToRezultzHubButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CheckConnectionToRezultzHubButtonOnClickExecuteAsync]";

        try
        {
            if (!CheckConnectionToRezultzHubButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.WakingServer);

            DeadenGui();

            var answer = await CheckConnectionToRezultzHubButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(answer);
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

        protected abstract Task<string> CheckConnectionToRezultzHubButtonOnClickAsync();

        #endregion

        #region LaunchWorkSessionButtonOnClickAsync

        private bool InitialiseWorkSessionButtonOnClickCanExecute()
    {
        return LaunchWorkSessionButtonVm.IsAuthorisedToOperate;
    }

        private async void LaunchWorkSessionButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[LaunchWorkSessionButtonOnClickExecuteAsync]";

        try
        {
            if (!InitialiseWorkSessionButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____looking_for_data);

            DeadenGui();

            var messageOk = await LaunchWorkSessionButtonOnClickAsync();

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

        protected abstract Task<string> LaunchWorkSessionButtonOnClickAsync();

        #endregion

        #region PullAllItemsFromHubButtonOnClickAsync

        private bool PullAllItemsFromHubButtonOnClickCanExecute()
    {
        return PullAllItemsFromHubButtonVm.IsAuthorisedToOperate;
    }

        private async void PullAllItemsFromHubButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[PullAllItemsFromHubButtonOnClickExecuteAsync]";

        try
        {
            if (!PullAllItemsFromHubButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____looking_for_data);

            DeadenGui();

            var messageOk = await PullAllItemsFromHubButtonOnClickAsync();

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

        protected abstract Task<string> PullAllItemsFromHubButtonOnClickAsync();

        #endregion

        #region PullAllItemsFromHubCancelButtonOnClickAsync

        private bool PullAllItemsFromHubCancelButtonOnClickCanExecute()
    {
        return true;
    }

        private void PullAllItemsFromHubCancelButtonOnClickExecuteAsync()
    {
        try
        {
            PullAllItemsFromHubCts?.Cancel();
        }
        catch (Exception)
        {
            //do nothing. this is where we end up if the cts has been disposed of.
        }
    }

        #endregion

        #region PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickAsync

        private bool PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickCanExecute()
    {
        return PushDataIncrementallyFromMemoryToRemoteHubButtonVm.IsAuthorisedToOperate;
    }

        private async void PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickExecuteAsync]";

        try
        {
            if (!PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____pushing_data_to_hub);

            DeadenGui();

            var report = await PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        // ReSharper disable once UnusedParameter.Local
        protected abstract Task<string> PushDataIncrementallyFromMemoryToRemoteHubButtonOnClickAsync();

        #endregion

        #region PushDataIncrementallyFromMemoryToRemoteHubCancelButtonOnClickAsync

        private bool PushDataIncrementallyFromMemoryToRemoteHubCancelButtonOnClickCanExecute()
    {
        return true;
    }

        private void PushDataIncrementallyFromMemoryToRemoteHubCancelButtonOnClickExecuteAsync()
    {
        try
        {
            PushDataIncrementallyFromMemoryToRemoteHubCts?.Cancel();
        }
        catch (Exception)
        {
            //do nothing. this is where we end up if the cts has been disposed of.
        }
    }

        #endregion

        #region ForcePushAllDataInMemoryToRemoteHubButtonOnClickAsync

        private bool ForcePushAllDataInMemoryToRemoteHubButtonOnClickCanExecute()
    {
        return ForcePushAllDataInMemoryToRemoteHubButtonVm.IsAuthorisedToOperate;
    }

        private async void ForcePushAllDataInMemoryToRemoteHubButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[ForcePushAllDataInMemoryToRemoteHubButtonOnClickExecuteAsync]";

        try
        {
            if (!ForcePushAllDataInMemoryToRemoteHubButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____pushing_data_to_hub);

            DeadenGui();

            var report = await ForcePushAllDataInMemoryToRemoteHubButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        // ReSharper disable once UnusedParameter.Local
        protected abstract Task<string> ForcePushAllDataInMemoryToRemoteHubButtonOnClickAsync();

        #endregion

        #region ForcePushAllDataInMemoryToRemoteHubCancelButtonOnClickAsync

        private bool ForcePushAllDataInMemoryToRemoteHubCancelButtonOnClickCanExecute()
    {
        return true;
    }

        private void ForcePushAllDataInMemoryToRemoteHubCancelButtonOnClickExecuteAsync()
    {
        try
        {
            ForcePushAllDataInMemoryToRemoteHubCts?.Cancel();
        }
        catch (Exception)
        {
            //do nothing. this is where we end up if the cts has been disposed of.
        }
    }

        #endregion

        #region CommitDataInMemoryIntoLocalStorageButtonOnClickAsync

        private bool CommitDataInMemoryIntoLocalStorageButtonOnClickCanExecute()
    {
        return CommitDataInMemoryIntoLocalStorageButtonVm.IsAuthorisedToOperate;
    }

        private async void CommitDataInMemoryIntoLocalStorageButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[CommitDataInMemoryIntoLocalStorageButtonOnClickExecuteAsync]";

        try
        {
            if (!CommitDataInMemoryIntoLocalStorageButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____committing_data);

            DeadenGui();

            var report = await CommitDataInMemoryIntoLocalStorageButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        protected abstract Task<string> CommitDataInMemoryIntoLocalStorageButtonOnClickAsync();

        #endregion

        #region DeleteAllDataButtonOnClickAsync

        private bool DeleteAllDataButtonOnClickCanExecute()
    {
        return DeleteAllDataButtonVm.IsAuthorisedToOperate;
    }

        private async void DeleteAllDataButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[DeleteAllDataButtonOnClickExecuteAsync]";

        try
        {
            if (!DeleteAllDataButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____deleting_all_data);

            DeadenGui();

            var report = await DeleteAllDataButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        protected abstract Task<string> DeleteAllDataButtonOnClickAsync();

        #endregion

        #region DeleteAllDataInMemoryCacheButtonOnClickAsync

        private bool DeleteAllDataInMemoryCacheButtonOnClickCanExecute()
    {
        return DeleteAllDataInMemoryButtonVm.IsAuthorisedToOperate;
    }

        private async void DeleteAllDataInMemoryCacheButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[DeleteAllDataInMemoryCacheButtonOnClickExecuteAsync]";

        try
        {
            if (!DeleteAllDataInMemoryCacheButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____deleting_all_data);

            DeadenGui();

            var report = await DeleteAllDataInMemoryCacheButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        protected abstract Task<string> DeleteAllDataInMemoryCacheButtonOnClickAsync();

        #endregion

        #region DeleteAllDataInLocalStorageButtonOnClickAsync

        private bool DeleteAllDataInLocalStorageButtonOnClickCanExecute()
    {
        return DeleteAllDataInLocalStorageButtonVm.IsAuthorisedToOperate;
    }

        private async void DeleteAllDataInLocalStorageButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[DeleteAllDataInLocalStorageButtonOnClickExecuteAsync]";

        try
        {
            if (!DeleteAllDataInLocalStorageButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____deleting_all_data);

            DeadenGui();

            var report = await DeleteAllDataInLocalStorageButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        public abstract Task<string> DeleteAllDataInLocalStorageButtonOnClickAsync();

        #endregion

        #region DataGridOfItemsInRepositoryOnSelectionChangedAsync

        private bool DataGridOfItemsInRepositoryOnSelectionChangedCanExecute()
    {
        return DataGridOfItemsInRepository.IsAuthorisedToOperate;
    }

        private async void DataGridOfItemsInRepositoryOnSelectionChangedExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[DataGridOfItemsInRepositoryOnSelectionChangedExecuteAsync]";

        try
        {
            if (!DataGridOfItemsInRepositoryOnSelectionChangedCanExecute()) return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____processing);

            DeadenGui();

            await DataGridOfItemsInRepositoryOnSelectionChangedAsync();

            EnlivenGui();
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex)) EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        protected abstract Task<bool> DataGridOfItemsInRepositoryOnSelectionChangedAsync();

        #endregion

        #region AcceptItemBeingEditedButtonOnClickAsync

        private bool AcceptItemBeingEditedButtonOnClickCanExecute()
    {
        return AcceptRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate;
    }

        private async void AcceptItemBeingEditedButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[AcceptItemBeingEditedButtonOnClickExecuteAsync]";

        try
        {
            if (!AcceptItemBeingEditedButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____adding_item_to_cache);

            DeadenGui();

            var report = await AcceptItemBeingEditedButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        protected abstract Task<string> AcceptItemBeingEditedButtonOnClickAsync();

        #endregion

        #region RejectItemBeingEditedButtonOnClick

        private bool RejectItemBeingEditedButtonOnClickCanExecute()
    {
        return RejectRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate;
    }

        private async void RejectItemBeingEditedButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RejectItemBeingEditedButtonOnClickExecuteAsync]";

        try
        {
            if (!RejectItemBeingEditedButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____abandoning_modification);

            DeadenGui();

            var report = await RejectItemBeingEditedButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(report);
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

        protected abstract Task<string> RejectItemBeingEditedButtonOnClickAsync();

        #endregion

        #region RefreshRepositoryDataGridAsync

        private bool RefreshRepositoryDataGridButtonOnClickCanExecute()
    {
        return RefreshRepositoryDataGridButtonVm.IsAuthorisedToOperate;
    }

        private async void RefreshRepositoryDataGridButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshRepositoryDataGridButtonOnClickExecuteAsync]";

        try
        {
            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working);

            var messageOk = await RefreshRepositoryDataGridButtonOnClickOrchestrateAsync();

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

        public async Task<string> RefreshRepositoryDataGridButtonOnClickOrchestrateAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshRepositoryDataGridButtonOnClickOrchestrateAsync]";

        try
        {
            if (!RefreshRepositoryDataGridButtonOnClickCanExecute())
                return string.Empty;

            DeadenGui();

            var messageOk = await RefreshRepositoryDataGridButtonOnClickAsync();

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

        protected async Task<string> RefreshRepositoryDataGridButtonOnClickAsync()
    {
        const string failure = "Unable to clean and refresh all data.";
        const string locus = "[RefreshRepositoryDataGridButtonOnClickAsync]";

        try
        {
            await RefreshRepositoryDataGridAsync();

            return string.Empty;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public abstract Task RefreshRepositoryDataGridAsync();

        #endregion

        #region RefreshLocalStorageDataGridAsync

        private bool RefreshLocalStorageDataGridButtonOnClickCanExecute()
    {
        return RefreshLocalStorageDataGridButtonVm.IsAuthorisedToOperate;
    }

        private async void RefreshLocalStorageDataGridButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshLocalStorageDataGridButtonOnClickExecuteAsync]";

        try
        {
            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working);

            var messageOk = await RefreshLocalStorageDataGridButtonOnClickOrchestrateAsync();

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

        public async Task<string> RefreshLocalStorageDataGridButtonOnClickOrchestrateAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshLocalStorageDataGridButtonOnClickOrchestrateAsync]";

        try
        {
            if (!RefreshLocalStorageDataGridButtonOnClickCanExecute())
                return string.Empty;

            DeadenGui();

            var messageOk = await RefreshLocalStorageDataGridButtonOnClickAsync();

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

        protected async Task<string> RefreshLocalStorageDataGridButtonOnClickAsync()
    {
        const string failure = "Unable to clean and refresh all data.";
        const string locus = "[RefreshLocalStorageDataGridButtonOnClickAsync]";

        try
        {
            await RefreshLocalStorageDataGridAsync();

            return string.Empty;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public abstract Task RefreshLocalStorageDataGridAsync();

        #endregion

        #region RefreshRemoteHubDataGridAsync

        private bool RefreshRemoteHubDataGridButtonOnClickCanExecute()
    {
        return RefreshRemoteHubDataGridButtonVm.IsAuthorisedToOperate;
    }

        private async void RefreshRemoteHubDataGridButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshRemoteHubDataGridButtonOnClickExecuteAsync]";

        try
        {
            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working);

            var messageOk = await RefreshRemoteHubDataGridButtonOnClickOrchestrateAsync();

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

        public async Task<string> RefreshRemoteHubDataGridButtonOnClickOrchestrateAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshRemoteHubDataGridButtonOnClickOrchestrateAsync]";

        try
        {
            if (!RefreshRemoteHubDataGridButtonOnClickCanExecute())
                return string.Empty;

            DeadenGui();

            var messageOk = await RefreshRemoteHubDataGridButtonOnClickAsync();

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

        protected async Task<string> RefreshRemoteHubDataGridButtonOnClickAsync()
    {
        const string failure = "Unable to clean and refresh all data.";
        const string locus = "[RefreshRemoteHubDataGridButtonOnClickAsync]";

        try
        {
            await RefreshRemoteHubDataGridAsync();

            return string.Empty;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public abstract Task RefreshRemoteHubDataGridAsync();

        #endregion

        #region RefreshAllDataGridsAndListViewsButtonOnClickAsync

        private bool RefreshAllDataGridsAndListViewsButtonOnClickCanExecute()
    {
        return RefreshAllDataGridsAndListViewsButtonVm.IsAuthorisedToOperate;
    }

        private async void RefreshAllDataGridsAndListViewsButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshAllDataGridsAndListViewsButtonOnClickExecuteAsync]";

        try
        {
            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working);

            var messageOk = await RefreshAllDataGridsAndListViewsButtonOnClickOrchestrateAsync();

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

        public async Task<string> RefreshAllDataGridsAndListViewsButtonOnClickOrchestrateAsync()
    {
        const string failure = "Unable to execute ICommand Execute method.";
        const string locus = "[RefreshAllDataGridsAndListViewsButtonOnClickOrchestrateAsync]";

        try
        {
            if (!RefreshAllDataGridsAndListViewsButtonOnClickCanExecute())
                return string.Empty;

            DeadenGui();

            var messageOk = await RefreshAllDataGridsAndListViewsButtonOnClickAsync();

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

        protected async Task<string> RefreshAllDataGridsAndListViewsButtonOnClickAsync()
    {
        const string failure = "Unable to clean and refresh all data.";
        const string locus = "[RefreshAllDataGridsAndListViewsButtonOnClickAsync]";

        try
        {
            await RefreshAllDataGridsAndListViewsAsync();

            return string.Empty;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        public abstract Task RefreshAllDataGridsAndListViewsAsync();

        #endregion

        #endregion

        #region search

        public virtual async Task<string[]> OnShortlistOfQuerySuggestionsRequestedFromTheSearchUniverseAsync(
            string queryText)
    {
        return await SearchFunctionVm.GetQueriesThatSatisfyUserEnteredHint(queryText);
    }

        public virtual async Task<bool> OnFinalSearchQuerySubmittedAsTextAsync(string finalQuerySubmitted)
    {
        const string failure = "Unable to complete search operation.";
        const string locus = "[OnFinalSearchQuerySubmittedAsTextAsync]";

        try
        {
            var searchResults
                = SearchFunctionVm.GetSubsetOfSearchQueryItemsThatEquateToSelectedSearchQuery(finalQuerySubmitted);

            await OrchestrateActionsToBeTakenWhenSearchOutcomeIsToHandAsync(
                searchResults.Where(z => z is not null).ToArray());
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion

        return await Task.FromResult(true);
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

        var prequels = 1; // arbitrary number of prequels to show in hte search results on the grid

        const string nothingFound = "Currently offscreen. Refresh screen. Expand the onscreen population. Do whatever is needed to surface the search target on the datagrid.";

        try
        {
            #region null checks

            if (DataGridOfItemsInRepository is null) return;

            if (discoveredQueryItems is null)
                throw new JghNullObjectInstanceException(nameof(discoveredQueryItems));

            if (!discoveredQueryItems.Any() || discoveredQueryItems.FirstOrDefault() is null)
                throw new JghAlertMessageException(nothingFound);

            #endregion

            var discoveredTag = discoveredQueryItems.FirstOrDefault()?.TagAsString; // the Tag is meant to contain the Guid of the item

            if (string.IsNullOrWhiteSpace(discoveredTag))
                throw new JghAlertMessageException(nothingFound);

            var firstItemWithMatchingGuid = DataGridOfItemsInRepository.ItemsSource.FirstOrDefault(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpaceIgnoreOrdinalCase(z.Guid, discoveredTag));

            if (firstItemWithMatchingGuid is null)
                throw new JghAlertMessageException(nothingFound);

            var skip = Math.Max(DataGridOfItemsInRepository.ItemsSource.IndexOf(firstItemWithMatchingGuid) - prequels, 0);

            var truncatedItemsSource = new ObservableCollection<THubItemDisplayObject>(DataGridOfItemsInRepository.ItemsSource.Skip(skip));

            await DataGridOfItemsInRepository.RefillItemsSourceAsync(truncatedItemsSource);
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region methods

        protected virtual async Task ZeroiseDisplayOfSimpleElementsOfGuiAsync()
    {
        HeadersVm.Zeroise();
        FootersVm.Zeroise();

        HeadlineItem = new THubItem();

        ButtonClickCounter = 0;

        await ForEnteringMultipleIdentifiersForDataGridRowFilter.ChangeTextAsync(string.Empty);

        await ForEnteringMultipleUserNamesOfPeopleWhoDidTheDataEntriesForDataGridRowFilter.ChangeTextAsync(string.Empty);

        SearchFunctionVm.ZeroiseItemsSources();

        //await EditTemplateForRepositoryItemBeingEdited.ZeroiseAsync(); // on balance prefer not to do this. it's nice for what we just did to remain visible
    }

        protected abstract Task PopulateCboLookupPrepopulatedCbosAsync();

        protected async Task PopulateCboLookUpFileFormatsCboAsync()
    {
        var kindsOfMoreInfoVm = new CboLookupItemDisplayObject[]
        {
            new() {Label = "XML data (.xml)", EnumString = EnumStrings.AsFlatFileXml},
            new() {Label = "JSON data (.json)", EnumString = EnumStrings.AsFlatFileJson},
            new() {Label = "CSV data (.csv)", EnumString = EnumStrings.AsCsvFile},
        };

        await CboLookUpFileFormatsVm.RefillItemsSourceAsync(kindsOfMoreInfoVm);

        CboLookUpFileFormatsVm.IsDropDownOpen = false;

        await CboLookUpFileFormatsVm.ChangeSelectedIndexToMatchItemEnumStringAsync(EnumStrings.AsFlatFileXml);

        CboLookUpFileFormatsVm.SaveSelectedIndexAsLastKnownGood();
    }

        public void UpdateLogOfActivity(string message)
    {
        SbFriendlyLogOfActivity.AppendLine(message);

        ForDisplayingLogOfActivityTextVm.Text = SbFriendlyLogOfActivity.ToString();
    }

        public void UpdateLogOfFilesThatWereTransferred(string blobNameOrFileName, string descriptionOfTransferAction, string descriptionOfDestination, string descriptionOfSizeAsBytes)
    {
        SbFriendlyLogOfBlobAndFileTransfers.AppendLine(
            $"'{blobNameOrFileName}' {descriptionOfTransferAction} '{descriptionOfDestination}'  {descriptionOfSizeAsBytes}");

        ForDisplayingLogOfFilesThatWereTransferredTextVm.Text =
            SbFriendlyLogOfBlobAndFileTransfers.ToString();
    }

        #endregion

        #region Gui stuff

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
    {
        var answer = new List<object>();


        AddToCollectionIfIHasIsAuthorisedToOperate(answer, DataGridOfItemsInRepository);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, EditTemplateForRepositoryItemBeingEdited);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, SearchFunctionVm);


        foreach (var cbo in MakeListOfCboViewModels())
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, cbo);


        foreach (var entryBox in MakeListOfTextBoxControlViewModels())
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, entryBox);

        foreach (var buttonControlViewModel in MakeListOfAllGuiButtonsThatTouchData())
            AddToCollectionIfIHasIsAuthorisedToOperate(answer, buttonControlViewModel);

        return answer;
    }

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
    {
        DataGridOfItemsInRepository.IsAuthorisedToOperate = true;

        foreach (var cbo in MakeListOfCboViewModels())
            cbo.IsAuthorisedToOperate = true;

        foreach (var entryBox in MakeListOfTextBoxControlViewModels())
            entryBox.IsAuthorisedToOperate = true;

        foreach (var buttonControlViewModel in MakeListOfAllGuiButtonsThatTouchData())
            buttonControlViewModel.IsAuthorisedToOperate = true;

        EditTemplateForRepositoryItemBeingEdited.IsAuthorisedToOperate =
            EditTemplateForRepositoryItemBeingEdited is not null
            && DataGridOfItemsInRepository.SelectedItem is not null;

        AcceptRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = false;
        RejectRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = EditTemplateForRepositoryItemBeingEdited.IsAuthorisedToOperate;

        SearchFunctionVm.IsAuthorisedToOperate = true;
        //SearchFunctionVm.IsAuthorisedToOperate = SearchFunctionVm.PopulationOfThingsToBeSearched.Any();
    }

        public void MakeAcceptItemBeingEditedButtonVmAuthorisedToOperate()
    {
        AcceptRepositoryItemBeingEditedButtonVm.IsAuthorisedToOperate = true;
    }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
    {
        HeadersVm.IsVisible = !HeadersVm.IsEmpty;
        FootersVm.IsVisible = !FootersVm.IsEmpty;

        foreach (var buttonVm in MakeListOfAllGuiButtonsThatTouchData())
            buttonVm.IsVisible = makeVisible;

        SeasonProfileAndIdentityValidationVm.SeasonProfileValidationIsVisible = ThisViewModelIsInitialised;

        SeasonProfileAndIdentityValidationVm.IdentityValidationIsVisible = ThisViewModelIsInitialised &&
                                                                           SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is not null;

        var readyToGo = ThisViewModelIsInitialised &&
                        SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is not null &&
                        SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem is not null;

        LaunchWorkSessionButtonVm.IsVisible = readyToGo;

        var tallyToBePushed = RepositoryOfHubStyleEntries.GetAllEntriesAsRawDataNotYetPushed().Length;

        PushDataIncrementallyFromMemoryToRemoteHubButtonVm.IsVisible = tallyToBePushed > 0;

        PushDataIncrementallyFromMemoryToRemoteHubButtonVm.Label = $"Push to hub + {tallyToBePushed}";

        DataGridOfItemsInRepository.IsVisible = DataGridOfItemsInRepository.ItemsSource.Any();

        CboLookUpFileFormatsVm.IsVisible = true;
    }

        protected virtual ButtonControlViewModel[] MakeListOfAllGuiButtonsThatTouchData()
    {
        var answer = new[]
        {
            CheckConnectionToRezultzHubButtonVm,
            LaunchWorkSessionButtonVm,
            PullAllItemsFromHubButtonVm,
            AcceptRepositoryItemBeingEditedButtonVm,
            RejectRepositoryItemBeingEditedButtonVm,
            PushDataIncrementallyFromMemoryToRemoteHubButtonVm,
            CommitDataInMemoryIntoLocalStorageButtonVm,
            DeleteAllDataButtonVm,
            DeleteAllDataInMemoryButtonVm,
            DeleteAllDataInLocalStorageButtonVm,

            DeleteAllDataInMemoryProtectionButtonVm,

            PullAllItemsFromHubProtectionButtonVm,
            ForcePushAllDataInMemoryToRemoteHubButtonVm,
            RefreshRepositoryDataGridButtonVm,
            RefreshLocalStorageDataGridButtonVm,
            RefreshRemoteHubDataGridButtonVm,
            RefreshAllDataGridsAndListViewsButtonVm
        };

        return answer;
    }

        protected virtual TextBoxControlViewModel[] MakeListOfTextBoxControlViewModels()
    {
        var answer = new[]
        {
            ForEnteringMultipleIdentifiersForDataGridRowFilter,
            ForEnteringMultipleUserNamesOfPeopleWhoDidTheDataEntriesForDataGridRowFilter
        };

        return answer;
    }

        protected virtual IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>[] MakeListOfCboViewModels()
    {
        var answer = new[]
        {
            CboLookUpFileFormatsVm
        };

        return answer;
    }

        #endregion

        #region GenesisAsLastKnownGood

        protected SeasonProfileItem SeasonProfileItemUponLaunchOfWorkSession;
        protected IdentityItem IdentityItemUponLaunchOfWorkSession;

        protected SeriesItemDisplayObject SeriesItemUponLaunchOfWorkSession;
        protected EventItemDisplayObject EventItemUponLaunchOfWorkSession;

        protected void SaveGenesisOfThisViewModelAsLastKnownGood()
    {
        SeasonProfileItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem;
        IdentityItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem;
        SeriesItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem;
        EventItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem;
    }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        if (SeasonProfileItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem)
            return true;

        if (IdentityItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem)
            return true;


        if (SeriesItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem)
            return true;

        if (EventItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem)
            return true;

        return false;
    }

        public void ThrowIfLastKnownGoodGenesisOfThisViewModeIsNull()
    {
        bool LastKnownGoodGenesisOfThisViewModelIsNull()
        {
            if (SeasonProfileItemUponLaunchOfWorkSession is null)
                return true;

            if (IdentityItemUponLaunchOfWorkSession is null)
                return true;

            if (SeriesItemUponLaunchOfWorkSession is null)
                return true;

            if (EventItemUponLaunchOfWorkSession is null)
                return true;

            return false;
        }

        if (LastKnownGoodGenesisOfThisViewModelIsNull())
            throw new JghAlertMessageException(StringsPortal.Must_complete_launch_of_work_session);
    }

        public void ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        string LastKnownGoodGenesisOfThisViewModelHasChangedReason()
        {
            const int width = 50;

            var prefix = "You have changed your work session particulars.  Please re-launch the work session if you wish to continue.";

            var seasonProfileItemHasChanged = SeasonProfileItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem;
            var identityItemHasChanged = IdentityItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CurrentlyAuthenticatedIdentityItem;
            var seriesItemHasChanged = SeriesItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CboLookupSeriesVm?.CurrentItem;
            var eventItemHasChanged = EventItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CboLookupEventVm?.CurrentItem;


            var changeOfSeasonProfileItemMsg = string.Empty;
            var changeOfSeasonProfileItemMsg2 = string.Empty;

            var changeOfIdentityItemMsg = string.Empty;
            var changeOfIdentityItemMsg2 = string.Empty;

            var changeOfSeriesMsg = string.Empty;
            var changeOfSeriesMsg2 = string.Empty;

            var changeOfEventMsg = string.Empty;
            var changeOfEventMsg2 = string.Empty;

            if (seasonProfileItemHasChanged)
            {
                changeOfSeasonProfileItemMsg = $"{JghString.LeftAlign("Launched season profile ID:", width)} {SeasonProfileItemUponLaunchOfWorkSession?.FragmentInFileNameOfAssociatedProfileFile}";
                changeOfSeasonProfileItemMsg2 = $"{JghString.LeftAlign("Current season profile ID:", width)} {SeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem?.FragmentInFileNameOfAssociatedProfileFile}";
            }

            if (identityItemHasChanged)
            {
                changeOfIdentityItemMsg = $"{JghString.LeftAlign("Launched identity:", width)} {IdentityItemUponLaunchOfWorkSession}";
                changeOfIdentityItemMsg2 = $"{JghString.LeftAlign("Current identity:", width)} {SeasonProfileAndIdentityValidationVm?.CurrentlyAuthenticatedIdentityItem}";
            }

            if (seriesItemHasChanged)
            {
                changeOfSeriesMsg = $"Launched series : {SeriesItemUponLaunchOfWorkSession?.Label}";
                changeOfSeriesMsg2 = $"Currently selected series : {SeasonProfileAndIdentityValidationVm?.CboLookupSeriesVm?.CurrentItem?.Label}";
            }

            if (eventItemHasChanged)
            {
                changeOfEventMsg = $"Launched event : {EventItemUponLaunchOfWorkSession?.Label}";
                changeOfEventMsg2 = $"Currently selected event : {SeasonProfileAndIdentityValidationVm?.CboLookupEventVm?.CurrentItem?.Label}";
            }

            var details = JghString.ConcatAsLines(changeOfSeasonProfileItemMsg, changeOfSeasonProfileItemMsg2, changeOfIdentityItemMsg, changeOfIdentityItemMsg2, changeOfEventMsg, changeOfEventMsg2, changeOfSeriesMsg, changeOfSeriesMsg2);

            var answer = JghString.ConcatAsParagraphs(prefix, "Details : -", details);

            return answer;
        }

        if (LastKnownGoodGenesisOfThisViewModelHasChanged())
            throw new JghAlertMessageException(LastKnownGoodGenesisOfThisViewModelHasChangedReason());
    }

        #endregion

        #region page access gatekeeping

        public void ThrowIfWorkSessionNotReadyForLaunch()
    {
        if (!SeasonProfileAndIdentityValidationVm.ThisViewModelIsInitialised)
            throw new JghAlertMessageException(StringsPortal.SeasonDataNotInitialised);

        if (SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is null)
            throw new JghAlertMessageException(StringsPortal.SeasonDataNotInitialised);

        if (SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem is null)
            throw new JghAlertMessageException(StringsPortal.SelectedSeriesIsNull);

        if (SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem is null)
            throw new JghAlertMessageException(StringsPortal.SelectedEventIsNull);
    }

        public void ThrowIfWorkSessionNotProperlyInitialised()
    {
        if (!ThisViewModelIsInitialised)
            throw new JghAlertMessageException(StringsPortal.WorkSessionNotLaunched);

        if (!SeasonProfileAndIdentityValidationVm.ThisViewModelIsInitialised)
            throw new JghAlertMessageException(StringsPortal.SeasonDataNotInitialised);

        if (SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is null)
            throw new JghAlertMessageException(StringsPortal.SeasonDataNotInitialised);

        if (SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem is null)
            throw new JghAlertMessageException(StringsPortal.IdentityNotAuthenticated);

        if (!SeasonProfileAndIdentityValidationVm.GetIfCurrentlyAuthenticatedIdentityUserIsAuthorisedForRequiredWorkRole())
            throw new JghAlertMessageException($"{StringsPortal.Sorry_not_authorised_for_workrole}");

        if (!SeasonProfileAndIdentityValidationVm.GetIfCurrentlyAuthenticatedIdentityUserIsAuthorisedForRequiredWorkRole())
            throw new JghAlertMessageException(StringsPortal.IdentityNotAuthorisedForWorkRole);

        if (!WorkSessionIsLaunched)
            throw new JghAlertMessageException(StringsPortal.WorkSessionNotLaunched);
    }

        #endregion
    }
}
