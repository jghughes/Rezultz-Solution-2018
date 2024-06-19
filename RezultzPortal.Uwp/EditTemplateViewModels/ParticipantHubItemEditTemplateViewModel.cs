using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.ViewModels01.April2022.Collections;
using NetStd.ViewModels01.April2022.UserControls;
using Rezultz.DataTypes.Nov2023.PortalHubItems;
using Rezultz.DataTypes.Nov2023.SeasonAndSeriesProfileItems;

// ReSharper disable UnusedMethodReturnValue.Local


namespace RezultzPortal.Uwp.EditTemplateViewModels
{
    public class ParticipantHubItemEditTemplateViewModel : HubItemEditTemplateViewModelBase, IHasZeroiseAsync
    {
        #region constants

        private const int DangerouslyBriefSafetyMarginForBindingEngineMilliSec = 50;

        #endregion

        #region fields

        private ClipBoardForParticipantProfile _myClipBoardForParticipantProfile;

        #endregion

        #region ctor

        public ParticipantHubItemEditTemplateViewModel()
    {
        CboLookUpGenderSpecificationItemsVm = new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(string.Empty, AnyEditTemplateEntryChanged, CboListOfGenderSpecificationItemsOnSelectionChangedCanExecute) {IsVisible = true};

        CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm =
            new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(string.Empty, CboListOfRaceSpecificationItemsBeforeTransitionOnSelectionChangedExecute, CboListOfRaceSpecificationItemsBeforeTransitionOnSelectionChangedCanExecute)
                {IsVisible = true};

        CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm =
            new IndexDrivenCollectionViewModel<CboLookupItemDisplayObject>(string.Empty, CboListOfRaceSpecificationItemsAfterTransitionOnSelectionChangedExecute, CboListOfRaceSpecificationItemsAfterTransitionOnSelectionChangedCanExecute)
                {IsVisible = true};

        // Note: I empirically proved in Aug 2022 that even on my super-fast machine anything less than 15 milliseconds destroys the binding engine during debug. 

        // I am 99% certain that the culprit that causes this high value is the existence of wiring up to a button that is external to this vm, AcceptRepositoryItemBeingEditedButtonVm, which is enabled/disabled
        // via AnyEditTemplateEntryChangedExecuteAction in the this templates base class, and in turn by the delegate assigned by the owning class of this vm.

        // See DataGridBaseHubItemEditTemplateVm.AnyEditTemplateEntryChangedExecuteAction = MakeAcceptItemBeingEditedButtonVmAuthorisedToOperate in PortalTimeKeepingAndParticipantProfileAdminPageViewModelBase for enlightenment.
        // Were it not for the external button, even as little as zero milliseconds would do just fine.
        // In the circumstances, I choose 50ms as an arbitrary safety margin to accomodate slower machines. This is only a quarter of the 200ms factory setting. Very noticeably faster.
        // This will need to be tested in real life on the machines that organisers actually use. Potentially very slow tablets.
        // Test it by seeing what happens to the Accept/Reject buttons. If they fail to be enabled and light up properly, increase the safety margin.

        //  If the design of this vm and/or its wiring up to the GUI evolves in any way, the testing must be redone on all buttons - both internal and especially external -
        //  to ensure their enabling behaviour is correctly affected by changes of SelectedIndex.


        CboLookUpGenderSpecificationItemsVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

        CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

        CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.OverrideAndEndangerFactorySettingForSafetyMarginForGuiBindingEngine(DangerouslyBriefSafetyMarginForBindingEngineMilliSec);

        DateOfRaceGroupTransitionCalendarPickerVm = new CalendarDatePickerControlViewModel(AnyEditTemplateEntryChanged, () => true) {Date = new DateTimeOffset(DateTime.Today)};
    }

        #endregion

        #region local type

