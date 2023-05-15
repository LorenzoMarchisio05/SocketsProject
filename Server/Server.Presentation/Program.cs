using System;
using System.Net;
using Server.Application;
using Server.Application.Loggers;
using Server.Model.Events;

namespace Server.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var logger = new FileLogger<Application.Server>("log.csv");

            void ServerSettings(ServerSettings settings)
            {
                settings.AddLogger(logger);
                settings.AddSecondsBetweenLogs(1);
            }

            using (var server = Application.Server.Create(IPAddress.Loopback, 6000, ServerSettings)) 
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