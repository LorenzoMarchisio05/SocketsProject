using System;
using System.Net;
using Client.Application;
using Client.Model.Events;

namespace Client.Presentation
{
    internal class Program
    { 
        public static void Main(string[] args)
        {
            CreateClient("station1");
        }

        private static void CreateClient(string stationName)
        {
            Action<ClientSettings> clientSettings = settings =>
            {
                settings.AddTimeBetweenGenerations(3000, 15_000);
                settings.AddTemperatureRange(-30, 100);
                settings.AddStationName(stationName);
            };
            
            using (var client = Application.Client.Create(IPAddress.Loopback, 6000, clientSettings))
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