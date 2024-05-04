using System;
using System.Threading.Tasks;
using NetStd.Exceptions.Mar2024.Helpers;
using NetStd.Goodies.Mar2022;

namespace NetStd.OnBoardServices01.July2018.AlertMessages
{
    /// <summary>
    ///     This heap powerful base class is where our intricate system of exception handling and reporting and alerting in Rezultz come together.
    ///     Do not change it, unless making the changes in lockstep with with the same class on all the other platforms that use common libraries
    ///     and viewmodels.
    /// </summary>
    public abstract class AlertMessageServiceBase
    {
        private const string Locus2 = nameof(AlertMessageServiceBase);
        private const string Locus3 = "[NetStd.OnBoardServices01.July2018]";

        #region public alert messsage methods

        #region ShowMessage and overloads

        /// <summary>
        ///     Shows a simple informational alert.
        ///     Returns true always.
        /// </summary>
        /// <param name="message">Message content</param>
        /// <returns>True</returns>
        public async Task ShowMessageAsync(string message)
        {
            await ShowMessageAsync(message, null);
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
        public async Task ShowMessageAsync(Exception ex)
        {
            await ShowMessageAsync(null, ex);
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
        public async Task ShowMessageAsync(string message, Exception ex)
        {
            var consolidatedMessage = ex == null
                ? message
                : JghString.ConcatAsSentences(message,
                    JghExceptionHelpers.PrintRedactedExceptionMessage(JghExceptionHelpers.ConvertToCarrier(null, null, ex)));

            await ShowOkDialogAsync(consolidatedMessage);
        }

        /// <summary>
        ///     Shows a informational alert, asking for a response.
        ///     Content is the message and secondary message.
        ///     Returns true if user clicks Ok button, false if she clicks Cancel or the ESC key or ENTER key, null if anything
        ///     else somehow happens.
        /// </summary>
        /// <param name="message">The primary message</param>
        /// <param name="title">ContentDialog box title</param>
        /// <param name="secondaryMessage">The secondary message</param>
        /// <returns>
        ///     Returns true if user clicks Ok button, false if she clicks Cancel or the ESC key or ENTER key, null if
        ///     anything else somehow happens
        /// </returns>
        public async Task<string> ShowMessageWithOKorCancelOptionsAsync(string message, string title,
            string secondaryMessage)
        {
            const string failure = "Unable to show message.";
            const string locus = "[ShowMessageWithOKorCancelOptionsAsync]";

            try
            {
                var consolidatedMessage = JghString.ConcatAsSentences(message, secondaryMessage);

                var result = await ShowOKorCancelDialogAsync(consolidatedMessage,
                    title ?? StringsForAlertMessages.Message);

                return result;
            }

            #region try catch handling

            catch (Exception ex)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, ex);
            }

            #endregion
        }

        #endregion

        #region ShowErrorMessage and overloads

        /// <summary>
        ///     Shows a reportable error alert.
        ///     Content is the (redacted) exception message plus how to report the error.
        ///     Returns true always.
        /// </summary>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>True</returns>
        public async Task ShowErrorMessageAsync(Exception ex)
        {
            const string failure = "Unable to show error message.";
            const string locus = "[ShowErrorMessageAsync]";

            try
            {
                await ShowErrorMessageAsync(null, ex);
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
        public async Task ShowErrorMessageAsync(string message, Exception ex)
        {
            const string failure = "Unable to show error message.";
            const string locus = "[ShowErrorMessageAsync]";

            try
            {
                var howToReportAnError = StringsForAlertMessages.HowToReportAnErrorV2;

                var consolidatedMessage = ex == null
                    ? message
                    : JghString.ConcatAsParagraphs(message, JghExceptionHelpers.PrintRedactedExceptionMessage(JghExceptionHelpers.ConvertToCarrier(null, null, ex)),
                        howToReportAnError);

                await ShowErrorDialog(consolidatedMessage);
            }

            #region try catch handling

            catch (Exception exx)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);
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
        public async Task<string> ShowErrorMessageWithOKorCancelOptionsAsync(string message, string title, Exception ex)
        {
            const string failure = "Unable to show error message.";
            const string locus = "[ShowErrorMessageWithOKorCancelOptionsAsync]";

            try
            {
                var howToReportAnError = StringsForAlertMessages.HowToReportAnErrorV2;

                var consolidatedMessage = ex == null
                    ? JghString.ConcatAsSentences(message)
                    : JghString.ConcatAsParagraphs(
                        message,
                        JghExceptionHelpers.PrintRedactedExceptionMessage(JghExceptionHelpers.ConvertToCarrier(null,
                            null, ex)),
                        howToReportAnError);

                var result =
                    await ShowOKorCancelDialogAsync(consolidatedMessage, title ?? StringsForAlertMessages.Issue);

                return result;
            }

            #region try catch handling

            catch (Exception exx)
            {
                throw JghExceptionHelpers.ConvertToCarrier(failure, locus, Locus2, Locus3, exx);
            }

            #endregion
        }

        #endregion

        #region ShowMessageOrErrorMessageAsCaseMayBeAsync

        /// <summary>
        ///     This is where our intricate system of exception handling and
        ///     user alerting link together. The system distinguishes between
        ///     alerts packaged in intentionally thrown custom exceptions, and
        ///     unexpected system-generated exceptions which require fuller treatment.
        ///     The system takes care of the hierarchy of exceptions and inner exceptions
        ///     that embedded async tasks generate prolifically.
        ///     Shows an informational alert or redacted error message depending
        ///     on the typeof innermost exception of an exception. If the innermost exception derives from
        ///     a JghAlertMessageException, displays a clean message box with simply
        ///     the innermost exception message (ShowMessageAsync). Otherwise drills down and displays a
        ///     redacted summary of the entire hierarchy of exception and inner
        ///     exception messages. (ShowErrorMessageAsync). Returns true always.
        ///     See NetStd.Exceptions.Mar2024.Helpers.JghExceptionHelpers.InnermostExceptionDerivesFromTypeOfAlertMessageException(Exception ex)
        ///     for how we determine the type of innermost exception.
        /// </summary>
        /// <param name="failure">
        ///     The intentionally thrown message or alternatively the originating exception message.
        /// </param>
        /// <param name="locus">The name of the originating method.</param>
        /// <param name="locus2">The name of the class containing the originating method.</param>
        /// <param name="locus3">The namespace of the class.</param>
        /// <param name="ex">The parent exception, consisting of an arbitrarily deep hierarchy of innerexceptions.</param>
        /// <returns>True</returns>
        public async Task ShowMessageOrErrorMessageAsCaseMayBeAsync(string failure, string locus, string locus2,
            string locus3,
            Exception ex)
        {
            if (JghExceptionHelpers.InnermostExceptionDerivesFromTypeOfAlertMessageException(ex))
                await ShowMessageAsync(JghExceptionHelpers.ConvertToCarrier(failure, locus, locus2, locus3, ex));
            else
                await ShowErrorMessageAsync(JghExceptionHelpers.ConvertToCarrier(failure, locus, locus2, locus3, ex));
        }

        #endregion

        #endregion

        #region private abstract page dialog methods - platform specific methods implemented in derived classes

        protected abstract Task ShowOkDialogAsync(string message);

        protected abstract Task ShowErrorDialog(string message);

        protected abstract Task<string> ShowOKorCancelDialogAsync(string message, string title);

        #endregion
    }
}