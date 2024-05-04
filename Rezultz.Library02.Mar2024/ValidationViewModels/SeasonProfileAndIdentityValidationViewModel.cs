using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using Jgh.Xamarin.Common.Jan2019;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Interfaces03.Apr2022;
using NetStd.OnBoardServices02.July2018.UserSettingsForRezultz;
using NetStd.Prism.July2018;
using NetStd.ServiceLocation.Aug2022;
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTransferObjects.Nov2023.SeasonAndSeriesProfiles;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;
using Rezultz.DataTypes.Nov2023.SeasonProfileViewModels;
using Rezultz.Library02.Mar2024.PageViewModelBases;
using Rezultz.Library02.Mar2024.Strings;

// ReSharper disable InconsistentNaming

// ReSharper disable UnusedMethodReturnValue.Local

/*
///     The SubmitSeasonProfileFilenameFragmentButtonVm uses _leaderboardResultsSvcAgent to make a single call via a remote svc.
///     It attempts to verify the existence of the Season profile blob named after the Season ID.
///     Naming convention is the FragmentInFileNameOfAssociatedProfileFile with a prefix of "seasonprofile-".
///     e.g. https://systemrezultzlevel1.blob.core.windows.net/seasonprofiles/seasonprofile-999.json
///     _leaderboardResultsSvcAgent.GetIfFileNameOfSeasonProfileIsRecognisedAsync throws JghCommunicationFailureException
///     for client-side problems such as if no network or internet, with an explanatory message.
///     Returns true if if svc sees the blob where it should be in Azure storage.
///     Returns false if svc has looked high and low in storage and can't find it.
*/

namespace Rezultz.Library02.Mar2024.ValidationViewModels;

public class SeasonProfileAndIdentityValidationViewModel : BaseViewViewModel, ISeasonProfileAndIdentityValidationViewModel
{
    private const string Locus2 = nameof(SeasonProfileAndIdentityValidationViewModel);
    private const string Locus3 = "[Rezultz.Library02.Mar2024]";

    #region constant

    private const string NameOfFolderContainingSeasonData = "FolderContainingSeasonMetadata";

    #endregion

    #region ctor

