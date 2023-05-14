using System;

namespace Server.Model
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