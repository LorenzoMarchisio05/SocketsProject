using Client.Entity.GeneratorsSettings;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Entity.Events;
using Client.Entity.Generators;
using System.Net;
using Client.Entity.Enums;
using Client.Entity;

namespace Client.Presentation_WinForm
{
    public partial class FrmClient : Form
    {
        private readonly static Random _random = new Random(420);

        private static CancellationTokenSource _cts;

        public FrmClient()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            listBoxClients.Items.Clear();
            _cts = new CancellationTokenSource();

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
            BeginInvoke(new Action(() => {
                listBoxClients.Items.Add($"Starting client {stationName}");
            }));

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(_random.Next(
                    generationSettings.MinimumTimeBetweenGenerations,
                    generationSettings.MaximumTimeBetweenGenerations)
                );

                if(cancellationToken.IsCancellationRequested)
                {
                    return;
                }

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
            if (!AnyFormOpen())
            {
                return;
            }

            var operation = e.DataDirection == ClientDataDirection.Sent ? "sent" : "received";
            var message = $"{e.IPEndPoint.Address} {operation} {e.Bytes.Length} bytes: {e.Data}";
            Console.WriteLine(message);
            BeginInvoke(new Action(() => {
                listBoxClients.Items.Add(message);
            }));
        }


        private void ClientConnectedHandler(ClientConnectionEventArgs e)
        {
            if (!AnyFormOpen())
            {
                return;
            }

            var message = $"Client ({e.IpEndPoint.Address}) connected at {e.ConnectedAt}";
            Console.WriteLine(message);
            BeginInvoke(new Action(() => {
                listBoxClients.Items.Add(message);
            }));
        }

        private void ClientDisconnectedHandler(ClientConnectionEventArgs e)
        {
            if (!AnyFormOpen())
            {
                return;
            }

            var message = $"Client ({e.IpEndPoint.Address}) dicconnected at {e.ConnectedAt}";
            Console.WriteLine(message);
            BeginInvoke(new Action(() => {
                listBoxClients.Items.Add(message);
            }));
        }

        private void StopClients()
        {
            listBoxClients.Items.Add("Stopping clients");
            _cts.Cancel();
        }

        private void FrmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopClients();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopClients();
        }

        private bool AnyFormOpen() => Application.OpenForms.OfType<FrmClient>().Count() != 0;
    }
}
