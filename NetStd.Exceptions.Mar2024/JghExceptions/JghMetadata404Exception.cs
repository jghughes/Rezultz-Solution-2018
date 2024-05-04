using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
	/// <inheritdoc />
	/// <summary>
	///     Exception to substitute for HTTP Status code = 404 or equivalent.
	///     Specifically intended for a missing Profile data file in Rezultz.
	///     In the systems, in the case of JghRedactedMessageExceptions, the
	///     originating exception message is redacted and displayed shorn of any
	///     stacktrace because the exception is reasonably forseeable and
	///     the originating message suffices to explain it.
	/// </summary>
	
	[DataContract(Name = "JghMetadata404Exception")]
	public class JghMetadata404Exception : Jgh404Exception
	{
		public JghMetadata404Exception(string message)
			: base(message)
		{
		}

		public JghMetadata404Exception(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
