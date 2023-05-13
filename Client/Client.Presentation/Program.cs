using System;
using System.Net;
using Client.Model.Events;

namespace Client.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var client = new Application.Client(IPAddress.Loopback, 6000))
            {
                client.Connected += ClientConnectedHandler;
                client.Disconnected += ClientDisconnectedHandler;
                
                client.StartConnection();
            }
        }

        private static void ClientConnectedHandler(ClientConnectionEventArgs e)
        {
            Console.WriteLine($"Client ({e.IpEndPoint.Address}) connected at {e.ConnectedAt}");
        }
        
        private static void ClientDisconnectedHandler(ClientConnectionEventArgs e)
        {
            Console.WriteLine($"Client ({e.IpEndPoint.Address}) dicconnected at {e.ConnectedAt}");
        }
    }
}