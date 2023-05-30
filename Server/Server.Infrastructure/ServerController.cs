using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.Entity.Interfaces;
using Server.Entity.Events;
using Weather.Commons.Models;

namespace Server.Infrastructure
{
    public class ServerController : IDisposable
    {
        private readonly ConcurrentQueue<WeatherStationData> _stationsData;
        public event ServerClientConnectionEvent ClientConnected;
        public event ServerClientConnectionEvent ClientDisconnected;
        public event ServerDataSentEvent Received;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly ServerSettings _settings;

        private static readonly object _lock = new object();

        // Server data
        private Socket _server;
        private readonly IPEndPoint _endpoint;
        
        private ServerController(IPAddress ip, int port, Action<ServerSettings> config)
        {
            _stationsData = new ConcurrentQueue<WeatherStationData>();
                
            _endpoint = new IPEndPoint(ip, port);

            _settings = new ServerSettings();
            config(_settings);

            Setup();
        }

        public static ServerController Create(IPAddress ip, int port, Action<ServerSettings> config) =>
            new ServerController(ip, port,  config);

        private void Setup()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _server.Bind(_endpoint);
            
            _server.Listen(5);

            Console.WriteLine($"Server listening on port: {_endpoint.Port}...");
        }
        
        
        
        public void Start()
        {
            if (!(_settings.Logger is null))
            {
                var loggingThread = new Thread(() => Log(_cts.Token));
                
                loggingThread.Start();
            }
            
            Accept();
        }

        private void Log(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                while (_stationsData.Count > 0)
                {
                    _stationsData.TryDequeue(out var station);
                    _settings.Logger.Log(station.ToCsvString());
                }

                Thread.Sleep(_settings.TimeBetweenLogs);
            }
        }

        private void Accept()
        {
            while (!_cts.IsCancellationRequested)
            {
                var socket = _server.Accept();

                var ipEndpoint = socket.RemoteEndPoint as IPEndPoint;
                var connectedAt = DateTime.Now.ToLongTimeString();
                
                SignalClientAccepted(ipEndpoint, connectedAt);
                
                var connectionThread = new Thread(() => HandleConnection(socket));
                connectionThread.Start();
            }
        }

        private void HandleConnection(Socket socket)
        {
            if (socket is null || !socket.Connected)
            {
                return;
            }

            var receivedBytes = ReceiveBytes(socket);

            if (receivedBytes.Length == 0)
            {
                DisconnectClient(socket);
                return;
            }
            
            SendAcknowledgement(socket);
            
            Received?.Invoke(ServerDataSentEventArgs.Create(receivedBytes));

            var json = Encoding.UTF8.GetString(receivedBytes);

            var stationData = JsonConvert
                .DeserializeObject<WeatherStationDataDto>(json)
                .ToWeatherStationData();

            InsertDataIntoDb(stationData);

            _stationsData.Enqueue(stationData);

            DisconnectClient(socket);
        }

        private void DisconnectClient(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Receive);

            var ipEndpoint = socket.RemoteEndPoint as IPEndPoint;
            Debug.Assert(ipEndpoint != null, nameof(ipEndpoint) + " != null");
            var disconnectedAt = DateTime.Now.ToLongTimeString();

            SignalClientDisconnected(ipEndpoint, disconnectedAt);
        }

        private void InsertDataIntoDb(WeatherStationData stationData)
        {
            if (_settings.AdoNetDBController is null)
            {
                return;
            }

            lock (_lock)
            {
                var query = @"INSERT INTO weatherdata 
                                    (stationName, temperature, humidity, datetime)
                                    VALUES 
                                    (@stationName, @temperature, @humidity, @dateTime)";
                var command = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = query,
                };

                command
                    .Parameters
                    .AddRange(new[]
                    {
                    new SqlParameter("@stationName", SqlDbType.VarChar) { Value = stationData.StationName },
                    new SqlParameter("@temperature", SqlDbType.Decimal) { Value = stationData.Temperature },
                    new SqlParameter("@humidity", SqlDbType.Int) { Value = stationData.Humidity },
                    new SqlParameter("@dateTime", SqlDbType.DateTime) { Value = stationData.DateTime },
                    });

                _settings
                    .AdoNetDBController
                    .ExecuteNonQuery(command);
            }
        }

        private static byte[] ReceiveBytes(Socket socket)
        {
            var dataReceived = new byte[1024];
            var receivedBytes = socket.Receive(dataReceived);
            Array.Resize(ref dataReceived, receivedBytes);
            return dataReceived;
        }

        private static void SendAcknowledgement(Socket socket)
        {
            const string responseText = "<ACK>";
            var responseBytes = Encoding.UTF8.GetBytes(responseText);
            socket.Send(responseBytes);
        }


        private void SignalClientAccepted(IPEndPoint ipEndpoint, string connectedAt)
        {
            Console.WriteLine($"Client {ipEndpoint.Address} connected at {connectedAt}");
            ClientConnected?.Invoke(ServerClientConnectionEventArgs
                .Create(ipEndpoint, connectedAt)
            );
        }
        
        private void SignalClientDisconnected(IPEndPoint ipEndpoint, string disconnectedAt)
        {
            Console.WriteLine($"Client {ipEndpoint.Address} disconnected at {disconnectedAt}");
            ClientDisconnected?.Invoke(ServerClientConnectionEventArgs
                .Create(ipEndpoint, disconnectedAt)
            );
        }

        public void Dispose()
        {
            _cts.Cancel();
         
            _server.Shutdown(SocketShutdown.Both);
            _server.Close();
        }

        ~ServerController() => Dispose();
    }
}