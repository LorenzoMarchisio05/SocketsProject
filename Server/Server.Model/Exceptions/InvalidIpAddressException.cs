using System;

namespace Server.Model.Exceptions
{
    public class InvalidIpAddressException : Exception
    { 
        public InvalidIpAddressException()
            : base("Invalid ip address")
        {
        }

        public InvalidIpAddressException(string message)
            : base(message)
        {
        }
        
    }
}