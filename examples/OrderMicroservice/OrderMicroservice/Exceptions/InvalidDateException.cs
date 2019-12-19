using System;

namespace OrderMicroservice.Exceptions
{
    public class InvalidDateException : Exception
    {
        public InvalidDateException(string? message) : base(message)
        {
        }
    }
}