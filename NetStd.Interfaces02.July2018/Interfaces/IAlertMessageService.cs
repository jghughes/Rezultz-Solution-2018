using System;
using System.Threading.Tasks;

namespace NetStd.Interfaces02.July2018.Interfaces
{
    /// <summary>
    ///     Shows messages to a user, optionally seeking a true/false response.
    ///     NB. ONLY CALL INSTANCES OF THE SERVICE FROM THE GUI-THREAD
    ///     TO AVOID CROSS THREADING
    /// </summary>
    public interface IAlertMessageService
    {
        #region async version

        /// <summary>
        ///     Shows a simple informational alert.
        ///     Returns zero always.
        /// </summary>
        /// <param name="message">Message content</param>
        /// <returns>zero</returns>
        Task<int> ShowOkAsync(string message);

        /// <summary>
        ///     Shows a simple informational alert.
        ///     Content is the (redacted) exception message.
        ///     Returns zero always.
        /// </summary>
        /// <param name="ex">The exception, which performs best if it is a JghCarrierException</param>
        /// <returns>
        ///     True
        /// </returns>
        Task<int> ShowOkAsync(Exception ex);

        /// <summary>
        ///     Shows a simple informational alert.
        ///     Content is the message and the (redacted) exception message.
        ///     Returns zero always.
        /// </summary>
        /// <param name="message">Primary message</param>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>
        ///     True
        /// </returns>
        Task<int> ShowOkAsync(string message, Exception ex);

        /// <summary>
        ///     Shows a reportable error alert.
        ///     Content is the (redacted) exception message plus how to report the error.
        ///     Returns zero always.
        /// </summary>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>zero</returns>
        Task<int> ShowErrorOkAsync(Exception ex);

        /// <summary>
        ///     Shows a reportable error alert.
        ///     Content is the message and the (redacted) exception message plus how to report the error.
        ///     Returns zero always.
        /// </summary>
        /// <param name="message">The primary message</param>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>zero</returns>
        Task<int> ShowErrorOkAsync(string message, Exception ex);

        /// <summary>
        ///     Shows a informational alert, asking for a response.
        ///     Content is the message and secondary message.
        ///     Returns 1 if user clicks Ok button, 2 if she clicks Cancel, 0 if clicks neither.
        /// </summary>
        /// <param name="message">The primary message</param>
        /// <param name="messageCaption">ContentDialog box title</param>
        /// <param name="secondaryMessage">The secondary message</param>
        /// <returns>Returns 1 if user clicks Ok button, 2 if she clicks Cancel, 0 if clicks neither</returns>
        Task<int> ShowOkCancelAsync(string message, string messageCaption, string secondaryMessage);

        /// <summary>
        ///     Shows a reportable error alert, asking for a response.
        ///     Content is the message and the (redacted) exception message plus how to report the error.
        ///     Returns 1 if user clicks Ok button, 2 if she clicks Cancel, 0 if clicks neither.
        /// </summary>
        /// <param name="message">The primary message</param>
        /// <param name="messageCaption">Dialog box title</param>
        /// <param name="ex">The exception, whose message formats best if it is a JghCarrierException</param>
        /// <returns>Returns 1 if user clicks Ok button, 2 if she clicks Cancel, 0 if clicks neither</returns>
        Task<int> ShowErrorOkCancelAsync(string message, string messageCaption, Exception ex);

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
        ///     Returns zero always
        /// </param>
        /// <param name="locus">The name of the originating method.</param>
        /// <param name="locus2">The name of the class containing the originating method.</param>
        /// <param name="locus3">The namespace of the class.</param>
        /// <param name="ex">The parent exception, consisting of an arbitrarily deep hierarchy of inner exceptions.</param>
        /// <returns>returns 0 always</returns>
        Task<int> ShowNotificationOrErrorMessageAsync(string failure, string locus, string locus2, string locus3,
            Exception ex);

        #endregion
    }
}