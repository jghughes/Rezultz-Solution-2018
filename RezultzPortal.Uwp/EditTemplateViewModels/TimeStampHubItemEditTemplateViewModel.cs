using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Jgh.SymbolsStringsConstants.Mar2022;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.ViewModels01.April2022.CollectionBases;
using Rezultz.DataTypes.Nov2023.PortalHubItems;

// ReSharper disable InconsistentNaming

namespace RezultzPortal.Uwp.EditTemplateViewModels;

public class TimeStampHubItemEditTemplateViewModel : HubItemEditTemplateViewModelBase, IHasZeroiseAsync
{
    #region fields

    private ClipBoardForTimeStamp _myClipBoardForTimeStamp;

    #endregion

    #region ctor

    public TimeStampHubItemEditTemplateViewModel()
    {
        _myClipBoardForTimeStamp = new ClipBoardForTimeStamp();

        TimeStampRadioButtonsVm = new ItemDrivenCollectionViewModelForRadioButtonsBase(string.Empty, TimeStampRadioButtonsVmOnSelectionChangedExecute, TimeStampRadioButtonsVmOnSelectionChangedCanExecute)
        {
            IsVisible = true,
            IsAuthorisedToOperate = true,
            ItemsSource = {LabelNone, LabelDnf, LabelTbd}
        };
    }

    #endregion

    #region local type

    private class ClipBoardForTimeStamp
    {
        public int SelectedDay;
        public int SelectedHour;
        public int SelectedMinute;
        public int SelectedMonth;
        public int SelectedSecond;
        public int SelectedSecondTenth;
        public int SelectedYear;
    }

    #endregion

    #region strings

    private const string LabelNone = "Normal";
    private const string LabelDnf = "Did not finish";
    private const string LabelTbd = "To be decided";

    #endregion

    #region props

    #region EditTemplate entries - all setters invoke AnyEditTemplateEntryChanged

    private int _backingstoreSelectedYear;

    public int SelectedYear
    {
        get => _backingstoreSelectedYear;
        set => SetProperty(ref _backingstoreSelectedYear, value, AnyEditTemplateEntryChanged);
    }

    private int _backingstoreSelectedMonth;

    public int SelectedMonth
    {
        get => _backingstoreSelectedMonth;
        set => SetProperty(ref _backingstoreSelectedMonth, value, AnyEditTemplateEntryChanged);
    }

    private int _backingstoreSelectedDay;

    public int SelectedDay
    {
        get => _backingstoreSelectedDay;
        set => SetProperty(ref _backingstoreSelectedDay, value, AnyEditTemplateEntryChanged);
    }

    private int _backingstoreSelectedHour;

    public int SelectedHour
    {
        get => _backingstoreSelectedHour;
        set => SetProperty(ref _backingstoreSelectedHour, value, AnyEditTemplateEntryChanged);
    }

    private int _backingstoreSelectedMinute;

    public int SelectedMinute
    {
        get => _backingstoreSelectedMinute;
        set => SetProperty(ref _backingstoreSelectedMinute, value, AnyEditTemplateEntryChanged);
    }

    private int _backingstoreSelectedSecond;

    public int SelectedSecond
    {
        get => _backingstoreSelectedSecond;
        set => SetProperty(ref _backingstoreSelectedSecond, value, AnyEditTemplateEntryChanged);
    }

    private int _backingstoreSelectedSecondTenth;

    public int SelectedSecondTenth
    {
        get => _backingstoreSelectedSecondTenth;
        set => SetProperty(ref _backingstoreSelectedSecondTenth, value, AnyEditTemplateEntryChanged);
    }


    public ItemDrivenCollectionViewModelForRadioButtonsBase TimeStampRadioButtonsVm { get; }

    #endregion

    #region sui generis IsEditable

    #region IsIdentifierEditable

    private bool _backingstoreIsIdentifierEditable;

    public bool IsIdentifierEditable
    {
        get => _backingstoreIsIdentifierEditable;
        set => SetProperty(ref _backingstoreIsIdentifierEditable, value);
    }

    #endregion

    #region IsDnxEditable

    private bool _backingstoreIsDnxEditable;

    public bool IsDnxEditable
    {
        get => _backingstoreIsDnxEditable;
        set => SetProperty(ref _backingstoreIsDnxEditable, value);
    }

    #endregion

