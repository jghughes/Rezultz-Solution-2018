using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NetStd.Exceptions.Mar2024.JghExceptions;

namespace NetStd.Exceptions.Mar2024.Helpers
{
    /// <summary>
    ///     Exception helpers for our integrated exception-handling and AlertMessaging systems.
    ///     Study the patterns and practices of both systems to understand how the integration works.
    ///     In the systems, the originating exception message of special purpose exceptions such as
    ///     JghCommunicationFailureException, JghCloudConnectionFailureException, JghAzureRequestException,
    ///     JghMetadata404Exception, JghSettingsData404Exception, JghUserAuthenticationMessageException,
    ///     and JghResultsData404Exception are displayed shorn of any stacktrace because they are regarded as reasonably forseeable.
    ///     Likewise JghAlertMessageException, which is the vehicle for chatting to the user.
    /// </summary>
    public static class JghExceptionHelpers
    {

        #region public methods

        public static void ThrowExceptionIfDebugElseInformativeMessage(string errorMessageNotifiedToUser)
        {
#if DEBUG
            throw new Exception(errorMessageNotifiedToUser);
#else
            throw new JghAlertMessageException(errorMessageNotifiedToUser);
#endif
        }

        public static void ThrowExceptionIfDebugElseInformativeMessage(string errorMessageNotifiedToUser,
            string nameOfCulpritXElement, XElement parentXElement)
        {
#if DEBUG
            throw new Exception(
                $"{errorMessageNotifiedToUser} It might be intentional but be warned that there is potentially a problem with the XElement named {nameOfCulpritXElement}. It is null or empty or otherwise potentially problematic. The parent element is {parentXElement}.");
#endif
#pragma warning disable CS0162 // Unreachable code detected
            // ReSharper disable once HeuristicUnreachableCode
            throw new JghAlertMessageException(errorMessageNotifiedToUser);
#pragma warning restore CS0162 // Unreachable code detected
        }

        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        public static JghCarrierException ConvertToCarrier(Exception ex)
        {
            return ConvertToCarrier(null, null, null, null, ex);
        }

        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        /// <param name="failureDescription">Friendly description of failed purpose of method at the locus of creation of wrapper</param>
        /// <param name="ex">Exception caught and/or conveyed</param>
        /// <returns>JghCarrierException</returns>
        public static JghCarrierException ConvertToCarrier(string failureDescription, Exception ex)
        {
            return ConvertToCarrier(failureDescription, null, null, null, ex);
        }

        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        /// <param name="failureDescription">Friendly description of failed purpose of method at the locus of creation of wrapper</param>
        /// <param name="locusDescription">Name of C# method</param>
        /// <param name="ex">Exception caught and/or conveyed</param>
        /// <returns>JghCarrierException</returns>
        public static JghCarrierException ConvertToCarrier(string failureDescription, string locusDescription,
            Exception ex)
        {
            return ConvertToCarrier(failureDescription, locusDescription, null, null, ex);
        }

        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        /// <param name="failureDescription">Friendly description of failed purpose of method at the locus of creation of wrapper</param>
        /// <param name="locusDescription">Name of C# method</param>
        /// <param name="locus2Description">Name of C# class</param>
        /// <param name="ex">Exception caught and/or conveyed</param>
        /// <returns>JghCarrierException</returns>
        public static JghCarrierException ConvertToCarrier(string failureDescription, string locusDescription,
            string locus2Description, Exception ex)
        {
            return ConvertToCarrier(failureDescription, locusDescription, locus2Description, null, ex);
        }

        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        /// <param name="failureDescription">Friendly description of failed purpose of method at the locus of creation of wrapper</param>
        /// <param name="locusDescription">Name of C# method</param>
        /// <param name="locus2Description">Name of C# class</param>
        /// <param name="locus3Description"></param>
        /// <param name="ex">Exception caught and/or conveyed</param>
        /// <returns>JghCarrierException</returns>
        public static JghCarrierException ConvertToCarrier(string failureDescription, string locusDescription, string locus2Description, string locus3Description, Exception ex)
        {
            var locus = StringHelpers.ConcatAsSentences(locusDescription, locus2Description, locus3Description);
            var info = StringHelpers.ConcatAsLines(failureDescription, locus);
            //var info = StringHelpers.ConcatAsLines(failureDescription, locusDescription, locus2Description, locus3Description);

            var carrier = new JghCarrierException(info, ex);

            return carrier;
        }

        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        /// <param name="failureDescription">Friendly description of failed purpose of method at the locus of creation of wrapper</param>
        /// <param name="locusDescription">Name of C# method</param>
        /// <param name="locus2Description">Name of C# class</param>
        /// <param name="locus3Description"></param>
        /// <param name="anything">any content you like</param>
        /// <param name="ex">Exception caught and/or conveyed</param>
        /// <returns>JghCarrierException</returns>
        public static JghCarrierException ConvertToCarrier(string failureDescription, string locusDescription, string locus2Description, string locus3Description, string anything, Exception ex)
        {
            var locus = StringHelpers.ConcatAsSentences(locusDescription, locus2Description, locus3Description);

            var info = StringHelpers.ConcatAsLines(failureDescription, locus, anything);
            //var info = StringHelpers.ConcatAsLines(failureDescription, locusDescription, locus2Description, locus3Description, anything);

            var carrier = new JghCarrierException(info, ex);

            return carrier;
        }

