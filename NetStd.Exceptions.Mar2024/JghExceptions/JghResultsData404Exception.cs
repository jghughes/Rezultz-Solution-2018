using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
	/// <inheritdoc />
	/// <summary>
	///     Exception to substitute for HTTP Status code = 404 or equivalent.
	///     Specifically intended for a missing Results data file in Rezultz.
	///     In the systems, in the case of JghRedactedMessageExceptions, the
	///     originating exception message is redacted and displayed shorn of any
	///     stacktrace because the exception is reasonably forseeable and
	///     the originating message suffices to explain it.
	/// </summary>
	
	[DataContract(Name = "JghResultsData404Exception")]
	public class JghResultsData404Exception : Jgh404Exception
	{
		public JghResultsData404Exception(string message)
			: base(message)
		{
		}

		public JghResultsData404Exception(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}