        private class ClipBoardForParticipantProfile
        {
            public string LastName { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public string MiddleInitial { get; set; } = string.Empty;

            public string Gender { get; set; } = string.Empty;

            public string BirthYear { get; set; } = "1900";

            public string City { get; set; } = string.Empty;

            public string Team { get; set; } = string.Empty;

            public bool IsRegradedRaceGroup { get; set; }

            public string RaceGroupBeforeTransition { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

            public string RaceGroupAfterTransition { get; set; } = string.Empty; // originates from Label element of RaceSpecificationItem in SeriesSettings

            public DateTimeOffset? DateOfRaceGroupTransition { get; set; } = DateTime.Today;

            public bool IsSeries { get; set; }

            public string SeriesIdentifier { get; set; } = string.Empty;

            public string EventIdentifiers { get; set; } = string.Empty;
        }

        #endregion

        #region props

        private string _backingstoreFirstName = string.Empty;

        public string FirstName
        {
            get => _backingstoreFirstName ?? string.Empty;
            set => SetProperty(ref _backingstoreFirstName, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreMiddleInitial = string.Empty;

        public string MiddleInitial
        {
            get => _backingstoreMiddleInitial ?? string.Empty;
            set => SetProperty(ref _backingstoreMiddleInitial, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreLastName = string.Empty;

        public string LastName
        {
            get => _backingstoreLastName ?? string.Empty;
            set => SetProperty(ref _backingstoreLastName, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreBirthYear = string.Empty;

        public string BirthYear
        {
            get => _backingstoreBirthYear ?? string.Empty;
            set => SetProperty(ref _backingstoreBirthYear, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreCity = string.Empty;

        public string City
        {
            get => _backingstoreCity ?? string.Empty;
            set => SetProperty(ref _backingstoreCity, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreTeam = string.Empty;

        public string Team
        {
            get => _backingstoreTeam ?? string.Empty;
            set => SetProperty(ref _backingstoreTeam, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreSeriesIdentifier = string.Empty;

        public string SeriesIdentifier
        {
            get => _backingstoreSeriesIdentifier ?? string.Empty;
            set => SetProperty(ref _backingstoreSeriesIdentifier, value, AnyEditTemplateEntryChanged);
        }

        private string _backingstoreEventIdentifiers = string.Empty;

        public string EventIdentifiers
        {
            get => _backingstoreEventIdentifiers ?? string.Empty;
            set => SetProperty(ref _backingstoreEventIdentifiers, value, AnyEditTemplateEntryChanged);
        }

        private bool _backingstoreIsSeries = true; // arbitrary default because it's the most common participant

        public bool IsSeries
        {
            get => _backingstoreIsSeries;
            set => SetProperty(ref _backingstoreIsSeries, value, AnyEditTemplateEntryChanged);
        }


        private bool _backingstoreIsRegradedRaceGroup = true;

        public bool IsRegradedRaceGroup
        {
            get => _backingstoreIsRegradedRaceGroup;
            set => SetProperty(ref _backingstoreIsRegradedRaceGroup, value, AnyEditTemplateEntryChanged);
        }


        private string _backingstoreWhenTouchedDateForDisplayOnly = string.Empty;

        public string WhenTouchedDateForDisplayOnly
        {
            get => _backingstoreWhenTouchedDateForDisplayOnly ?? string.Empty;
            set => SetProperty(ref _backingstoreWhenTouchedDateForDisplayOnly, value, AnyEditTemplateEntryChanged);
        }

        public CalendarDatePickerControlViewModel DateOfRaceGroupTransitionCalendarPickerVm { get; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookUpGenderSpecificationItemsVm { get; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm { get; }

        public IndexDrivenCollectionViewModel<CboLookupItemDisplayObject> CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm { get; }

        #endregion

        #region commands

        #region CopyHubItemButtonOnClickAsync

        protected override async Task<string> CopyHubItemButtonOnClickAsync()
    {
        if (OneOrMoreEntriesAreInvalid(out var errorReason))
            return await Task.FromResult(errorReason);

        _myClipBoardForParticipantProfile = new ClipBoardForParticipantProfile
        {
            LastName = LastName,
            FirstName = FirstName,
            MiddleInitial = MiddleInitial,
            Gender = CboLookUpGenderSpecificationItemsVm.CurrentItem.Label,
            BirthYear = BirthYear,
            City = City,
            Team = Team,
            IsRegradedRaceGroup = IsRegradedRaceGroup,
            RaceGroupBeforeTransition = CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm?.CurrentItem?.Label,
            RaceGroupAfterTransition = CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm?.CurrentItem?.Label,
            DateOfRaceGroupTransition = DateOfRaceGroupTransitionCalendarPickerVm?.Date,
            IsSeries = IsSeries,
            SeriesIdentifier = SeriesIdentifier,
            EventIdentifiers = EventIdentifiers
        };

        return await Task.FromResult(Copied);
    }

        #endregion

        #region PasteHubItemButtonOnClick

        protected override async Task<string> PasteHubItemButtonOnClickAsync()
    {
        _myClipBoardForParticipantProfile ??= new ClipBoardForParticipantProfile();

        LastName = _myClipBoardForParticipantProfile.LastName;
        FirstName = _myClipBoardForParticipantProfile.FirstName;
        MiddleInitial = _myClipBoardForParticipantProfile.MiddleInitial;
        await CboLookUpGenderSpecificationItemsVm.ChangeSelectedIndexToMatchItemLabelAsync(_myClipBoardForParticipantProfile.Gender);
        BirthYear = _myClipBoardForParticipantProfile.BirthYear;
        City = _myClipBoardForParticipantProfile.City;
        Team = _myClipBoardForParticipantProfile.Team;
        IsRegradedRaceGroup = _myClipBoardForParticipantProfile.IsRegradedRaceGroup;
        await CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.ChangeSelectedIndexToMatchItemLabelAsync(_myClipBoardForParticipantProfile.RaceGroupBeforeTransition);
        await CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.ChangeSelectedIndexToMatchItemLabelAsync(_myClipBoardForParticipantProfile.RaceGroupAfterTransition);
        DateOfRaceGroupTransitionCalendarPickerVm.Date = _myClipBoardForParticipantProfile.DateOfRaceGroupTransition;
        IsSeries = _myClipBoardForParticipantProfile.IsSeries;
        SeriesIdentifier = _myClipBoardForParticipantProfile.SeriesIdentifier;
        EventIdentifiers = _myClipBoardForParticipantProfile.EventIdentifiers;

        return Pasted;
    }

        #endregion

        #region CboListOfGenderSpecificationItemsOnSelectionChangedAsync

        private bool CboListOfGenderSpecificationItemsOnSelectionChangedCanExecute()
    {
        return CboLookUpGenderSpecificationItemsVm.IsAuthorisedToOperate &&
               IsAuthorisedToOperate;
    }

        #endregion

        #region CboListOfRaceSpecificationItemsBeforeTransitionOnSelectionChangedAsync

        private bool CboListOfRaceSpecificationItemsBeforeTransitionOnSelectionChangedCanExecute()
    {
        return CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.IsAuthorisedToOperate && IsAuthorisedToOperate;
    }

        private void CboListOfRaceSpecificationItemsBeforeTransitionOnSelectionChangedExecute()
    {
        try
        {
            if (!CboListOfRaceSpecificationItemsBeforeTransitionOnSelectionChangedCanExecute())
                return;

            DeadenGui();

            AnyEditTemplateEntryChanged();

            EnlivenGui();
        }

        #region try catch

        catch (Exception)
        {
            RestoreGui();
        }

        #endregion
    }

        #endregion

        #region CboListOfRaceSpecificationItemsAfterTransitionOnSelectionChangedAsync

        private bool CboListOfRaceSpecificationItemsAfterTransitionOnSelectionChangedCanExecute()
    {
        return CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.IsAuthorisedToOperate && IsAuthorisedToOperate;
    }

        private void CboListOfRaceSpecificationItemsAfterTransitionOnSelectionChangedExecute()
    {
        try
        {
            if (!CboListOfRaceSpecificationItemsAfterTransitionOnSelectionChangedCanExecute())
                return;

            DeadenGui();

            AnyEditTemplateEntryChanged();

            EnlivenGui();
        }

        #region try catch

        catch (Exception)
        {
            RestoreGui();
        }

        #endregion
    }

        #endregion

        #endregion

        #region methods

        public async Task<bool> PopulateWithItemBeingModifiedAsync(ParticipantHubItem itemBeingModified, RaceSpecificationItem[] raceSpecificationItems)
    {
        #region Step 1. copy across all the straightforward property values from itemBeingModified to this template

        IsAuthorisedToOperate = false; // just to start with while we are working on it...

        if (itemBeingModified is null) return true;

        Bib = JghString.TmLr(itemBeingModified.Bib);
        Rfid = JghString.TmLr(itemBeingModified.Rfid);
        FirstName = itemBeingModified.FirstName;
        MiddleInitial = itemBeingModified.MiddleInitial;
        LastName = itemBeingModified.LastName;
        BirthYear = itemBeingModified.BirthYear.ToString();
        City = itemBeingModified.City;
        Team = itemBeingModified.Team;
        MiddleInitial = itemBeingModified.MiddleInitial;
        DateOfRaceGroupTransitionCalendarPickerVm.Date = new DateTimeOffset(itemBeingModified.DateOfRaceGroupTransition.Date);
        IsSeries = itemBeingModified.IsSeries;
        SeriesIdentifier = itemBeingModified.Series;
        EventIdentifiers = itemBeingModified.EventIdentifiers;
        MustDitchOriginatingItem = itemBeingModified.MustDitchOriginatingItem;
        WhenTouchedDateForDisplayOnly = JghDateTime.ToLongDate(itemBeingModified.WhenTouchedBinaryFormat); // for display only (titled "When added")

        #endregion


        #region Step 2. copy across Gender and Race - unfortunately this is what slows down this method. every time we populate an cbo or change its selectedindex we incur a 200ms penalty. no way to avoid this

        if (IsFirstTimeThrough)
        {
            await CboLookUpGenderSpecificationItemsVm.RefillItemsSourceAsync(new CboLookupItemDisplayObject[]
            {
                new() {Label = Symbols.SymbolMale},
                new() {Label = Symbols.SymbolFemale},
                new() {Label = Symbols.SymbolNonBinary}
            }); // slow - DangerouslyBriefSafetyMarginForBindingEngineMilliSec

            if (raceSpecificationItems is not null)
            {
                var races = raceSpecificationItems.Select(z => new CboLookupItemDisplayObject {Label = z.Label}).ToArray();

                await CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.RefillItemsSourceAsync(races.ToArray()); // slow - DangerouslyBriefSafetyMarginForBindingEngineMilliSec
                await CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.RefillItemsSourceAsync(races.ToArray()); // slow - DangerouslyBriefSafetyMarginForBindingEngineMilliSec
            }

            IsFirstTimeThrough = false;
        }

        await CboLookUpGenderSpecificationItemsVm.ChangeSelectedIndexToMatchItemLabelAsync(itemBeingModified.Gender); // will end up being -1 if no match can be obtained. // slow - DangerouslyBriefSafetyMarginForBindingEngineMilliSec

        await CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm
            .ChangeSelectedIndexToMatchItemLabelAsync(itemBeingModified.RaceGroupBeforeTransition); // will end up being -1 if no match can be obtained. // slow - DangerouslyBriefSafetyMarginForBindingEngineMilliSec

        await CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm
            .ChangeSelectedIndexToMatchItemLabelAsync(itemBeingModified.RaceGroupAfterTransition); // will end up being -1 if no match can be obtained. // slow - DangerouslyBriefSafetyMarginForBindingEngineMilliSec

        #endregion

        AsInitiallyPopulatedSemanticValue = CurrentSemanticValue();

        WasTouched = false;
        // the act of touching any of the above properties automatically causes answer.WasTouched to be
        // set to True (see AnyEditTemplateEntryChanged() and maybe their setters for those properties that are not vms). so we must change it back finally here below

        EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData();

        EvaluateVisibilityOfAllGuiControlsThatTouchData(true);

        IsAuthorisedToOperate = true;

        return true;
    }

        public ParticipantHubItem MergeEditsBackIntoItemBeingModified(ParticipantHubItem itemBeingModified, string touchedBy)
    {
        if (itemBeingModified is null)
            return null;

        // last line of defense. ideally this should be handled in the vm before now
        if (OneOrMoreEntriesAreInvalid(out var errorMessage))
            throw new JghAlertMessageException(errorMessage);

        #region Step 1. new up a pre-populated draft answer

        var answer = itemBeingModified.ToShallowMemberwiseClone();

        #endregion

        #region Step 2. insert editable fields into the "new" Modify item

        // non-editable fields in template - new plan is to deny Bib to be changed - if it was entered wrong then delete the item and re-enter it
        //if (string.IsNullOrWhiteSpace(Bib))
        //    answer.Bib = Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, System.Guid.NewGuid().ToString());
        //else
        //    answer.Bib = JghString.TmLr(JghString.CleanAndConvertToLetterOrDigitOrHyphen(JghString.TmLr(Bib)));

        // editable fields in template
        answer.Rfid = JghString.TmLr(Rfid);
        answer.FirstName = JghString.TmLr(FirstName);
        answer.MiddleInitial = JghString.TmLr(MiddleInitial);
        answer.LastName = JghString.TmLr(LastName);
        answer.Gender = CboLookUpGenderSpecificationItemsVm?.CurrentItem?.Label ?? string.Empty;
        JghConvert.TryConvertToInt32(BirthYear, out var yearAsInt, out _);
        answer.BirthYear = yearAsInt;

        answer.City = JghString.TmLr(City);
        answer.Team = JghString.TmLr(Team);
        answer.RaceGroupBeforeTransition = CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm?.CurrentItem?.Label ?? string.Empty;
        answer.RaceGroupAfterTransition = CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm?.CurrentItem?.Label ?? string.Empty;
        answer.DateOfRaceGroupTransition = DateOfRaceGroupTransitionCalendarPickerVm.Date?.Date ?? DateTime.Today;

        answer.IsSeries = IsSeries;
        answer.Series = JghString.TmLr(SeriesIdentifier);
        answer.EventIdentifiers = JghString.TmLr(EventIdentifiers);
        answer.MustDitchOriginatingItem = MustDitchOriginatingItem;
        answer.TouchedBy = string.IsNullOrWhiteSpace(touchedBy) ? "anonymous" : JghString.TmLr(touchedBy);

        answer.IsStillToBeBackedUp = true;
        answer.IsStillToBePushed = true;
        answer.DatabaseActionEnum = EnumStrings.DatabaseModify;
        answer.WhenTouchedBinaryFormat = DateTime.Now.ToBinary();

        answer.Guid = System.Guid.NewGuid().ToString();
        // NB Guid assigned here at moment of population by user (not in ctor). the ctor does not create the Guid fields. only in ParticipantHubItem.CreateItem() and ParticipantHubItemEditTemplateViewModel.MergeEditsBackIntoItemBeingModified()

        answer.Label = JghString.Concat(answer.Bib, answer.FirstName, answer.LastName);
        //answer.Label = JghString.Concat(answer.Bib, answer.FirstName, answer.LastName, answer.RaceGroupBeforeTransition);

        #endregion

        return answer;
    }

        public bool OneOrMoreEntriesAreInvalid(out string errorMessage)
    {
        if (MustDitchOriginatingItem)
        {
            // no checks required or desired because you are ridding yourself of the item. do nothing. exit.

            errorMessage = string.Empty;

            return false;
        }

        // NB> ensure this list identical to all the fields of ParticipantHubItem

        var sb = new StringBuilder();

        if (!JghString.IsOnlyLettersOrHyphenOrApostropheOrSpace(FirstName) || FirstName.Length < 2) sb.AppendLine("First name must be two or more letters. May include a hyphen.");
        if (!JghString.IsOnlyLetters(MiddleInitial) || MiddleInitial.Length > 1) sb.AppendLine("Middle initial must be a single letter or blank.");
        if (!JghString.IsOnlyLettersOrHyphenOrApostropheOrSpace(LastName) || LastName.Length < 2) sb.AppendLine("Last name must be two or more letters. May include a hyphen or apostrophe.");
        if (CboLookUpGenderSpecificationItemsVm.SelectedIndex == -1) sb.AppendLine("Gender must be specified.");
        if (CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.SelectedIndex == -1) sb.AppendLine("Race must be specified.");
        if (!JghString.IsOnlyLettersOrHyphenOrApostropheOrSpace(City)) sb.AppendLine("City name must be letters or blank. May include a hyphen or apostrophe.");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphenOrSpace(Team)) sb.AppendLine("Team name must be letters and/or digits or blank.");
        if (!JghString.IsOnlyDigits(BirthYear) || BirthYear.Length is < 4 or > 4) sb.AppendLine("Year of birth must be four digits.");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphenOrSpace(SeriesIdentifier)) sb.AppendLine("SeriesIdentifier identifier must be letters or digits or blank.");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphen(Bib)) sb.AppendLine("ID must consist of letters, digits, or hyphens (or be temporarily blank).");

        if (sb.Length <= 0)
        {
            errorMessage = string.Empty;
            return false;
        }

        errorMessage = sb.ToString();

        return true;
    }

        public bool AllEntriesAreUnchangedSinceInitiallyPopulated()
    {
        var before = AsInitiallyPopulatedSemanticValue;

        var after = CurrentSemanticValue();

        return before == after;
    }

        public override async Task<bool> ZeroiseAsync()
    {
        await base.ZeroiseAsync();

        Bib = string.Empty; // is this correct. or should we leave it untouched?
        Rfid = string.Empty; // is this correct. or should we leave it untouched?
        FirstName = string.Empty;
        MiddleInitial = string.Empty;
        LastName = string.Empty;
        BirthYear = string.Empty;
        City = string.Empty;
        Team = string.Empty;
        SeriesIdentifier = string.Empty;
        EventIdentifiers = string.Empty;
        IsSeries = true; // default

        SynchroniseIsAuthorisedToOperateValueOfConstituentControls();

        AsInitiallyPopulatedSemanticValue = CurrentSemanticValue();

        return true;
    }

        protected override void SynchroniseIsAuthorisedToOperateValueOfConstituentControls()
    {
        CopyHubItemButtonVm.IsAuthorisedToOperate = IsAuthorisedToOperate;
        PasteHubItemButtonVm.IsAuthorisedToOperate = IsAuthorisedToOperate;

        CboLookUpGenderSpecificationItemsVm.IsAuthorisedToOperate = IsAuthorisedToOperate;
        CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.IsAuthorisedToOperate = IsAuthorisedToOperate;
        CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.IsAuthorisedToOperate = IsAuthorisedToOperate;
    }

        #region helpers

        private string CurrentSemanticValue()
    {
        var answer = JghString.Concat(
            ValueOrDummy(JghString.TmLr(Bib)),
            ValueOrDummy(Rfid),
            ValueOrDummy(FirstName),
            ValueOrDummy(MiddleInitial),
            ValueOrDummy(LastName),
            ValueOrDummy(CboLookUpGenderSpecificationItemsVm?.CurrentItem?.Label),
            ValueOrDummy(BirthYear),
            ValueOrDummy(City),
            ValueOrDummy(Team),
            ValueOrDummy(SeriesIdentifier),
            IsSeries.ToString(),
            ValueOrDummy(EventIdentifiers),
            ValueOrDummy(CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm?.CurrentItem?.Label),
            ValueOrDummy(CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm?.CurrentItem?.Label),
            ValueOrDummy(DateOfRaceGroupTransitionCalendarPickerVm.Date?.Date.ToShortDateString()),
            MustDitchOriginatingItem.ToString()
        );
        //nb don't include the Guid fields. want to use the semantic comparison 
        //to discern between machine-generated "new" objects and user 'created' objects
        //and provide a comparison of only user-entered data, not machine generated IDs

        return answer;
    }

        private static string ValueOrDummy(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "dummy" : value;
    }

        #endregion

        #endregion

        #region Gui stuff

        public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
    {
        CopyHubItemButtonVm.IsAuthorisedToOperate = !OneOrMoreEntriesAreInvalid(out var dummy);
        PasteHubItemButtonVm.IsAuthorisedToOperate = false;

        CboLookUpGenderSpecificationItemsVm.IsAuthorisedToOperate = CboLookUpGenderSpecificationItemsVm.ItemsSource.Any();
        CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.IsAuthorisedToOperate = CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.ItemsSource.Any();
        CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.IsAuthorisedToOperate = CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.ItemsSource.Any();

        DateOfRaceGroupTransitionCalendarPickerVm.IsAuthorisedToOperate = CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.CurrentItem is not null
                                                                          && CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.CurrentItem is not null
                                                                          && CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.CurrentItem.Label != CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.CurrentItem.Label;

        IsRegradedRaceGroup = CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.CurrentItem is not null
                              && CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.CurrentItem is not null
                              && CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.CurrentItem.Label != CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.CurrentItem.Label;
    }

        protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
    {
        CopyHubItemButtonVm.IsVisible = makeVisible;
        PasteHubItemButtonVm.IsVisible = makeVisible;

        CboLookUpGenderSpecificationItemsVm.IsVisible = makeVisible;
        CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.IsVisible = makeVisible;
        CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.IsVisible = CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.CurrentItem is not null;

        DateOfRaceGroupTransitionCalendarPickerVm.IsVisible = CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.CurrentItem is not null
                                                              && CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.CurrentItem is not null
                                                              && CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm.CurrentItem.Label != CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm.CurrentItem.Label;
    }

        protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
    {
        var answer = new List<object>();

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CopyHubItemButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, PasteHubItemButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookUpGenderSpecificationItemsVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookUpRaceGroupSpecificationItemsForBeforeTransitionVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CboLookUpRaceGroupSpecificationItemsForAfterTransitionVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, DateOfRaceGroupTransitionCalendarPickerVm);

        return answer;
    }

        public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        return false;
    }

        #endregion
    }
}