        /// <summary>
        ///     Prints an error message determined by the contents
        ///     of the exception and its inner exceptions. Looks for the
        ///     all occurrences of
        ///     JghAlertMessageException and/or
        ///     JghCommunicationFailureException and/or
        ///     JghCloudConnectionFailureException and/or
        ///     JghAzureRequestException and/or
        ///     JghMetadata404Exception and/or
        ///     JghSettingsData404Exception and/or
        ///     JghResultsData404Exception and/or
        ///		JghSeasonDataFile404Exception and/or
        ///		JghDataConversionModuleProfileFile404Exception and/or
        ///     JghUserAuthenticationMessageException and/or
        ///     JghTimingTentDataConversionFailureException
        ///     Prints the associated error messages of all.
        ///     For other exceptions, excluding Aggregate
        ///     exceptions which are skipped, prints the innermost exception message
        ///     and the trail of accompanying JghCarrierException messages. The print
        ///     is intended to serve as a layman's stack trace of the innermost exception.
        ///     In debug mode, the full .Net stack trace is suffixed.
        /// </summary>
        /// <param name="ex">Exception scanned</param>
        /// <returns>Censored message</returns>
        public static string PrintRedactedExceptionMessage(Exception ex)
        {
            var failure = "Unable to generate consolidated exception error message.";
            var locus = "[JghExceptionHelpers.PrintRedactedExceptionMessage]";

            try
            {
                #region handle exceptions listed as being 100% adequately described by their originating message alone

                // check the type of uppermost innermost exception. happy days if it's one of the following deemed mild. we're done. don't translate or restate. just print the message

                if (FindUppermostExceptionOfSpecifiedType<JghAlertMessageException>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghAlertMessageException>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghCommunicationFailureException>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghCommunicationFailureException>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghAzureRequestException>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghAzureRequestException>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghSettingsData404Exception>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghSettingsData404Exception>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghResultsData404Exception>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghResultsData404Exception>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghSeasonDataFile404Exception>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghSeasonDataFile404Exception>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghPublisherProfileFile404Exception>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghPublisherProfileFile404Exception>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghUserAuthenticationMessageException>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghUserAuthenticationMessageException>(ex).Message;

                if (FindUppermostExceptionOfSpecifiedType<JghPublisherServiceFaultException>(ex) is not null)
                    return FindUppermostExceptionOfSpecifiedType<JghPublisherServiceFaultException>(ex).Message;

                #endregion

                #region handle exceptions deemed to deserve a full stacktrace because they are not reasonably forseeable

                // oh dear. possibly harmful exception. Look for the innermost exception and publish layman's stacktrace of that
                // innermost exception is (meant) to be conveyed at the root of the hierarchy of inner exceptions of type JghCarrierException 

                var innermostExceptionMsg = FindInnermostException(ex).Message;

                var innerMostExceptionTypeDescription = FindInnermostException(ex).GetType().ToString();

                var hierarchyOfExceptions = ConvertHierarchyOfExceptionsToList(ex);

                if (!hierarchyOfExceptions.Any())
                    return string.Empty;

                // temporarily remove the innermost exception i.e. the last on the list
                hierarchyOfExceptions.RemoveAt(hierarchyOfExceptions.Count - 1);

                // rinse out all AggregateExceptions, it's their inner exception messages we're after 
                var hierarchyOfExceptionMessages = hierarchyOfExceptions
                    .Where(z => !IsOfSpecifiedType<AggregateException>(z))
                    .Where(z => z is not null)
                    .Where(z => !string.IsNullOrWhiteSpace(z.Message))
                    .Select(z => z.Message)
                    .ToList();

                var innermostExceptionSb = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(innermostExceptionMsg))
                    innermostExceptionSb.AppendLine(innermostExceptionMsg);

                if (!string.IsNullOrWhiteSpace(innerMostExceptionTypeDescription))
                    innermostExceptionSb.AppendLine(innerMostExceptionTypeDescription);

                if (!string.IsNullOrWhiteSpace(innermostExceptionSb.ToString()))
                    hierarchyOfExceptionMessages.Add(innermostExceptionSb.ToString());

                hierarchyOfExceptionMessages.Reverse();

                var answer = StringHelpers.ConcatAsLines(hierarchyOfExceptionMessages.ToArray());
                //var answer = StringHelpers.ConcatAsParagraphs(hierarchyOfExceptionMessages.ToArray());
#if DEBUG
                if (!string.IsNullOrWhiteSpace(ex.StackTrace))
                    answer = StringHelpers.ConcatAsParagraphs(
                        answer,
                        "You are only seeing the following stacktrace to help you in debug mode. In production mode, the stack trace will not be included.",
                        ex.StackTrace);
#endif
                return answer;

                #endregion
            }
            catch (Exception e)
            {
                return StringHelpers.ConcatAsParagraphs(e.Message, FindInnermostException(ex).Message, failure, locus);
            }
        }

