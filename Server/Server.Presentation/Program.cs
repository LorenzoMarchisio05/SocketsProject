using System;
using System.Net;
using Server.Model.Events;

namespace Server.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var server = new Application.Server(IPAddress.Loopback, 6000))
            {
                server.ClientConnected += ClientConnetedHandler;
                server.ClientDisconnected += ClientDisconnetedHandler;

                server.Start();
                
                Console.ReadKey();
            }
        }

        private static void ClientConnetedHandler(ServerClientConnectionEventArgs e)
        {
            Console.WriteLine($"client ({e.IpEndPoint.Address}) connected at: {e.ConnectedAt}");
        }
        
        private static void ClientDisconnetedHandler(ServerClientConnectionEventArgs e)
        {
            Console.WriteLine($"client ({e.IpEndPoint.Address}) disconnected at: {e.ConnectedAt}");
        }
    }
}