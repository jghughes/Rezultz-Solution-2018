using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Jgh.Uwp.Common.July2018.Strings;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;

namespace Jgh.Uwp.Common.July2018.OnBoardServices
{
    /// <summary>
    ///     Abstraction of the MessageDialog class available to UWP pages
    /// </summary>
    public static class AlertMessageHelperUwp
    {
        private const string Locus2 = nameof(AlertMessageHelperUwp);
        private const string Locus3 = "[Jgh.Uwp.Common.July2018]";

        #region fields

        private static bool _isShowing;

        #endregion

        #region methods

        /// <summary>
        ///     This is where our intricate system of exception handling and
        ///     user alerting link together. The system distinguishes between
        ///     alerts packaged in intentionally thrown custom exceptions, and
        ///     unexpected system-generated exceptions which require fuller treatment.
        ///     The system takes care of the hierarchy of exceptions and inner exceptions
        ///     that embedded async tasks generate prolifically.
        /// 
        ///     Shows an informational alert or redacted error message depending
        ///     on the typeof innermost exception of an exception. If the innermost exception derives from
        ///     a JghAlertMessageException, displays a clean message box with simply
        ///     the innermost exception message (ShowMessageAsync). Otherwise drills down and displays a
        ///     redacted summary of the entire hierarchy of exception and inner
        ///     exception messages. (ShowErrorMessageAsync). Returns true always.
        /// 
        ///     See NetStd.Exceptions.Mar2024.Helpers.JghExceptionHelpers.InnermostExceptionDerivesFromTypeOfAlertMessageException(Exception ex)
        ///     for how we determine the type of innermost exception.
        /// </summary>
        /// <param name="failure">
        ///     The intentionally thrown message or alternatively the originating exception message.
        /// </param>
        /// <param name="locus">The name of the originating method.</param>
        /// <param name="locus2">The name of the class containing the originating method.</param>
        /// <param name="locus3">The namespace of the class.</param>
        /// <param name="ex">The parent exception, consisting of an arbitrarily deep hierarchy of inner exceptions.</param>
        /// <returns>True</returns>
        public static async Task<int> ShowMessageOrErrorMessageAsCaseMayBeAsync(string failure, string locus, string locus2, string locus3,
            Exception ex)
        {
            if (JghExceptionHelpers.InnermostExceptionDerivesFromTypeOfAlertMessageException(ex))
                await ShowMessageAsync(JghExceptionHelpers.ConvertToCarrier(failure, locus, locus2, locus3, ex));
            else
                await ShowErrorMessageAsync(JghExceptionHelpers.ConvertToCarrier(failure, locus, locus2, locus3, ex));

            return 0;
        }


        /// <summary>
        ///     Shows a simple informational alert.
        ///     Returns true always.
        /// </summary>
        /// <param name="message">Message content</param>
        /// <returns>True</returns>
        public static async Task<int> ShowMessageAsync(string message)
        {
            return await ShowMessageAsync(message, null);
        }

        /// <summary>
        ///     Shows a simple informational alert.
        ///     Content is the (redacted) exception message.
        ///     Returns true always.
        /// </summary>
        /// <param name="ex">The exception, which performs best if it is a JghCarrierException</param>
        /// <returns>
        ///     True
        /// </returns>
        public static async Task<int> ShowMessageAsync(Exception ex)
        {
            return await ShowMessageAsync(null, ex);
        }

        /// <summary>
        ///     Shows a simple informational alert.
        ///     Content is the message and the (redacted) exception message.
        ///     Returns true always.
        /// </summary>
        /// <param name="message">Primary message</param>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>
        ///     True
        /// </returns>
        public static async Task<int> ShowMessageAsync(string message, Exception ex)
        {
            var consolidatedMessage = (ex is null)
                ? message
                : JghString.ConcatAsSentences(message,
                    JghExceptionHelpers.PrintRedactedExceptionMessage(JghExceptionHelpers.ConvertToCarrier(null, null, ex)));

            // Only show one dialog at a time.
            if (_isShowing)
                return 0;

            var dialog = new ContentDialog()
            {
                
                Title = "Message",
                Content = consolidatedMessage,
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Close
            };

            _isShowing = true;

            await dialog.ShowAsync();

            _isShowing = false;

            return 0;
        }

        /// <summary>
        ///     Shows a reportable error alert.
        ///     Content is the (redacted) exception message plus how to report the error.
        ///     Returns true always.
        /// </summary>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>True</returns>
        public static async Task<int> ShowErrorMessageAsync(Exception ex)
        {
            const string failure = "Unable to show error message.";
            const string locus = "[ShowErrorMessageAsync]";

            try
            {
                await ShowErrorMessageAsync(null, ex);

                return 0;
            }

            #region try catch handling

            catch (Exception exx)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);
            }

