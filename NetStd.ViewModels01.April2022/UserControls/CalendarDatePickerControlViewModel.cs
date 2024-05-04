using System;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class CalendarDatePickerControlViewModel : BindableBase, IHasIsAuthorisedToOperate, IHasIsVisible
    {
        #region ctor

        public CalendarDatePickerControlViewModel(Action onDateChangedExecuteAction, Func<bool> canExecuteFunc)
        {

            onDateChangedExecuteAction ??= ExecuteNothing;

            canExecuteFunc ??= CannotExecute;

            _asInstantiatedDelegateCommand =
                new DelegateCommand(onDateChangedExecuteAction, canExecuteFunc);

            Date = null;

            OnDateChangedCommand = _asInstantiatedDelegateCommand;

            _asInstantiatedDelegateCommandGeneric =
                new DelegateCommand<string>(ExecuteNothingGeneric, CannotExecuteGeneric);

            OnDateChangedCommandWithParameter = _asInstantiatedDelegateCommandGeneric;

        }

        // use this ctor for accommodating a command parameter, in this example a string 
        public CalendarDatePickerControlViewModel(Action<string> onDateChangedExecuteAction, Func<string, bool> canExecuteFunc)
        {

            onDateChangedExecuteAction ??= ExecuteNothingGeneric;

            canExecuteFunc ??= CannotExecuteGeneric;

            _asInstantiatedDelegateCommandGeneric =
                new DelegateCommand<string>(onDateChangedExecuteAction, canExecuteFunc);

            Date = null;

            OnDateChangedCommandWithParameter = _asInstantiatedDelegateCommandGeneric;

            _asInstantiatedDelegateCommand =
                new DelegateCommand(ExecuteNothing, CannotExecute);
            OnDateChangedCommand = _asInstantiatedDelegateCommand;
        }

        #endregion

        #region fields

        private readonly DelegateCommand _asInstantiatedDelegateCommand;
        private readonly DelegateCommand _delegateCommandThatDoesNothing = new(ExecuteNothing, CannotExecute);

        private readonly DelegateCommand<string> _asInstantiatedDelegateCommandGeneric;
        private readonly DelegateCommand<string> _delegateCommandThatDoesNothingGeneric = new(ExecuteNothingGeneric, CannotExecuteGeneric);

        #endregion

        #region props

        #region Label

        private string _backingstoreLabel;

        public string Label
        {
            get => _backingstoreLabel ??= string.Empty;
            set => SetProperty(ref _backingstoreLabel, value);
        }

        #endregion

        #region Date

        private DateTimeOffset? _backingstoreDate;

        public DateTimeOffset? Date
        {
            get => _backingstoreDate;
            set => SetProperty(ref _backingstoreDate, value);
        }

        #endregion

        #region IsCalendarOpen

        private bool _backingstoreIsCalendarOpen;

        public bool IsCalendarOpen
        {
            get => _backingstoreIsCalendarOpen;
            set => SetProperty(ref _backingstoreIsCalendarOpen, value);
        }

        #endregion

        #region IsVisible

        private bool _backingstoreIsVisible;

        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        #endregion

        #region IsAuthorisedToOperate

        private bool _backingstoreIsAuthorisedToOperate;

        public bool IsAuthorisedToOperate
        {
            get => _backingstoreIsAuthorisedToOperate;
            set => SetProperty(ref _backingstoreIsAuthorisedToOperate, value);
        }

        private bool _capturedIsAuthorisedToOperateValue;

        #endregion

        #region OnDateChangedCommand

        // N.B. private _backingstore with a private setter. 
        // slightly risky practice to allow this to be protected as opposed to private. intent is that it is allowed to be 
        // assigned in the ctor and nowhere else. Internally it can be swapped in and out as a consequence 
        // of ExtinguishOnClickCommand(), and RestoreOnClickCommand() 
        // - both of which are private. this is a clever defensive approach

        private DelegateCommand _backingstoreOnDateChangedCommand;

        public DelegateCommand OnDateChangedCommand
        {
            get => _backingstoreOnDateChangedCommand;
            private set => SetProperty(ref _backingstoreOnDateChangedCommand, value);
        }

        #endregion

        #region OnDateChangedCommandWithParameter

        // N.B. private _backingstore with a private setter. 
        // slightly risky practice to allow this to be protected as opposed to private. intent is that it is allowed to be 
        // assigned in the ctor and nowhere else. Internally it can be swapped in and out as a consequence 
        // of ExtinguishOnClickCommandGeneric(), and RestoreOnClickCommandGeneric() 
        // - both of which are private. this is a clever defensive approach

        private DelegateCommand<string> _backingstoreOnDateChangedCommandWithParameter;

        public DelegateCommand<string> OnDateChangedCommandWithParameter
        {
            get => _backingstoreOnDateChangedCommandWithParameter;
            private set => SetProperty(ref _backingstoreOnDateChangedCommandWithParameter, value);
        }

        #endregion

        #endregion

        #region methods

        public bool Zeroise()
        {

            Label = string.Empty;
            Date = null;
            IsCalendarOpen = false;
            IsVisible = false;
            IsAuthorisedToOperate = false;
            OnDateChangedCommand = _asInstantiatedDelegateCommand;
            OnDateChangedCommandWithParameter = _asInstantiatedDelegateCommandGeneric;

            return true;
        }

        public void CaptureIsAuthorisedToOperateValue()
        {
            _capturedIsAuthorisedToOperateValue = _backingstoreIsAuthorisedToOperate;
        }

        public void RestoreCapturedIsAuthorisedToOperateValue()
        {
            IsAuthorisedToOperate = _capturedIsAuthorisedToOperateValue;
        }

        // unused for now. placeholder for the future, for whenever this class evolves to become self-referential 
        // ReSharper disable once UnusedMember.Local
        private void ExtinguishOnClickCommand()
        {
            OnDateChangedCommand = _delegateCommandThatDoesNothing;
            OnDateChangedCommandWithParameter = _delegateCommandThatDoesNothingGeneric;
        }

        // unused for now. paired with the above. see ItemsControlViewModel for a brilliant example 
        // ReSharper disable once UnusedMember.Local
        private void RestoreOnClickCommand()
        {
            OnDateChangedCommand = _asInstantiatedDelegateCommand;
            OnDateChangedCommandWithParameter = _asInstantiatedDelegateCommandGeneric;
        }

        #endregion

        #region prism delegate command helpers

        private static void ExecuteNothing()
        {
            // do nothing
        }

        private static void ExecuteNothingGeneric(string dummyCommandParameter)
        {
            // do nothing
        }

        private static bool CannotExecute()
        {
            return false;
        }

        private static bool CannotExecuteGeneric(string dummyCommandParameter)
        {
            return false;
        }

        #endregion
    }
}