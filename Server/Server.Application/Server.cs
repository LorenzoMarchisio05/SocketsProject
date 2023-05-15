using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.Model.Interfaces;
using Server.Model.Events;
using Wheater.Commons.Models;

namespace Server.Application
{
    public class Server : IDisposable
    {
        private readonly Queue<WeatherStationData> _stationsData;
        public event ServerClientConnectionEvent ClientConnected;
        public event ServerClientConnectionEvent ClientDisconnected;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly int _timeBetweenLogs;
        private readonly ILogger<Server> _logger;

        private Socket _server;

        private byte[] _buffer;
        
        private readonly IPEndPoint _endpoint;
        
        private Server(IPAddress ip, int port, Action<ServerSettings> config)
        {
            _stationsData = new Queue<WeatherStationData>();
                
            _endpoint = new IPEndPoint(ip, port);

            var serverSettings = new ServerSettings();
            config(serverSettings);
            
            _logger = serverSettings.Logger;
            _timeBetweenLogs = serverSettings.TimeBetweenLogs;
            
            Setup();
        }

        public static Server Create(IPAddress ip, int port, Action<ServerSettings> config) =>
            new Server(ip, port,  config);

        public void Start()
        {
            var loggingThread = new Thread(() => Log(_cts.Token));
            
            loggingThread.Start();
            
            Accept();
        }

        private void Log(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                while (_stationsData.Count > 0)
                {
                    _logger.Log(_stationsData
                        .Dequeue()
                        .ToCsvString());
                }
                
                Thread.Sleep(_timeBetweenLogs);
            }
        }

        private void Setup()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            _buffer = new byte[1024];

            _server.Bind(_endpoint);
            
            _server.Listen(5);

            Console.WriteLine($"Server listening on port: {_endpoint.Port}...");
        }
        
        private void Accept()
        {
            while (true)
            {
                var socket = _server.Accept();
                
                var ipEndpoint = socket.RemoteEndPoint as IPEndPoint;
                Debug.Assert(ipEndpoint != null, nameof(ipEndpoint) + " != null");
                var connectedAt = DateTime.Now.ToLongTimeString();
                
                SignalClientAccepted(ipEndpoint, connectedAt);
                
                var connectionThread = new Thread(() => HandleConnection(socket));
                connectionThread.Start();
            }
        }

        private void HandleConnection(Socket socket)
        {
            try
            {
                while (socket.Connected)
                {
                    var receivedBytes = socket.Receive(_buffer);
                    var dataReceived = new byte[1024];
                    Array.Copy(_buffer, dataReceived, receivedBytes);
                    var json = Encoding.UTF8.GetString(dataReceived);
                    
                    Console.WriteLine($"received {receivedBytes} bytes: {json}");
                    
                    var stationData = JsonConvert
                        .DeserializeObject<WeatherStationDataDto>(json)
                        .ToWeatherStationData();
                    
                    _stationsData.Enqueue(stationData);

                    SendAcknowledgement(socket);
                }
            }
            catch (SocketException)
            {
                if (!socket.Connected)
                {
                    var ipEndpoint = socket.RemoteEndPoint as IPEndPoint;
                    Debug.Assert(ipEndpoint != null, nameof(ipEndpoint) + " != null");
                    var disconnectedAt = DateTime.Now.ToLongTimeString();
                    
                    SignalClientDisconnected(ipEndpoint, disconnectedAt);
                }
            }
            
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

            _cts.Dispose();
            _server.Close();
        }

        ~Server() => Dispose();
    }
}