            #endregion
        }

        /// <summary>
        ///     Shows a reportable error alert.
        ///     Content is the message and the (redacted) exception message plus how to report the error.
        ///     Returns true always.
        /// </summary>
        /// <param name="message">The primary message</param>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>True</returns>
        public static async Task<int> ShowErrorMessageAsync(string message, Exception ex)
        {
            const string failure = "Unable to show error message.";
            const string locus = "[ShowErrorMessageAsync]";

            try
            {
                var howToReportAnError = StringsUwpCommon.HowToReportAnErrorV2;

                var consolidatedMessage = ex is null
                    ? message
                    : JghString.ConcatAsParagraphs(
                        message,
                        JghExceptionHelpers.PrintRedactedExceptionMessage(JghExceptionHelpers.ConvertToCarrier(null,
                            null, ex)),
                        howToReportAnError);

                // Only show one dialog at a time.
                if (_isShowing)
                    return 0;

                var dialog = new ContentDialog()
                {
                    

                    Title = StringsUwpCommon.Issue,
                    Content = consolidatedMessage,
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Close
                };

                _isShowing = true;

                await dialog.ShowAsync();

                _isShowing = false;

                return 0;
            }

            #region try catch handling

            catch (Exception exx)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);
            }

            #endregion
        }

        /// <summary>
        ///     Shows a informational alert, asking for a response.
        ///     Content is the message and secondary message.
        ///     Returns 1 if user clicks Ok button, 2 if she clicks Cancel or the ESC key or ENTER key, 0 if anything
        ///     else somehow happens.
        /// </summary>
        /// <param name="message">The primary message</param>
        /// <param name="title">ContentDialog box title</param>
        /// <param name="secondaryMessage">The secondary message</param>
        /// <returns>
        ///     Returns 1 if user clicks Ok button, 2 if she clicks Cancel or the ESC key or ENTER key, 0 if anything
        ///     else somehow happens.
        /// </returns>
        public static async Task<int> ShowMessageWithOKorCancelOptionsAsync(string message, string title,
            string secondaryMessage)
        {
            const string failure = "Unable to show message.";
            const string locus = "[ShowMessageWithOKorCancelOptionsAsync]";

            try
            {
                var consolidatedMessage = JghString.ConcatAsSentences(message, secondaryMessage);

                var result = await ShowOKorCancelDecisionDialogAsync(consolidatedMessage, title ?? StringsUwpCommon.Issue);

                int answer;

                switch (result)
                {
                    case ContentDialogResult.None:
                        answer = 0;
                        break;
                    case ContentDialogResult.Primary:
                        answer = 1;
                        break;
                    case ContentDialogResult.Secondary:
                        answer = 2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        /// <summary>
        ///     Shows a reportable error alert, asking for a response.
        ///     Content is the message and the (redacted) exception message plus how to report the error.
        ///     Returns true if clicks Ok button, false if she clicks Cancel, null if clicks neither.
        /// </summary>
        /// <param name="message">The primary message</param>
        /// <param name="title">Dialog box title</param>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>Returns true if user clicks Ok button, false if she clicks Cancel, null if clicks neither</returns>
        public static async Task<int> ShowErrorMessageWithOKorCancelOptionsAsync(string message, string title, Exception ex)
        {
            const string failure = "Unable to show error message.";
            const string locus = "[ShowErrorMessageWithOKorCancelOptionsAsync]";

            try
            {
                var howToReportAnError = StringsUwpCommon.HowToReportAnErrorV2;

                var consolidatedMessage = ex is null
                    ? JghString.ConcatAsSentences(message)
                    : JghString.ConcatAsParagraphs(
                        message,
                        JghExceptionHelpers.PrintRedactedExceptionMessage(JghExceptionHelpers.ConvertToCarrier(null,
                            null, ex)),
                        howToReportAnError);

                var result = await ShowOKorCancelDecisionDialogAsync(consolidatedMessage, title ?? StringsUwpCommon.Issue);

                int answer;

                switch (result)
                {
                    case ContentDialogResult.None:
                        answer = 0;
                        break;
                    case ContentDialogResult.Primary:
                        answer = 1;
                        break;
                    case ContentDialogResult.Secondary:
                        answer = 2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return answer;
            }

            #region try catch handling

            catch (Exception exx)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);

            }

            #endregion
        }

        #endregion

        #region helpers

        private static async Task<ContentDialogResult> ShowOKorCancelDecisionDialogAsync(string message, string title)
        {
            // Only show one dialog at a time.
            if (_isShowing)
                return ContentDialogResult.None;

            var dialog = new ContentDialog()
            {


                Title = title ?? string.Empty,
                Content = message ?? string.Empty,
                CloseButtonText = "Close",
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };

            dialog.MinWidth = 600;
            dialog.MaxWidth = 3600;

            _isShowing = true;

            var result = await dialog.ShowAsync();

            _isShowing = false;

            return result;
        }


        #endregion
    }
}