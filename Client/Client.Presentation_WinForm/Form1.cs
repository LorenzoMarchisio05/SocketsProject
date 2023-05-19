using Client.Model.GeneratorsSettings;
using Client.Model;
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
using Client.Model.Events;
using Client.Model.Generators;
using System.Net;
using Client.Model.Enums;

namespace Client.Presentation_WinForm
{
    public partial class Form1 : Form
    {
        private readonly static Random _random = new Random(420);

        private readonly static CancellationTokenSource _cts = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var generationSettings = new WeatherStationDataGenerationSettings()
                .AddTimeBetweenGenerations(3000, 15_000)
                .AddTemperatureRange(-30, 100)
                .AddDateTimeProvider(new DateTimeProvider());

            var stations = Enumerable
                .Range(1, (int)nudClientNumber.Value)
                .Select(id => $"Station {id}");

            Parallel.ForEach(stations, station =>
            {
                var thread = new Thread(() => CreateClient(_cts.Token, station, generationSettings));
                thread.Start();
            });
        }

        private void CreateClient(
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

        private void ClientTrafficDataHandler(ClientDataTrafficEventArgs e)
        {
            var operation = e.DataDirection == ClientDataDirection.Sent ? "sent" : "received";
            var message = $"{e.IPEndPoint.Address} {operation} {e.Bytes.Length} bytes: {e.Data}";
            Console.WriteLine(message);
            listBoxClients.Items.Add(message);
        }


        private void ClientConnectedHandler(ClientConnectionEventArgs e)
        {
            var message = $"Client ({e.IpEndPoint.Address}) connected at {e.ConnectedAt}";
            Console.WriteLine(message);
            listBoxClients.Items.Add(message);
        }

        private void ClientDisconnectedHandler(ClientConnectionEventArgs e)
        {
            var message = $"Client ({e.IpEndPoint.Address}) dicconnected at {e.ConnectedAt}";
            Console.WriteLine(message);
            listBoxClients.Items.Add(message);
        }
    }
}
