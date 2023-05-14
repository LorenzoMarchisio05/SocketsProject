using System;
using System.Net;
using Client.Model.Events;
using Server.Model;

namespace Client.Presentation
{
    internal class Program
    {
        private static Random _random = new Random(420);
        public static void Main(string[] args)
        {
            CreateClient("station 1");
        }

        private static void CreateClient(string stationName)
        {
            using (var client = Application.Client.Create(IPAddress.Loopback, 6000))
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