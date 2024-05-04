using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Jgh.Wpf.Common.July2018.Strings__;
using NetStd.Goodies.Mar2022;
using NetStd.Goodies.Mar2022.Interfaces;
using NetStd.OnBoardServices01.July2018.AlertMessages;

namespace Jgh.Wpf.Common.July2018.Page___bases
{
    /// <summary>
    ///     Derived pages should register/de-register themselves as the IAlertMessageService provider with an Ioc Container
    ///     when they Load/Unload. Because pages are transient, the service should be injected into viewmodels by means of
    ///     property injection rather than constructor injection.
    /// </summary>
    public class PageBase : Page, IAlertMessageService, IDialogService
    {
        #region ctor

        public PageBase()
        {
            _alertMessageService = new AlertMessageService(this);
        }

        #endregion

        #region fields

        private readonly AlertMessageService _alertMessageService;

        private static bool _isShowing;

        #endregion

        #region Implementation of IAlertMessageService - async version - to be deleted

        //public async Task<int> ShowOkAsync(string message)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    await _alertMessageService.ShowMessageAsync(message);

        //    _isShowing = false;

        //    return 0;
        //}

        //public async Task<int> ShowOkAsync(Exception ex)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    await _alertMessageService.ShowMessageAsync(ex);

        //    _isShowing = false;

        //    return 0;
        //}

        //public async Task<int> ShowOkAsync(string message, Exception ex)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    var answer = await AlertMessageHelper.ShowMessageAsync(message, ex);

        //    _isShowing = false;

        //    return answer;
        //}

        //public async Task<int> ShowErrorOkAsync(Exception ex)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    var answer = await AlertMessageHelper.ShowErrorMessageAsync(ex);

        //    _isShowing = false;

        //    return answer;
        //}

        //public async Task<int> ShowErrorOkAsync(string message, Exception ex)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    var answer = await AlertMessageHelper.ShowErrorMessageAsync(message, ex);

        //    _isShowing = false;

        //    return answer;
        //}

        //public async Task<int> ShowOkCancelAsync(string message, string messageCaption, string secondaryMessage)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    var answer = await AlertMessageHelper.ShowMessageWithOKorCancelOptionsAsync(message, messageCaption, secondaryMessage);

        //    _isShowing = false;

        //    return answer;
        //}

        //public async Task<int> ShowErrorOkCancelAsync(string message, string messageCaption, Exception ex)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    var answer = await AlertMessageHelper.ShowErrorMessageWithOKorCancelOptionsAsync(message, messageCaption, ex);

        //    _isShowing = false;

        //    return answer;
        //}

        //public async Task<int> ShowNotificationOrErrorMessageAsync(string failure, string locus, string locus2,
        //    string locus3,
        //    Exception ex)
        //{
        //    if (_isShowing)
        //        return 0;

        //    _isShowing = true;

        //    var answer = await AlertMessageHelper.ShowMessageOrErrorMessageAsCaseMayBeAsync(failure, locus, locus2, locus3, ex);

        //    _isShowing = false;

        //    return answer;
        //}

        #endregion

        #region Implementation of IAlertMessageService

        public async Task<int> ShowOkAsync(string message)
        {
            if (_isShowing)
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

            return answer switch
            {
                AlertMessageEnum.Ok => 1,
                AlertMessageEnum.Cancel => 2,
                AlertMessageEnum.None => 0,
                _ => 0
            };
        }

        public async Task<int> ShowErrorOkCancelAsync(string message, string messageCaption, Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;

            var answer =
                await _alertMessageService.ShowErrorMessageWithOKorCancelOptionsAsync(message, messageCaption, ex);

            _isShowing = false;

            return answer switch
            {
                AlertMessageEnum.Ok => 1,
                AlertMessageEnum.Cancel => 2,
                AlertMessageEnum.None => 0,
                _ => 0
            };
        }

        public async Task<int> ShowNotificationOrErrorMessageAsync(string failure, string locus, string locus2,
            string locus3,
            Exception ex)
        {
            if (_isShowing)
                return 0;

            _isShowing = true;

            await _alertMessageService.ShowMessageOrErrorMessageAsCaseMayBeAsync(failure, locus, locus2, locus3, ex);

            _isShowing = false;

            return 0;
        }

        #endregion

