using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions;

/// <summary>
///     Exception to substitute for HTTP Status code = 404 or equivalent.
///     In the system, in the case of JghRedactedMessageExceptions, the
///     originating exception message is redacted and displayed shorn of any
///     stacktrace because the exception is reasonably forseeable and
///     the originating message suffices to explain it.
/// </summary>

[DataContract(Name = "JghPublisherProfileFile404Exception")]
public class JghPublisherProfileFile404Exception : Jgh404Exception
{
    public JghPublisherProfileFile404Exception(string message)
        : base(message)
    {
    }

    public JghPublisherProfileFile404Exception(string message, Exception ex)
        : base(message, ex)
    {
    }
}