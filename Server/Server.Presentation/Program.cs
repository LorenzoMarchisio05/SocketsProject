using System;
using System.Net;
using Server.Model.Events;

namespace Server.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var server = Application.Server.Create(IPAddress.Loopback, 6000))
            {
                server.ClientConnected += ClientConnectedHandler;
                server.ClientDisconnected += ClientDisconnetedHandler;

                server.Start();
                
                Console.ReadKey();
            }
        }

        private static void ClientConnectedHandler(ServerClientConnectionEventArgs e)
        {
            Console.WriteLine($"client ({e.IpEndPoint.Address}) connected at: {e.ConnectedAt}");
        }
        
        private static void ClientDisconnetedHandler(ServerClientConnectionEventArgs e)
        {
            Console.WriteLine($"client ({e.IpEndPoint.Address}) disconnected at: {e.ConnectedAt}");
        }
    }
}