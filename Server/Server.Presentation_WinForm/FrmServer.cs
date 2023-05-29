using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Server.Entity.Events;
using System.Net;
using Server.Infrastructure;
using Server.Entity.Loggers;
using Wheater.Commons.DB;
using System.IO;

namespace Client.Presentation_WinForm
{
    public partial class FrmServer : Form
    {
        private const int _port = 6000;
        
        private static readonly string DB_FILE = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "DB", "weatherStationDB.mdf");

        private static readonly string _connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={DB_FILE};Integrated Security=True;Connect Timeout=30";

        public FrmServer()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var serverThread = new Thread(serverThreadHandler);
            serverThread.Start();
        }

        private void serverThreadHandler()
        {
            var adonetController = new AdoNetController(_connectionString);

            var logger = new FileLogger<ServerController>("log.csv");

            var server = ServerController.Create(
                IPAddress.Loopback,
                _port,
                settings =>
                {
                    settings.AddLogger(logger);
                    settings.AddSecondsBetweenLogs(1);
                    settings.AddDBConnection(adonetController);
                });

            server.ClientConnected += ServerClientConnectedHandler;
            server.ClientDisconnected += ServerClientDisconnectedHandler;
            server.Received += ServerClientReceivedDataHandler;

            using (server)
            {
                BeginInvoke(new Action(() => {
                    listBoxServer.Items.Add($"Server listening at port {_port}");            
                }));
                server.Start();

                Console.ReadKey();
            }
        }

        private void ServerClientReceivedDataHandler(ServerDataSentEventArgs e)
        {
            if (!AnyFormOpen())
            {
                return;
            }
            
            var message = $"Received {e.Bytes.Length} bytes: {e.Data}";
            Console.WriteLine(message);
            BeginInvoke(new Action(() => {
                listBoxServer.Items.Add(message);
            }));
        }

        private void ServerClientConnectedHandler(ServerClientConnectionEventArgs e)
        {
            if (!AnyFormOpen())
            {
                return;
            }

            var message = $"Client {e.IpEndPoint.Address} connected at {e.ConnectedAt}";
            Console.WriteLine(message);
            BeginInvoke(new Action(() => {
                listBoxServer.Items.Add(message);
            }));
        }

        private void ServerClientDisconnectedHandler(ServerClientConnectionEventArgs e)
        {
            if(!AnyFormOpen())
            {
                return;
            }

            var message = $"Client {e.IpEndPoint.Address} disconnected at {e.ConnectedAt}";
            Console.WriteLine(message);
            BeginInvoke(new Action(() => {
                listBoxServer.Items.Add(message);
            }));
        }

        private bool AnyFormOpen() => Application.OpenForms.OfType<FrmServer>().Count() != 0;
    }
}