        #region WPF implementation of IDialogService - injected into the ctor of the brilliant generalised NetStd implementation of IAlertMessageService in NetStd.Services.July2018
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="okButtonLabel">Ignored. MessageBox displays an OK button</param>
        /// <returns></returns>
        public Task DisplayAlertAsync(string title, string message, string okButtonLabel)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="acceptButtonLabel">Ignored. MessageBox displays an OK button</param>
        /// <param name="cancelButtonLabel">Ignored. MessageBox displays a Cancel button</param>
        /// <returns></returns>
        public Task<bool> DisplayAlertAsync(string title, string message, string acceptButtonLabel,
            string cancelButtonLabel)
        {
            var buttons = MessageBoxButton.OKCancel;

            var result = MessageBox.Show(message ?? string.Empty, title ?? WpfCommonStrings.Decision, buttons);

            bool answer = result switch
            {
                MessageBoxResult.None => false,
                MessageBoxResult.OK => true,
                MessageBoxResult.Cancel => false,
                _ => false
            };

            return Task.FromResult(answer);
        }

        public Task<string> DisplayActionSheetAsync(string title, string cancelLabel, string destruction, params string[] buttons)
        {
            throw new NotImplementedException("Jgh.Uwp.Common.July2018.PageBase.DisplayActionSheetAsync");
        }

        #endregion

        #region Implementation of IPageNavigationService

        /// <summary>
        /// Navigates to the page with the specified page token, passing the specified parameter.
        /// </summary>
        /// <param name="pageToken">The name of the page without any leading filepath characters or trailing file-type designation e.g. "Home" not "/Home.xaml".</param>
        /// <param name="parameter">The parameter. Which in this Silverlight implementation is expected to be a Dictionary of query parameters to add to the pageToken to make a valid uri.></param>
        /// <returns>Returns <c>true</c> if navigation succeeds; otherwise, <c>false</c></returns>
        protected bool NavigateByPageToken(string pageToken, object parameter)
        {
            Debug.WriteLine($"enter NavigateByPageToken = {pageToken}");

            if (pageToken == null)
                pageToken = string.Empty;

            var uriAsString = $"/{pageToken}";

            if (parameter is Dictionary<string, string> navigationContextQueryStringParametersAsDictionary)
            {
                var navigationContextQueryStringParametersAsNameValuePairs =
                    navigationContextQueryStringParametersAsDictionary.Select(
                        kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)).ToArray();

                uriAsString = JghString.AppendNameValuePairsToUriInQueryStringFormat(
                    uriAsString,
                    navigationContextQueryStringParametersAsNameValuePairs);
            }

            var pageUri = new Uri(uriAsString, UriKind.RelativeOrAbsolute);

            if (NavigationService != null && NavigationService.Source != new Uri($"/{pageToken}", UriKind.RelativeOrAbsolute))
            {
                Debug.WriteLine($"begin NavigationService.Navigate(pageUri) = {pageUri}");

                NavigationService.Navigate(pageUri);

                Debug.WriteLine($"end NavigationService.Navigate(pageUri) = {pageUri}");
            }

            Debug.WriteLine($"exit NavigateByPageToken = {pageToken}");

            return true;
        }

        /// <summary>
        /// Goes to the previous page in the navigation stack if there is one.
        /// </summary>
        public void GoBack()
        {
            if (NavigationService is {CanGoBack: true})
            {
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Goes to the subsequent page in the navigation stack if there is one.
        /// </summary>
        public void GoForward()
        {
            if (NavigationService is {CanGoForward: true})
            {
                NavigationService.GoForward();
            }
        }

        /// <summary>
        /// Determines whether the navigation service can navigate to the following page or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the navigation service can go forward, otherwise <c>false</c>.
        /// </returns>
        public bool CanGoForward()
        {
            return NavigationService is {CanGoForward: true};
        }


        /// <summary>
        /// Determines whether the navigation service can navigate to the previous page or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the navigation service can go back; otherwise, <c>false</c>.
        /// </returns>
        public bool CanGoBack()
        {
            return NavigationService is {CanGoBack: true};
        }

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        public void ClearHistory()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restores the saved navigation.
        /// </summary>
        public void RestoreSavedNavigation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Used for navigating away from the current view model due to a suspension event, in this way you can execute additional logic to handle suspensions.
        /// </summary>
        public void Suspending()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}