    #endregion

    #endregion

    #region commands

    #region CopyHubItemButtonOnClickAsync

    protected override async Task<string> CopyHubItemButtonOnClickAsync()
    {
        if (OneOrMoreEntriesAreInvalid(out var errorReason))
            return await Task.FromResult(errorReason);

        //_myClipBoardForTimeStamp ??= new ClipBoardForTimeStamp();
        _myClipBoardForTimeStamp = new ClipBoardForTimeStamp
        {
            SelectedYear = SelectedYear,
            SelectedMonth = SelectedMonth,
            SelectedDay = SelectedDay,
            SelectedHour = SelectedHour,
            SelectedMinute = SelectedMinute,
            SelectedSecond = SelectedSecond,
            SelectedSecondTenth = SelectedSecondTenth
        };

        return await Task.FromResult(Copied);
    }

    #endregion

    #region PasteHubItemButtonOnClick

    protected override async Task<string> PasteHubItemButtonOnClickAsync()
    {
        _myClipBoardForTimeStamp ??= new ClipBoardForTimeStamp();

        SelectedYear = _myClipBoardForTimeStamp.SelectedYear;
        SelectedMonth = _myClipBoardForTimeStamp.SelectedMonth;
        SelectedDay = _myClipBoardForTimeStamp.SelectedDay;
        SelectedHour = _myClipBoardForTimeStamp.SelectedHour;
        SelectedMinute = _myClipBoardForTimeStamp.SelectedMinute;
        SelectedSecond = _myClipBoardForTimeStamp.SelectedSecond;
        SelectedSecondTenth = _myClipBoardForTimeStamp.SelectedSecondTenth;

        return await Task.FromResult(string.Empty);
    }

    #endregion

    #region TimeStampRadioButtonsVmOnSelectionChanged - mickey mouse

    protected bool TimeStampRadioButtonsVmOnSelectionChangedCanExecute()
    {
        return TimeStampRadioButtonsVm.IsAuthorisedToOperate;
    }

    private void TimeStampRadioButtonsVmOnSelectionChangedExecute()
    {
        AnyEditTemplateEntryChanged();

        EvaluateGui();
    }

    #endregion

    #endregion

    #region methods

    public async Task<bool> PopulateWithItemBeingModifiedAsync(TimeStampHubItem itemBeingModified)
    {
        if (itemBeingModified == null)
            return true;

        _kindOfTimeStampEnum = itemBeingModified.RecordingModeEnum;
        KindOfTimeStampEnumText = _kindOfTimeStampEnum;

        Bib = JghString.TmLr(itemBeingModified.Bib);
        Rfid = JghString.TmLr(itemBeingModified.Rfid);
        SelectedYear = DateTime.FromBinary(itemBeingModified.TimeStampBinaryFormat).ToLocalTime().Year;
        SelectedMonth = DateTime.FromBinary(itemBeingModified.TimeStampBinaryFormat).ToLocalTime().Month;
        SelectedDay = DateTime.FromBinary(itemBeingModified.TimeStampBinaryFormat).ToLocalTime().Day;
        SelectedHour = DateTime.FromBinary(itemBeingModified.TimeStampBinaryFormat).ToLocalTime().Hour;
        SelectedMinute = DateTime.FromBinary(itemBeingModified.TimeStampBinaryFormat).Minute;
        SelectedSecond = DateTime.FromBinary(itemBeingModified.TimeStampBinaryFormat).Second;
        SelectedSecondTenth = DateTime.FromBinary(itemBeingModified.TimeStampBinaryFormat).Millisecond / 100;
        MustDitchOriginatingItem = itemBeingModified.MustDitchOriginatingItem;
        WasTouched = false;

        //IsDnf = !string.IsNullOrWhiteSpace(itemBeingModified.DnxSymbol);


        if (string.IsNullOrWhiteSpace(itemBeingModified.DnxSymbol))
            await TimeStampRadioButtonsVm.ChangeSelectedItemAsync(LabelNone);
        else if (itemBeingModified.DnxSymbol == Symbols.SymbolDnf)
            await TimeStampRadioButtonsVm.ChangeSelectedItemAsync(LabelDnf);
        else if (itemBeingModified.DnxSymbol == Symbols.SymbolTbd) await TimeStampRadioButtonsVm.ChangeSelectedItemAsync(LabelTbd);


        // the act of setting all these properties automatically causes answer.WasTouched to be
        // set to True, so we must change it back at the end

        AsInitiallyPopulatedSemanticValue = CurrentSemanticValue();

        return true;
    }