        public static string MakeSimpleErrorMessage(string exceptionMessage, string failure, string postscript,
            string locus, string locus2, string locus3)
        {
            var error = StringHelpers.ConcatAsSentences(exceptionMessage, failure, postscript);

            var message = StringHelpers.ConcatAsLines(error, locus, locus2, locus3);

            return message;
        }

        /// <summary>
        ///     Finds the deepest exception of given type including subclasses.
        /// </summary>
        /// <typeparam name="T">Given type</typeparam>
        /// <param name="ex">Exception scanned</param>
        /// <returns>Exception found or null</returns>
        public static Exception FindInnermostExceptionOfSpecifiedType<T>(Exception ex) where T : Exception
        {
            if (ex is null) return null;

            var wantedList = FindAllExceptionsOfSpecifiedType<T>(ex);

            return wantedList.Any() ? wantedList.Last() : null;
        }

        /// <summary>
        ///     Finds the parent exception or topmost inner exception of given type including subclasses.
        /// </summary>
        /// <typeparam name="T">Given type</typeparam>
        /// <param name="ex">Exception scanned</param>
        /// <returns>Exception found or null</returns>
        public static Exception FindUppermostExceptionOfSpecifiedType<T>(Exception ex) where T : Exception
        {
            if (ex is null) return null;

            var wantedList = FindAllExceptionsOfSpecifiedType<T>(ex);

            return wantedList.Any() ? wantedList.First() : null;
        }

        /// <summary>
        ///     Finds the parent exception and all inner
        ///     exceptions of given type including subclasses
        /// </summary>
        /// <typeparam name="T">Given type of exception</typeparam>
        /// <param name="ex">Exception scanned</param>
        /// <returns>Collection of exceptions found, or empty if none.</returns>
        public static List<Exception> FindAllExceptionsOfSpecifiedType<T>(Exception ex) where T : Exception
        {
            if (ex is null) return [];

            var listOfE = ConvertHierarchyOfExceptionsToList(ex);

            var answer = listOfE.Where(IsOfSpecifiedType<T>).ToList();

            return answer;
        }

        /// <summary>
        ///     Determines if an innermost exception is of a given type
        /// </summary>
        /// <typeparam name="T">Given type of exception</typeparam>
        /// <param name="ex">Exception scanned</param>
        /// <returns>True if of the given type, including subclasses</returns>
        public static bool InnermostExceptionIsOfSpecifiedType<T>(Exception ex) where T : Exception
        {
            if (IsOfSpecifiedType<T>(FindInnermostException(ex)))
                return true;

            return false;
        }

        /// <summary>
        ///     Determines if exception contains a given type
        /// </summary>
        /// <typeparam name="T">Given type</typeparam>
        /// <param name="ex">Exception inspected</param>
        /// <returns>True if of given type, including subclasses</returns>
        public static bool Contains<T>(Exception ex) where T : Exception
        {
            if (ex is null) return false;

            var zz = FindAllExceptionsOfSpecifiedType<T>(ex);

            if (zz.Any())
                return true;

            return false;
        }

        /// <summary>
        ///     Determines if exception is of a given type
        /// </summary>
        /// <typeparam name="T">Given type</typeparam>
        /// <param name="ex">Exception inspected</param>
        /// <returns>True if of given type, including subclasses</returns>
        private static bool IsOfSpecifiedType<T>(Exception ex) where T : Exception
        {
            // if ex is a derived type, this doesn't check out its base type, it only checks itself

            if (ex is null) return false;

            var wantedType = typeof(T);

            if (ex.GetType() == wantedType)
                return true;

            return false;
        }

