using System;

namespace DevOidc.Core.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException()
        {
        }

        public InvalidRequestException(string? message) : base(message)
        {
        }
    }
}
