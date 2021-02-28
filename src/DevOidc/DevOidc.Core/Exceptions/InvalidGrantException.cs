using System;

namespace DevOidc.Core.Exceptions
{
    public class InvalidGrantException : Exception
    {
        public InvalidGrantException()
        {
        }

        public InvalidGrantException(string? message) : base(message)
        {
        }
    }
}
