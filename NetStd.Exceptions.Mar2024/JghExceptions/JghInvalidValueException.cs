using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
	/// <inheritdoc />
	/// <summary>
	///     Exception intended as an alternative to System.Argument
	///     exceptions and siblings. Use this exception to flag
	///     an argument error in authored code in contrast to a system library.
	/// </summary>
	
	[DataContract(Name = "JghInvalidValueException")]
	public class JghInvalidValueException : Exception
	{
		public JghInvalidValueException(string message)
			: base(message)
		{
		}

		public JghInvalidValueException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}