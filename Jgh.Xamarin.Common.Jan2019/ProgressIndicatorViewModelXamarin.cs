using System;
using NetStd.Goodies.Mar2022;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.Prism.July2018;
using Xamarin.Forms;

namespace Jgh.Xamarin.Common.Jan2019
{
    /// <summary>
	/// This version of the vm relies on Xamarin.Forms, meaning that all classes and vms that use it can only run compiled
	/// for a UI environment supported by Xamarin.Forms.
	/// They cant run in a Console app for example, let alone a server. 
	/// </summary>
	public class ProgressIndicatorViewModelXamarin : BindableBase, IProgressIndicatorViewModel, IProgressIndicatorService
    {
        #region ctor

        public ProgressIndicatorViewModelXamarin()
        {

            ProgressMessage = string.Empty;
            IsVisible = false;
            SpinnerIsVisible = false;

            _dateTimeUponStart = DateTime.Now;
            _progressIndicatorIsRunning = false;

            _descriptionOfWhatsHappening = string.Empty;
            _candidateFrozenProgessMessage = string.Empty;

            var targetTicks = (Math.Truncate(DateTime.Now.Ticks * 10E9) + 1) / 10E9;

            while (targetTicks > DateTime.Now.Ticks)
            {
                //wait until we reach a round number of seconds
            }

            Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
            {
                Device.BeginInvokeOnMainThread(OnTimerGeneratedEvent);
                return true;
            });
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

        #region fields

        private DateTime _dateTimeUponStart;

        private bool _progressMessageIsFrozen;
        private bool _progressIndicatorIsRunning;

        private string _descriptionOfWhatsHappening;
        private string _candidateFrozenProgessMessage;

        #endregion

        #region methods

        public void OpenProgressIndicator(string descriptionOfWhatsHappening)
        {
            _dateTimeUponStart = DateTime.Now;

            IsBusy = true; // NB. not used at time of writing, but since this indicator is cached in DI container and injected in all major page vms, it could be employed as a global busy indicator for myriad CanExecute methods 
            IsVisible = true; // only of any use if we bind to it
            SpinnerIsVisible = true; // ditto

            _progressIndicatorIsRunning = true;
            _progressMessageIsFrozen = false;
            _descriptionOfWhatsHappening = descriptionOfWhatsHappening;
        }

        public void FreezeProgressIndicator()
        {
            _progressMessageIsFrozen = true;
        }

        public void CloseProgressIndicator()
        {
            IsBusy = false;
            IsVisible = false;
            SpinnerIsVisible = false;
            ProgressMessage = string.Empty;
            ProgressMessageOrWhiteOut = string.Empty;

            _progressIndicatorIsRunning = false;
            _progressMessageIsFrozen = false;
        }

        #endregion

        #region Heap clever - timed event handler to display active progress message

        private void OnTimerGeneratedEvent()
        {
            if (_progressMessageIsFrozen)
            {
                ProgressMessage = _candidateFrozenProgessMessage;
                ProgressMessageOrWhiteOut = _candidateFrozenProgessMessage;
            }
            else
            {
                var dateTimeNow = DateTime.Now;

                var elapsedTimeSpan = dateTimeNow - _dateTimeUponStart;

                var prettyTimeNow = JghDateTime.ToTimeLocalhhmmss(dateTimeNow.ToBinary());

                var elapsedSecondsSinceStart = elapsedTimeSpan.TotalSeconds.ToString(JghFormatSpecifiers.DecimalFormat1Dp);

                var runningProgressMessage = string.Concat(_descriptionOfWhatsHappening, "  ", elapsedSecondsSinceStart, " sec");

                ProgressMessage = _progressIndicatorIsRunning ? runningProgressMessage : prettyTimeNow;
                ProgressMessageOrWhiteOut = _progressIndicatorIsRunning ? runningProgressMessage : string.Empty;

                _candidateFrozenProgessMessage = ProgressMessage;
            }

        }

        #endregion

    }

}