    public TimeStampHubItem MergeEditsBackIntoItemBeingModified(TimeStampHubItem itemBeingModified, string touchedBy)
    {
        if (itemBeingModified == null)
            return null;

        // last line of defense. ideally this should be handled in the vm before now
        if (OneOrMoreEntriesAreInvalid(out var errorMessage))
            throw new JghAlertMessageException(errorMessage);

        #region Step 1. new up a pre-populated draft answer

        var answer = itemBeingModified.ToShallowMemberwiseClone(); //

        #endregion

        #region Step 2. insert editable fields into the "new" Modify item

        // editable fields in template

        if (string.IsNullOrWhiteSpace(Bib))
            answer.Bib = JghString.TmLr(Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, System.Guid.NewGuid().ToString()));
        else
            answer.Bib = JghString.TmLr(JghString.CleanAndConvertToLetterOrDigitOrHyphen(Bib));

        if (string.IsNullOrWhiteSpace(Rfid))
            answer.Rfid = JghString.TmLr(Symbols.SymbolUnspecified + "-" + JghString.Substring(0, 3, System.Guid.NewGuid().ToString()));
        else
            answer.Rfid = Rfid;

        answer.TimeStampBinaryFormat = GetBinaryTimeStamp();

        //answer.DnxSymbol = IsDnf ? Symbols.SymbolDnf : string.Empty;
        answer.MustDitchOriginatingItem = MustDitchOriginatingItem;

        answer.DnxSymbol = TimeStampRadioButtonsVm.SelectedItem switch
        {
            LabelNone => string.Empty,
            LabelDnf => Symbols.SymbolDnf,
            LabelTbd => Symbols.SymbolTbd,
            _ => string.Empty
        };

        // parameter 

        answer.TouchedBy = string.IsNullOrWhiteSpace(touchedBy) ? "anonymous" : JghString.TmLr(touchedBy);

        // auto generated

        answer.IsStillToBeBackedUp = true;
        answer.IsStillToBePushed = true;
        answer.DatabaseActionEnum = EnumStrings.DatabaseModify;
        answer.WhenTouchedBinaryFormat = DateTime.Now.ToBinary();
        answer.Guid = System.Guid.NewGuid().ToString();
        // NB Guid assigned here at moment of population by user (not in ctor). the ctor does not create the Guid fields. only in TimeStampHubItem.CreateItem() and TimeStampHubItemEditTemplateViewModel.MergeEditsBackIntoItemBeingModified()

        answer.Label = JghString.Concat(answer.Bib, answer.RecordingModeEnum, answer.DatabaseActionEnum);

        #endregion

