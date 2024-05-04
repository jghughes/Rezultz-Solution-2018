using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
    /// <summary>
    ///     Exception to substitute for HTTP Status code = 404 or equivalent.
    ///     Specifically intended for a missing Profile data file in Rezultz.
    ///     In the systems, in the case of JghRedactedMessageExceptions, the
    ///     originating exception message is redacted and displayed shorn of any
    ///     stacktrace because the exception is reasonably forseeable and
    ///     the originating message suffices to explain it.
    /// </summary>

    [DataContract(Name = "JghSeasonDataFile404Exception")]
    public class JghSeasonDataFile404Exception : Jgh404Exception
    {
        public JghSeasonDataFile404Exception(string message)
            : base(message)
        {
        }

        public JghSeasonDataFile404Exception(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
