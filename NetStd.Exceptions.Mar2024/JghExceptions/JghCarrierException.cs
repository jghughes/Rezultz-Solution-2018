using System;
using System.Runtime.Serialization;
using NetStd.Exceptions.Mar2024.Helpers;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Specialist exception wrapper. To be used in
    ///     for conveying exceptions to the topmost GUI level i.e. the Gui thread.
    ///     Intended purpose is to wrap all exceptions with added locale information
    ///     and carry the information onwards and upwards to the point of publication
    ///     and final exception handling.
    /// </summary>
    //
    [DataContract(Name = "JghCarrierException")]
    public class JghCarrierException : Exception
    {
	    #region ctor 

	    public JghCarrierException()
		    : base(null, null)
	    {
	    }

	    public JghCarrierException(string message, Exception ex)
		    : base(message, ex)
	    {
	    }

        #endregion

        #region methods 

        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        /// <param name="ex">Exception caught and/or conveyed</param>
        /// <returns>JghCarrierException</returns>
        public static JghCarrierException ConvertToCarrier(
	        Exception ex)
        {
	        return ConvertToCarrier(null, null, null, null, ex);
        }
        /// <summary>
        ///     Exception wrapper for clever exception reporting
        /// </summary>
        /// <param name="failureDescription">Friendly description of failed purpose of method at the locus of creation of wrapper</param>
        /// <param name="ex">Exception caught and/or conveyed</param>
        /// <returns>JghCarrierException</returns>
        public static JghCarrierException ConvertToCarrier(string failureDescription, 
	        Exception ex)
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
        public static JghCarrierException ConvertToCarrier(string failureDescription, string locusDescription,
            string locus2Description, string locus3Description, Exception ex)
        {
            var info = StringHelpers.ConcatAsLines(failureDescription, locusDescription, locus2Description,
                locus3Description);

            var carrier = new JghCarrierException(info, ex);

            return carrier;
        }

        #endregion

    }
}