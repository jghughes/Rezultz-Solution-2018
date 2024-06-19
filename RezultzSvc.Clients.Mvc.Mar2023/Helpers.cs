using NetStd.DataTransferObjects.Mar2024;
using NetStd.DataTypes.Mar2024;
using NetStd.Exceptions.Mar2024.JghExceptions;
using NetStd.Goodies.Mar2022;

namespace RezultzSvc.Clients.Mvc.Mar2023
{
    /// <summary>
    /// At time of writing, the class is a simple helper class to handle specific types returned with (400)
    /// and (500) status codes by the Rezultz MVC services. It could be easily extended to include other helper methods
    /// for other types and other controllers if desired.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// (400) status code type
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string MakeAlertMessage(JghApiException<string> ex)
        {
            // if we are here, the design intention is that it's a simple one-liner informative message to be unpacked
            // from the FaultObject and surfaced to the user. In this case the FaultObject is known to be a string

            //var answer = ex.Message; // Note: omit. this is of no interest to the user

            var answer = string.IsNullOrWhiteSpace(ex.Result) ? string.Empty: ex.Result;

            return answer;
        }

        /// <summary>
        /// (500) status code type
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string MakeExceptionMessage(JghApiException<ExceptionDto> ex)
        {
            string answer;

            // if we are here, it's some sort of rare or careless show-stopping exception that occurred and was
            // returned from the remote Rezultz service in a Code=400 response, most likely originally buried
            // in a JghCarrierException, hence the objective to dig out the innermost exception that was
            // the culprit and present full details. The read only Description prop of JghMvcApiClientException
            // returns a pretty-printed version of the exception in all its glory, because we want to present
            // the full Description to the user

            var deSerialisedExceptionItem = JghSerialisableException.FromDataTransferObject(ex.Result);

            if (deSerialisedExceptionItem is not null)
            {
                var innermostException = JghSerialisableException.FindInnermostException(deSerialisedExceptionItem);

                answer = JghString.ConcatAsParagraphs(innermostException.Message, ex.Description);
            }
            else
            {
                answer = ex.Description;
            }

            return answer;
        }
    }
}
