using System;

namespace Starnet.Aggregates
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException()
        {
        }

        public ConcurrencyException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}