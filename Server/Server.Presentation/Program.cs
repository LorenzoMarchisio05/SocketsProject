using System;
using System.Net;
using Server.Model.Events;
using Server.Model.Exceptions;

namespace Server.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (var server = new Application.Server(IPAddress.Loopback, 6000))
                {
                    server.ClientConnected += ClientConnetedHandler;
                    server.ClientDisconnected += ClientDisconnetedHandler;

                    server.Start();
                    
                    Console.ReadKey();
                }
            }
            catch (InvalidIpAddressException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ClientConnetedHandler(ServerClientEventArgs e)
        {
            Console.WriteLine($"client ({e.IpEndPoint.Address}) connected at: {e.ConnectedAt}");
        }
        
        private static void ClientDisconnetedHandler(ServerClientEventArgs e)
        {
            Console.WriteLine($"client ({e.IpEndPoint.Address}) disconnected at: {e.ConnectedAt}");
        }
    }
}