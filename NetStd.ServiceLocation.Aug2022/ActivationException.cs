using System;

namespace NetStd.ServiceLocation.Aug2022
{
    public class ActivationException : Exception
    {
        public ActivationException()
        {
        }

        public ActivationException(string message)
            : base(message)
        {
        }

        public ActivationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}