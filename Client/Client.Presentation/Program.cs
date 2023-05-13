using System;
using System.Net;

namespace Client.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = new Application.Client(IPAddress.Loopback, 6000);
            
            client.StartConnection();
        }
    }
}