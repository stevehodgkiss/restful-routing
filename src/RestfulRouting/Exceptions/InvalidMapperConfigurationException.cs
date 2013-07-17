using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RestfulRouting.Exceptions
{
    [Serializable]
    public class InvalidMapperConfigurationException : Exception
    {
        public InvalidMapperConfigurationException() { }
        public InvalidMapperConfigurationException(string message) : base(message) { }
        public InvalidMapperConfigurationException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMapperConfigurationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }
}
