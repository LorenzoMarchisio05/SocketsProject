﻿using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Client.Entity.Enums;
using Client.Entity.Events;
using Client.Entity.Generators;
using Client.Entity.GeneratorsSettings;
using Client.Entity;

namespace Client.Presentation_Console
{
    internal class Program
    {
        private static Random _random = new Random(420);

        private static CancellationTokenSource _cts = new CancellationTokenSource();
        public static void Main(string[] args)
        {
            var generationSettings = new WeatherStationDataGenerationSettings()
                .AddTimeBetweenGenerations(3000, 15_000)
                .AddTemperatureRange(-30, 100)
                .AddDateTimeProvider(new DateTimeProvider());
            
            var stations = Enumerable
                .Range(1, 10)
                .Select(id => $"Station {id}");

            Parallel.ForEach(stations, station =>
            {
                var thread = new Thread(() => CreateClient(_cts.Token, station, generationSettings));
                thread.Start();
            });
        }
        

        private static void CreateClient(
            CancellationToken cancellationToken, 
            string stationName, 
            WeatherStationDataGenerationSettings generationSettings) 
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(_random.Next(
                    generationSettings.MinimumTimeBetweenGenerations, 
                    generationSettings.MaximumTimeBetweenGenerations)
                );
                
                using (var client = Infrastructure.ClientController.Create(IPAddress.Loopback, 6000))
                {
                    client.Connected += ClientConnectedHandler;
                    client.Disconnected += ClientDisconnectedHandler;
                    client.ReceiveSent += ClientTrafficDataHandler;

                    if (!client.TryStartConnection())
                    {
                        _cts.Cancel();
                        return;
                    }

                    var stationData = StationDataGenerator
                        .Generate(generationSettings
                            .AddStationName(stationName)
                        );

                    client.Send(stationData);
                }
            }
        }
        
        
        private static void ClientTrafficDataHandler(ClientDataTrafficEventArgs e)
        {
            var operation = e.DataDirection == ClientDataDirection.Sent ? "sent" : "received";
            var message = $"{e.IPEndPoint.Address} {operation} {e.Bytes.Length} bytes: {e.Data}";
            Console.WriteLine(message);
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