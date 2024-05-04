using System;
using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;

namespace NetStd.OnBoardServices01.July2018.AlertMessages
{
    public class AlertMessageService : AlertMessageServiceBase
    {
        #region ctor

        public AlertMessageService(IDialogService dialogService)
        {
            _dialogServiceInstance = dialogService;
        }

        #endregion

        #region fields

        private static bool _isShowing;

        private readonly IDialogService _dialogServiceInstance;
        // general idea is that Pages, or classes that inherit from Pages, should implement IDialogService

        #endregion

        #region methods

        protected override async Task ShowOkDialogAsync(string message)
        {
            // Only show one dialog at a time.
            if (_isShowing)
                return;

            _isShowing = true;

            try
            {
                await _dialogServiceInstance.DisplayAlertAsync(StringsForAlertMessages.Message,
                    message ?? string.Empty, AlertMessageEnum.Ok);
            }
            finally
            {
                _isShowing = false;
            }
        }

        protected override async Task ShowErrorDialog(string message)
        {
            // Only show one dialog at a time.
            if (_isShowing)
                return;

            _isShowing = true;

            try
            {
                await _dialogServiceInstance.DisplayAlertAsync(StringsForAlertMessages.Issue, message ?? string.Empty,
                    AlertMessageEnum.Ok);
            }
            finally
            {
                _isShowing = false;
            }
        }

        protected override async Task<string> ShowOKorCancelDialogAsync(string message, string title)
        {
            //const string failure = "Unable to to do what this method does.";
            //const string locus = "[ShowOkDialogAsync]";

            // Only show one dialog at a time.
            if (_isShowing)
                return AlertMessageEnum.None;

            _isShowing = true;

            string answer;

            try
            {
                var booleanOutcome = await _dialogServiceInstance.DisplayAlertAsync(title ?? string.Empty,
                    message ?? string.Empty, AlertMessageEnum.Ok, AlertMessageEnum.Cancel);

                answer = booleanOutcome ? AlertMessageEnum.Ok : AlertMessageEnum.Cancel;
            }
            catch (Exception)
            {
                answer = AlertMessageEnum.None;
            }
            finally
            {
                _isShowing = false;
            }

            return answer;
        }

        #endregion
    }
}