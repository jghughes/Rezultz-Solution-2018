using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Xamarin.Common.Jan2019;
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
using Rezultz.DataTransferObjects.Nov2023.Results;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.PublisherModuleItems;
using Rezultz.DataTypes.Nov2023.RezultzItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library01.Mar2024.Repositories;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.ValidationViewModels;
using RezultzPortal.Uwp.Strings;
using RezultzSvc.Agents.Mar2024.SvcAgents;

// ReSharper disable InconsistentNaming

// ReSharper disable UnusedMethodReturnValue.Local


namespace RezultzPortal.Uwp.PageViewModels
{
    public class PublishSingleEventResultsViewModel : BaseViewViewModel
    {
        private const string Locus2 = nameof(PublishSingleEventResultsViewModel);
        private const string Locus3 = "[RezultzPortal.Uwp]";

        //private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 50;

        #region ctor

        // NB public, not protected
        public PublishSingleEventResultsViewModel(
            IRaceResultsPublishingSvcAgent raceResultsPublishingSvcAgent,
            ITimeKeepingSvcAgent timeKeepingSvcAgent,
            IRegistrationSvcAgent registrationSvcAgent,
            ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent,
            IThingsPersistedInLocalStorage thingsPersistedInLocalStorage,
            ILocalStorageService localStorageService)
    {
        const string failure = "Unable to construct object PublishSingleEventResultsViewModel.";
        const string locus = "[ctor]";

        try
        {
            #region assign ctor IOC injections

            //_azureStorageSvcAgent = azureStorageSvcAgent;
            _raceResultsPublishingSvcAgent = raceResultsPublishingSvcAgent;
            _timeKeepingSvcAgent = timeKeepingSvcAgent;
            _registrationSvcAgent = registrationSvcAgent;

            #endregion

            #region instantiate validation vm

            SeasonProfileAndIdentityValidationVm = new SeasonProfileAndIdentityValidationViewModel(leaderboardResultsSvcAgent, thingsPersistedInLocalStorage, localStorageService)
            {
                OwnerOfThisServiceIsPortalNotRezultz = true,
                CurrentRequiredWorkRole = EnumStringsForTimingSystemWorkRoles.Publishing
            };

            SeasonProfileAndIdentityValidationVm.PropertyChanged += SeasonProfileAndIdentityValidationVmINotifyPropertyChangedEventHandler;

            //SeasonProfileAndIdentityValidationVm.CboLookupSeasonVm.CurrentItem.PropertyChanged += SeasonProfileAndIdentityValidationVmINotifyPropertyChangedEventHandler;
            //SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.PropertyChanged += SeasonProfileAndIdentityValidationVmINotifyPropertyChangedEventHandler;
            //SeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem.PropertyChanged += SeasonProfileAndIdentityValidationVmINotifyPropertyChangedEventHandler;
            //SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm.CurrentItem.PropertyChanged += SeasonProfileAndIdentityValidationVmINotifyPropertyChangedEventHandler;

            #endregion

            #region instantiate ButtonVms

            CheckConnectionToRezultzHubButtonVm = new ButtonControlViewModel(CheckConnectionToRezultzHubButtonOnClickExecuteAsync, CheckConnectionToRezultzHubButtonOnClickCanExecute);
            CheckAvailabilityOfPublishingServiceButtonVm = new ButtonControlViewModel(CheckAvailabilityOfPublishingServiceButtonOnClickExecuteAsync, CheckAvailabilityOfPublishingServiceButtonOnClickCanExecute);

            SubmitPublishingProfileFileNameForValidationButtonVm = new ButtonControlViewModel(SubmitPublishingProfileFileNameForValidationButtonOnClickExecuteAsync, SubmitPublishingProfileFileNameForValidationButtonOnClickCanExecute)
                {IsVisible = true};
            ClearPublishingProfileFileNameButtonVm = new ButtonControlViewModel(ClearPublishingProfileFileNameButtonOnClickExecuteAsync, ClearPublishingProfileFileNameButtonOnClickCanExecute) {IsVisible = true};

            LaunchWorkSessionButtonVm = new ButtonControlViewModel(LaunchWorkSessionButtonVmOnClickExecuteAsync, LaunchWorkSessionButtonVmOnClickCanExecute);
            CleanAndRefreshPublishingSequenceButtonVm = new ButtonControlViewModel(CleanAndRefreshPublishingSequenceButtonOnClickExecuteAsync, CleanAndRefreshPublishingSequenceButtonOnClickCanExecute);

            PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm =
                new PublishingModuleButtonControlViewModel(PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickExecuteAsync, PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickCanExecute);
            PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm =
                new PublishingModuleButtonControlViewModel(PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickExecuteAsync, PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickCanExecute);
            PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm =
                new PublishingModuleButtonControlViewModel(PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickExecuteAsync, PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickCanExecute);

            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01 = new PublishingModuleButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02 = new PublishingModuleButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03 = new PublishingModuleButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04 = new PublishingModuleButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05 = new PublishingModuleButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06 = new PublishingModuleButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07 = new PublishingModuleButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page

            ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonVm =
                new ButtonControlViewModel(ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickExecuteAsync, ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickCanExecute);

            ExportProcessingReportToHardDriveButtonVm = new ButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page
            ExportLeaderboardToHardDriveButtonVm = new ButtonControlViewModel(() => { }, () => false); // click event handled in code-behind of page

            UploadLeaderboardToPreviewStorageButtonVm =
                new ButtonControlViewModel(UploadLeaderboardToPreviewStorageButtonOnClickExecuteAsync, UploadLeaderboardToPreviewStorageButtonOnClickCanExecute);

            UploadLeaderboardToPublishedStorageButtonVm =
                new ButtonControlViewModel(UploadLeaderboardToPublishedStorageButtonOnClickExecuteAsync, UploadLeaderboardToPublishedStorageButtonOnClickCanExecute);

            #endregion

            #region instantiate Textbox

            TextBoxForEnteringPublishingProfileFileNameFragmentVm = new TextBoxControlViewModel(TextBoxForEnteringPublishingProfileFileNameOnTextChangedExecuteAsync, TextBoxForEnteringPublishingProfileFileNameOnTextChangedCanExecute);

            #endregion

            #region instantiate Cbo

            CboLookupItemOfWorkingsForDisplayVm = new ItemDrivenCollectionViewModel<CboLookupItemDisplayObject>(
                    "Choose item",
                    CboLookupItemOfWorkingsOnSelectionChangedExecuteAsync,
                    CboLookupItemOfWorkingsOnSelectionChangedCanExecute)
                {Label = "Work item"};

            #endregion
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        #endregion

        #region event handler - INPC

        private void SeasonProfileAndIdentityValidationVmINotifyPropertyChangedEventHandler(object o, PropertyChangedEventArgs e)
    {
        if (e.PropertyName
            is nameof(SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem)
            or nameof(SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem)
            or nameof(SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem)
            or nameof(SeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem)
            or nameof(SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm.CurrentItem)
           )
        {
            NextThingToDoEnum = NextThingToDo.MakeControlsForLaunchingWorkSessionActive;

            EvaluateGui();
        }
    }

        #endregion

        #region EnumStrings

        private enum EnumForResultsDatabaseDestinations
        {
            Draft,

            Publish
            //Post
        }

        #endregion

        #region strings

        private const string Filename_not_recognised = "ID (filename) of publishing module not recognised.";

        private const string Working____validating = "Working .... validating";

        private const string FilenameIsBlankErrorMessage = "ID (filename) is incorrect. ID is blank.";
        private const string XmlFilename_not_yet_entered = "not yet submitted";
        private const string XmlFileName_cleared = "Profile cleared";
        private const string FilenameSuccessfullyConfirmed = "Publishing module loaded.";

        #endregion

        #region const

        private const int lhsWidth = 30;

        private const int lhsWidthPlus1 = lhsWidth + 1;
        private const int lhsWidthPlus2 = lhsWidth + 2;
        private const int lhsWidthPlus3 = lhsWidth + 3;
        private const int lhsWidthPlus5 = lhsWidth + 5;

        private const int lhsWidthLess1 = lhsWidth - 1;
        private const int lhsWidthLess3 = lhsWidth - 3;
        private const int lhsWidthLess4 = lhsWidth - 4;
        private const int lhsWidthLess5 = lhsWidth - 5;
        private const int lhsWidthLess6 = lhsWidth - 6;

        #endregion

        #region fields

        private StringBuilder _sbFriendlyLogOfActivity = new();
        private StringBuilder _sbFriendlyLogOfBlobAndFileTransfers = new();

        public NextThingToDo NextThingToDoEnum;

        public enum NextThingToDo
        {
            MakeControlsForCleanAndRefreshOfPublishingSequenceActive,
            MakeControlsForLaunchingWorkSessionActive,
            MakeControlsForBrowsingForLocalFilesAndDownloadingDataFromRemoteHubActive,
            MakeControlsForPreprocessingActive,
            MakeControlsForPublishingActive,
            MakeControlsForPullingConvertingAndUploadingDeactivated
        }

        #endregion

        #region global props

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

        private readonly IRaceResultsPublishingSvcAgent _raceResultsPublishingSvcAgent;
        private readonly ITimeKeepingSvcAgent _timeKeepingSvcAgent;
        private readonly IRegistrationSvcAgent _registrationSvcAgent;

        #endregion

        #region props

        #region validation vm

        public SeasonProfileAndIdentityValidationViewModel SeasonProfileAndIdentityValidationVm { get; }

        public ProgressIndicatorViewModelXamarin PublishingProfileValidationProgressIndicatorVm { get; } = new();

        #endregion

        #region simple props

        public bool WorkSessionIsLaunched;

        public bool ThisViewModelIsInitialised;

        public PublisherModuleProfileItem PublishingModuleProfile { get; private set; }

        public string EnumStringForTimestampsFromHub = "TimestampsEnum";
        public string EnumStringForParticipantsFromHub = "ParticipantsEnum";
        public string EnumStringForDraftLeaderboardFromHub = "DraftLeaderboardEnum";
        public string EnumStringForComputedLeaderboard = "ComputedLeaderboardEnum";

        public string EnumStringForLogOfFileTransfers = "LogOfFileTransfersEnum";
        public string EnumStringForProcessingReport = "ProcessingReportEnum";

        //public string EnumStringForImportedFile01 = "ImportedFile01Enum";
        //public string EnumStringForImportedFile02 = "ImportedFile02Enum";
        //public string EnumStringForImportedFile03 = "ImportedFile03Enum";
        //public string EnumStringForImportedFile04 = "ImportedFile04Enum";
        //public string EnumStringForImportedFile05 = "ImportedFile05Enum";

        public string SuccessfullyComputedLeaderboardAsXml { get; set; } = string.Empty;

        #endregion

        #region PublishingModuleValidationUserControlIsVisible

        private bool _backingstorePublishingModuleValidationUserControlIsVisible;

        public bool PublishingModuleValidationUserControlIsVisible
        {
            get => _backingstorePublishingModuleValidationUserControlIsVisible;
            set => SetProperty(ref _backingstorePublishingModuleValidationUserControlIsVisible, value);
        }

        #endregion

        #region headers and footers

        public HeaderOrFooterViewModel FootersVm { get; } = new();

        public HeaderOrFooterViewModel HeadersVm { get; } = new();

        #endregion

        #region TextBox presenters

        public TextBoxControlViewModel TextBoxForEnteringPublishingProfileFileNameFragmentVm { get; }

        #endregion

        #region TextBlock presenters

        public TextBlockControlViewModel CSharpPublisherModuleCodeNameTextVm { get; } = new() { IsVisible = true };
        public TextBlockControlViewModel CSharpPublisherModuleVersionNumberTextVm { get; } = new() { IsVisible = true };
        public TextBlockControlViewModel CSharpPublisherModuleVeryShortDescriptionTextVm { get; } = new() { IsVisible = true };
        public TextBlockControlViewModel CSharpPublisherModuleShortDescriptionTextVm { get; } = new() { IsVisible = true };
        public TextBlockControlViewModel CSharpPublisherModuleGeneralOverviewTextVm { get; } = new() { IsVisible = true };

        public TextBlockControlViewModel ProcessingReportTextVm { get; } = new();
        public TextBlockControlViewModel SavedFileNameOfProcessingReportTextVm { get; } = new();

        public TextBlockControlViewModel OutcomeOfProcessingOperationTextVm { get; } = new();

        public TextBlockControlViewModel SavedFileNameOfSuccessfullyProcessedLeaderboardTextVm { get; } = new();
        public TextBlockControlViewModel RanToCompletionMessageForPreviewLeaderboardTextVm { get; } = new();
        public TextBlockControlViewModel RanToCompletionMessageForPublishedLeaderboardTextVm { get; } = new();

        #endregion

        #region Button presenters

        public ButtonControlViewModel CheckConnectionToRezultzHubButtonVm { get; }
        public ButtonControlViewModel CheckAvailabilityOfPublishingServiceButtonVm { get; }

        public ButtonControlViewModel SubmitPublishingProfileFileNameForValidationButtonVm { get; }
        public ButtonControlViewModel ClearPublishingProfileFileNameButtonVm { get; }

        public ButtonControlViewModel LaunchWorkSessionButtonVm { get; }
        public ButtonControlViewModel CleanAndRefreshPublishingSequenceButtonVm { get; }

        public PublishingModuleButtonControlViewModel PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm { get; }
        public PublishingModuleButtonControlViewModel PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm { get; }
        public PublishingModuleButtonControlViewModel PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm { get; }

        public PublishingModuleButtonControlViewModel BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01 { get; }
        public PublishingModuleButtonControlViewModel BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02 { get; }
        public PublishingModuleButtonControlViewModel BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03 { get; }
        public PublishingModuleButtonControlViewModel BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04 { get; }
        public PublishingModuleButtonControlViewModel BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05 { get; }
        public PublishingModuleButtonControlViewModel BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06 { get; }
        public PublishingModuleButtonControlViewModel BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07 { get; }

        public ButtonControlViewModel ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonVm { get; }

        public ButtonControlViewModel ExportProcessingReportToHardDriveButtonVm { get; }
        public ButtonControlViewModel ExportLeaderboardToHardDriveButtonVm { get; }
        public ButtonControlViewModel UploadLeaderboardToPreviewStorageButtonVm { get; }
        public ButtonControlViewModel UploadLeaderboardToPublishedStorageButtonVm { get; }

        #endregion

        #region CboLookupItem presenters

        public ItemDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookupItemOfWorkingsForDisplayVm { get; }

        #endregion

        #endregion

        #region methods called directly or indirectly AFTER page to which this vm is the data context has completed loading

        public async Task<string> BeInitialisedFromPageCodeBehindOrchestrateAsync()
    {
        var failure = StringsForXamlPages.UnableToInitialiseViewmodel;
        const string locus = nameof(BeInitialisedFromPageCodeBehindOrchestrateAsync);

        try
        {
            if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                return string.Empty;

            DeadenGui();

            await BeInitialisedFromPageCodeBehindAsync();

            EnlivenGui();

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

        private async Task<string> BeInitialisedFromPageCodeBehindAsync()
    {
        const string failure = "Unable to initialise contents of page.";
        const string locus = "[BeInitialisedFromPageCodeBehindAsync]";

        try
        {
            FootersVm.Populate(StringsPortal.Welcome__);

            await ZeroisePublishingProfileAsync();

            await ZeroisePublishingSequence();

            await SeasonProfileAndIdentityValidationVm.BeInitialisedForRezultzPortalOrchestrateAsync();

            SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.Label = StringsPortal.Not_yet_launched;
            SeasonProfileAndIdentityValidationVm.CboLookupEventVm.Label = StringsPortal.Not_yet_launched;
            SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm.Label = StringsPortal.Not_yet_launched;

            await PopulateCboLookupItemOfWorkingsForDisplayVmAsync();

            ThisViewModelIsInitialised = true;

            NextThingToDoEnum = NextThingToDo.MakeControlsForLaunchingWorkSessionActive;

            return string.Empty;
        }
        catch (Exception ex)
        {
            ThisViewModelIsInitialised = false;

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

        private async Task PopulateCboLookupItemOfWorkingsForDisplayVmAsync()
    {
        await CboLookupItemOfWorkingsForDisplayVm.ZeroiseItemsSourceAsync();

        await CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject { Label = Symbols.SymbolNotApplicable, EnumString = Symbols.SymbolNotApplicable });
        await CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject { Label = "Processing report", EnumString = EnumStringForProcessingReport });
        await CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject {Label = "File movements", EnumString = EnumStringForLogOfFileTransfers});

        CboLookupItemOfWorkingsForDisplayVm.IsDropDownOpen = false;

        CboLookupItemOfWorkingsForDisplayVm.SaveSelectedItemAsLastKnownGood();

        CboLookupItemOfWorkingsForDisplayVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
    }

        #endregion

        #region commands

        #region CheckConnectionToRezultzHubButtonOnClickAsync

        protected virtual bool CheckConnectionToRezultzHubButtonOnClickCanExecute()
    {
        return CheckConnectionToRezultzHubButtonVm.IsAuthorisedToOperate;
    }

        private async void CheckConnectionToRezultzHubButtonOnClickExecuteAsync()
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

        public async Task<string> CheckConnectionToRezultzHubButtonOnClickAsync()
    {
        //const string failure = "Unable to do what this method does.";
        //const string locus = "[CheckConnectionToRezultzHubButtonOnClickAsync]";

        if (!NetworkInterface.GetIsNetworkAvailable())
            return StringsPortal.NoConnection;

        try
        {
            await _timeKeepingSvcAgent.ThrowIfNoServiceConnectionAsync();
        }
        catch (Exception e)
        {
            return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
        }

        return StringsPortal.ServiceUpAndRunning;
    }

        #endregion

        #region CheckAvailabilityOfPublishingServiceButtonOnClickAsync

        protected virtual bool CheckAvailabilityOfPublishingServiceButtonOnClickCanExecute()
    {
        return CheckAvailabilityOfPublishingServiceButtonVm.IsAuthorisedToOperate;
    }

        private async void CheckAvailabilityOfPublishingServiceButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CheckAvailabilityOfPublishingServiceButtonOnClickExecuteAsync]";

        try
        {
            if (!CheckAvailabilityOfPublishingServiceButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.WakingServer);

            DeadenGui();

            var answer = await CheckAvailabilityOfPublishingServiceButtonOnClickAsync();

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

        public async Task<string> CheckAvailabilityOfPublishingServiceButtonOnClickAsync()
    {
        //const string failure = "Unable to do what this method does.";
        //const string locus = "[CheckAvailabilityOfPublishingServiceButtonOnClickAsync]";

        if (!NetworkInterface.GetIsNetworkAvailable())
            return StringsPortal.NoConnection;

        try
        {
            await _raceResultsPublishingSvcAgent.ThrowIfNoServiceConnectionAsync();
        }
        catch (Exception e)
        {
            return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
        }

        return StringsPortal.ServiceUpAndRunning;
    }

        #endregion

        #region TextBoxForEnteringPublishingProfileFileNameOnTextChangedExecute

        private bool TextBoxForEnteringPublishingProfileFileNameOnTextChangedCanExecute()
    {
        return TextBoxForEnteringPublishingProfileFileNameFragmentVm.IsAuthorisedToOperate;
    }

        private void TextBoxForEnteringPublishingProfileFileNameOnTextChangedExecuteAsync()
    {
        if (!TextBoxForEnteringPublishingProfileFileNameOnTextChangedCanExecute())
            return;

        SubmitPublishingProfileFileNameForValidationButtonVm.IsAuthorisedToOperate = true;
    }

        #endregion

        #region SubmitPublishingProfileFileNameForValidationButtonOnClickAsync

        protected bool SubmitPublishingProfileFileNameForValidationButtonOnClickCanExecute()
    {
        return SubmitPublishingProfileFileNameForValidationButtonVm.IsAuthorisedToOperate;
    }

        private async void SubmitPublishingProfileFileNameForValidationButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[SubmitPublishingProfileFileNameForValidationButtonOnClickExecuteAsync]";

        try
        {
            if (!SubmitPublishingProfileFileNameForValidationButtonOnClickCanExecute())
                return;

            PublishingProfileValidationProgressIndicatorVm.OpenProgressIndicator(Working____validating);

            DeadenGui();

            var outcome = await SubmitPublishingProfileFileNameForValidationButtonOnClickAsync();

            EnlivenGui();

            PublishingProfileValidationProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(outcome);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                EvaluateGui();

            PublishingProfileValidationProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                Locus3, ex);
        }
        finally
        {
            PublishingProfileValidationProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        private async Task<string> SubmitPublishingProfileFileNameForValidationButtonOnClickAsync()
    {
        const string failure = "Unable to submit module file name and/or successfully obtain module profile.";
        const string locus = "[SubmitPublishingProfileFileNameForValidationButtonOnClickAsync]";

        try
        {
            #region bale?

            if (string.IsNullOrWhiteSpace(TextBoxForEnteringPublishingProfileFileNameFragmentVm.Text)) return JghString.ConcatAsSentences(FilenameIsBlankErrorMessage);

            if (!NetworkInterface.GetIsNetworkAvailable())
                return StringsPortal.NoConnection;

            await _raceResultsPublishingSvcAgent.ThrowIfNoServiceConnectionAsync();

            #endregion

            #region get ModuleSpecificationItemBelongingToThisViewModel

            var isRecognised = await _raceResultsPublishingSvcAgent.GetIfFileNameFragmentOfPublishingProfileIsRecognisedAsync(TextBoxForEnteringPublishingProfileFileNameFragmentVm.Text, CancellationToken.None);

            if (!isRecognised)
            {
                var listOfFileNameFragments = await _raceResultsPublishingSvcAgent.GetFileNameFragmentsOfAllPublishingProfilesAsync();

                var sb = new StringBuilder();

                foreach (var file in listOfFileNameFragments) sb.AppendLine(file);

                var preamble = JghString.ConcatAsSentences(Filename_not_recognised, "The available file names are as follows:");

                var messageNotOk = JghString.ConcatAsParagraphs(preamble, sb.ToString());

                return messageNotOk;
            }

            // following call throws an exception if anything at all goes wrong, especially deserialisation of the xml profile, hopefully providing a useful message

            PublishingModuleProfile = await _raceResultsPublishingSvcAgent.GetPublishingProfileAsync(TextBoxForEnteringPublishingProfileFileNameFragmentVm.Text, CancellationToken.None);

            #endregion

            #region success. load info. save and confirm

            TextBoxForEnteringPublishingProfileFileNameFragmentVm.Label = TextBoxForEnteringPublishingProfileFileNameFragmentVm.Text;

            var messageOk = $"{FilenameSuccessfullyConfirmed} ID=<{TextBoxForEnteringPublishingProfileFileNameFragmentVm.Label}>\r\n\r\n{PublishingModuleProfile.GeneralOverviewOfModule}";

            CSharpPublisherModuleShortDescriptionTextVm.Text = PublishingModuleProfile.ShortDescriptionOfModule;

            CSharpPublisherModuleGeneralOverviewTextVm.Text = PublishingModuleProfile.GeneralOverviewOfModule;

            await TextBoxForEnteringPublishingProfileFileNameFragmentVm.ChangeTextAsync(string.Empty);

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

        #region ClearPublishingProfileFileNameButtonOnClickAsync

        protected bool ClearPublishingProfileFileNameButtonOnClickCanExecute()
    {
        return ClearPublishingProfileFileNameButtonVm.IsAuthorisedToOperate;
    }

        private async void ClearPublishingProfileFileNameButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[ClearPublishingProfileFileNameButtonOnClickExecuteAsync]";

        try
        {
            if (!ClearPublishingProfileFileNameButtonOnClickCanExecute())
                return;

            DeadenGui();

            var messageOk = await ClearPublishingProfileFileNameButtonOnClickAsync();

            EnlivenGui();

            await AlertMessageService.ShowOkAsync(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                EvaluateGui();

            PublishingProfileValidationProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                Locus3, ex);
        }

        #endregion
    }

        private async Task<string> ClearPublishingProfileFileNameButtonOnClickAsync()
    {
        await ZeroisePublishingProfileAsync();

        return XmlFileName_cleared;
    }

        #endregion

        #region LaunchWorkSessionButtonVmOnClickAsync

        protected virtual bool LaunchWorkSessionButtonVmOnClickCanExecute()
    {
        return LaunchWorkSessionButtonVm.IsAuthorisedToOperate;
    }

        private async void LaunchWorkSessionButtonVmOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[LaunchWorkSessionButtonVmOnClickExecuteAsync]";

        try
        {
            if (!LaunchWorkSessionButtonVmOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working_____processing);

            DeadenGui();

            //THE MEAT
            var messageOk = await LaunchWorkSessionButtonVmOnClickAsync();

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

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        public async Task<string> LaunchWorkSessionButtonVmOnClickAsync()
    {
        const string failure = "Unable to clear or initialise data fields.";
        const string locus = "[LaunchWorkSessionButtonVmOnClickAsync]";

        try
        {
            WorkSessionIsLaunched = false;

            ThrowIfWorkSessionNotReadyForLaunch();

            await ZeroisePublishingSequence();

            foreach (var publishingModuleButtonControlViewModel in MakeListOfDatasetInputButtonVms())
                publishingModuleButtonControlViewModel.Zeroise();

            PopulatePublishingProfileRelatedTextFields();

            ConfigurePublisherDatasetImportButtons(DatasetImportButtonInitialiseFromProfile);

            await FetchExamplesOfDatasetsAssociatedWithButtonsAsync();

            SaveGenesisOfThisViewModelAsLastKnownGood();

            SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.Label = $"{_seriesItemUponLaunchOfWorkSession?.Label} launched at {DateTime.Now:HH:mm}";
            SeasonProfileAndIdentityValidationVm.CboLookupEventVm.Label = $"{_eventItemUponLaunchOfWorkSession?.Label} launched at {DateTime.Now:HH:mm}";
            SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm.Label = $"{BlobTargetOfPublishedResultsUponLaunchOfWorkSession?.Label} launched at {DateTime.Now:HH:mm}";

            HeadersVm.Populate(StringsPortal.Target_event, _seriesItemUponLaunchOfWorkSession?.Label, _eventItemUponLaunchOfWorkSession?.Label, BlobTargetOfPublishedResultsUponLaunchOfWorkSession?.Label,
                $"Work session launched at {DateTime.Now:HH:mm}");

            WorkSessionIsLaunched = true;

            NextThingToDoEnum = NextThingToDo.MakeControlsForCleanAndRefreshOfPublishingSequenceActive;

            HeadersVm.IsVisible = true;

            HeadersVm.SaveAsLastKnownGood();


            var messageOk = StringsPortal.Launch_succeeded;

            AppendToConversionReportLog(messageOk);

            return await Task.FromResult(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
            WorkSessionIsLaunched = false;

            HeadersVm.Populate(StringsPortal.Target_event, StringsPortal.Not_yet_launched);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region CleanAndRefreshPublishingSequenceButtonOnClickAsync

        protected virtual bool CleanAndRefreshPublishingSequenceButtonOnClickCanExecute()
    {
        return CleanAndRefreshPublishingSequenceButtonVm.IsAuthorisedToOperate;
    }

        private async void CleanAndRefreshPublishingSequenceButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CleanAndRefreshPublishingSequenceButtonOnClickExecuteAsync]";

        try
        {
            if (!CleanAndRefreshPublishingSequenceButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator("Cleaning...");

            DeadenGui();

            var messageOk = await CleanAndRefreshPublishingSequenceButtonOnClickAsync();

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

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        public async Task<string> CleanAndRefreshPublishingSequenceButtonOnClickAsync()
    {
        const string failure = "Unable to clear or initialise data fields.";
        const string locus = "[CleanAndRefreshPublishingSequenceButtonOnClickAsync]";

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            await CleanAndRefreshPublishingSequenceAsync();

            AppendToConversionReportLog("Everything refreshed in order to begin/restart publishing sequence.");

            NextThingToDoEnum = NextThingToDo.MakeControlsForBrowsingForLocalFilesAndDownloadingDataFromRemoteHubActive;

            var messageOk = "All clear. Ready to begin.";

            return await Task.FromResult(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickAsync

        protected virtual bool PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickCanExecute()
    {
        return PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm.IsAuthorisedToOperate;
    }

        private async void PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickExecuteAsync]";

        try
        {
            if (!PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator($"{StringsPortal.Working_____downloading}");

            DeadenGui();

            var progressReport = await PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(progressReport);
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

        private async Task<string> PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickAsync()
    {
        const string failure = "Unable to execute button click method.";
        const string locus = "[PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVmOnClickAsync]";

        var startDateTime = DateTime.UtcNow;

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            #region get ready

            var clockDataLocation =
                TimeKeepingSvcAgent.MakeDataLocationForStorageOfClockDataOnRemoteHub(SeriesItemDisplayObject.ObtainSourceModel(_seriesItemUponLaunchOfWorkSession),
                    EventItemDisplayObject.ObtainSourceModel(_eventItemUponLaunchOfWorkSession));

            #endregion

            #region check connection

            if (!NetworkInterface.GetIsNetworkAvailable())
                return StringsPortal.NoConnection;

            try
            {
                await _timeKeepingSvcAgent.ThrowIfNoServiceConnectionAsync();
            }
            catch (Exception e)
            {
                return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
            }

            #endregion

            #region get timestamps

            #region null check

            var exists = await _timeKeepingSvcAgent.GetIfContainerExistsAsync(clockDataLocation.Item1, clockDataLocation.Item2, CancellationToken.None);

            if (!exists)
            {
                PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetAsRawString = string.Empty;
                throw new JghAlertMessageException("No container of timestamps on hub. Hub was empty.");
            }

            #endregion

            #region pull

            AppendToConversionReportLog(StringsPortal.Working______pulling_timestamps_from_rezultz_hub);

            var downLoadedClockHubItems = await _timeKeepingSvcAgent.GetTimeStampItemArrayAsync(clockDataLocation.Item1, clockDataLocation.Item2, CancellationToken.None);

            if (downLoadedClockHubItems.Length == 0)
            {
                PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetAsRawString = string.Empty;
                throw new JghAlertMessageException("No timestamps received. Hub was empty.");
            }

            var getTimestampsReport = $"{downLoadedClockHubItems.Length} raw timestamp data entries received from hub.";

            AppendToConversionReportLog(getTimestampsReport);

            UpdateLogOfFilesThatWereTransferred("array of timestamps", "was imported into", "this app", $"{downLoadedClockHubItems.Length} line-items.");

            #endregion

            #endregion

            #region load timestamps

            AppendToConversionReportLog(StringsPortal.Working______loading_timestamps_into_repository);

            RepositoryOfHubStyleEntries<TimeStampHubItem> repositoryOfTimeStampHubItems = new();

            var createTimeStampHubItemRepositoryOutcome = repositoryOfTimeStampHubItems.TryAddRangeNoDuplicates(downLoadedClockHubItems, out var clockErrorMessage);

            if (createTimeStampHubItemRepositoryOutcome == false) throw new JghAlertMessageException($"Unable to proceed. {clockErrorMessage}");

            var loadTimestampRepositoryReport = $"{JghString.LeftAlign("Obtained from hub:", lhsWidthLess6)} {downLoadedClockHubItems.Length} timestamp-related line-items";

            AppendToConversionReportLog(loadTimestampRepositoryReport);

            #endregion

            #region consolidate list of SplitIntervalsPerParticipantItem

            AppendToConversionReportLog(StringsPortal.Working______consolidating_timestamps_into_consolidated_splitintervals_for_each_participant);

            var repositoryOfSplitIntervals = new RepositoryOfSplitDurationsPerParticipant();

            repositoryOfSplitIntervals.LoadRepository(EventItemDisplayObject.ObtainSourceModel(_eventItemUponLaunchOfWorkSession), repositoryOfTimeStampHubItems, null);

            var dataTransferObjects = repositoryOfSplitIntervals.GetTimeStampsAsSplitDurationsPerPersonInRankOrder(int.MaxValue, 0, 0);

            if (dataTransferObjects is null || !dataTransferObjects.Any()) throw new JghAlertMessageException("No valid split-intervals available to upload.");

            var dataTransferObjectsAsJson = JghSerialisation.ToJsonFromObject(dataTransferObjects);

            var consolidationReport = $"{JghString.LeftAlign("Consolidation:", lhsWidthLess1)} {dataTransferObjects.Length} consolidated split-intervals created";

            AppendToConversionReportLog(consolidationReport);

            #endregion

            #region upload dataset

            AppendToConversionReportLog(StringsPortal.Working______uploading);

            var accountName = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DatabaseAccountName;
            var containerName = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DataContainerName;
            var datasetEntityName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('_',
                $"{DateTime.Now.ToString(JghDateTime.SortablePattern)}___{EnumsForPublisherModule.TimestampsAsConsolidatedSplitIntervalsFromRemotePortalHubAsJson}"); // create an artificial but meaningful file name and assign to SingleEventResultsObtainedFromHubDescriptionOfOrigin

            var uploadDidSucceed = await _raceResultsPublishingSvcAgent.UploadSourceDatasetToBeProcessedSubsequentlyAsync(EnumsForPublisherModule.TimestampsAsConsolidatedSplitIntervalsFromRemotePortalHubAsJson,
                new EntityLocationItem(accountName, containerName, datasetEntityName),
                dataTransferObjectsAsJson, CancellationToken.None);

            if (!uploadDidSucceed)
            {
                var uploadDatasetFailureReport = $"{JghString.LeftAlign("Dataset upload:", lhsWidthLess4)} Failure. <{datasetEntityName}> failed to upload to <{containerName}>";

                AppendToConversionReportLog(uploadDatasetFailureReport);

                throw new JghAlertMessageException(uploadDatasetFailureReport);
            }

            var fileSizeDescription = JghConvert.SizeOfBytesInHighestUnitOfMeasure(JghConvert.ToBytesUtf8FromString(dataTransferObjectsAsJson).Length);

            var uploadTimestampDatasetSuccessReport = $"{JghString.LeftAlign("Dataset uploaded:", lhsWidthLess4)} <{datasetEntityName}>";

            AppendToConversionReportLog(uploadTimestampDatasetSuccessReport);

            UpdateLogOfFilesThatWereTransferred($"{datasetEntityName}", "was uploaded in preparation for processing to", $"{containerName}", fileSizeDescription);

            #endregion

            #region update GUI

            await CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject {Label = "Timestamps", Blurb = dataTransferObjectsAsJson, EnumString = EnumStringForTimestampsFromHub});

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            var ranToCompletionMsgSb = new JghStringBuilder();
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Operation ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine(loadTimestampRepositoryReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{clockDataLocation.Item2}>");
            ranToCompletionMsgSb.AppendLine(consolidationReport);
            ranToCompletionMsgSb.AppendLine(uploadTimestampDatasetSuccessReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{containerName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("File size:", lhsWidthPlus5)} {fileSizeDescription}");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. Dataset uploaded into position for processing.");

            #endregion

            #region update button

            PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetHasBeenUploaded = true;
            PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetFileNameForUpload = datasetEntityName;
            PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetFileUploadOutcomeReport = ranToCompletionMsgSb.ToString();
            PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetAsRawString = dataTransferObjectsAsJson;

            #endregion

            if (!MakeListOfDatasetInputButtonVms().Any(z => z.IsDesignated && !z.DatasetHasBeenUploaded)) NextThingToDoEnum = NextThingToDo.MakeControlsForPreprocessingActive;

            return ranToCompletionMsgSb.ToString();
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickAsync

        protected virtual bool PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickCanExecute()
    {
        return PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm.IsAuthorisedToOperate;
    }

        private async void PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[PullParticipantsRezultzHubAndUploadButtonOnClickExecuteAsync]";

        try
        {
            if (!PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator($"{StringsPortal.Working_____downloading}");

            DeadenGui();

            var progressReport = await PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(progressReport);
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

        private async Task<string> PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonOnClickAsync()
    {
        const string failure = "Unable to execute button click method.";
        const string locus = "[PullParticipantsRezultzHubAndUploadButtonOnClickAsync]";

        var startDateTime = DateTime.UtcNow;

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            #region get ready

            var participantDataLocation =
                ParticipantRegistrationSvcAgent.MakeDataLocationForStorageOfParticipantDataOnRemoteHub(SeriesItemDisplayObject.ObtainSourceModel(_seriesItemUponLaunchOfWorkSession),
                    EventItemDisplayObject.ObtainSourceModel(_eventItemUponLaunchOfWorkSession));

            #endregion

            #region check connection

            if (!NetworkInterface.GetIsNetworkAvailable())
                return StringsPortal.NoConnection;

            try
            {
                await _timeKeepingSvcAgent.ThrowIfNoServiceConnectionAsync();
            }
            catch (Exception e)
            {
                return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
            }

            #endregion

            #region get participants

            #region null check

            var containerDoesExist = await _registrationSvcAgent.GetIfContainerExistsAsync(participantDataLocation.Item1, participantDataLocation.Item2, CancellationToken.None);

            if (!containerDoesExist) throw new JghAlertMessageException("Unable to proceed. No participant items received. Hub was empty.");

            #endregion

            #region pull

            AppendToConversionReportLog(StringsPortal.Working______pulling_participant_profiles_from_rezultz_hub);

            var downLoadedParticipantHubItems = await _registrationSvcAgent.GetParticipantItemArrayAsync(participantDataLocation.Item1, participantDataLocation.Item2, CancellationToken.None);

            if (downLoadedParticipantHubItems.Length == 0) throw new JghAlertMessageException("Unable to proceed. No participant items received. Hub was empty.");

            var pullParticipantsReport = $"{downLoadedParticipantHubItems.Length} participant related line-items received from hub.";

            AppendToConversionReportLog(pullParticipantsReport);

            #endregion

            #endregion

            #region load participants

            AppendToConversionReportLog(StringsPortal.Working______loading_participants_into_repository);

            RepositoryOfHubStyleEntries<ParticipantHubItem> repositoryOfParticipantHubItemEntries = new();

            var createParticipantHubItemRepositoryOutcome = repositoryOfParticipantHubItemEntries.TryAddRangeNoDuplicates(downLoadedParticipantHubItems, out var participantErrorMessage);

            if (createParticipantHubItemRepositoryOutcome == false) throw new JghAlertMessageException($"Unable to proceed. {participantErrorMessage}");

            var loadParticipantRepositoryReport = $"{JghString.LeftAlign("Obtained from hub:", lhsWidthLess3)} {downLoadedParticipantHubItems.Length} participant-related line-items";

            AppendToConversionReportLog(loadParticipantRepositoryReport);

            #endregion

            #region synthesise list of ParticipantHubItemDto and assign to ParticipantMasterListObtainedFromHub

            AppendToConversionReportLog(StringsPortal.Working______generating_participant_master_list);

            var participantDatabase = new ParticipantDatabase();

            participantDatabase.LoadDatabaseV2(repositoryOfParticipantHubItemEntries);

            var dataTransferObjects = ParticipantHubItem.ToDataTransferObject(participantDatabase.GetMasterList());

            if (dataTransferObjects is null || !dataTransferObjects.Any()) throw new JghAlertMessageException("No participants available.");

            var dataTransferObjectsAsJson = JghSerialisation.ToJsonFromObject(dataTransferObjects);

            var consolidationReport = $"{JghString.LeftAlign("Consolidation:", lhsWidthLess1)} {dataTransferObjects.Length} participants created";

            AppendToConversionReportLog(consolidationReport);

            #endregion

            #region upload dataset

            AppendToConversionReportLog(StringsPortal.Working______uploading);

            var accountName = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DatabaseAccountName;
            var containerName = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DataContainerName;
            var datasetEntityName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('_',
                $"{DateTime.Now.ToString(JghDateTime.SortablePattern)}___{EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub}");

            var uploadDidSucceed = await _raceResultsPublishingSvcAgent.UploadSourceDatasetToBeProcessedSubsequentlyAsync(EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub,
                new EntityLocationItem(accountName, containerName, datasetEntityName),
                dataTransferObjectsAsJson,
                CancellationToken.None);

            if (!uploadDidSucceed)
            {
                var uploadDatasetFailureReport = $"{JghString.LeftAlign("Dataset upload:", lhsWidthLess4)} Failure. <{datasetEntityName}> failed to upload to <{containerName}>";

                AppendToConversionReportLog(uploadDatasetFailureReport);

                throw new JghAlertMessageException(uploadDatasetFailureReport);
            }

            var fileSizeDescription = JghConvert.SizeOfBytesInHighestUnitOfMeasure(JghConvert.ToBytesUtf8FromString(dataTransferObjectsAsJson).Length);

            var uploadParticipantDatasetSuccessReport = $"{JghString.LeftAlign("Dataset uploaded:", lhsWidthLess4)} <{datasetEntityName}>";
            //var uploadParticipantDatasetSuccessReport = $"{JghString.LeftAlign("Dataset uploaded:", lhsWidthLess4)} <{datasetEntityName}> {fileSizeDescription}";

            AppendToConversionReportLog(uploadParticipantDatasetSuccessReport);

            UpdateLogOfFilesThatWereTransferred($"{datasetEntityName}", "was uploaded in preparation for processing to", $"{containerName}", fileSizeDescription);

            #endregion

            #region update GUI

            await CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject {Label = "Participants", Blurb = dataTransferObjectsAsJson, EnumString = EnumStringForParticipantsFromHub});

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            var ranToCompletionMsgSb = new JghStringBuilder();
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Operation ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine(loadParticipantRepositoryReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{participantDataLocation.Item2}>");
            ranToCompletionMsgSb.AppendLine(consolidationReport);
            ranToCompletionMsgSb.AppendLine(uploadParticipantDatasetSuccessReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{containerName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("File size:", lhsWidthPlus5)} {fileSizeDescription}");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. Dataset uploaded into position for processing.");

            #endregion

            #region update button

            PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetHasBeenUploaded = true;
            PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetFileNameForUpload = datasetEntityName;
            PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetAsRawString = dataTransferObjectsAsJson;
            PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetFileUploadOutcomeReport = ranToCompletionMsgSb.ToString();

            #endregion

            if (!MakeListOfDatasetInputButtonVms().Any(z => z.IsDesignated && !z.DatasetHasBeenUploaded)) NextThingToDoEnum = NextThingToDo.MakeControlsForPreprocessingActive;

            return ranToCompletionMsgSb.ToString();
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickAsync

        protected virtual bool PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickCanExecute()
    {
        return PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.IsAuthorisedToOperate;
    }

        private async void PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickExecuteAsync]";

        try
        {
            if (!PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator($"{StringsPortal.Working_____downloading}");

            DeadenGui();

            var progressReport = await PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(progressReport);
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

        private async Task<string> PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickAsync()
    {
        const string failure = "Unable to execute button click method.";
        const string locus = "[PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonOnClickAsync]";

        var startDateTime = DateTime.UtcNow;

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            #region get ready

            var clockDataLocation =
                TimeKeepingSvcAgent.MakeDataLocationForStorageOfClockDataOnRemoteHub(SeriesItemDisplayObject.ObtainSourceModel(_seriesItemUponLaunchOfWorkSession),
                    EventItemDisplayObject.ObtainSourceModel(_eventItemUponLaunchOfWorkSession));

            var participantDataLocation =
                ParticipantRegistrationSvcAgent.MakeDataLocationForStorageOfParticipantDataOnRemoteHub(SeriesItemDisplayObject.ObtainSourceModel(_seriesItemUponLaunchOfWorkSession),
                    EventItemDisplayObject.ObtainSourceModel(_eventItemUponLaunchOfWorkSession));

            #endregion

            #region check connection

            if (!NetworkInterface.GetIsNetworkAvailable())
                return StringsPortal.NoConnection;

            try
            {
                await _timeKeepingSvcAgent.ThrowIfNoServiceConnectionAsync();
            }
            catch (Exception e)
            {
                return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
            }

            #endregion

            #region get timestamps

            #region null check

            var exists = await _timeKeepingSvcAgent.GetIfContainerExistsAsync(clockDataLocation.Item1, clockDataLocation.Item2, CancellationToken.None);

            if (!exists)
            {
                PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetAsRawString = string.Empty;
                throw new JghAlertMessageException("No container of timestamps on hub. Hub was empty.");
            }

            #endregion

            #region pull

            AppendToConversionReportLog(StringsPortal.Working______pulling_timestamps_from_rezultz_hub);

            var downLoadedClockHubItems = await _timeKeepingSvcAgent.GetTimeStampItemArrayAsync(clockDataLocation.Item1, clockDataLocation.Item2, CancellationToken.None);

            if (downLoadedClockHubItems.Length == 0)
            {
                PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetAsRawString = string.Empty;
                throw new JghAlertMessageException("No timestamps received. Hub was empty.");
            }

            #endregion

            #region load timestamps

            AppendToConversionReportLog(StringsPortal.Working______loading_timestamps_into_repository);

            RepositoryOfHubStyleEntries<TimeStampHubItem> repositoryOfTimeStampHubItems = new();

            var createTimeStampHubItemRepositoryOutcome = repositoryOfTimeStampHubItems.TryAddRangeNoDuplicates(downLoadedClockHubItems, out var clockErrorMessage);

            if (createTimeStampHubItemRepositoryOutcome == false) throw new JghAlertMessageException($"Unable to proceed. {clockErrorMessage}");

            var loadTimeStampRepositoryReport = $"{JghString.LeftAlign("Obtained from hub:", lhsWidthLess5)} {downLoadedClockHubItems.Length} timestamp-related line-items";

            AppendToConversionReportLog(loadTimeStampRepositoryReport);

            #endregion

            #endregion

            #region get participants

            #region null check

            var containerDoesExist = await _registrationSvcAgent.GetIfContainerExistsAsync(participantDataLocation.Item1, participantDataLocation.Item2, CancellationToken.None);

            if (!containerDoesExist) throw new JghAlertMessageException("Unable to proceed. No participant items received. Hub was empty.");

            #endregion

            #region pull

            AppendToConversionReportLog(StringsPortal.Working______pulling_participant_profiles_from_rezultz_hub);

            var downLoadedParticipantHubItems = await _registrationSvcAgent.GetParticipantItemArrayAsync(participantDataLocation.Item1, participantDataLocation.Item2, CancellationToken.None);

            if (downLoadedParticipantHubItems.Length == 0) throw new JghAlertMessageException("Unable to proceed. No participant items received. Hub was empty.");

            #endregion

            #region load participants into repository

            AppendToConversionReportLog(StringsPortal.Working______loading_participants_into_repository);

            RepositoryOfHubStyleEntries<ParticipantHubItem> repositoryOfParticipantHubItemEntries = new();

            var createParticipantHubItemRepositoryOutcome = repositoryOfParticipantHubItemEntries.TryAddRangeNoDuplicates(downLoadedParticipantHubItems, out var participantErrorMessage);

            if (createParticipantHubItemRepositoryOutcome == false) throw new JghAlertMessageException($"Unable to proceed. {participantErrorMessage}");

            var loadParticipantRepositoryReport = $"{JghString.LeftAlign("Obtained from hub:", lhsWidthLess5)} {downLoadedParticipantHubItems.Length} participant-related line-items";

            AppendToConversionReportLog(loadParticipantRepositoryReport);

            #endregion

            #endregion

            #region generate results

            AppendToConversionReportLog(StringsPortal.Working______generating_results);

            var repositoryOfSplitIntervals = new RepositoryOfSplitDurationsPerParticipant();

            repositoryOfSplitIntervals.LoadRepository(EventItemDisplayObject.ObtainSourceModel(_eventItemUponLaunchOfWorkSession), repositoryOfTimeStampHubItems, repositoryOfParticipantHubItemEntries);

            var dataTransferObjects = repositoryOfSplitIntervals.GetDraftResultItemDataTransferObjectForAllContestantsInRankOrder();

            if (dataTransferObjects is null || !dataTransferObjects.Any())
            {
                var missingDataReport = $"{JghString.LeftAlign("Dataset anomaly:", lhsWidth)} Failure. No valid results available to upload.";

                AppendToConversionReportLog(missingDataReport);

                throw new JghAlertMessageException(missingDataReport);
            }

            var dataTransferObjectsAsXml = JghSerialisation.ToXmlFromObject(dataTransferObjects, [typeof(ResultDto[])]);

            var consolidationReport = $"{JghString.LeftAlign("Consolidation:", lhsWidthLess1)} {dataTransferObjects.Length} results created";

            AppendToConversionReportLog(consolidationReport);

            #endregion

            #region upload obtained results

            AppendToConversionReportLog(StringsPortal.Working______uploading);

            var accountName = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DatabaseAccountName;
            var containerName = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.LocationOfCustomDatasetsUploadedForProcessing.DataContainerName;
            var datasetEntityName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-',
                $"{DateTime.Now.ToString(JghDateTime.SortablePattern)}+{EnumsForPublisherModule.ResultItemsAsXmlFromPortalNativeTimingSystem}");

            var uploadDidSucceed = await _raceResultsPublishingSvcAgent.UploadSourceDatasetToBeProcessedSubsequentlyAsync(EnumsForPublisherModule.ResultItemsAsXmlFromPortalNativeTimingSystem,
                new EntityLocationItem(accountName, containerName, datasetEntityName),
                dataTransferObjectsAsXml,
                CancellationToken.None);

            if (!uploadDidSucceed)
            {
                var uploadDatasetFailureReport = $"{JghString.LeftAlign("Dataset upload:", lhsWidthLess4)} Failure. <{datasetEntityName}> failed to upload to <{containerName}>";

                AppendToConversionReportLog(uploadDatasetFailureReport);

                throw new JghAlertMessageException(uploadDatasetFailureReport);
            }

            var fileSizeDescription = JghConvert.SizeOfBytesInHighestUnitOfMeasure(JghConvert.ToBytesUtf8FromString(dataTransferObjectsAsXml).Length);

            var uploadDatasetSuccessReport = $"{JghString.LeftAlign("Dataset uploaded:", lhsWidthLess4)} <{datasetEntityName}>";

            AppendToConversionReportLog(uploadDatasetSuccessReport);

            UpdateLogOfFilesThatWereTransferred($"{datasetEntityName}", "was uploaded in preparation for processing to", $"{containerName}", fileSizeDescription);

            #endregion

            #region update GUI

            await CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject
                {Label = "Draft dataset of results obtained from hub", Blurb = dataTransferObjectsAsXml, EnumString = EnumStringForDraftLeaderboardFromHub});

            var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;

            var ranToCompletionMsgSb = new JghStringBuilder();
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Operation ran to completion");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");
            ranToCompletionMsgSb.AppendLine(loadTimeStampRepositoryReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus2)} <{clockDataLocation.Item2}>");
            ranToCompletionMsgSb.AppendLine(loadParticipantRepositoryReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus2)} <{participantDataLocation.Item2}>");
            ranToCompletionMsgSb.AppendLine(consolidationReport);
            ranToCompletionMsgSb.AppendLine(uploadDatasetSuccessReport);
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus2)} <{containerName}>");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("File size:", lhsWidthPlus5)} {fileSizeDescription}");
            ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. Dataset uploaded into position for processing.");

            #endregion

            #region update button

            PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetHasBeenUploaded = true;
            PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetFileNameForUpload = datasetEntityName;
            PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetFileUploadOutcomeReport = ranToCompletionMsgSb.ToString();
            PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetAsRawString = dataTransferObjectsAsXml;

            #endregion

            if (!MakeListOfDatasetInputButtonVms().Any(z => z.IsDesignated && !z.DatasetHasBeenUploaded)) NextThingToDoEnum = NextThingToDo.MakeControlsForPreprocessingActive;

            return ranToCompletionMsgSb.ToString();
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        // commands/click-events for all BrowseHardDriveForFileAndUploadAsSourceDataButtons are to be found in the page code-behind

        #region ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickAsync

        protected virtual bool ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickCanExecute()
    {
        return ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonVm.IsAuthorisedToOperate;
    }

        private async void ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[PullResultsForSingleEventProcessedFromPreviouslyUploadedDatasetsButtonOnClickExecuteAsync]";

        try
        {
            ThrowIfWorkSessionNotProperlyInitialised();


            if (!ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickCanExecute()) return;

            GlobalProgressIndicatorVm.OpenProgressIndicator($"{StringsPortal.Working_____converting}");

            DeadenGui();

            var progressReport = await ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickAsync();

            EnlivenGui();

            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(progressReport);
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

        private async Task<string> ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickAsync()
    {
        #region local helpers

        PublisherImportFileTargetItem[] GatherTargetsForDatasetsImportedAndPreviouslyUploadedForSubsequentProcessing()
        {
            const string failure2 = "Unable to do what this method does.";
            const string locus2 = "[GatherTargetsForDatasetsImportedAndPreviouslyUploadedForSubsequentProcessing]";

            var datasetsToBeProcessed = new List<PublisherImportFileTargetItem>();

            void AddDataset(PublishingModuleButtonControlViewModel buttonViewModel)
            {
                if (buttonViewModel is null || buttonViewModel.IsDesignated == false)
                    return;

                PublisherImportFileTargetItem importFileTargetItem = new(
                    buttonViewModel.DatasetIdentifyingEnum,
                    buttonViewModel.DatasetFileNameForUpload);

                datasetsToBeProcessed.Add(importFileTargetItem);
            }

            try
            {
                AddDataset(PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm);
                AddDataset(PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm);
                AddDataset(PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm);
                AddDataset(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01);
                AddDataset(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02);
                AddDataset(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03);
                AddDataset(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04);
                AddDataset(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05);
                AddDataset(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06);
                AddDataset(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07);

                    return datasetsToBeProcessed.ToArray();
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure2, locus2, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        const string failure = "Unable to execute button click method.";
        const string locus = "[ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonOnClickAsync]";

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            #region bale if no data to process

            if (string.IsNullOrWhiteSpace(PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06.DatasetAsRawString)
                && string.IsNullOrWhiteSpace(BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07.DatasetAsRawString))
                throw new JghAlertMessageException(StringsPortal.Unable_to_proceed__No_datasets_as_yet);

            #endregion

            #region obtain series buttonProfile etc

            AppendToConversionReportLog(StringsPortal.Working_____processing);

            var seriesItemVm = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem;

            var seriesProfile = SeriesItemDisplayObject.ObtainSourceModel(seriesItemVm);

            var seriesLabelAsIdentifier = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm.CurrentItem.Label;
            var eventLabelAsIdentifier = SeasonProfileAndIdentityValidationVm.CboLookupEventVm.CurrentItem.Label;

            #endregion

            #region do the preprocessing

            PublisherOutputItem publisherOutputItem = new();

            try
            {
                AppendToConversionReportLog(StringsPortal.Working_____processing);

                var datasetTargetsToBeProcessed = GatherTargetsForDatasetsImportedAndPreviouslyUploadedForSubsequentProcessing();

                
                publisherOutputItem = await _raceResultsPublishingSvcAgent.ProcessPreviouslyUploadedSourceDataIntoPublishableResultsForSingleEventAsync(
                    TextBoxForEnteringPublishingProfileFileNameFragmentVm.Label,
                    seriesLabelAsIdentifier, eventLabelAsIdentifier, seriesProfile, datasetTargetsToBeProcessed, CancellationToken.None);

                if (publisherOutputItem.ConversionDidFail)
                    throw new Exception(publisherOutputItem.RanToCompletionMessage);

                var computedResultsDto = ResultItem.ToDataTransferObject(publisherOutputItem.ComputedResults);

                var computedResultsAsXml = JghSerialisation.ToXmlFromObject(computedResultsDto, [typeof(ResultDto)]);

                ProcessingReportTextVm.Text = publisherOutputItem.ConversionReport;

                SuccessfullyComputedLeaderboardAsXml = computedResultsAsXml; // crucial assignment - this is what we pick up to be published eventually

                await CboLookupItemOfWorkingsForDisplayVm.AddItemToItemsSourceAsync(new CboLookupItemDisplayObject
                    {Label = "Computed results for publishing to leaderboard", Blurb = computedResultsAsXml, EnumString = EnumStringForComputedLeaderboard});

                AppendToConversionReportLog(publisherOutputItem.ConversionReport);
            }
            catch (Exception ex)
            {
                AppendToConversionReportLog(publisherOutputItem.ConversionReport);

                ProcessingReportTextVm.Text = string.Empty;

                SuccessfullyComputedLeaderboardAsXml = string.Empty;

                OutcomeOfProcessingOperationTextVm.Text = ex.Message;

                AppendToConversionReportLog(JghExceptionHelpers.FindInnermostException(ex).Message);

                throw;
            }

            var ranToCompletionReport = publisherOutputItem.RanToCompletionMessage;

            #endregion

            NextThingToDoEnum = NextThingToDo.MakeControlsForPublishingActive;

            OutcomeOfProcessingOperationTextVm.Text = ranToCompletionReport;

            return ranToCompletionReport;
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region UploadLeaderboardToPreviewStorageButtonOnClickAsync

        protected virtual bool UploadLeaderboardToPreviewStorageButtonOnClickCanExecute()
    {
        return UploadLeaderboardToPreviewStorageButtonVm.IsAuthorisedToOperate;
    }

        private async void UploadLeaderboardToPreviewStorageButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[UploadLeaderboardToPreviewStorageButtonOnClickExecuteAsync]";

        try
        {
            if (!UploadLeaderboardToPreviewStorageButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working______uploading);

            DeadenGui();

            var messageOk = await UploadLeaderboardToPreviewStorageButtonOnClickAsync();

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

        private async Task<string> UploadLeaderboardToPreviewStorageButtonOnClickAsync()
    {
        const string failure = "Unable to execute button click method.";
        const string locus = "[UploadLeaderboardToPreviewStorageButtonOnClickAsync]";

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            var messageOk = await UploadProcessedArrayOfResultItemAsync(SuccessfullyComputedLeaderboardAsXml, EnumForResultsDatabaseDestinations.Draft);

            if (!string.IsNullOrWhiteSpace(messageOk)) RanToCompletionMessageForPreviewLeaderboardTextVm.Text = messageOk;

            NextThingToDoEnum = NextThingToDo.MakeControlsForPublishingActive;

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

        #region UploadLeaderboardToPublishedStorageButtonOnClickAsync

        protected virtual bool UploadLeaderboardToPublishedStorageButtonOnClickCanExecute()
    {
        return UploadLeaderboardToPublishedStorageButtonVm.IsAuthorisedToOperate;
    }

        private async void UploadLeaderboardToPublishedStorageButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[UploadLeaderboardToPublishedStorageButtonOnClickExecuteAsync]";

        try
        {
            if (!UploadLeaderboardToPublishedStorageButtonOnClickCanExecute())
                return;

            GlobalProgressIndicatorVm.OpenProgressIndicator(StringsPortal.Working______uploading);


            DeadenGui();

            //THE MEAT
            var messageOk = await UploadLeaderboardToPublishedStorageButtonOnClickAsync();

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

        private async Task<string> UploadLeaderboardToPublishedStorageButtonOnClickAsync()
    {
        const string failure = "Unable to upload converted data to production data storage location.";
        const string locus = "[UploadLeaderboardToPublishedStorageButtonOnClickAsync]";

        try
        {
            #region bale if something is out of whack

            ThrowIfWorkSessionNotProperlyInitialised();

            ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged();

            #endregion

            var ranToCompletionMessage = await UploadProcessedArrayOfResultItemAsync(SuccessfullyComputedLeaderboardAsXml, EnumForResultsDatabaseDestinations.Publish);

            if (!string.IsNullOrWhiteSpace(ranToCompletionMessage)) RanToCompletionMessageForPublishedLeaderboardTextVm.Text = ranToCompletionMessage;

            NextThingToDoEnum = NextThingToDo.MakeControlsForPullingConvertingAndUploadingDeactivated;

            return ranToCompletionMessage;
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        #endregion

        #region CboLookupItemOfWorkingsOnSelectionChangedAsync

        private bool CboLookupItemOfWorkingsOnSelectionChangedCanExecute()
    {
        return CboLookupItemOfWorkingsForDisplayVm.IsAuthorisedToOperate;
    }

        private async void CboLookupItemOfWorkingsOnSelectionChangedExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CboLookupItemOfWorkingsOnSelectionChangedExecuteAsync]";

        try
        {
            GlobalProgressIndicatorVm.OpenProgressIndicator("Working ....");

            // basically a dummy. does pretty much nothing at time of writing
            CboLookupItemOfWorkingsOnSelectionChangedAsync();
        }

        #region try catch

        catch (Exception ex)
        {
            GlobalProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            GlobalProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

        private void CboLookupItemOfWorkingsOnSelectionChangedAsync()
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[CboLookupItemOfWorkingsOnSelectionChangedAsync]";

        try
        {
            CboLookupItemOfWorkingsForDisplayVm.SaveSelectedItemAsLastKnownGood();
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

        #region delegate and delegate methods

        //private string MakeControlsForLaunchingWorkSessionActive()
        //{
        //    NextThingToDoEnum = NextThingToDo.MakeControlsForLaunchingWorkSessionActive;

        //    EnlivenGui();

        //    return string.Empty;
        //}

        private delegate bool ConfigureDatasetImportButtonDelegate(PublisherButtonProfileItem buttonProfile, PublishingModuleButtonControlViewModel buttonVm);

        private string ConfigurePublisherDatasetImportButtons(ConfigureDatasetImportButtonDelegate configureButtonVmMethod)
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[ConfigurePublisherDatasetImportButtons]";

        try
        {
            var publisherModuleProfile = PublishingModuleProfile;

            if (publisherModuleProfile is null) return string.Empty;

            foreach (var buttonProfile in publisherModuleProfile.GuiButtonProfilesForPullingDatasetsFromPortalHub)
            {
                if (buttonProfile is null)
                    continue;

                switch (buttonProfile.IdentifierOfAssociatedDataset)
                {
                    case EnumsForPublisherModule.TimestampsAsConsolidatedSplitIntervalsFromRemotePortalHubAsJson:
                        configureButtonVmMethod(buttonProfile, PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm);
                        break;
                    case EnumsForPublisherModule.ParticipantsAsJsonFromRemotePortalHub:
                        configureButtonVmMethod(buttonProfile, PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm);
                        break;
                    case EnumsForPublisherModule.ResultItemsAsXmlFromPortalNativeTimingSystem:
                        configureButtonVmMethod(buttonProfile, PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm);
                        break;
                }
            }

            var i = 1;

            foreach (var buttonProfile in publisherModuleProfile.GuiButtonProfilesForBrowsingFileSystemForDatasets)
            {
                if (buttonProfile is null)
                    continue;

                switch (i)
                {
                    case 1:
                        configureButtonVmMethod(buttonProfile, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01);
                        break;
                    case 2:
                        configureButtonVmMethod(buttonProfile, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02);
                        break;
                    case 3:
                        configureButtonVmMethod(buttonProfile, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03);
                        break;
                    case 4:
                        configureButtonVmMethod(buttonProfile, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04);
                        break;
                    case 5:
                        configureButtonVmMethod(buttonProfile, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05);
                        break;
                    case 6:
                        configureButtonVmMethod(buttonProfile, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06);
                        break;
                    case 7:
                        configureButtonVmMethod(buttonProfile, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07);
                        break;
                }

                    i++;
            }

            return string.Empty;
        }

        #region try catch handling

        catch (Exception ex)
        {
            ThisViewModelIsInitialised = JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex);

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

        private bool DatasetImportButtonZeroise(PublisherButtonProfileItem buttonProfile, PublishingModuleButtonControlViewModel buttonVm)
    {
        if (buttonProfile is null || buttonVm is null)
            return true;

        buttonVm.Zeroise();

        return true;
    }

        private bool DatasetImportButtonInitialiseFromProfile(PublisherButtonProfileItem buttonProfile, PublishingModuleButtonControlViewModel buttonVm)
    {
        if (buttonProfile is null || buttonVm is null)
            return true;

        buttonVm.IsDesignated = true;
        buttonVm.IsAuthorisedToOperate = true;
        buttonVm.IsVisible = true;
        //buttonVm.DatasetHasBeenImported = false;
        buttonVm.DatasetHasBeenUploaded = false;
        buttonVm.Content = buttonProfile.GuiButtonContent;
        buttonVm.DatasetIdentifyingEnum = buttonProfile.IdentifierOfAssociatedDataset;

        buttonVm.DatasetExampleBlobName = buttonProfile.FileNameOfExampleOfAssociatedDataset;
        buttonVm.DatasetShortDescription = buttonProfile.ShortDescriptionOfAssociatedDataset;
        buttonVm.DatasetFileNameExtensionFilters = buttonProfile.FileNameExtensionFiltersForBrowsingHardDrive; // don't worry. for buttons that Pull data the values in the buttonProfile will be blank

        return true;
    }

        private bool DatasetImportButtonPurgePreviouslyObtainedDataset(PublisherButtonProfileItem buttonProfile, PublishingModuleButtonControlViewModel buttonVm)
    {
        if (buttonProfile is null || buttonVm is null)
            return true;

        //buttonVm.IsDesignated = false;
        buttonVm.DatasetFileNameForUpload = string.Empty; // we only know this when the file is imported
        buttonVm.DatasetFileUploadOutcomeReport = string.Empty;
        buttonVm.DatasetAsRawString = string.Empty;
        buttonVm.DatasetHasBeenUploaded = false;

        return true;
    }

        private bool DatasetImportButtonIsAuthorisedToOperateToTrue(PublisherButtonProfileItem buttonProfile, PublishingModuleButtonControlViewModel buttonVm)
    {
        if (buttonProfile is null || buttonVm is null)
            return true;

        buttonVm.IsAuthorisedToOperate = true;

        return true;
    }

        private bool DatasetImportButtonIsAuthorisedToOperateToFalse(PublisherButtonProfileItem buttonProfile, PublishingModuleButtonControlViewModel buttonVm)
    {
        if (buttonProfile is null || buttonVm is null)
            return true;

        buttonVm.IsAuthorisedToOperate = false;

        return true;
    }

        private bool DatasetImportButtonIsDesignatedToIsVisible(PublisherButtonProfileItem buttonProfile, PublishingModuleButtonControlViewModel buttonVm)
    {
        if (buttonProfile is null || buttonVm is null)
            return true;

        if (buttonVm.IsDesignated)
            buttonVm.IsVisible = true;

        return true;
    }

        #endregion

        #region helpers

        public string PopulatePublishingProfileRelatedTextFields()
    {
        if (PublishingModuleProfile is null)
        {
            CSharpPublisherModuleVeryShortDescriptionTextVm.Text = string.Empty;
            CSharpPublisherModuleShortDescriptionTextVm.Text = string.Empty;
            CSharpPublisherModuleGeneralOverviewTextVm.Text = string.Empty;
            CSharpPublisherModuleVersionNumberTextVm.Text = string.Empty;
            CSharpPublisherModuleCodeNameTextVm.Text = string.Empty;

            return string.Empty;
        }

        var p = PublishingModuleProfile;

        CSharpPublisherModuleVeryShortDescriptionTextVm.Text = p.VeryShortDescriptionOfModule?.Replace(@"\n", "");
        CSharpPublisherModuleShortDescriptionTextVm.Text = p.ShortDescriptionOfModule?.Replace(@"\n", "");
        CSharpPublisherModuleGeneralOverviewTextVm.Text = p.GeneralOverviewOfModule?.Replace(@"\n", "");
        CSharpPublisherModuleVersionNumberTextVm.Text = p.CSharpModuleVersionNumber;
        CSharpPublisherModuleCodeNameTextVm.Text = p.CSharpModuleCodeName;

        var messageOk = FilenameSuccessfullyConfirmed + p.FragmentInNameOfFile;

        return messageOk;
    }

        public async Task ZeroisePublishingProfileAsync()
    {
        PublishingModuleProfile = null;
        _publisherModuleProfileItemUponLaunchOfWorkSession = null;

        CSharpPublisherModuleVeryShortDescriptionTextVm.Text = string.Empty;
        CSharpPublisherModuleShortDescriptionTextVm.Text = string.Empty;
        CSharpPublisherModuleGeneralOverviewTextVm.Text = string.Empty;
        CSharpPublisherModuleVersionNumberTextVm.Text = string.Empty;
        CSharpPublisherModuleCodeNameTextVm.Text = string.Empty;

        await TextBoxForEnteringPublishingProfileFileNameFragmentVm.ChangeTextAsync(string.Empty);
        TextBoxForEnteringPublishingProfileFileNameFragmentVm.Label = XmlFilename_not_yet_entered;
    }

        public async Task ZeroisePublishingSequence()
    {
        ConfigurePublisherDatasetImportButtons(DatasetImportButtonZeroise);

        _sbFriendlyLogOfActivity = new StringBuilder();
        _sbFriendlyLogOfBlobAndFileTransfers = new StringBuilder();

        ProcessingReportTextVm.Text = string.Empty;
        SavedFileNameOfProcessingReportTextVm.Text = string.Empty;

        SuccessfullyComputedLeaderboardAsXml = string.Empty;
        SavedFileNameOfSuccessfullyProcessedLeaderboardTextVm.Text = string.Empty;
        RanToCompletionMessageForPreviewLeaderboardTextVm.Text = string.Empty;
        RanToCompletionMessageForPublishedLeaderboardTextVm.Text = string.Empty;
        OutcomeOfProcessingOperationTextVm.Text = string.Empty;


        await PopulateCboLookupItemOfWorkingsForDisplayVmAsync();
    }

        public async Task CleanAndRefreshPublishingSequenceAsync()
    {
        ConfigurePublisherDatasetImportButtons(DatasetImportButtonPurgePreviouslyObtainedDataset);

        _sbFriendlyLogOfActivity = new StringBuilder();
        _sbFriendlyLogOfBlobAndFileTransfers = new StringBuilder();

        ProcessingReportTextVm.Text = string.Empty;
        SavedFileNameOfProcessingReportTextVm.Text = string.Empty;

        SuccessfullyComputedLeaderboardAsXml = string.Empty;
        SavedFileNameOfSuccessfullyProcessedLeaderboardTextVm.Text = string.Empty;
        RanToCompletionMessageForPreviewLeaderboardTextVm.Text = string.Empty;
        RanToCompletionMessageForPublishedLeaderboardTextVm.Text = string.Empty;
        OutcomeOfProcessingOperationTextVm.Text = string.Empty;

        await PopulateCboLookupItemOfWorkingsForDisplayVmAsync();
    }

        private async Task<bool> FetchExamplesOfDatasetsAssociatedWithButtonsAsync()
    {
        var inputButtonMs = MakeListOfDatasetInputButtonVms();

        foreach (var buttonVm in inputButtonMs.Where(buttonVm => buttonVm is {IsDesignated: true} && !string.IsNullOrWhiteSpace(buttonVm.DatasetExampleBlobName)))
            buttonVm.DatasetExampleSnippet = await _raceResultsPublishingSvcAgent.GetIllustrativeExampleOfSourceDatasetExpectedByPublishingServiceAsync(buttonVm.DatasetExampleBlobName, CancellationToken.None);

        return true;
    }

        private async Task<string> UploadProcessedArrayOfResultItemAsync(string successfullyPreprocessedData, EnumForResultsDatabaseDestinations kindOfDestinationEnum)
    {
        var startDateTime = DateTime.UtcNow;

        #region local helpers

        EntityLocationItem FigureOutUploadTargetAccordingToKindOfDestination(EnumForResultsDatabaseDestinations destinationEnum)
        {
            const string failure = "Unable to determine upload target.";
            const string locus = "[FigureOutUploadTargetAccordingToKindOfDestination]";

            try
            {
                var targetStorageItem = new EntityLocationItem();

                var currentSeries = SeriesItemDisplayObject.ObtainSourceModel(_seriesItemUponLaunchOfWorkSession);

                var userSelectedBlobName = BlobTargetOfPublishedResultsUponLaunchOfWorkSession?.DataItemName ??
                                           throw new JghInvalidValueException(StringsPortal.Unable_to_proceed__Unable_to_deduce_a_blob_name_for_this_upload);

                switch (destinationEnum)
                {
                    case EnumForResultsDatabaseDestinations.Draft:
                    {
                        var drafted = currentSeries?.ContainerForResultsDataFilesPreviewed ?? throw new JghInvalidValueException(StringsPortal.Unable_to_proceed__Unable_to_deduce_a_destination_for_upload_to_staging);

                        targetStorageItem.AccountName = drafted.AccountName;
                        targetStorageItem.ContainerName = drafted.ContainerName;
                        targetStorageItem.EntityName = userSelectedBlobName;
                        break;
                    }
                    case EnumForResultsDatabaseDestinations.Publish:
                    {
                        var published = currentSeries?.ContainerForResultsDataFilesPublished ?? throw new JghInvalidValueException(StringsPortal.Unable_to_proceed__Unable_to_deduce_a_destination_for_upload_to_production);

                        targetStorageItem.AccountName = published.AccountName;
                        targetStorageItem.ContainerName = published.ContainerName;
                        targetStorageItem.EntityName = userSelectedBlobName;
                        break;
                    }
                    default:
                        throw new JghInvalidValueException(StringsPortal.Unable_to_proceed__Unable_to_deduce_a_destination_for_upload);
                }

                return targetStorageItem;
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        async Task UploadWorkAsync(EntityLocationItem target, string stringToBeUploaded)
        {
            const string failure = "Unable to upload work to Azure.";
            const string locus = "[UploadWorkAsync]";

            try
            {
                target.EntityName = JghFilePathValidator.EliminateSpaces('-', target.EntityName);
                // just in case contains spaces or is longer than 255 characters

                var uploadDidSucceed = await _raceResultsPublishingSvcAgent.UploadPublishableResultsForSingleEventAsync(target, stringToBeUploaded);

                if (!uploadDidSucceed)
                {
                    var uploadDatasetFailureReport = $"{JghString.LeftAlign("Dataset upload:", lhsWidthLess4)} Failure. <{target.EntityName}> failed to upload to [{target.ContainerName}].";

                    AppendToConversionReportLog(uploadDatasetFailureReport);

                    throw new JghAlertMessageException(uploadDatasetFailureReport);
                }

                var descriptionOfSize = JghConvert.SizeOfBytesInHighestUnitOfMeasure(JghConvert.ToBytesUtf8FromString(stringToBeUploaded).Length);

                UpdateLogOfFilesThatWereTransferred(target.EntityName, "was uploaded to", target.ContainerName, descriptionOfSize);
            }

            #region trycatch

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        StringBuilder MakeUploadProgressReport(string containerName, string blobName, string payload)
        {
            var descriptionOfSizeAsBytes = JghConvert.SizeOfBytesInHighestUnitOfMeasure(JghConvert.SizeOfStringInBytes(payload));

            var sb = new StringBuilder();

            sb.AppendLine($"{JghString.LeftAlign("File uploaded:", lhsWidthLess1)} <{blobName}>");
            sb.AppendLine($"{JghString.LeftAlign("Container:", lhsWidthPlus1)} <{containerName}>");
            sb.AppendLine($"{JghString.LeftAlign("File size:", lhsWidthPlus5)} {descriptionOfSizeAsBytes}");
            sb.AppendLine($"{JghString.LeftAlign("When:", lhsWidthPlus3)} {DateTime.Now:HH:mm}");

            return sb;
        }

        #endregion

        #region preflight checks

        if (SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem is null)
            throw new JghAlertMessageException(StringsPortal.Unable_to_proceed__You_need_to_authenticate_yourself_before_proceeding_);

        if (!SeasonProfileAndIdentityValidationVm.GetIfCurrentlyAuthenticatedIdentityUserIsAuthorisedForRequiredWorkRole())
            throw new JghAlertMessageException($"{StringsPortal.Sorry_not_authorised_for_workrole}");

        #endregion

        var target = FigureOutUploadTargetAccordingToKindOfDestination(kindOfDestinationEnum);

        await UploadWorkAsync(target, successfullyPreprocessedData); // throws if failure

        #region report progress

        var prettyDuration = (DateTime.UtcNow - startDateTime).TotalSeconds;
        var ranToCompletionMsgSb = new StringBuilder();

        ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Outcome:", lhsWidth)} Operation ran to completion");
        ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Operation duration:", lhsWidthLess4)} {prettyDuration} seconds");

        switch (kindOfDestinationEnum)
        {
            case EnumForResultsDatabaseDestinations.Draft:
                ranToCompletionMsgSb.Append(MakeUploadProgressReport(target.ContainerName, target.EntityName, successfullyPreprocessedData));
                ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. Results uploaded for previewing.");
                break;
            case EnumForResultsDatabaseDestinations.Publish:
                ranToCompletionMsgSb.Append(MakeUploadProgressReport(target.ContainerName, target.EntityName, successfullyPreprocessedData));
                ranToCompletionMsgSb.AppendLine($"{JghString.LeftAlign("Conclusion:", lhsWidthPlus1)} Success. Results published.");
                break;
        }

        #endregion

        //AppendToConversionReportLog(ranToCompletionMsgSb.ToString());

        return ranToCompletionMsgSb.ToString();
    }

        public void AppendToConversionReportLog(string message)
    {
        _sbFriendlyLogOfActivity.AppendLine($"{DateTime.Now:HH:mm:ss}    {message}");

        var xx = CboLookupItemOfWorkingsForDisplayVm.GetItemByItemEnumString(EnumStringForProcessingReport);

        if (xx is not null) xx.Blurb = _sbFriendlyLogOfActivity.ToString();
    }

        public void UpdateLogOfFilesThatWereTransferred(string blobNameOrFileName, string descriptionOfTransferAction, string descriptionOfDestination, string descriptionOfSizeAsBytes)
    {
        _sbFriendlyLogOfBlobAndFileTransfers.AppendLine($"<{blobNameOrFileName}> {descriptionOfTransferAction} <{descriptionOfDestination}>  ({descriptionOfSizeAsBytes})");

        var xx = CboLookupItemOfWorkingsForDisplayVm.GetItemByItemEnumString(EnumStringForLogOfFileTransfers);

        if (xx is not null) xx.Blurb = _sbFriendlyLogOfBlobAndFileTransfers.ToString();
    }

        #endregion

        #region page access gatekeeping

        protected void ThrowIfWorkSessionNotReadyForLaunch()
    {
        if (!SeasonProfileAndIdentityValidationVm.ThisViewModelIsInitialised)
            throw new JghAlertMessageException(StringsPortal.SeasonDataNotInitialised);

        if (SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is null)
            throw new JghAlertMessageException(StringsPortal.SeasonDataNotInitialised);

        if (SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem is null)
            throw new JghAlertMessageException(StringsPortal.SelectedSeriesIsNull);

        if (PublishingModuleProfile is null)
            throw new JghAlertMessageException(StringsPortal.PublishingProfileNotFound);
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

        if (PublishingModuleProfile is null)
            throw new JghAlertMessageException(StringsPortal.PublishingProfileNotFound);

        if (!WorkSessionIsLaunched)
            throw new JghAlertMessageException(StringsPortal.WorkSessionNotLaunched);
    }

        #endregion

        #region Gui stuff

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
    {
        foreach (var controlVm in MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate())
            if (controlVm is IHasIsAuthorisedToOperate vm)
                vm.IsAuthorisedToOperate = false;
        // note: totally different logic than in other page view models. here, the default is false not true. easier this way

        CheckConnectionToRezultzHubButtonVm.IsAuthorisedToOperate = true;
        CheckAvailabilityOfPublishingServiceButtonVm.IsAuthorisedToOperate = true;

        TextBoxForEnteringPublishingProfileFileNameFragmentVm.IsAuthorisedToOperate = true;
        SubmitPublishingProfileFileNameForValidationButtonVm.IsAuthorisedToOperate = false;
        ClearPublishingProfileFileNameButtonVm.IsAuthorisedToOperate = true;


        CboLookupItemOfWorkingsForDisplayVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (NextThingToDoEnum)
        {
            case NextThingToDo.MakeControlsForLaunchingWorkSessionActive:
                ConfigurePublisherDatasetImportButtons(DatasetImportButtonIsAuthorisedToOperateToFalse);
                LaunchWorkSessionButtonVm.IsAuthorisedToOperate = true;
                break;
            case NextThingToDo.MakeControlsForCleanAndRefreshOfPublishingSequenceActive:
                ConfigurePublisherDatasetImportButtons(DatasetImportButtonIsAuthorisedToOperateToFalse);
                LaunchWorkSessionButtonVm.IsAuthorisedToOperate = true;
                CleanAndRefreshPublishingSequenceButtonVm.IsAuthorisedToOperate = true;
                break;
            case NextThingToDo.MakeControlsForBrowsingForLocalFilesAndDownloadingDataFromRemoteHubActive:
                ConfigurePublisherDatasetImportButtons(DatasetImportButtonIsAuthorisedToOperateToTrue);
                LaunchWorkSessionButtonVm.IsAuthorisedToOperate = true;
                CleanAndRefreshPublishingSequenceButtonVm.IsAuthorisedToOperate = true;
                break;
            case NextThingToDo.MakeControlsForPreprocessingActive:
                ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonVm.IsAuthorisedToOperate = true;
                LaunchWorkSessionButtonVm.IsAuthorisedToOperate = true;
                CleanAndRefreshPublishingSequenceButtonVm.IsAuthorisedToOperate = true;
                break;
            case NextThingToDo.MakeControlsForPublishingActive:
                ConfigurePublisherDatasetImportButtons(DatasetImportButtonIsAuthorisedToOperateToFalse);
                LaunchWorkSessionButtonVm.IsAuthorisedToOperate = true;
                ExportProcessingReportToHardDriveButtonVm.IsAuthorisedToOperate = true;
                ExportLeaderboardToHardDriveButtonVm.IsAuthorisedToOperate = true;
                UploadLeaderboardToPreviewStorageButtonVm.IsAuthorisedToOperate = true;
                UploadLeaderboardToPublishedStorageButtonVm.IsAuthorisedToOperate = true;
                CleanAndRefreshPublishingSequenceButtonVm.IsAuthorisedToOperate = true;
                break;
            case NextThingToDo.MakeControlsForPullingConvertingAndUploadingDeactivated:
                LaunchWorkSessionButtonVm.IsAuthorisedToOperate = true;
                PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm.IsAuthorisedToOperate = false;
                PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm.IsAuthorisedToOperate = false;
                PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm.IsAuthorisedToOperate = false;

                BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01.IsAuthorisedToOperate = false;
                BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02.IsAuthorisedToOperate = false;
                BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03.IsAuthorisedToOperate = false;
                BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04.IsAuthorisedToOperate = false;
                BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05.IsAuthorisedToOperate = false;
                BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06.IsAuthorisedToOperate = false;
                BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07.IsAuthorisedToOperate = false;

                ExportProcessingReportToHardDriveButtonVm.IsAuthorisedToOperate = true;
                ExportLeaderboardToHardDriveButtonVm.IsAuthorisedToOperate = true;
                UploadLeaderboardToPreviewStorageButtonVm.IsAuthorisedToOperate = false;
                UploadLeaderboardToPublishedStorageButtonVm.IsAuthorisedToOperate = false;
                CleanAndRefreshPublishingSequenceButtonVm.IsAuthorisedToOperate = true;
                break;
        }
    }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
    {
        // Note: makeVisible is irrelevant in this method because at the time of writing we don't
        // toggle any visibilities when we call DeadenGui() and EnlivenGui() in every Command

        HeadersVm.IsVisible = !HeadersVm.IsEmpty;
        FootersVm.IsVisible = !FootersVm.IsEmpty;

        SeasonProfileAndIdentityValidationVm.SeasonProfileValidationIsVisible = ThisViewModelIsInitialised;

        SeasonProfileAndIdentityValidationVm.IdentityValidationIsVisible = ThisViewModelIsInitialised &&
                                                                           SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is not null;

        PublishingModuleValidationUserControlIsVisible = ThisViewModelIsInitialised &&
                                                         SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is not null &&
                                                         SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem is not null;

        var readyToGo = ThisViewModelIsInitialised &&
                        SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem is not null &&
                        SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem is not null &&
                        PublishingModuleProfile is not null;

        LaunchWorkSessionButtonVm.IsVisible = readyToGo;

        CleanAndRefreshPublishingSequenceButtonVm.IsVisible = readyToGo && WorkSessionIsLaunched;

        ConfigurePublisherDatasetImportButtons(DatasetImportButtonIsDesignatedToIsVisible);

        CboLookupItemOfWorkingsForDisplayVm.IsVisible = readyToGo && WorkSessionIsLaunched;
    }

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
    {
        var answer = new List<object>();

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CheckConnectionToRezultzHubButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CheckAvailabilityOfPublishingServiceButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, LaunchWorkSessionButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CleanAndRefreshPublishingSequenceButtonVm);


        AddToCollectionIfIHasIsAuthorisedToOperate(answer, PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07);

            AddToCollectionIfIHasIsAuthorisedToOperate(answer, ProcessPreviouslyUploadedSourceDataIntoLeaderboardForSingleEventButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, ExportProcessingReportToHardDriveButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, ExportLeaderboardToHardDriveButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, UploadLeaderboardToPreviewStorageButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, UploadLeaderboardToPublishedStorageButtonVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupItemOfWorkingsForDisplayVm);

        return answer;
    }

        public List<PublishingModuleButtonControlViewModel> MakeListOfDatasetInputButtonVms()
    {
        var answer = new List<PublishingModuleButtonControlViewModel>
        {
            PullTimestampsFromRezultzHubAndUploadAsSourceDataButtonVm,
            PullParticipantsFromRezultzHubAndUploadAsSourceDataButtonVm,
            PullTimestampsAndParticipantsFromRezultzHubAndUploadComputedLeaderboardAsSourceDataButtonVm,
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm01,
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm02,
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm03,
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm04,
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm05,
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm06,
            BrowseHardDriveForFileAndUploadAsSourceDataButtonVm07
        };


        return answer;
    }

        #endregion

        #region GenesisAsLastKnownGood

        private SeasonProfileItem _seasonProfileItemUponLaunchOfWorkSession;
        private IdentityItem _identityItemUponLaunchOfWorkSession;
        private PublisherModuleProfileItem _publisherModuleProfileItemUponLaunchOfWorkSession;

        private SeriesItemDisplayObject _seriesItemUponLaunchOfWorkSession;
        private EventItemDisplayObject _eventItemUponLaunchOfWorkSession;
        public EntityLocationItemDisplayObject BlobTargetOfPublishedResultsUponLaunchOfWorkSession;

        protected void SaveGenesisOfThisViewModelAsLastKnownGood()
    {
        _seasonProfileItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem;
        _identityItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem;

        _seriesItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem;
        _eventItemUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem;
        BlobTargetOfPublishedResultsUponLaunchOfWorkSession = SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm?.CurrentItem;
        _publisherModuleProfileItemUponLaunchOfWorkSession = PublishingModuleProfile;
    }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        if (_seasonProfileItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CurrentlyValidatedSeasonProfileItem)
            return true;

        if (_identityItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CurrentlyAuthenticatedIdentityItem)
            return true;

        if (_publisherModuleProfileItemUponLaunchOfWorkSession != PublishingModuleProfile)
            return true;

        if (_seriesItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CboLookupSeriesVm?.CurrentItem)
            return true;

        if (_eventItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CboLookupEventVm?.CurrentItem)
            return true;

        if (BlobTargetOfPublishedResultsUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm.CboLookupBlobNameToPublishResultsVm?.CurrentItem)
            return true;


        return false;
    }

        public void ThrowIfLastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        if (LastKnownGoodGenesisOfThisViewModelHasChanged())
            throw new JghAlertMessageException(LastKnownGoodGenesisOfThisViewModelHasChangedReason());
    }

        public string LastKnownGoodGenesisOfThisViewModelHasChangedReason()
    {
        var prefix = "You have changed your work session particulars.  Please re-launch the work session if you wish to continue.";

        var seasonProfileItemHasChanged = _seasonProfileItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem;
        var identityItemHasChanged = _identityItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CurrentlyAuthenticatedIdentityItem;
        var publishingModuleHasChanged = _publisherModuleProfileItemUponLaunchOfWorkSession != PublishingModuleProfile;

        var seriesItemHasChanged = _seriesItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CboLookupSeriesVm?.CurrentItem;
        var eventItemHasChanged = _eventItemUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CboLookupEventVm?.CurrentItem;
        var blobTargetOfPublishedResultsHasChanged = BlobTargetOfPublishedResultsUponLaunchOfWorkSession != SeasonProfileAndIdentityValidationVm?.CboLookupBlobNameToPublishResultsVm?.CurrentItem;

        var changeOfSeasonProfileItemMsg = string.Empty;
        var changeOfSeasonProfileItemMsg2 = string.Empty;

        var changeOfIdentityItemMsg = string.Empty;
        var changeOfIdentityItemMsg2 = string.Empty;

        var changeOfPublisherIdMsg = string.Empty;
        var changeOfPublisherIdMsg2 = string.Empty;

        var changeOfSeriesMsg = string.Empty;
        var changeOfSeriesMsg2 = string.Empty;

        var changeOfEventMsg = string.Empty;
        var changeOfEventMsg2 = string.Empty;

        var changeOfBlobTargetMsg = string.Empty;
        var changeOfBlobTargetMsg2 = string.Empty;

        var changeOfTimeKeepingDataSourceMsg = string.Empty;
        var changeOfTimeKeepingDataSourceMsg2 = string.Empty;

        var changeOfParticipantDataSourceMsg = string.Empty;
        var changeOfParticipantDataSourceMsg2 = string.Empty;

        var width = 40;
        //int widthPlus2 = width + 2;
        var widthPlus3 = width + 3;
        var widthPlus4 = width + 4;
        var widthPlus5 = width + 5;
        var widthPlus6 = width + 6;
        var widthPlus7 = width + 7;
        var widthPlus8 = width + 8;
        //int widthPlus9 = width + 9;
        var widthPlus10 = width + 10;
        var widthPlus11 = width + 11;

        if (seasonProfileItemHasChanged)
        {
            changeOfSeasonProfileItemMsg = $"{JghString.LeftAlign("Launched season profile ID:", widthPlus6)} {_seasonProfileItemUponLaunchOfWorkSession?.FragmentInFileNameOfAssociatedProfileFile}";
            changeOfSeasonProfileItemMsg2 = $"{JghString.LeftAlign("Current season profile ID:", widthPlus7)} {SeasonProfileAndIdentityValidationVm?.CurrentlyValidatedSeasonProfileItem.FragmentInFileNameOfAssociatedProfileFile}";
        }

        if (identityItemHasChanged)
        {
            changeOfIdentityItemMsg = $"{JghString.LeftAlign("Launched identity:", widthPlus7)} {_identityItemUponLaunchOfWorkSession}";
            changeOfIdentityItemMsg2 = $"{JghString.LeftAlign("Current identity:", widthPlus4)} {SeasonProfileAndIdentityValidationVm?.CurrentlyAuthenticatedIdentityItem}";
        }

        if (publishingModuleHasChanged)
        {
            changeOfPublisherIdMsg = $"{JghString.LeftAlign("Previous publishing module:", widthPlus5)} {_publisherModuleProfileItemUponLaunchOfWorkSession?.FragmentInNameOfFile}";
            changeOfPublisherIdMsg2 = $"{JghString.LeftAlign("Current publishing module:", widthPlus5)} {TextBoxForEnteringPublishingProfileFileNameFragmentVm?.Label}";
        }

        if (seriesItemHasChanged)
        {
            changeOfSeriesMsg = $"{JghString.LeftAlign("Launched series:", widthPlus10)} {_seriesItemUponLaunchOfWorkSession?.Label}";
            changeOfSeriesMsg2 = $"{JghString.LeftAlign("Currently selected series:", widthPlus7)} {SeasonProfileAndIdentityValidationVm?.CboLookupSeriesVm?.CurrentItem?.Label}";
        }

        if (eventItemHasChanged)
        {
            changeOfEventMsg = $"{JghString.LeftAlign("Launched event:", widthPlus11)} {_eventItemUponLaunchOfWorkSession?.Label}";
            changeOfEventMsg2 = $"{JghString.LeftAlign("Currently selected event:", widthPlus8)} {SeasonProfileAndIdentityValidationVm?.CboLookupEventVm?.CurrentItem?.Label}";
        }


        if (blobTargetOfPublishedResultsHasChanged)
        {
            changeOfBlobTargetMsg = $"{JghString.LeftAlign("Launched publishing filename:", widthPlus3)} {BlobTargetOfPublishedResultsUponLaunchOfWorkSession?.Label}";
            changeOfBlobTargetMsg2 = $"{JghString.LeftAlign("Currently selected publishing filename:", width)} {SeasonProfileAndIdentityValidationVm?.CboLookupBlobNameToPublishResultsVm?.CurrentItem?.Label}";
        }


        var details = JghString.ConcatAsLines(changeOfSeasonProfileItemMsg, changeOfSeasonProfileItemMsg2, changeOfIdentityItemMsg, changeOfIdentityItemMsg2,
            changeOfEventMsg, changeOfEventMsg2, changeOfSeriesMsg, changeOfSeriesMsg2,
            changeOfBlobTargetMsg, changeOfBlobTargetMsg2, changeOfPublisherIdMsg, changeOfPublisherIdMsg2,
            changeOfTimeKeepingDataSourceMsg, changeOfTimeKeepingDataSourceMsg2, changeOfParticipantDataSourceMsg, changeOfParticipantDataSourceMsg2);

        var answer = JghString.ConcatAsParagraphs(prefix, "Details:", details);

        return answer;
    }

        public void ThrowIfLastKnownGoodGenesisOfThisViewModeIsNull()
    {
        if (LastKnownGoodGenesisOfThisViewModelIsNull())
            throw new JghAlertMessageException(LastKnownGoodGenesisOfThisViewModelIsNullReason());
    }

        public bool LastKnownGoodGenesisOfThisViewModelIsNull()
    {
        if (_seasonProfileItemUponLaunchOfWorkSession is null)
            return true;

        if (_identityItemUponLaunchOfWorkSession is null)
            return true;

        if (_seriesItemUponLaunchOfWorkSession is null)
            return true;

        if (_eventItemUponLaunchOfWorkSession is null)
            return true;

        if (BlobTargetOfPublishedResultsUponLaunchOfWorkSession is null)
            return true;

        if (PublishingModuleProfile is null)
            return true;

        //if (string.IsNullOrWhiteSpace(TextBoxForEnteringPublishingProfileFileNameFragmentVm.Label))
        //    return true;

        return false;
    }

        public string LastKnownGoodGenesisOfThisViewModelIsNullReason()
    {
        return StringsPortal.Must_complete_launch_of_work_session;
    }

        #endregion
    }
}
