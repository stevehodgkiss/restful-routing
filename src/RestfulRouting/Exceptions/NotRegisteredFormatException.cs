using System;
using System.Runtime.Serialization;

namespace RestfulRouting.Exceptions
{
    [Serializable]
    public class NotRegisteredFormatException : Exception
    {
        public NotRegisteredFormatException()
        {
        }

        public NotRegisteredFormatException(string message)
            : base(message)
        {
        }

        public NotRegisteredFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NotRegisteredFormatException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}