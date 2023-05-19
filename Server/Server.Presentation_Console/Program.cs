using System;
using System.IO;
using System.Net;
using Server.Infrastructure;
using Server.Model.Loggers;

namespace Server.Presentation_Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var adonetController = new AdoNetController("");
            
            var logger = new FileLogger<ServerController>("log.csv");

            var server = ServerController.Create(
                IPAddress.Loopback, 
                6000, 
                settings =>
                {
                    settings.AddLogger(logger);
                    settings.AddSecondsBetweenLogs(1);
                    //settings.AddDBConnection(adonetController);
                });

            server.Received += (e) => 
                    Console.WriteLine($"received {e.Bytes.Length}: {e.Data}");
            
            using (server) 
            {
                server.Start();

                Console.ReadKey();
            }
        }

    }
}