using System;

namespace LeagueBroadcast.Common.Exceptions
{
    public class InvalidConfigException : Exception
    {
        public InvalidConfigException() :base() { }

        public InvalidConfigException(string message) : base(message) { }

        public InvalidConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}
