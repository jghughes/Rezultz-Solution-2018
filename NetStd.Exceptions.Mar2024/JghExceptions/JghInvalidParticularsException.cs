using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
	/// <inheritdoc />
	/// <summary>
	///     Exception intended as a catchall for edit templates that contain invalid data.
	///     This is a special purpose exception used in our integrated
	///     exception-handling and Alert Messaging systems. Study both
	///     systems to appreciate how the magic integration happens.
	///     In the systems, in the case of JghRedactedMessageExceptions, the
	///     originating exception message is redacted and displayed shorn of any
	///     stacktrace because the exception is reasonably forseeable and
	///     the originating message suffices to explain it.
	/// </summary>

	[DataContract(Name = "JghInvalidParticularsException")]
	public class JghInvalidParticularsException : JghAlertMessageException
	{
		public JghInvalidParticularsException(string message)
			: base(message)
		{
		}

		public JghInvalidParticularsException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}