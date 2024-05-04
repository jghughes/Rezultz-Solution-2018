using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NetStd.Interfaces02.July2018.Interfaces;
using NetStd.OnBoardServices01.July2018.AlertMessages;

namespace Jgh.Xamarin.Common.Jan2019.Page_bases
{
    /// <summary>
    ///     A class intended for Xamarin only. Iff using the approach in Xamarin that the AppShell should provide the IAlertMessageService, the Shell must register itself
    ///     as the IAlertMessageService provider in its ctor. the service should be injected into viewmodels by means of
    ///     property injection rather than constructor injection.
    /// </summary>
    public partial class AppShellBase : IAlertMessageService, IDialogService
    {
        private readonly AlertMessageService _alertMessageService;

        public AppShellBase()
        {
            _alertMessageService = new AlertMessageService(this);
        }

        #region Implementation of IAlertMessageService 

        public async Task<int> ShowOkAsync(string message)
        {
            await _alertMessageService.ShowMessageAsync(message);

            return 0;
        }

        public async Task<int> ShowOkAsync(Exception ex)
        {
            await _alertMessageService.ShowMessageAsync(ex);

            return 0;
        }

        public async Task<int> ShowOkAsync(string message, Exception ex)
        {
            await _alertMessageService.ShowMessageAsync(message, ex);

            return 0;
        }

        public async Task<int> ShowErrorOkAsync(Exception ex)
        {
            await _alertMessageService.ShowErrorMessageAsync(ex);

            return 0;
        }

        public async Task<int> ShowErrorOkAsync(string message, Exception ex)
        {
            await _alertMessageService.ShowErrorMessageAsync(message, ex);

            return 0;
        }

        public async Task<int> ShowOkCancelAsync(string message, string messageCaption, string secondaryMessage)
        {
            var answer =
                await _alertMessageService.ShowMessageWithOKorCancelOptionsAsync(message, messageCaption,
                    secondaryMessage);

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
            var answer =
                await _alertMessageService.ShowErrorMessageWithOKorCancelOptionsAsync(message, messageCaption, ex);

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
            await _alertMessageService.ShowMessageOrErrorMessageAsCaseMayBeAsync(failure, locus, locus2, locus3, ex);

            return 0;
        }

        #endregion

        #region Implementation of IDialogService 

        public Task DisplayAlertAsync(string title, string message, string okButtonLabel)
        {
            return DisplayAlert(title, message, okButtonLabel);
        }

        public Task<bool> DisplayAlertAsync(string title, string message, string acceptButtonLabel,
            string cancelButtonLabel)
        {
            return DisplayAlert(title, message, acceptButtonLabel, cancelButtonLabel);
        }

        public Task<string> DisplayActionSheetAsync(string title, string cancelLabel, string destruction,
            params string[] buttons)
        {
            return DisplayActionSheet(title, cancelLabel, destruction, buttons);
        }

        #endregion

        #region navigation methods

        /// <summary>
        ///     Navigates to the page with the specified page token, passing the specified parameter.
        /// </summary>
        /// <param name="pageToken">
        ///     The route of the ShellContent item that is the page. MvcRoutes must be lowercase only.
        ///     e.g. "home".
        /// </param>
        /// <param name="parameter">
        ///     The parameter. Which in this Silverlight/xamarin implementation is expected to be a Dictionary
        ///     of query parameters to add to the pageToken to make a valid uri.>
        /// </param>
        /// <returns>Returns <c>true</c> if navigation succeeds; otherwise, <c>false</c></returns>
        protected async void NavigateByPageToken(string pageToken, object parameter)
        {
            Debug.WriteLine($"enter NavigateByPageToken = {pageToken}");

            if (pageToken == null)
                pageToken = string.Empty;

            var destinationRoute = pageToken;

            var destinationRoutePlusQuery = AddQueryAttributes(destinationRoute, parameter);

            await GoToAsync($"{destinationRoutePlusQuery}");
        }

        protected static string AddQueryAttributes(string uriString, object parameter)
        {
            var uriStringPlusQuery = uriString;

            if (parameter is not Dictionary<string, string> queriesAsDictionary)
                return uriStringPlusQuery;

            var queriesAsArray =
                queriesAsDictionary.Select(
                    kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)).ToArray();

            uriStringPlusQuery = AppendNameValuePairsToUriInQueryStringFormat(
                uriString,
                queriesAsArray);

            return uriStringPlusQuery;
        }

        public static string AppendNameValuePairsToUriInQueryStringFormat(string uriAsString,
            IEnumerable<KeyValuePair<string, string>> nameValuePairs)
        {
            if (string.IsNullOrWhiteSpace(uriAsString))
                return string.Empty;

            if (nameValuePairs == null)
                return uriAsString;

            var keyValuePairs = nameValuePairs.ToArray();

            if (!keyValuePairs.Any())
                return uriAsString;

            var filteredKeyValuePairs = keyValuePairs
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value)).ToArray();

            if (!filteredKeyValuePairs.Any())
                return uriAsString;

            if (!uriAsString.EndsWith("?"))
                uriAsString = string.Concat(uriAsString, "?");

            var queryParamsAsStrings = filteredKeyValuePairs
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}").ToArray();

            var formattedAsUri = string.Concat(uriAsString, string.Join("&", queryParamsAsStrings));

            return formattedAsUri;
        }

        #endregion
    }


}