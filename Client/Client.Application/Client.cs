using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Client.Model.Events;
using Server.Model;

namespace Client.Application
{
    public sealed class Client : IDisposable
    {
        public static Random _random = new Random(420);
        public event ClientConnectionEvent Connected;
        public event ClientConnectionEvent Disconnected;

        private readonly ClientSettings _settings;
        
        private readonly Socket _client;

        private readonly IPEndPoint _endpoint;

        private Client(IPAddress ip, int port, Action<ClientSettings> config)
        {
            _settings = new ClientSettings();
            config(_settings);
            
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            _endpoint = new IPEndPoint(ip, port);
        }

        public static Client Create(IPAddress ip, int port, Action<ClientSettings> config) =>
            new Client(ip, port, config);
        
        public void StartConnection()
        {
            var connected = TryConnect();

            if (!connected)
            {
                Console.WriteLine("Connection error: too many attemps of connection");
                return;
            }
            
            Console.WriteLine($"Connected to port: {_endpoint.Port}");
            
            SignalClientConnection();
            SendLoop();
        }

        private bool TryConnect()
        {
            var attempts = 0;
            while (!_client.Connected && attempts < 10)
            {
                try
                {
                    attempts++;
                    _client.Connect(_endpoint);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine($"Connnection attemps {attempts}");
                }
                
                Thread.Sleep(1000);
            }

            return attempts < 10;
        }

        private void SendLoop()
        {
            try
            {
                while (_client.Connected)
                {
                    var stationData = StationDataGenerator.Generate(_settings);

                    var json = stationData.ToJsonString();
                    
                    Console.WriteLine($"Sending: {json}");
                    
                    var bytes = Encoding.UTF8.GetBytes(json);
                    _client.Send(bytes);

                    var responseBytes = new byte[1024];
                    var receivedBytes = _client.Receive(responseBytes);
                    Array.Resize(ref responseBytes, receivedBytes);
                    
                    var response = Encoding.UTF8.GetString(responseBytes);
                    Console.WriteLine(response);

                    if (_client.Connected)
                    {
                        Thread.Sleep(_random.Next(
                            _settings.MinimumTimeBetweenGenerations,
                            _settings.MaximumTimeBetweenGenerations)
                        );
                    }
                }
            }
            catch (SocketException)
            {
                if (!_client.Connected)
                {
                    SignalClientDisconnection();
                }
            }
        }
        
        private void SignalClientConnection()
        {
            Connected?.Invoke(ClientConnectionEventArgs
                .Create(_endpoint, DateTime.Now.ToLongTimeString())
            );
        }
        
        private void SignalClientDisconnection()
        {
            Disconnected?.Invoke(ClientConnectionEventArgs
                .Create(_endpoint, DateTime.Now.ToLongTimeString())
            );
        }

        public void Dispose() => _client?.Dispose();

        ~Client() => Dispose();
    }
}