using System;
using System.Net;
using Server.Infrastructure;
using Server.Model.Loggers;

namespace Server.Presentation
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var adonetController = new AdoNetController("connectionstring...");
            
            var logger = new FileLogger<ServerController>("log.csv");
            
            using (var server = ServerController.Create(
                           IPAddress.Loopback, 
                           6000, 
                           settings => 
                   {
                       settings.AddLogger(logger);
                       settings.AddSecondsBetweenLogs(1);
                       settings.AddDBConnection(adonetController);
                   })) 
            {
                server.Start();
            }
        }

    }
}