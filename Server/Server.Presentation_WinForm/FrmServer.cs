﻿using Server.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server.Model.Events;
using System.Net;
using Server.Infrastructure;
using Server.Model.Loggers;

namespace Client.Presentation_WinForm
{
    public partial class FrmServer : Form
    {
        private const int _port = 6000;

        public FrmServer()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var adonetController = new AdoNetController("");
            
            var logger = new FileLogger<ServerController>("log.csv");

            var server = ServerController.Create(
                IPAddress.Loopback, 
                _port, 
                settings =>
                {
                    settings.AddLogger(logger);
                    settings.AddSecondsBetweenLogs(1);
                    //settings.AddDBConnection(adonetController);
                });

            server.ClientConnected += ServerClientConnectedHandler;
            server.ClientDisconnected += ServerClientDisconnectedHandler;
            server.Received += ServerClientReceivedDataHandler;
            
            using (server)
            {
                listBoxServer.Items.Add($"Server listening at port {_port}");
                server.Start();

                Console.ReadKey();
            }
        }

        private void ServerClientReceivedDataHandler(ServerDataSentEventArgs e)
        {
            var message = $"Received {e.Bytes.Length} bytes: {e.Data}";
            Console.WriteLine(message);
            listBoxServer.Items.Add(message);
        }

        private void ServerClientConnectedHandler(ServerClientConnectionEventArgs e)
        {
            var message = $"Client {e.IpEndPoint.Address} connected at {e.ConnectedAt}";
            Console.WriteLine(message);
            listBoxServer.Items.Add(message);
        }

        private void ServerClientDisconnectedHandler(ServerClientConnectionEventArgs e)
        {
            var message = $"Client {e.IpEndPoint.Address} disconnected at {e.ConnectedAt}";
            Console.WriteLine(message);
            listBoxServer.Items.Add(message);
        }
    }
}