        return answer;
    }

    public bool OneOrMoreEntriesAreInvalid(out string errorMessage)
    {
        var sb = new StringBuilder();

        if (SelectedYear < 0) sb.AppendLine("Years cannot be negative.");
        if (SelectedMonth is > 12 or < 1) sb.AppendLine("Months must be 1 to 12.");
        if (SelectedDay is > 31 or < 1) sb.AppendLine("Days must be 1 to 31 max.");
        if (SelectedHour is > 23 or < 0) sb.AppendLine("Hours must be 0 to 23.");
        if (SelectedMinute is > 59 or < 0) sb.AppendLine("Minutes must be 0 to 59.");
        if (SelectedSecond is > 59 or < 0) sb.AppendLine("Seconds must be 0 to 59.");
        if (SelectedSecondTenth is > 9 or < 0) sb.AppendLine("Tenths must be 0 to 9.");
        if (SelectedSecondTenth is > 9 or < 0) sb.AppendLine("Tenths must be 0 to 9.");
        if (!JghString.IsOnlyLettersOrDigitsOrHyphen(Bib))
            sb.AppendLine("ID must consist of letters, digits, or hyphens (or be temporarily blank).");

        if (sb.Length <= 0)
            try
            {
                // ReSharper disable once UnusedVariable
                var dummmy = new DateTime(SelectedYear, SelectedMonth, SelectedDay, SelectedHour, SelectedMinute,
                    SelectedSecond, SelectedSecondTenth);
            }
            catch (Exception e)
            {
                sb.AppendLine("The combination of date/time particulars is invalid.");
                sb.AppendLine();
                sb.AppendLine(e.Message);
            }

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


    public bool IsEmpty()
    {
        return SelectedYear == 0 &&
               SelectedMonth == 0 &&
               SelectedDay == 0 &&
               SelectedHour == 0 &&
               SelectedMinute == 0 &&
               SelectedSecond == 0 &&
               SelectedSecondTenth == 0;
    }

    public override async Task<bool> ZeroiseAsync()
    {
        await base.ZeroiseAsync();

        _kindOfTimeStampEnum = string.Empty;

        KindOfTimeStampEnumText = string.Empty;

        Bib = string.Empty;

        SelectedYear = 0;
        SelectedMonth = 0;
        SelectedDay = 0;
        SelectedHour = 0;
        SelectedMinute = 0;
        SelectedSecond = 0;
        SelectedSecondTenth = 0;
        await TimeStampRadioButtonsVm.ChangeSelectedItemAsync(LabelNone);
        MustDitchOriginatingItem = false;

        SynchroniseIsAuthorisedToOperateValueOfConstituentControls();

        AsInitiallyPopulatedSemanticValue = CurrentSemanticValue();

        return true;
    }

    protected override void SynchroniseIsAuthorisedToOperateValueOfConstituentControls()
    {
        // see setter of editTemplateBase.IsAuthorisedToOperate in base class. thus we keep IsAuthorisedToOperate in sync

        CopyHubItemButtonVm.IsAuthorisedToOperate = IsAuthorisedToOperate;
        PasteHubItemButtonVm.IsAuthorisedToOperate = IsAuthorisedToOperate;

        IsIdentifierEditable = IsAuthorisedToOperate
                               && _kindOfTimeStampEnum != EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody
                               && _kindOfTimeStampEnum != EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup;

        IsDnxEditable = IsAuthorisedToOperate
                        && _kindOfTimeStampEnum != EnumStrings.KindOfEntryIsTimeStampForGunStartForEverybody
                        && _kindOfTimeStampEnum != EnumStrings.KindOfEntryIsTimeStampForGunStartForGroup;

        TimeStampRadioButtonsVm.IsAuthorisedToOperate = IsDnxEditable;
    }

    #endregion

    #region helpers


    public long GetBinaryTimeStamp()
    {
        var answer = new DateTime(SelectedYear, SelectedMonth, SelectedDay, SelectedHour, SelectedMinute, SelectedSecond, SelectedSecondTenth * 100, DateTimeKind.Local).ToBinary();

        return answer;
    }

    private string CurrentSemanticValue()
    {
        var answer = JghString.Concat(
            ValueOrDummy(JghString.TmLr(Bib)),
            SelectedYear.ToString(),
            SelectedMonth.ToString(),
            SelectedDay.ToString(),
            SelectedHour.ToString(),
            SelectedMinute.ToString(),
            SelectedSecond.ToString(),
            SelectedSecondTenth.ToString(),
            TimeStampRadioButtonsVm.SelectedItem,
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

    #region Gui stuff

    public override void EvaluateIsAuthorisedToOperateValueOfAllGuiControlsThatTouchData()
    {
        CopyHubItemButtonVm.IsAuthorisedToOperate = !OneOrMoreEntriesAreInvalid(out var dummy);
        PasteHubItemButtonVm.IsAuthorisedToOperate = false;

        TimeStampRadioButtonsVm.IsAuthorisedToOperate = true;
    }

    protected override void EvaluateVisibilityOfAllGuiControlsThatTouchData(bool makeVisible)
    {
        CopyHubItemButtonVm.IsVisible = makeVisible;
        PasteHubItemButtonVm.IsVisible = makeVisible;
    }

    protected override List<object> MakeListOfAllObjectsSatisfyingIHasIsAuthorisedToOperate()
    {
        var answer = new List<object>();

        AddToCollectionIfIHasIsAuthorisedToOperate(answer, CopyHubItemButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, PasteHubItemButtonVm);
        AddToCollectionIfIHasIsAuthorisedToOperate(answer, TimeStampRadioButtonsVm);

        return answer;
    }

    public override bool LastKnownGoodGenesisOfThisViewModelHasChanged()
    {
        return false;
    }

    #endregion
}
