using System;
using System.Threading.Tasks;
using NetStd.Interfaces01.July2018.HasProperty;
using NetStd.Prism.July2018;

namespace NetStd.ViewModels01.April2022.UserControls
{
    // Note. i have given this vm command-like functionality. this powerful feature is non-trivial. 
    // It means we can use Blend behaviours core:InvokeCommandAction Command = "{Binding OnTextChangedCommand}" to equal the deeply robust functionality pioneered in my ItemsControlViewModel, so look for examples of that in use for the proper way to handle TextChanged events
    public class TextBoxControlViewModel : BindableBase, IHasIsAuthorisedToOperate, IHasIsVisible
    {
        #region ctor

        public TextBoxControlViewModel(Action onTextChangedExecuteAction,
            Func<bool> onTextChangedCanExecuteFunc)
        {
            if (onTextChangedExecuteAction is null)
                throw new ArgumentNullException(nameof(onTextChangedExecuteAction));

            if (onTextChangedCanExecuteFunc is null)
                throw new ArgumentNullException(nameof(onTextChangedCanExecuteFunc));

            _defaultOnTextChangedCommand =
                new DelegateCommand(
                    onTextChangedExecuteAction,
                    onTextChangedCanExecuteFunc);


            OnTextChangedCommand = _defaultOnTextChangedCommand;

            _defaultCommandThatDoesNothing =
                new DelegateCommand(
                    DummyCommandExecuteActionThatDoesNothing,
                    DummyCommandCanExecuteFuncAlwaysFalse);
        }

        #endregion

        #region constants

        /* on a Dell xps15 a delay of 200 ms has done the job over the years but i have determined empirically
         * that this is too short for even fast new phones. 
         */

        private const int FactorySettingForSafeDelayForBindingEngineMilliSec = 200; // the default

        #endregion

        #region fields

        private readonly DelegateCommand _defaultCommandThatDoesNothing;

        private readonly DelegateCommand _defaultOnTextChangedCommand;

        private bool _capturedIsAuthorisedToOperateValue;

        #endregion

        #region props

        #region Text

        private string _backingstoreText;

        public string Text
        {
            get => _backingstoreText ??= string.Empty;
            set => SetProperty(ref _backingstoreText, value);
        }

        #endregion

        #region Label

        private string _backingstoreLabel;

        public string Label
        {
            get => _backingstoreLabel ??= string.Empty;
            set => SetProperty(ref _backingstoreLabel, value);
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

        // this functionality is a carbon copy of a Button style Command with its intrinsic RaiseCanExecuteChanged() 
        // functionality, except that this is not a button, hence the copying out of inapplicable RaiseCanExecuteChanged() stuff

        private bool _backingstoreIsAuthorisedToOperate;

        public bool IsAuthorisedToOperate
        {
            get => _backingstoreIsAuthorisedToOperate;
            set => SetProperty(ref _backingstoreIsAuthorisedToOperate, value);
        }

        #endregion

        #endregion

        #region OnTextChangedCommand

        private DelegateCommand _backingstoreOnTextChangedCommand;

        public DelegateCommand OnTextChangedCommand
        {
            get => _backingstoreOnTextChangedCommand;
            set => SetProperty(ref _backingstoreOnTextChangedCommand, value);
        }

        #endregion

        #region methods

        public bool Zeroise()
        {
            Label = string.Empty;

            IsAuthorisedToOperate = false;

            IsVisible = true;

            OnTextChangedCommand = _defaultOnTextChangedCommand;

            return true;
        }

        public async Task<bool> ChangeTextAsync(string text)
        {
            ExtinguishOnTextChangedCommandAction();

            Text = text ?? string.Empty;

            await Task.Delay(FactorySettingForSafeDelayForBindingEngineMilliSec);
            // remove this at your utmost peril. Prism trick.  must provide enough time for the change to loop back through the binding, trigger the text changed event and be smothered before continuing here

            RestoreInstantiatedOnTextChangedCommandAction();

            return await Task.FromResult(true);
        }

        public void CaptureIsAuthorisedToOperateValue()
        {
            _capturedIsAuthorisedToOperateValue = _backingstoreIsAuthorisedToOperate;
        }

        public void RestoreCapturedIsAuthorisedToOperateValue()
        {
            IsAuthorisedToOperate = _capturedIsAuthorisedToOperateValue;
        }

        public override string ToString()
        {
            return Text;
        }


        #endregion

        #region helpers

        private void ExtinguishOnTextChangedCommandAction()
        {
            OnTextChangedCommand = _defaultCommandThatDoesNothing;
        }

        private void RestoreInstantiatedOnTextChangedCommandAction()
        {
            OnTextChangedCommand = _defaultOnTextChangedCommand;
        }

        private static void DummyCommandExecuteActionThatDoesNothing()
        {
            // do nothing
        }

        private static bool DummyCommandCanExecuteFuncAlwaysFalse()
        {
            return false;
        }

        #endregion
    }
}