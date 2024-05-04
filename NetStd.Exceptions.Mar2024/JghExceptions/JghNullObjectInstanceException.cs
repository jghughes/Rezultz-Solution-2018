using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
	/// <inheritdoc />
	/// <summary>
	///     Exception intended as an alternative to System.NullReference
	///     exceptions and siblings. Use this exception to flag
	///     an argument error in authored code in contrast to a system library.
	/// </summary>
	
	[DataContract(Name = "JghNullObjectInstanceException")]
	public class JghNullObjectInstanceException : Exception
	{
		public JghNullObjectInstanceException(string message)
			: base(message)
		{
		}

		public JghNullObjectInstanceException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}