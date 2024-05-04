using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     Exception to substitute for HTTP Status code = 404 or equivalent.
    ///     Specifically intended for a missing Settings data file in Rezultz.
    ///     In the systems, in the case of JghRedactedMessageExceptions, the
    ///     originating exception message is redacted and displayed shorn of any
    ///     stacktrace because the exception is reasonably forseeable and
    ///     the originating message suffices to explain it.
    /// </summary>
    
    [DataContract(Name = "JghSettingsData404Exception")]
    public class JghSettingsData404Exception : Jgh404Exception
    {
        public JghSettingsData404Exception(string message)
            : base(message)
        {
        }

        public JghSettingsData404Exception(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}