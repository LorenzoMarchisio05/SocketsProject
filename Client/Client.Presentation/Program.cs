using System;
using System.Linq;
using System.Net;
using System.Threading;
using Client.Model.Events;
using Client.Model.Generators;
using Client.Model.GeneratorsSettings;

namespace Client.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*
            var stations = Enumerable
                .Range(1, 10)
                .Select(id => $"Station {id}");

            foreach (var station in stations)
            {
                var thread = new Thread(() => CreateClient(station));
                thread.Start();
            }
            */

            CreateClient("Station 1");

        }
        

        private static void CreateClient(string stationName)
        {
            var generationSettings = new WeatherStationDataGenerationSettings()
                .AddTimeBetweenGenerations(3000, 15_000)
                .AddTemperatureRange(-30, 100)
                .AddStationName(stationName);
            
            using (var client = Infrastructure.ClientHandler.Create(IPAddress.Loopback, 6000))
            {
                client.Connected += ClientConnectedHandler;
                client.Disconnected += ClientDisconnectedHandler;

                client.StartConnection();

                var stationData = StationDataGenerator.Generate(generationSettings);

                client.Send(stationData);
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