    public SeasonProfileAndIdentityValidationViewModel(ILeaderboardResultsSvcAgent leaderboardResultsSvcAgent, IThingsPersistedInLocalStorage thingsPersistedInLocalStorage, ILocalStorageService localStorageServiceInstance)
    {
        const string failure = "Unable to construct object SeasonProfileAndIdentityValidationViewModel.";
        const string locus = "[ctor]";

        try
        {
            #region assign ctor IOC injections

            _storageService = localStorageServiceInstance ?? throw new JghNullObjectInstanceException(nameof(localStorageServiceInstance));

            _thingsPersistedInLocalStorage = thingsPersistedInLocalStorage ?? throw new JghNullObjectInstanceException(nameof(thingsPersistedInLocalStorage));

            _leaderboardResultsSvcAgent = leaderboardResultsSvcAgent ?? throw new JghNullObjectInstanceException(nameof(leaderboardResultsSvcAgent));

            #endregion

            #region instantiate Textboxes

            TextBoxForEnteringSeasonProfileFileNameFragmentVm = new TextBoxControlViewModel(TextBoxForEnteringSeasonProfileFilenameFragmentOnTextChangedExecute, DummyOnTextChangedCanExecuteAlwaysTrue)
                {Label = Not_yet_chosen};

            TextBoxForEnteringSeasonProfileSecurityAccessCodeVm = new TextBoxControlViewModel(TextBoxForEnteringSeasonProfileSecurityAccessCodeOnTextChangedExecute, DummyOnTextChangedCanExecuteAlwaysTrue)
                {Label = Access_code};

            TextBoxForEnteringIdentityUserNameVm = new TextBoxControlViewModel(TextBoxesForEnteringIdentityOnTextChangedExecute, DummyOnTextChangedCanExecuteAlwaysTrue) { Label = Not_yet_chosen };
            TextBoxForEnteringIdentityPasswordVm = new TextBoxControlViewModel(TextBoxesForEnteringIdentityOnTextChangedExecute, DummyOnTextChangedCanExecuteAlwaysTrue);

            #endregion

            #region instantiate ButtonVms

            SubmitSeasonProfileFileNameFragmentButtonVm = new ButtonControlViewModel(
                SubmitSeasonProfileFilenameFragmentButtonOnClickExecuteAsync, SubmitSeasonProfileFilenameFragmentButtonOnClickCanExecute);

            ClearSeasonProfileFilenameFragmentButtonVm = new ButtonControlViewModel(
                ClearSeasonProfileFilenameFragmentButtonOnClickExecuteAsync, ClearSeasonProfileFilenameFragmentButtonOnClickCanExecute);

            SubmitSeasonProfileFilenameFragmentOfflineButtonVm = new ButtonControlViewModel(
                SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickExecuteAsync, SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickCanExecute);

            SubmitIdentityButtonVm = new ButtonControlViewModel(SubmitIdentityButtonOnClickExecuteAsync, SubmitIdentityButtonOnClickCanExecute);

            ClearIdentityButtonVm = new ButtonControlViewModel(ClearIdentityButtonOnClickExecuteAsync, ClearIdentityButtonOnClickCanExecute);

            #endregion

            #region instantiate cbos

            CboLookupSeasonVm = new IndexDrivenCollectionViewModel<SeasonItemDisplayObject>(
                    Other_Season,
                    CboLookupSeasonOnSelectionChangedExecuteAsync,
                    CboLookupSeasonOnSelectionChangedCanExecute)
                {Label = Not_yet_launched};


            CboLookupSeriesVm = new IndexDrivenCollectionViewModel<SeriesItemDisplayObject>(
                    Other_series,
                    CboLookupSeriesOnSelectionChangedExecuteAsync,
                    CboLookupSeriesOnSelectionChangedCanExecute)
                {Label = Not_yet_launched};

            CboLookupEventVm = new IndexDrivenCollectionViewModel<EventItemDisplayObject>(
                    Other_events,
                    CboLookupEventOnSelectionChangedExecuteAsync,
                    CboLookupEventOnSelectionChangedCanExecute)
                {Label = Not_yet_launched};

            CboLookupBlobNameToPublishResultsVm = new IndexDrivenCollectionViewModel<EntityLocationItemDisplayObject>(
                    Strings2017.Select_blob,
                    CboLookupBlobNameToPublishResultsOnSelectionChangedExecuteAsync,
                    CboLookupBlobNameToPublishResultsOnSelectionChangedCanExecute)
                {Label = Not_yet_launched};

            CboLookupSeasonVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
            CboLookupSeriesVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
            CboLookupEventVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);
            CboLookupBlobNameToPublishResultsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(_dangerouslyBriefSafetyMarginForBindingEngineMilliSec);

            #endregion
        }
        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }
    }

    #endregion

    #region method/s called on first time load and initialisation

    public async Task<string> BeInitialisedForRezultzOrchestrateAsync()
    {
        const string failure = StringsForXamlPages.UnableToInitialiseViewmodel;
        const string locus = nameof(BeInitialisedForRezultzOrchestrateAsync);

        try
        {
            if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                return string.Empty;

            DeadenGui();

            await PopulateSeriesEventAndBlobNameCboAsync(); // 1st kick. draft initialisation

            var messageOk = await LoadListOfShallowSeasonItemsAsync();

            EnlivenGui();

            //SeasonProfileFileNameFragmentIsValidated = false;

            ThisViewModelIsInitialised = true;

            return messageOk;
        }

        #region try catch handling

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
            {
                EvaluateGui();

                ThisViewModelIsInitialised = true;
            }
            else
            {
                EvaluateGui(); // somewhat unusual, but special case

                ThisViewModelIsInitialised = false;
            }

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    public async Task<string> BeInitialisedForRezultzPortalOrchestrateAsync()
    {
        const string failure = StringsForXamlPages.UnableToInitialiseViewmodel;
        const string locus = nameof(BeInitialisedForRezultzOrchestrateAsync);

        try
        {
            if (ThisViewModelIsInitialised && LastKnownGoodGenesisOfThisViewModelHasNotChanged())
                return string.Empty;

            DeadenGui();

            await PopulateSeriesEventAndBlobNameCboAsync(); // 1st stab. basic initialisation

            EnlivenGui();

            ThisViewModelIsInitialised = true;

            return string.Empty;
        }

        #region try catch handling

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
            {
                EvaluateGui();

                ThisViewModelIsInitialised = true;
            }
            else
            {
                EvaluateGui(); // somewhat unusual, but special case

                ThisViewModelIsInitialised = false;
            }

            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region strings

    private const string Unable_to_retrieve_instance = "Code error. Unable to obtain instance from dependency injection container of";

    private const string Please_enter_access_code = "Please enter secure access code for season profile.";
    private const string Access_code_not_recognised = "Mismatch. Season profile does not include this access code.";

    private const string Please_submit_and_validate_SeasonID = "Season access code not yet submitted.";

    private const string Going_offline_blurb = "If you need to access this machine offline in the near future, try using the same ID and security access code you just succeeded in using. It should work.";

    private const string Working____ = "Working ....";

    private const string Working_____validating_and_saving_seasonId = "Working .... processing ID (filename)";

    private const string SeasonProfileFile_is_blank =
        "Season ID confirmed but the associated profile file is empty. The file exists, but it has no content as things stand. This might or might not be intentional on the part of the author of the file.";

    private const string Not_yet_chosen = "not yet chosen";
    private const string Not_yet_launched = "not yet launched";
    private const string Not_yet_authenticated = "not yet authenticated";

    private const string SeasonMetadataId_not_recognised = "Season ID not recognised.";
    private const string SeasonMetadata_not_found_on_this_machine_message = "Season profile file not found stored on this machine matching this Season ID.";
    private const string Access_code = "access code for season profile";

    private const string SeasonData_contains_no_access_codes =
        "Access is not possible.  The secure access code that is meant to be present in the season profile file is blank as things stand.  This might or might not be intentional on the part of the author of the file.";

    private const string Access_code_is_public =
        "Note: this access code enables public, read-only access to the associated series and events.  It does not allow deeper access.  The data cannot be operated on by organiser-admins, timekeepers, or anyone else.  A different access code is needed for this.";

    private const string SeasonProfile_loaded = "Season profile loaded and saved.";
    private const string SeasonID_cleared = "Season ID cleared.";
    private const string SeasonProfileIdFormatErrorWarningMessage = "A validly formatted ID is a number between 100 and 999.";

    private const string No_series_items_found =
        "Warning: the series profile for the one or more series in the selected season is not found. Either no series are listed for the season, or no information is found in the season profile file about any of the series listed.";

    private const string Credentials_authenticated = "Credentials successfully located in season profile file. Identity confirmed.";

    private const string Other_Season = "Other season.";
    private const string Other_series = "Other series.";
    private const string Other_events = "Other events.";

    private const string Working_____authenticating = "Working .... authenticating";

    private const string User_cleared_message = "Identity cleared";
    private const string UserName_not_recognised_message = "Username not found in season profile.";
    private const string Password_not_recognised_message = "Password not recognised in season profile.";
    private const string Credentials_not_recognised_message = "Identity not found in season profile.";
    private const string Not_authorised_for_this_work_role = "Not authorised for required work role.";

    #endregion

    #region fields

    private SeasonProfileItem _lastKnownGoodSeasonProfileItemToWhichThisVmBelongs;

    private IdentityItem _lastKnownGoodIdentityItemToWhichThisVmBelongs;

    private readonly ILocalStorageService _storageService;

    private bool _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData;

    //private bool _mustOfferUserAlternativeOfSearchingLocalStorageForUserCredentials;

    private readonly int _dangerouslyBriefSafetyMarginForBindingEngineMilliSec = 50;

    #endregion

    #region global props

    private readonly IThingsPersistedInLocalStorage _thingsPersistedInLocalStorage;

    private readonly ILeaderboardResultsSvcAgent _leaderboardResultsSvcAgent;

    private static IAlertMessageService AlertMessageService
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
                    JghString.ConcatAsSentences(Unable_to_retrieve_instance,
                        $"'{nameof(IAlertMessageService)}");

                var locus = $"Property getter of '{nameof(AlertMessageService)}]";
                throw JghExceptionHelpers.ConvertToCarrier(msg, locus, Locus2, Locus3, ex);
            }
        }
    }

    #endregion

    #region INPC

    #region SeasonProfileValidationIsVisible - don't set this internally - owning vm does so

    private bool _backingstoreSeasonProfileValidationIsVisible = true;

    public bool SeasonProfileValidationIsVisible
    {
        get => _backingstoreSeasonProfileValidationIsVisible;
        set => SetProperty(ref _backingstoreSeasonProfileValidationIsVisible, value);
    }

    #endregion

    #region IdentityValidationIsVisible - don't set this internally - owning vm does so

    private bool _backingstoreIdentityValidationIsVisible = true;

    public bool IdentityValidationIsVisible
    {
        get => _backingstoreIdentityValidationIsVisible;
        set => SetProperty(ref _backingstoreIdentityValidationIsVisible, value);
    }

    #endregion

    #endregion

    #region special INPC props - see ctor of parent vm where there is wiring up to an event handler to observe and do something when these props fire their property changed events

    #region CurrentlyValidatedSeasonProfileItem

    private SeasonProfileItem _backingstoreCurrentlyValidatedSeasonProfileItem;

    public SeasonProfileItem CurrentlyValidatedSeasonProfileItem
    {
        get => _backingstoreCurrentlyValidatedSeasonProfileItem;
        set => SetProperty(ref _backingstoreCurrentlyValidatedSeasonProfileItem, value);
    }

    #endregion

    #region CurrentlyAuthenticatedIdentityItem

    private IdentityItem _backingstoreCurrentlyAuthenticatedIdentityItem;

    public IdentityItem CurrentlyAuthenticatedIdentityItem
    {
        get => _backingstoreCurrentlyAuthenticatedIdentityItem;
        set => SetProperty(ref _backingstoreCurrentlyAuthenticatedIdentityItem, value);
    }

    #endregion

    #endregion

    #region props

    public bool ThisViewModelIsInitialised { get; protected set; }
    public bool MustHideCboLookupEvent { get; set; }
    public bool OwnerOfThisServiceIsPortalNotRezultz { get; set; }
    public bool MustHideCboLookupBlobNameToPublishResults { get; set; }


    public string CurrentRequiredWorkRole { get; set; } // set in the page viewmodel that owns this viewmodel upon instantiation

    public HeaderOrFooterViewModel HeadersVm { get; } = new();
    public ProgressIndicatorViewModelXamarin SeasonProfileProgressIndicatorVm { get; } = new();

    public TextBoxControlViewModel TextBoxForEnteringSeasonProfileSecurityAccessCodeVm { get; }
    public TextBoxControlViewModel TextBoxForEnteringSeasonProfileFileNameFragmentVm { get; }
    public TextBoxControlViewModel TextBoxForEnteringIdentityUserNameVm { get; }
    public TextBoxControlViewModel TextBoxForEnteringIdentityPasswordVm { get; }


    public ButtonControlViewModel SubmitSeasonProfileFileNameFragmentButtonVm { get; }
    public ButtonControlViewModel ClearSeasonProfileFilenameFragmentButtonVm { get; }
    public ButtonControlViewModel SubmitSeasonProfileFilenameFragmentOfflineButtonVm { get; }
    public ButtonControlViewModel SubmitIdentityButtonVm { get; }
    public ButtonControlViewModel ClearIdentityButtonVm { get; }

    public IndexDrivenCollectionViewModel<SeasonItemDisplayObject> CboLookupSeasonVm { get; }
    public IndexDrivenCollectionViewModel<SeriesItemDisplayObject> CboLookupSeriesVm { get; }
    public IndexDrivenCollectionViewModel<EventItemDisplayObject> CboLookupEventVm { get; }
    public IndexDrivenCollectionViewModel<EntityLocationItemDisplayObject> CboLookupBlobNameToPublishResultsVm { get; }


    public DelegateCommand OnTextChangedCommand { get; set; } // serves no earthly purpose other than to silence the buggy XAML Binding Failures window during debugging
    public DelegateCommand OnSelectionChangedCommand { get; set; } // serves no earthly purpose other than to silence the buggy XAML Binding Failures window during debugging

    #endregion

    #region commands

    #region Season profile entry/validation

    #region TextBoxForEnteringSeasonProfileFilenameFragmentOnTextChangedExecute

    private void TextBoxForEnteringSeasonProfileFilenameFragmentOnTextChangedExecute()
    {
        SubmitSeasonProfileFileNameFragmentButtonVm.IsAuthorisedToOperate = true;
    }

    #endregion

    #region TextBoxForEnteringSeasonProfileSecurityAccessCodeOnTextChangedExecute

    private void TextBoxForEnteringSeasonProfileSecurityAccessCodeOnTextChangedExecute()
    {
        //dummy method. placeholder. do absolutely nothing. 
    }

    #endregion

    #region SubmitSeasonProfileFilenameFragmentButtonOnClickAsync

    /// Note. Reasonably foreseeable errors that can occur here are:-
    /// lead to a JghSeasonData404Exception being thrown. In such cases ThisViewModelIsInitialised = false.
    /// Other exceptions must be deemed show-stoppers.
    /// There is a single call inside this method to a remote service to fetch the Season data. 
    /// - If there is a client side error, such as no internet connection, innermost exception will be a
    /// JghSeason404Exception
    /// - If Season profile file is not obtained by the remote svc, or any constituent series-settings file is not
    /// obtained for any reason whatsoever, the LoadSeasonAsync() call throws a JghSeasonData404Exception.
    /// https://systemrezultzlevel1.blob.core.windows.net/seasons/season-id-999.json and
    /// https://customerkelso.blob.core.windows.net/series/series-Kelso2022-mtb.json
    private bool SubmitSeasonProfileFilenameFragmentButtonOnClickCanExecute()
    {
        return SubmitSeasonProfileFileNameFragmentButtonVm.IsAuthorisedToOperate;
    }

    private async void SubmitSeasonProfileFilenameFragmentButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[SubmitSeasonProfileFilenameFragmentButtonOnClickExecuteAsync]";

        try
        {
            if (!SubmitSeasonProfileFilenameFragmentButtonOnClickCanExecute())
                return;

            OpenProgressIndicator(Working_____validating_and_saving_seasonId);

            DeadenGui();

            var outcome = await SubmitSeasonProfileFilenameFragmentButtonOnClickAsync();

            EnlivenGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(outcome);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex)
                || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<Jgh404Exception>(ex)
                || JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghCommunicationFailureException>(ex))
                EvaluateGui();
            else
                EvaluateVisibilityOfAllGuiControlsThatTouchData(false);

            FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2,
                Locus3, ex);
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    private async Task<string> SubmitSeasonProfileFilenameFragmentButtonOnClickAsync()
    {
        const string failure = "Unable to save submit, verify, and save seasonID.";
        const string locus = "[SubmitSeasonProfileFilenameFragmentButtonOnClickAsync]";

        try
        {
            #region null checks

            if (!JghFilePathValidator.IsSuperficiallyValidXmlFileNameOfSeasonProfile(TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text))
                return JghString.ConcatAsSentences(SeasonMetadataId_not_recognised, SeasonProfileIdFormatErrorWarningMessage);

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData = true;

                return StringsRezultz02.NoConnection;
            }

            #endregion

            #region get deeply populated SeasonProfileItem

            try
            {
                await _leaderboardResultsSvcAgent.ThrowIfNoServiceConnectionAsync();
            }
            catch (Exception e)
            {
                _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData = true;

                return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
            }

            if (string.IsNullOrWhiteSpace(TextBoxForEnteringSeasonProfileSecurityAccessCodeVm.Text)) return Please_enter_access_code;

            var doesExist = await _leaderboardResultsSvcAgent.GetIfFileNameOfSeasonProfileIsRecognisedAsync(TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text, CancellationToken.None);

            if (!doesExist) return SeasonMetadataId_not_recognised;

            var candidateSeasonItem = await _leaderboardResultsSvcAgent.GetSeasonProfileAsync(TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text, CancellationToken.None); // success!?

            if (candidateSeasonItem == null)
                return SeasonProfileFile_is_blank;

            if (string.IsNullOrWhiteSpace(candidateSeasonItem.AccessCodes)) return SeasonData_contains_no_access_codes;

            var listOfAccessCodes = candidateSeasonItem.AccessCodes.Split(',');

            var accessCodeIsPublic = listOfAccessCodes.Contains(EnumStrings.AccessCodeForPublicAccessToSeasonData);

            if (accessCodeIsPublic) return Access_code_is_public;

            var accessCodeIsValid = listOfAccessCodes
                .Contains(TextBoxForEnteringSeasonProfileSecurityAccessCodeVm.Text);

            if (!accessCodeIsValid) return Access_code_not_recognised;

            CurrentlyValidatedSeasonProfileItem = candidateSeasonItem;

            #endregion

            #region success. save and confirm

            _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData = false;

            //ThisViewModelIsInitialised = true;

            var seasonIdAsInt = Convert.ToInt32(TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text);

            TextBoxForEnteringSeasonProfileFileNameFragmentVm.Label = $"{TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text} validated";

            await CboLookupSeasonVm.ZeroiseItemsSourceAsync(); // NB when driving the FragmentInFileNameOfAssociatedProfileFile via Submit button we zeroise, and we hide it (see EvaluateVisibilityOfAllGuiControlsThatTouchData())

            await PopulateSeriesEventAndBlobNameCboAsync();

            SaveGenesisOfThisSeasonItemAsLastKnownGood();

            await TextBoxForEnteringSeasonProfileFileNameFragmentVm.ChangeTextAsync(""); // neat and tidy

            await _thingsPersistedInLocalStorage.SaveSeasonMetadataIdAsync(seasonIdAsInt);

            await SaveSeasonMetadataItemToLocalStorageBackupAsync(NameOfFolderContainingSeasonData, seasonIdAsInt.ToString(), CurrentlyValidatedSeasonProfileItem);

            var messageOK = OwnerOfThisServiceIsPortalNotRezultz
                ? $"{SeasonProfile_loaded}  ID=<{seasonIdAsInt}>\r\n\r\n{Going_offline_blurb}"
                : $"{SeasonProfile_loaded}  ID=<{seasonIdAsInt}>";

            var seriesItemsExist = CurrentlyValidatedSeasonProfileItem.SeriesProfiles.Any();

            if (seriesItemsExist) return messageOK;

            messageOK = JghString.ConcatAsParagraphs(messageOK, No_series_items_found);

            return messageOK;

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

    #region SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickAsync

    private bool SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickCanExecute()
    {
        return SubmitSeasonProfileFilenameFragmentOfflineButtonVm.IsAuthorisedToOperate;
    }

    private async void SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickExecuteAsync]";

        try
        {
            OpenProgressIndicator(Working_____validating_and_saving_seasonId);

            var outcome = await SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickOrchestrateAsync();

            FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(outcome);
        }

        #region try catch

        catch (Exception ex)
        {
            FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    public async Task<string> SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickOrchestrateAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickOrchestrateAsync]";

        try
        {
            if (!SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickCanExecute())
                return string.Empty;

            DeadenGui();

            var outcome = await SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickAsync();

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

    private async Task<string> SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickAsync()
    {
        const string failure = "Unable to save settings SeasonData ID.";
        const string locus = "[SubmitSeasonProfileFilenameFragmentOfflineButtonOnClickAsync]";

        try
        {
            #region bale if invalid

            if (!JghFilePathValidator.IsSuperficiallyValidXmlFileNameOfSeasonProfile(TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text))
                return JghString.ConcatAsSentences(SeasonMetadataId_not_recognised, SeasonProfileIdFormatErrorWarningMessage);

            var discoveredSeasonProfileItem = await GetSeasonMetadataItemFromLocalStorageAsync(NameOfFolderContainingSeasonData, TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text); // success!?

            if (discoveredSeasonProfileItem == null) return SeasonMetadata_not_found_on_this_machine_message;

            CurrentlyValidatedSeasonProfileItem = discoveredSeasonProfileItem;

            #endregion

            #region success. save and confirm

            _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData = false;

            //SeasonProfileFileNameFragmentIsValidated = true;

            var seasonIdAsInt = Convert.ToInt32(TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text);

            TextBoxForEnteringSeasonProfileFileNameFragmentVm.Label = $"{TextBoxForEnteringSeasonProfileFileNameFragmentVm.Text} validated";

            await PopulateSeriesEventAndBlobNameCboAsync();

            SaveGenesisOfThisSeasonItemAsLastKnownGood();

            await TextBoxForEnteringSeasonProfileFileNameFragmentVm.ChangeTextAsync(string.Empty);
            await TextBoxForEnteringSeasonProfileSecurityAccessCodeVm.ChangeTextAsync(string.Empty);

            await _thingsPersistedInLocalStorage.SaveSeasonMetadataIdAsync(seasonIdAsInt);

            var messageOK = $"{SeasonProfile_loaded}  ID=<{seasonIdAsInt}>";

            #endregion

            return messageOK;
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region ClearSeasonProfileFilenameFragmentButtonOnClickAsync

    private bool ClearSeasonProfileFilenameFragmentButtonOnClickCanExecute()
    {
        return ClearSeasonProfileFilenameFragmentButtonVm.IsAuthorisedToOperate;
    }

    private async void ClearSeasonProfileFilenameFragmentButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[ClearSeasonProfileFilenameFragmentButtonOnClickExecuteAsync]";

        try
        {
            if (!ClearSeasonProfileFilenameFragmentButtonOnClickCanExecute())
                return;

            DeadenGui();

            var messageOk = await ClearSeasonIdButtonOnClickAsync();

            EnlivenGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
            {
                EvaluateGui();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
            else
            {
                FreezeProgressIndicator();

                await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
            }
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    private async Task<string> ClearSeasonIdButtonOnClickAsync()
    {
        await ZeroiseSeasonProfileAndIdentityAsync();

        //ThisViewModelIsInitialised = false;

        return SeasonID_cleared;
    }

    #endregion

    #endregion

    #region Identity entry/validation

    #region TextBoxesForEnteringIdentityOnTextChangedExecute

    private void TextBoxesForEnteringIdentityOnTextChangedExecute()
    {
        SubmitIdentityButtonVm.IsAuthorisedToOperate = true;
    }

    #endregion

    #region SubmitIdentityButtonOnClickAsync

    private bool SubmitIdentityButtonOnClickCanExecute()
    {
        return SubmitIdentityButtonVm.IsAuthorisedToOperate;
    }

    private async void SubmitIdentityButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[SubmitIdentityButtonOnClickExecuteAsync]";

        try
        {
            if (!SubmitIdentityButtonOnClickCanExecute())
                return;

            OpenProgressIndicator(Working_____authenticating);

            DeadenGui();

            var outcome = await SubmitIdentityButtonOnClickAsync();

            EnlivenGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(outcome);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                EvaluateGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    private async Task<string> SubmitIdentityButtonOnClickAsync()
    {
        const string failure = "Unable to authenticate identity.";
        const string locus = "[SubmitIdentityButtonOnClickAsync]";

        try
        {
            #region do work

            #region get identityUser

            if (CurrentlyValidatedSeasonProfileItem == null)
                throw new JghAlertMessageException(Please_submit_and_validate_SeasonID);

            var userNameIsRecognised = GetIfIdentityUserNameIsRecognised(TextBoxForEnteringIdentityUserNameVm.Text);

            if (!userNameIsRecognised)
                throw new JghAlertMessageException(UserName_not_recognised_message);

            if (string.IsNullOrWhiteSpace(TextBoxForEnteringIdentityPasswordVm.Text))
                throw new JghAlertMessageException(Password_not_recognised_message);

            var _candidateIdentity = GetAuthenticatedIdentityUser(TextBoxForEnteringIdentityUserNameVm.Text, TextBoxForEnteringIdentityPasswordVm.Text);

            if (_candidateIdentity == null)
                throw new JghAlertMessageException(Credentials_not_recognised_message);

            var workRoleIsOk = _candidateIdentity.ArrayOfAuthorisedWorkRoles.Any(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpace(z, CurrentRequiredWorkRole));

            if (!workRoleIsOk)
                throw new JghAlertMessageException($"{Not_authorised_for_this_work_role}  Required work role is [{CurrentRequiredWorkRole}].");

            #endregion

            #region success. save and confirm

            //_mustOfferUserAlternativeOfSearchingLocalStorageForUserCredentials = false;

            CurrentlyAuthenticatedIdentityItem = _candidateIdentity;

            TextBoxForEnteringIdentityUserNameVm.Label = $"{CurrentlyAuthenticatedIdentityItem.UserName} authenticated";

            SaveGenesisOfThisIdentityItemAsLastKnownGood();

            await TextBoxForEnteringIdentityUserNameVm.ChangeTextAsync(""); // neat and tidy
            await TextBoxForEnteringIdentityPasswordVm.ChangeTextAsync(""); // hide afterwards for confidentiality

            var messageOK = Credentials_authenticated;

            #endregion

            return messageOK;

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

    #region ClearIdentityButtonOnClickAsync

    private bool ClearIdentityButtonOnClickCanExecute()
    {
        return ClearIdentityButtonVm.IsAuthorisedToOperate;
    }

    private async void ClearIdentityButtonOnClickExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[ClearButtonOnClickExecute]";

        try
        {
            if (!ClearIdentityButtonOnClickCanExecute())
                return;

            DeadenGui();

            var messageOk = await ClearIdentityButtonOnClickAsync();

            EnlivenGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(messageOk);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                EvaluateGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    private async Task<string> ClearIdentityButtonOnClickAsync()
    {
        CurrentlyAuthenticatedIdentityItem = null;

        await TextBoxForEnteringIdentityUserNameVm.ChangeTextAsync("");

        await TextBoxForEnteringIdentityPasswordVm.ChangeTextAsync("");

        TextBoxForEnteringIdentityUserNameVm.Label = Not_yet_authenticated;

        return User_cleared_message;
    }

    #endregion

    #endregion

    private bool DummyOnTextChangedCanExecuteAlwaysTrue()
    {
        return true;
    }


    #region lookup selectors

    #region CboLookupSeasonOnSelectionChangedAsync i.e. clone of SubmitSeasonProfileFilenameFragmentButtonOnClickAsync

    private bool CboLookupSeasonOnSelectionChangedCanExecute()
    {
        return CboLookupSeasonVm.IsAuthorisedToOperate;
    }

    private async void CboLookupSeasonOnSelectionChangedExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CboLookupSeasonIdOnSelectionChangedExecute]";

        void MoveForwardUponSuccess()
        {
            EvaluateGui();
        }

        try
        {
            if (!CboLookupSeasonOnSelectionChangedCanExecute())
                return;

            OpenProgressIndicator(Working____);

            DeadenGui();

            var outcome = await CboLookupSeasonOnSelectionChanged();

            EnlivenGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(outcome);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                MoveForwardUponSuccess();

            FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    private async Task<string> CboLookupSeasonOnSelectionChanged()
    {
        const string failure = "Unable to fully load dataset associated with specified FragmentInFileNameOfAssociatedProfileFile.";
        const string locus = "[CboLookupSeasonOnSelectionChanged]";

        async Task ZeroiseTextBoxForEnteringSeasonIdManually()
        {
            await TextBoxForEnteringSeasonProfileFileNameFragmentVm.ChangeTextAsync(""); // neat and tidy

            TextBoxForEnteringSeasonProfileFileNameFragmentVm.Label = string.Empty;
        }

        async Task PopulateTextBoxForEnteringSeasonIdManually(string selectedSeasonIdAsText)
        {
            await TextBoxForEnteringSeasonProfileFileNameFragmentVm.ChangeTextAsync(""); // neat and tidy

            TextBoxForEnteringSeasonProfileFileNameFragmentVm.Label = $"{selectedSeasonIdAsText} validated";
        }

        try
        {
            await ZeroiseTextBoxForEnteringSeasonIdManually();

            var selectedSeasonIdAsText = CboLookupSeasonVm.CurrentItem?.FragmentInFileNameOfAssociatedProfileFile;

            #region bale if invalid

            if (!JghFilePathValidator.IsSuperficiallyValidXmlFileNameOfSeasonProfile(selectedSeasonIdAsText))
                return JghString.ConcatAsSentences(SeasonMetadataId_not_recognised, SeasonProfileIdFormatErrorWarningMessage);

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData = true;

                return StringsRezultz02.NoConnection;
            }

            try
            {
                await _leaderboardResultsSvcAgent.ThrowIfNoServiceConnectionAsync();
            }
            catch (Exception e)
            {
                _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData = true;

                return JghExceptionHelpers.PrintRedactedExceptionMessage(e);
            }

            var doesExist = await _leaderboardResultsSvcAgent.GetIfFileNameOfSeasonProfileIsRecognisedAsync(selectedSeasonIdAsText, CancellationToken.None);

            if (!doesExist) return SeasonMetadataId_not_recognised;

            CurrentlyValidatedSeasonProfileItem = await _leaderboardResultsSvcAgent.GetSeasonProfileAsync(selectedSeasonIdAsText, CancellationToken.None); // success!?

            if (CurrentlyValidatedSeasonProfileItem == null) return SeasonProfileFile_is_blank;

            #endregion

            #region success. save and confirm

            _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData = false;

            //SeasonProfileFileNameFragmentIsValidated = true;

            var seasonIdAsInt = Convert.ToInt32(selectedSeasonIdAsText);

            await PopulateTextBoxForEnteringSeasonIdManually(selectedSeasonIdAsText);

            await PopulateSeriesEventAndBlobNameCboAsync();

            SaveGenesisOfThisSeasonItemAsLastKnownGood();

            await _thingsPersistedInLocalStorage.SaveSeasonMetadataIdAsync(seasonIdAsInt);

            await SaveSeasonMetadataItemToLocalStorageBackupAsync(NameOfFolderContainingSeasonData, seasonIdAsInt.ToString(), CurrentlyValidatedSeasonProfileItem);

            var messageOK = OwnerOfThisServiceIsPortalNotRezultz
                ? $"{SeasonProfile_loaded}  ID=<{seasonIdAsInt}>  {Going_offline_blurb}"
                : $"{SeasonProfile_loaded}  ID=<{seasonIdAsInt}>";

            var seriesItemsExist = CurrentlyValidatedSeasonProfileItem.SeriesProfiles.Any();

            if (seriesItemsExist) return messageOK;

            messageOK = JghString.ConcatAsParagraphs(messageOK, No_series_items_found);

            #endregion

            return messageOK;
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region CboLookupSeriesOnSelectionChangedAsync

    private bool CboLookupSeriesOnSelectionChangedCanExecute()
    {
        return CboLookupSeriesVm.IsAuthorisedToOperate;
    }

    private async void CboLookupSeriesOnSelectionChangedExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CboLookupSeriesOnSelectionChangedExecute]";

        void MoveForwardUponSuccess()
        {
            EvaluateGui();
        }

        try
        {
            if (!CboLookupSeriesOnSelectionChangedCanExecute())
                return;

            OpenProgressIndicator(Working____);

            DeadenGui();

            var outcome = await CboLookupSeriesOnSelectionChanged();

            EnlivenGui();

            FreezeProgressIndicator();

            await AlertMessageService.ShowOkAsync(outcome);
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                MoveForwardUponSuccess();

            FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    private async Task<string> CboLookupSeriesOnSelectionChanged()
    {
        const string failure = "Unable to fully load dataset associated with specified series.";
        const string locus = "[CboLookupSeriesOnSelectionChanged]";

        try
        {
            CboLookupSeriesVm.SaveSelectedIndexAsLastKnownGood();

            await PopulateCboLookupEventAsync();

            HeadersVm.Populate(CboLookupSeriesVm.CurrentItem?.Label, CboLookupEventVm.CurrentItem?.Label);

            await CboLookupEventOnSelectionChangedAsync(); // changing series is tantamount to changing event. restart

            var outcome = "Series loaded.";

            return outcome;
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region CboLookupEventOnSelectionChangedAsync - mickey mouse

    private bool CboLookupEventOnSelectionChangedCanExecute()
    {
        return CboLookupEventVm.IsAuthorisedToOperate;
    }

    private async void CboLookupEventOnSelectionChangedExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CboLookupEventOnSelectionChangedExecute]";

        void MoveForwardUponSuccess()
        {
            EvaluateGui();
        }

        try
        {
            if (!CboLookupEventOnSelectionChangedCanExecute())
                return;


            OpenProgressIndicator(Working____);

            DeadenGui();

            await CboLookupEventOnSelectionChangedAsync();

            EnlivenGui();
        }

        #region try catch

        catch (Exception ex)
        {
            RestoreGui();

            if (JghExceptionHelpers.InnermostExceptionIsOfSpecifiedType<JghAlertMessageException>(ex))
                MoveForwardUponSuccess();

            FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            CloseProgressIndicator();
        }

        #endregion
    }

    private async Task<bool> CboLookupEventOnSelectionChangedAsync()
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[CboLookupEventOnSelectionChangedAsync]";

        try
        {
            CboLookupEventVm.SaveSelectedIndexAsLastKnownGood();

            HeadersVm.Populate(CboLookupSeriesVm.CurrentItem.Label, CboLookupEventVm.CurrentItem.Label);

            await PopulateCboLookupBlobNameForPublishedResultsAsync();

            CboLookupBlobNameToPublishResultsOnSelectionChangedAsync();

            return true;
        }

        #region try catch

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    #endregion

    #region CboLookupBlobNameToPublishResultsOnSelectionChangedAsync

    private bool CboLookupBlobNameToPublishResultsOnSelectionChangedCanExecute()
    {
        return CboLookupBlobNameToPublishResultsVm.IsAuthorisedToOperate;
    }

    private async void CboLookupBlobNameToPublishResultsOnSelectionChangedExecuteAsync()
    {
        const string failure = "Unable to complete ICommand Execute action.";
        const string locus = "[CboLookupBlobNameToPublishResultsOnSelectionChangedExecuteAsync]";

        try
        {
            SeasonProfileProgressIndicatorVm.OpenProgressIndicator("Working ....");

            // dummy
            CboLookupBlobNameToPublishResultsOnSelectionChangedAsync();
        }

        #region try catch

        catch (Exception ex)
        {
            SeasonProfileProgressIndicatorVm.FreezeProgressIndicator();

            await AlertMessageService.ShowNotificationOrErrorMessageAsync(failure, locus, Locus2, Locus3, ex);
        }
        finally
        {
            SeasonProfileProgressIndicatorVm.CloseProgressIndicator();
        }

        #endregion
    }

    private void CboLookupBlobNameToPublishResultsOnSelectionChangedAsync()
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[CboLookupBlobNameToPublishResultsOnSelectionChangedAsync]";

        try
        {
            CboLookupBlobNameToPublishResultsVm.SaveSelectedIndexAsLastKnownGood();

            // do nothing
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

    #endregion

    #region methods

    public bool GetIfIdentityUserNameIsRecognised(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return false;

        var identityItems = CurrentlyValidatedSeasonProfileItem?.AuthorisedIdentities;

        if (identityItems == null)
            return false;

        var answer = identityItems.Any(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpace(z.UserName, userName));

        return answer;
    }

    public IdentityItem GetAuthenticatedIdentityUser(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            return null;

        var identityItems = CurrentlyValidatedSeasonProfileItem?.AuthorisedIdentities;

        if (identityItems == null)
            return null;

        var answers = identityItems.Where(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpace(z.UserName, userName) && JghString.AreEqualAndNeitherIsNullOrWhiteSpace(z.Password, password));

        var answer = answers.FirstOrDefault();

        return answer;
    }

    public bool GetIfCurrentlyAuthenticatedIdentityUserIsAuthorisedForRequiredWorkRole()
    {
        if (CurrentlyAuthenticatedIdentityItem?.ArrayOfAuthorisedWorkRoles == null)
            return false;

        var workRoleIsOk = CurrentlyAuthenticatedIdentityItem.ArrayOfAuthorisedWorkRoles.Any(z => JghString.AreEqualAndNeitherIsNullOrWhiteSpace(z, CurrentRequiredWorkRole));

        return workRoleIsOk;
    }

    #endregion

    #region helpers

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
    private async Task<string> LoadListOfShallowSeasonItemsAsync()
    {
        const string failure = "Unable to do what this method does.";
        const string locus = "[LoadListOfShallowSeasonItemsAsync]";

        try
        {
            #region null checks

            ThrowExceptionIfNoConnection();

            #endregion

            #region populate CboLookupSeasonVm

            var arrayOfSeasonItem = await _leaderboardResultsSvcAgent.GetAllSeasonProfilesAsync(CancellationToken.None);

            if (arrayOfSeasonItem == null || !arrayOfSeasonItem.Any())
                throw new JghResultsData404Exception("Season information is empty.");

            arrayOfSeasonItem = arrayOfSeasonItem
                .Where(z => JghString.JghContains(EnumStrings.AccessCodeForPublicAccessToSeasonData, z.AccessCodes))
                .OrderByDescending(z => z.AdvertisedDate).ToArray();

            var itemsSource = SeasonItemDisplayObject.FromModel(arrayOfSeasonItem);

            foreach (var seasonItemVm in itemsSource) seasonItemVm.Tag = JghString.ConcatWithSeparator(" ", seasonItemVm.AdvertisedDateTime.ToString(JghDateTime.ShortDatePattern), seasonItemVm.Label);

            await CboLookupSeasonVm.RefillItemsSourceAsync(itemsSource);

            CboLookupSeasonVm.IsVisible = true;

            CboLookupSeasonVm.IsDropDownOpen = false;

            CboLookupSeasonVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

            await CboLookupSeasonVm.ChangeSelectedIndexToNullAsync();

            CboLookupSeasonVm.SaveSelectedIndexAsLastKnownGood();

            var messageOk = "Options loaded successfully. Select your desired option.";

            #endregion

            return messageOk;
        }

        #region try catch handling

        catch (Exception ex)
        {
            throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
        }

        #endregion
    }

    private async Task PopulateSeriesEventAndBlobNameCboAsync()
    {
        await PopulateCboLookupSeriesAsync();

        await PopulateCboLookupEventAsync();

        await PopulateCboLookupBlobNameForPublishedResultsAsync();

        HeadersVm.Populate(CboLookupSeriesVm?.CurrentItem?.Label, CboLookupEventVm?.CurrentItem?.Label);
    }

    private async Task PopulateCboLookupSeriesAsync()
    {
        if (CurrentlyValidatedSeasonProfileItem == null)
        {
            await CboLookupSeriesVm.ZeroiseItemsSourceAsync();

            return;
        }

        if (CurrentlyValidatedSeasonProfileItem.SeriesProfiles == null || !CurrentlyValidatedSeasonProfileItem.SeriesProfiles.Any())
        {
            await CboLookupSeriesVm.ZeroiseItemsSourceAsync();

            return;
        }

        var arrayOfSeriesItems = SeriesItemDisplayObject.FromModel(CurrentlyValidatedSeasonProfileItem.SeriesProfiles);

        arrayOfSeriesItems = arrayOfSeriesItems
            .Where(z => z != null)
            .OrderBy(z => z.DisplayRank)
            .ThenByDescending(z => z.AdvertisedDateTime)
            .ThenBy(z => z.Label)
            .ToArray();

        await CboLookupSeriesVm.RefillItemsSourceAsync(arrayOfSeriesItems);

        var bestGuessChoiceOfSeriesItemToKickOffWith =
            JghArrayHelpers.SelectMostRecentItemBeforeDateTimeNowInArrayOfItemsOrFailingThatPickTheEarliest(SeriesItemDisplayObject.ObtainSourceModel(CboLookupSeriesVm.ItemsSource.ToArray()));

        //if (bestGuessChoiceOfSeriesItemToKickOffWith == null)
        //    throw new Jgh404Exception("Sorry. Unexpected code error on local machine. bestGuessChoiceOfSeriesItemToKickOffWith is null. Unable to proceed.");

        await CboLookupSeriesVm.ChangeSelectedIndexToMatchItemLabelAsync(bestGuessChoiceOfSeriesItemToKickOffWith?.Label);

        CboLookupSeriesVm.IsDropDownOpen = false;

        CboLookupSeriesVm.SaveSelectedIndexAsLastKnownGood();

        CboLookupSeriesVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

        CboLookupSeriesVm.MakeVisibleIfItemsSourceIsGreaterThanOne();

        //if (CboLookupSeriesVm.CurrentItem == null)
        //    throw new JghNullObjectInstanceException("Sorry. Unexpected code error on local machine. CboLookupSeriesVm.CurrentItem is null. Unable to proceed."); // exceedingly farfetched. belt and braces

        //CboLookupSeriesVm.Label = CboLookupSeriesVm.CurrentItem?.Label;
    }

    private async Task PopulateCboLookupEventAsync()
    {
        if (CurrentlyValidatedSeasonProfileItem == null)
        {
            await CboLookupEventVm.ZeroiseItemsSourceAsync();

            return;
        }

        if (CboLookupSeriesVm.CurrentItem == null)
        {
            await CboLookupEventVm.ZeroiseItemsSourceAsync();

            return;
        }

        var listOfEventItems = CboLookupSeriesVm.CurrentItem?.ArrayOfEventItems ?? Array.Empty<EventItemDisplayObject>();

        listOfEventItems = listOfEventItems
            .Where(z => z != null)
            .OrderBy(z => z.DisplayRank)
            .ThenByDescending(z => z.AdvertisedDateTime)
            .ThenBy(z => z.Label)
            .ToArray();

        await CboLookupEventVm.RefillItemsSourceAsync(listOfEventItems);

        var bestGuessChoiceOfEventItemToKickOffWith =
            JghArrayHelpers.SelectMostRecentItemBeforeDateTimeNowInArrayOfItemsOrFailingThatPickTheEarliest(EventItemDisplayObject.ObtainSourceModel(CboLookupEventVm.ItemsSource.ToArray()));

        //if (bestGuessChoiceOfEventItemToKickOffWith == null)
        //    throw new Jgh404Exception(
        //        "Sorry. No Event particulars found. bestGuessChoiceOfEventItemToKickOffWith is null. Unable to proceed.");

        await CboLookupEventVm.ChangeSelectedIndexToMatchItemLabelAsync(bestGuessChoiceOfEventItemToKickOffWith?.Label);

        CboLookupEventVm.IsDropDownOpen = false;

        CboLookupEventVm.SaveSelectedIndexAsLastKnownGood();

        CboLookupEventVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

        CboLookupEventVm.MakeVisibleIfItemsSourceIsGreaterThanOne();

        //if (CboLookupEventVm.CurrentItem == null)
        //    throw new JghNullObjectInstanceException("CboLookupEventVm.CurrentItem is null"); // exceedingly farfetched. belt and braces

        //CboLookupEventVm.Label = CboLookupEventVm.CurrentItem?.Label;
    }

    private async Task PopulateCboLookupBlobNameForPublishedResultsAsync()
    {
        if (CurrentlyValidatedSeasonProfileItem == null)
        {
            await CboLookupEventVm.ZeroiseItemsSourceAsync();

            return;
        }

        if (CboLookupEventVm.CurrentItem == null)
        {
            await CboLookupEventVm.ZeroiseItemsSourceAsync();

            return;
        }

        var itemsSourceForCboBlobs = CboLookupEventVm.CurrentItem.ArrayOfLocationOfPreprocessedResultsDataFiles ?? Array.Empty<EntityLocationItemDisplayObject>()
            .Where(z => z != null)
            .OrderBy(z => z.DataItemName)
            .ToArray();

        await CboLookupBlobNameToPublishResultsVm.RefillItemsSourceAsync(itemsSourceForCboBlobs);

        await CboLookupBlobNameToPublishResultsVm.ChangeSelectedIndexAsync(0);

        CboLookupBlobNameToPublishResultsVm.IsDropDownOpen = false;

        CboLookupBlobNameToPublishResultsVm.SaveSelectedIndexAsLastKnownGood();

        CboLookupBlobNameToPublishResultsVm.MakeAuthorisedToOperateIfItemsSourceIsAny();

        CboLookupBlobNameToPublishResultsVm.MakeVisibleIfItemsSourceIsGreaterThanOne();
    }

    private async Task ZeroiseIdentityAsync()
    {
        CurrentlyAuthenticatedIdentityItem = null;

        await TextBoxForEnteringIdentityUserNameVm.ChangeTextAsync(string.Empty);
        await TextBoxForEnteringIdentityPasswordVm.ChangeTextAsync(string.Empty);
        TextBoxForEnteringIdentityUserNameVm.Label = Not_yet_authenticated;
    }

    private async Task ZeroiseSeasonProfileAndIdentityAsync()
    {
        CurrentlyValidatedSeasonProfileItem = null;
        await TextBoxForEnteringSeasonProfileSecurityAccessCodeVm.ChangeTextAsync(string.Empty);
        await TextBoxForEnteringSeasonProfileFileNameFragmentVm.ChangeTextAsync(string.Empty);
        TextBoxForEnteringSeasonProfileFileNameFragmentVm.Label = Not_yet_chosen;


        await CboLookupSeasonVm.ZeroiseAsync();
        await CboLookupSeriesVm.ZeroiseAsync();
        await CboLookupEventVm.ZeroiseAsync();
        await CboLookupBlobNameToPublishResultsVm.ZeroiseAsync();

        await _thingsPersistedInLocalStorage.SaveSeasonMetadataIdAsync(0);

        await ZeroiseIdentityAsync();
    }

    private async Task SaveSeasonMetadataItemToLocalStorageBackupAsync(string folderName, string thisSeasonId, SeasonProfileItem thisSeasonProfileItem)
    {
        if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(thisSeasonId) || thisSeasonProfileItem == null)
            return;

        var seasonItemDtoToBeSaved = SeasonProfileItem.ToDataTransferObject(thisSeasonProfileItem);

        await _storageService.SaveSerialisableObjectAsync(
            folderName,
            MakeFileNameFromSeasonId(thisSeasonId),
            seasonItemDtoToBeSaved);
    }

    private async Task<SeasonProfileItem> GetSeasonMetadataItemFromLocalStorageAsync(string folderName, string thisSeasonId)
    {
        if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(thisSeasonId))
            return null;

        SeasonProfileDto recoveredMetaDataDto;

        try
        {
            recoveredMetaDataDto =
                await _storageService.ReadSerializedObjectAsync<SeasonProfileDto>(folderName, MakeFileNameFromSeasonId(thisSeasonId));

            if (recoveredMetaDataDto == null || !JghString.AreEqualAndNeitherIsNullOrWhiteSpace(thisSeasonId, recoveredMetaDataDto.FragmentInFileNameOfAssociatedProfileFile))
                return null;
        }
        catch (Exception)
        {
            return null;
        }

        var answer = SeasonProfileItem.FromDataTransferObject(recoveredMetaDataDto);

        return answer;
    }

    // ReSharper disable once UnusedMember.Local
    private async Task ClearLocalStorageBackupAsync(string folderName)
    {
        // The whole idea here is to expunge local storage, including local storage that might have become corrupted, or contain
        // something other than a SeasonProfileItem, and which will therefore be unreadable and cause the
        // read attempt to throw an exception.

        if (string.IsNullOrWhiteSpace(folderName))
            return;

        await _storageService.DeleteDirectoryAsync(folderName);
    }

    private string MakeFileNameFromSeasonId(string seasonId)
    {
        if (string.IsNullOrWhiteSpace(seasonId))
            return null;

        var useableFileName = JghFilePathValidator.AttemptToMakeValidNtfsFileOrFolderNameByReplacingInvalidCharacters('-', seasonId);

        var fileName = $"{useableFileName}.json";

        return fileName;
    }

    private void ThrowExceptionIfNoConnection()
    {
        if (!NetworkInterface.GetIsNetworkAvailable()) throw new JghCommunicationFailureException(StringsRezultz02.NoConnection);
    }

    #endregion

    #region progress indicating

    private void OpenProgressIndicator(string descriptionOfWhatsHappening)
    {
        SeasonProfileProgressIndicatorVm.OpenProgressIndicator(descriptionOfWhatsHappening);
    }

    private void FreezeProgressIndicator()
    {
        SeasonProfileProgressIndicatorVm.FreezeProgressIndicator();
    }

    private void CloseProgressIndicator()
    {
        SeasonProfileProgressIndicatorVm.CloseProgressIndicator();
    }

    #endregion

    #region Gui stuff

    public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
    {
        ClearSeasonProfileFilenameFragmentButtonVm.IsAuthorisedToOperate = true; // always enabled
        SubmitSeasonProfileFileNameFragmentButtonVm.IsAuthorisedToOperate = false;
        SubmitSeasonProfileFilenameFragmentOfflineButtonVm.IsAuthorisedToOperate = _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData;
        TextBoxForEnteringSeasonProfileSecurityAccessCodeVm.IsAuthorisedToOperate = true; // ditto
        TextBoxForEnteringSeasonProfileFileNameFragmentVm.IsAuthorisedToOperate = true; // ditto

        ClearIdentityButtonVm.IsAuthorisedToOperate = true; // always enabled
        SubmitIdentityButtonVm.IsAuthorisedToOperate = false;
        TextBoxForEnteringIdentityUserNameVm.IsAuthorisedToOperate = true;
        TextBoxForEnteringIdentityPasswordVm.IsAuthorisedToOperate = true;

        CboLookupSeasonVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
        CboLookupSeriesVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
        CboLookupEventVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
        CboLookupBlobNameToPublishResultsVm.MakeAuthorisedToOperateIfItemsSourceIsAny();
    }

    protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
    {
        ClearSeasonProfileFilenameFragmentButtonVm.IsVisible = true;
        SubmitSeasonProfileFileNameFragmentButtonVm.IsVisible = true;
        SubmitSeasonProfileFilenameFragmentOfflineButtonVm.IsVisible = _mustOfferUserTheAlternativeOfSearchingLocalStorageForSeasonData;
        TextBoxForEnteringSeasonProfileSecurityAccessCodeVm.IsVisible = true;
        TextBoxForEnteringSeasonProfileFileNameFragmentVm.IsVisible = true;

        ClearIdentityButtonVm.IsVisible = true;
        SubmitIdentityButtonVm.IsVisible = true;
        //SearchLocalStorageForUserCredentialsButtonVm.IsVisible = _mustOfferUserAlternativeOfSearchingLocalStorageForUserCredentials;
        TextBoxForEnteringIdentityUserNameVm.IsVisible = true;
        TextBoxForEnteringIdentityPasswordVm.IsVisible = true;

        CboLookupSeasonVm.IsVisible = !OwnerOfThisServiceIsPortalNotRezultz;
        CboLookupSeriesVm.IsVisible = true;
        CboLookupEventVm.IsVisible = !MustHideCboLookupEvent;
        CboLookupBlobNameToPublishResultsVm.IsVisible = !MustHideCboLookupBlobNameToPublishResults;
    }

    protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
    {
        var answer = new List<object>();

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, SubmitSeasonProfileFileNameFragmentButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, SubmitSeasonProfileFilenameFragmentOfflineButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, ClearSeasonProfileFilenameFragmentButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringSeasonProfileSecurityAccessCodeVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringSeasonProfileFileNameFragmentVm);

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, SubmitIdentityButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, ClearIdentityButtonVm);
        //AddToCollectionIfIHasIsAuthorisedToOperate(answer, SearchLocalStorageForUserCredentialsButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringIdentityUserNameVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, TextBoxForEnteringIdentityPasswordVm);


        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupSeriesVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupEventVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookupBlobNameToPublishResultsVm);

        return answer;
    }

    #endregion

    #region GenesisAsLastKnownGood

    protected void SaveGenesisOfThisSeasonItemAsLastKnownGood()
    {
        _lastKnownGoodSeasonProfileItemToWhichThisVmBelongs = CurrentlyValidatedSeasonProfileItem;
    }

    protected void SaveGenesisOfThisIdentityItemAsLastKnownGood()
    {
        _lastKnownGoodIdentityItemToWhichThisVmBelongs = CurrentlyAuthenticatedIdentityItem;
    }

    //protected void SaveGenesisOfThisViewModelAsLastKnownGood()
    //{
    //    _lastKnownGoodSeasonProfileItemToWhichThisVmBelongs = CurrentlyValidatedSeasonProfileItem;
    //    _lastKnownGoodIdentityItemToWhichThisVmBelongs = CurrentlyAuthenticatedIdentityItem;
    //}

    public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        if (_lastKnownGoodSeasonProfileItemToWhichThisVmBelongs != CurrentlyValidatedSeasonProfileItem)
            return true;

        if (_lastKnownGoodIdentityItemToWhichThisVmBelongs != CurrentlyAuthenticatedIdentityItem)
            return true;

        return false;
    }

    #endregion
}