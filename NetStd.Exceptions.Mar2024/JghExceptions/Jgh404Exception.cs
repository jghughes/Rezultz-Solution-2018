using System;
using System.Runtime.Serialization;

namespace NetStd.Exceptions.Mar2024.JghExceptions
{
	[DataContract(Name = "Jgh404Exception")]
    public class Jgh404Exception : JghAlertMessageException
    {
	    public Jgh404Exception(string message)
		    : base(message)
	    {
	    }

	    public Jgh404Exception(string message, Exception ex)
		    : base(message, ex)
	    {
	    }
    }
}