        public static List<Exception> ConvertHierarchyOfExceptionsToList(Exception ex)
        {
            if (ex is null) return [];

            var listOfE = new List<Exception>();

            var e = ex;

            listOfE.Add(e);

            while (e.InnerException is not null)
            {
                listOfE.Add(e.InnerException);

                e = e.InnerException;
            }
            // reached the bottom. we're done
            return listOfE;
        }

        /// <summary>
        ///     Finds the full type name of innermost non-null exception
        /// </summary>
        /// <param name="ex">Exception to be scanned</param>
        /// <returns>Innermost exception</returns>
        public static string FindTypeNameOfInnermostException(Exception ex)
        {
            if (ex is null)
                return null;

            var e = ex;

            while (e.InnerException is not null)
                e = e.InnerException;

            // reached the bottom. we're done
            return e.GetType().FullName;
        }

        /// <summary>
        ///     Finds the innermost non-null exception
        /// </summary>
        /// <param name="ex">Exception to be scanned</param>
        /// <returns>Innermost exception</returns>
        public static Exception FindInnermostException(Exception ex)
        {
            if (ex is null)
                return null;

            var e = ex;

            while (e.InnerException is not null)
                e = e.InnerException;

            return e;
        }

        /// <summary>
        ///     Flattens and concatenates the hierarchy of a parent exception message and all its inner exception messages.
        /// </summary>
        /// <param name="ex">Any type of exception</param>
        /// <returns>Concantenated sentence</returns>
        public static string ConcatenateHierarchyOfExceptionMessagesAsSentences(Exception ex)
        {
            if (ex is null)
                return string.Empty;

            var exceptions = ConvertHierarchyOfExceptionsToList(ex);

            if (!exceptions.Any())
                return string.Empty;

            var hierarchyOfExceptionMessages = exceptions
                .Where(z => z is not null)
                .Where(z => !string.IsNullOrWhiteSpace(z.Message))
                .Select(z => z.Message)
                .ToList();

            var answer = StringHelpers.ConcatAsSentences(hierarchyOfExceptionMessages.ToArray());

            return answer;
        }

        /// <summary>
        ///     This is where our intricate system of exception handling and
        ///     user alerting link together. The system distinguishes between
        ///     alerts packaged in intentionally thrown custom exceptions, and
        ///     unexpected system-generated exceptions which require fuller treatment.
        ///     The system takes care of the hierarchy of exceptions and inner exceptions
        ///     that embedded async tasks generate prolifically.
        ///  
        ///     See Jgh.Uwp.Common.July2018.AlertMessageHelperUwp.ShowOkOrErrorOkAsync(....)
        ///     for how we display user alerts and what content they contain. In summary, we show a
        ///     simple unadorned originating message if the typeof originating exception is a custom type
        ///     that derives from a typeof JghAlertMessageException. Any other typeof originating exception is regarded
        ///     as out of the ordinary and therefore paraded with a full but intelligible (i.e. redacted) stacktrace.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool InnermostExceptionDerivesFromTypeOfAlertMessageException(Exception ex)
        {
            // this is where we choose choose which types of exceptions are deemed to only deserve a plain messagebox and opposed to an error messagebox

            // if there was a trivial message intended for the user without adornement, innermost exception will be a JghAlertMessageException 
            // if any glitch where an exception is thrown by a call to Azure, innermost exception will be JghAzureRequestException
            // if settings data file is not found, innermost exception will be JghSettingsData404Exception
            // if results data file is not found, innermost exception will be JghResultsData404Exception
            // if there is no internet connection, innermost exception will be a JghCommunicationFailureException 
            // if user not authenticated, innermost exception will be a JghCommunicationFailureException 
            // if timekeeping tent data converter cannot be verified, innermost exception will be JghDataConversionModuleCharpCodeNameNotFoundException


            if (ex is null)
                return false;

            if (ex is JghAlertMessageException) return true;

            var requiredType = typeof(JghAlertMessageException);

            var exceptionToBeTested = FindInnermostException(ex);

            var typeToBeTested = exceptionToBeTested.GetType();

            while (typeToBeTested is not null)
            {
                if (typeToBeTested == requiredType)
                    return true;

                typeToBeTested = typeToBeTested.BaseType;
            }

            return false;
        }

        #endregion
    }
}