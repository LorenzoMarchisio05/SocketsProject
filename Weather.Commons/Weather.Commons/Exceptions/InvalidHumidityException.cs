using System;

namespace Weather.Commons.Exceptions
{
    public class InvalidHumidityException : Exception
    {
        public InvalidHumidityException()
            : base ("Invalid humidity value")
        {
        }

        public InvalidHumidityException(string message)
            : base(message)
        {
        }
    }
}