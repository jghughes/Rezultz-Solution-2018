using System;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;
// ReSharper disable UnusedMember.Local

//CalendarDatePicker
namespace NetStd.ViewModels01.April2022.UserControls
{
    /* Important note: omission of an IsEnabled property is skillful and powerful.
     * Bind the XAML IsEnabled to IsAuthorisedToOperate whenever you need to do so explicitly.
     * An explicit bind is superfluous - for functionally stand-alone button and a stand-alone button vm.
     * This is because of the cunning wiring in this vm in association with the way the
     * UWP XAML button ICommand property works. The way the command works with ButtonBase in UWP Xaml is that the
     * binding to a command automatically takes care of graying-out the button whenever the button is caused to execute
     * a ICommand.CanExecute() that returns false. Because of the genius wiring in this prop, whenever
     * the IsAuthorisedToOperate prop changes (in a viewmodel for example), OnClickCommand.RaiseCanExecuteChanged()
     * is fired and then canExecuteFunc gets executed and ICommand.CanExecute() fires and the outcome of
     * that is reacted to by the XAML button automatically.
     *
     * HOWEVER, often you definitely want an explicit bind because your command might be non-existent or null or your command methods
     * might be intentionally empty because they are not used and because your logic of a button is not handled by the
     * Command, but by a click-event in the code-behind. This is a common occurrence.
     *
     * SIMILARLY, I have determined empirically that you also need an explicit bind where you simplify
     * your vm by binding multiple xaml buttons to a shared button vm. For some reason the evaluation of
     * ICommand.CanExecute() logic breaks down and doesn't work. You have no option but to explicitly
     * bind each xaml button that shares the button vm. This situation is common in the portal.
    */


    public class ButtonControlViewModel : BindableBase, IHasIsAuthorisedToOperate, IHasIsVisible
    {
        #region ctor

        public ButtonControlViewModel(Action executeAction, Func<bool> canExecuteFunc)
        {

            executeAction ??= ExecuteNothing;

            canExecuteFunc ??= CannotExecute;

            _asInstantiatedDelegateCommand =
                new DelegateCommand(executeAction, canExecuteFunc);

            Content = string.Empty;

            OnClickCommand = _asInstantiatedDelegateCommand;

            _asInstantiatedDelegateCommandGeneric =
                new DelegateCommand<string>(ExecuteNothingGeneric, CannotExecuteGeneric);

            OnClickCommandWithParameter = _asInstantiatedDelegateCommandGeneric;

        }

        // use this ctor for accommodating a command pararmeter, in this example a string 
        public ButtonControlViewModel(Action<string> executeAction, Func<string, bool> canExecuteFunc)
        {

            executeAction ??= ExecuteNothingGeneric;

            canExecuteFunc ??= CannotExecuteGeneric;

            _asInstantiatedDelegateCommandGeneric =
                new DelegateCommand<string>(executeAction, canExecuteFunc);

            Content = string.Empty;

            OnClickCommandWithParameter = _asInstantiatedDelegateCommandGeneric;

            _asInstantiatedDelegateCommand =
                new DelegateCommand(ExecuteNothing, CannotExecute);
            OnClickCommand = _asInstantiatedDelegateCommand;
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

        //[DataMember]
        public string Label
        {
            get => _backingstoreLabel ??= string.Empty;
            set => SetProperty(ref _backingstoreLabel, value);
        }

        #endregion

        #region Content

        private object _backingstoreContent;

        public object Content
        {
            get => _backingstoreContent ??= new object();
            set => SetProperty(ref _backingstoreContent, value);
        }

        #endregion

        #region IsChecked

        private bool _backingstoreIsChecked;

        public bool IsChecked
        {
            get => _backingstoreIsChecked;
            set => SetProperty(ref _backingstoreIsChecked, value);
        }

        #endregion

        #region IsVisible

        private bool _backingstoreIsVisible;

        //[DataMember]
        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        #endregion

        #region Tag

        private string _backingstoreTag;

        //[DataMember]
        public string Tag
        {
            get => _backingstoreTag ??= string.Empty;
            set => SetProperty(ref _backingstoreTag, value);
        }

        #endregion

        #region IsAuthorisedToOperate - fires OnClickCommand.RaiseCanExecuteChanged()


        private bool _backingstoreIsAuthorisedToOperate;

        public bool IsAuthorisedToOperate
        {
            get => _backingstoreIsAuthorisedToOperate;
            set
            {
                var oldValue = _backingstoreIsAuthorisedToOperate;

                SetProperty(ref _backingstoreIsAuthorisedToOperate, value);

                var newValue = _backingstoreIsAuthorisedToOperate;

                if (oldValue != newValue)
                    OnClickCommand.RaiseCanExecuteChanged(); // genius
            }
        }

        private bool _capturedIsAuthorisedToOperateValue;

        #endregion

        #region OnClickCommand

        // N.B. private _backingstore with a private setter. 
        // slightly risky practice to allow this to be protected as opposed to private. intent is that it is allowed to be 
        // assigned in the ctor and nowhere else. Internally it can be swapped in and out as a consequence 
        // of ExtinguishOnClickCommand(), and RestoreOnClickCommand() 
        // - both of which are private. this is a clever defensive approach

        private DelegateCommand _backingstoreOnClickCommand;

        public DelegateCommand OnClickCommand
        {
            get => _backingstoreOnClickCommand;
            private set => SetProperty(ref _backingstoreOnClickCommand, value);
        }

        #endregion

        #region OnClickCommandWithParameter

        // N.B. private _backingstore with a private setter. 
        // slightly risky practice to allow this to be protected as opposed to private. intent is that it is allowed to be 
        // assigned in the ctor and nowhere else. Internally it can be swapped in and out as a consequence 
        // of ExtinguishOnClickCommandGeneric(), and RestoreOnClickCommandGeneric() 
        // - both of which are private. this is a clever defensive approach

        private DelegateCommand<string> _backingstoreOnClickCommandWithParameter;

        public DelegateCommand<string> OnClickCommandWithParameter
        {
            get => _backingstoreOnClickCommandWithParameter;
            private set => SetProperty(ref _backingstoreOnClickCommandWithParameter, value);
        }

        #endregion

        #endregion

        #region methods

        public bool Zeroise()
        {

            Label = string.Empty;
            Content = string.Empty;
            IsChecked = false;
            IsVisible = false;
            Tag = string.Empty;
            IsAuthorisedToOperate = false;
            OnClickCommand = _asInstantiatedDelegateCommand;
            OnClickCommandWithParameter = _asInstantiatedDelegateCommandGeneric;

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
        private void ExtinguishOnClickCommand()
        {
            OnClickCommand = _delegateCommandThatDoesNothing;
            OnClickCommandWithParameter = _delegateCommandThatDoesNothingGeneric;
        }

        // unused for now. paired with the above. see ItemsControlViewModel for a brilliant example 
        private void RestoreOnClickCommand()
        {
            OnClickCommand = _asInstantiatedDelegateCommand;
            OnClickCommandWithParameter = _asInstantiatedDelegateCommandGeneric;
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