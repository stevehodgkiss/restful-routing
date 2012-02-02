using System;

namespace RestfulRouting.Exceptions
{
    public class InvalidRestfulMethodException : Exception {
        public InvalidRestfulMethodException(string controllerName, string[] actions)
            :base(string.Format("the controller '{0}' only has methods {1}.",  controllerName, string.Join(", ", actions)))
        {}
    }
}
