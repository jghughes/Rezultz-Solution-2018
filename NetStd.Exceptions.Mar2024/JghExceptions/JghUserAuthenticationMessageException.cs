using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
	/// <inheritdoc />
	/// <summary>
	///     Exception intended for trivial alert messages that neither
	///     expect nor await a user response.
	///     This is a special purpose exception used in our integrated
	///     exception-handling and Alert Messaging systems. Study both
	///     systems to appreciate how the magic integration happens.
	///     In the systems, in the case of JghUserAuthenticationMessageException, the
	///     originating exception message is redacted and displayed shorn of any
	///     stacktrace because the exception is reasonably forseeable and
	///     the originating message suffices to explain it.
	/// </summary>
	
	[DataContract(Name = "JghUserAuthenticationMessageException")]
	public class JghUserAuthenticationMessageException : JghAlertMessageException
	{
		public JghUserAuthenticationMessageException(string message)
			: base(message)
		{
		}

		// ReSharper disable once MemberCanBeProtected.Global
		public JghUserAuthenticationMessageException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}