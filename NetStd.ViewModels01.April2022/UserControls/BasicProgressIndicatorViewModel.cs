using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Prism.July2018;

namespace NetStd.ViewModels01.April2022.UserControls
{
    public class BasicProgressIndicatorViewModel : BindableBase, IProgressIndicatorViewModel
    {
        #region ctor

        public BasicProgressIndicatorViewModel()
        {
            ProgressMessage = string.Empty;
            IsVisible = false;
            SpinnerIsVisible = false;
        }

        #endregion

        #region props

        string _backingstoreProgressMessage;
        public string ProgressMessage
        {
            get => _backingstoreProgressMessage;
            set => SetProperty(ref _backingstoreProgressMessage, value);
        }

        string _backingstoreProgressMessageOrWhiteOut;
        public string ProgressMessageOrWhiteOut
        {
            get => _backingstoreProgressMessageOrWhiteOut;
            set => SetProperty(ref _backingstoreProgressMessageOrWhiteOut, value);
        }

        private bool _backingstoreIsBusy;
        public bool IsBusy
        {
            get => _backingstoreIsBusy;
            set => SetProperty(ref _backingstoreIsBusy, value);
        }

        private bool _backingstoreIsVisible;
        public bool IsVisible
        {
            get => _backingstoreIsVisible;
            set => SetProperty(ref _backingstoreIsVisible, value);
        }

        private bool _backingstoreSpinnerIsVisible;
        public bool SpinnerIsVisible
        {
            get => _backingstoreSpinnerIsVisible;
            set => SetProperty(ref _backingstoreSpinnerIsVisible, value);
        }

        #endregion


        #region methods

        public void StartProgressRing()
        {
            SpinnerIsVisible = true;
        }

        public void StopProgressRing()
        {
            SpinnerIsVisible = false;
        }

        public void OpenProgressIndicator()
        {
            OpenProgressIndicator("Busy .... ");
        }

        public void OpenProgressIndicator(string descriptionOfWhatsHappening)
        {
            ProgressMessage = descriptionOfWhatsHappening;
            ProgressMessageOrWhiteOut = descriptionOfWhatsHappening;
            IsBusy = true;
            IsVisible = true;
            SpinnerIsVisible = true;
        }

        public void FreezeProgressIndicator()
        {
            // nothing to do here in this Mickey Mouse vm
        }

        public void CloseProgressIndicator()
        {
            ProgressMessage = string.Empty;
            ProgressMessageOrWhiteOut = string.Empty;
            IsBusy = false;
            IsVisible = false;
            SpinnerIsVisible = false;
        }

        #endregion


    }
}