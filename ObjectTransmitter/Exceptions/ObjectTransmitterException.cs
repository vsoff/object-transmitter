using System;

namespace ObjectTransmitter.Exceptions
{
    internal class ObjectTransmitterException : Exception
    {
        public ObjectTransmitterException(string message) : base(message)
        {
        }

        public ObjectTransmitterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
