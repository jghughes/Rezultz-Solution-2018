using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using NetStd.OnBoardServices01.July2018.AlertMessages;
using Windows.UI.Xaml;
using NetStd.Interfaces02.July2018.Interfaces;

namespace Jgh.Uwp.Common.July2018.PageBase
{
    /// <summary>
    ///     Derived pages should register/de-register themselves as the IAlertMessageService provider with the dependency injection container.
    ///     They should do this in their Page_Loaded/OnNavigatingFrom page level events. Because pages are transient, the service should be
    ///     obtained as necessary by viewmodels by means of property injection rather than constructor injection.
    ///     Derived pages should also register/de-register themselves or their viewmodel as the IProgressIndicatorService in similar fashion.
    ///     At time of writing, the viewmodel is considered the most handy.
    /// </summary>
    public abstract class UwpPageBase : Page, IAlertMessageService, IDialogService
    {
        #region ctor

        protected UwpPageBase()
        {
            _alertMessageService = new AlertMessageService(this);
        }

        #endregion

        #region fields

        private readonly AlertMessageService _alertMessageService;

        private static bool _isShowing;

        #endregion

        #region event handlers

        protected abstract void OnPageHasCompletedLoading(object sender, RoutedEventArgs e);

        #endregion

        #region Implementation of IAlertMessageService

        public async Task<int> ShowOkAsync(string message)
        {
            if (_isShowing)
                return 0;

            if (string.IsNullOrWhiteSpace(message))
                return 0;

            _isShowing = true;

            await _alertMessageService.ShowMessageAsync(message);

            _isShowing = false;

            return 0;
        }

        public async Task<int> ShowOkAsync(Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;

            await _alertMessageService.ShowMessageAsync(ex);

            _isShowing = false;

            return 0;
        }

        public async Task<int> ShowOkAsync(string message, Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;

            await _alertMessageService.ShowMessageAsync(message, ex);

            _isShowing = false;

            return 0;
        }

        public async Task<int> ShowErrorOkAsync(Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;


            await _alertMessageService.ShowErrorMessageAsync(ex);

            _isShowing = false;

            return 0;
        }

        public async Task<int> ShowErrorOkAsync(string message, Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;

            await _alertMessageService.ShowErrorMessageAsync(message, ex);

            _isShowing = false;

            return 0;
        }

        public async Task<int> ShowOkCancelAsync(string message, string messageCaption, string secondaryMessage)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;


            var answer =
                await _alertMessageService.ShowMessageWithOKorCancelOptionsAsync(message, messageCaption,
                    secondaryMessage);

            _isShowing = false;

            switch (answer)
            {
                case AlertMessageEnum.Ok:
                    return 1;
                case AlertMessageEnum.Cancel:
                    return 2;
                case AlertMessageEnum.None:
                    return 0;
                default:
                    return 0;
            }
        }

        public async Task<int> ShowErrorOkCancelAsync(string message, string messageCaption, Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;

            var answer =
                await _alertMessageService.ShowErrorMessageWithOKorCancelOptionsAsync(message, messageCaption, ex);

            _isShowing = false;

            switch (answer)
            {
                case AlertMessageEnum.Ok:
                    return 1;
                case AlertMessageEnum.Cancel:
                    return 2;
                case AlertMessageEnum.None:
                    return 0;
                default:
                    return 0;
            }
        }

        public async Task<int> ShowNotificationOrErrorMessageAsync(string failure, string locus, string locus2, string locus3, Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;

            await _alertMessageService.ShowMessageOrErrorMessageAsCaseMayBeAsync(failure, locus, locus2, locus3, ex);

            _isShowing = false;

            return 0;
        }

        #endregion

        #region UWP implementation of IDialogService - injected into the ctor of the brilliant generalised NetStd implementation of IAlertMessageService in NetStd.OnBoardServices01.July2018

        /// <summary>
        ///     Shows a informational alert, with a Close button.
        /// </summary>
        /// <param name="title">Ignored. The ContentDialog is entitled 'Message'</param>
        /// <param name="message">The information.</param>
        /// <param name="okButtonLabel"></param>
        /// <returns></returns>
        public async Task DisplayAlertAsync(string title, string message, string okButtonLabel)
        {
            // Todo. bug. ContentDialog user control isn't respecting the MaxWidth and MinWidth properties. i am trying to make the dialog wider.

            var dialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                CloseButtonText = okButtonLabel,
                DefaultButton = ContentDialogButton.Close,
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        ///     Shows a informational alert, asking for a response.
        ///     Content is the message and secondary message.
        ///     Returns 1 if user clicks Ok button, 2 if she clicks Cancel or the ESC key or ENTER key, 0 if anything
        ///     else somehow happens.
        /// </summary>
        /// <param name="title">ContentDialog box title</param>
        /// <param name="message">The message</param>
        /// <param name="acceptButtonLabel"></param>
        /// <param name="cancelButtonLabel"></param>
        /// <returns>
        ///     Returns true if user clicks Ok button, false if she clicks Cancel or the ESC key or ENTER key or Close button,
        ///     false if anything
        ///     else somehow happens.
        /// </returns>
        public async Task<bool> DisplayAlertAsync(string title, string message, string acceptButtonLabel, string cancelButtonLabel)
        {
            // Todo bug. user control isn't respecting the MaxWidth and MinWidth properties.

            var dialog = new ContentDialog()
            {
                Title = title ?? string.Empty,
                Content = message ?? string.Empty,
                CloseButtonText = "Close",
                PrimaryButtonText = acceptButtonLabel,
                SecondaryButtonText = cancelButtonLabel,
                DefaultButton = ContentDialogButton.Secondary,
            };

            var result = await dialog.ShowAsync();

            bool answer;

            switch (result)
            {
                case ContentDialogResult.None:
                    answer = false;
                    break;
                case ContentDialogResult.Primary:
                    answer = true;
                    break;
                case ContentDialogResult.Secondary:
                    answer = false;
                    break;
                default:
                    answer = false;
                    break;
            }

            return answer;
        }

        /// <summary>
        ///     not implemented yet
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cancelLabel"></param>
        /// <param name="destruction"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public Task<string> DisplayActionSheetAsync(string title, string cancelLabel, string destruction,
            params string[] buttons)
        {
            throw new NotImplementedException("Jgh.Uwp.Common.July2018.PageBase.DisplayActionSheetAsync");
        }

        #endregion
    }
}