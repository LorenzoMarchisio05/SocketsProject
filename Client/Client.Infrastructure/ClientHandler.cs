using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Client.Model.Events;
using Wheater.Commons.Models;

namespace Client.Infrastructure
{
    public sealed class ClientHandler : IDisposable
    {
        public event ClientConnectionEvent Connected;
        public event ClientConnectionEvent Disconnected;

        private readonly Socket _client;

        private readonly IPEndPoint _endpoint;

        private ClientHandler(IPAddress ip, int port)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            _endpoint = new IPEndPoint(ip, port);
        }

        public static ClientHandler Create(IPAddress ip, int port) =>
            new ClientHandler(ip, port);
        
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

        public void Send(WeatherStationData stationData)
        {
            try
            {
                var json = stationData.ToJsonString();
                
                Console.WriteLine($"Sending: {json}");
                
                var bytes = Encoding.UTF8.GetBytes(json);
                _client.Send(bytes);

                var responseBytes = new byte[1024];
                var receivedBytes = _client.Receive(responseBytes);
                Array.Resize(ref responseBytes, receivedBytes);
                
                var response = Encoding.UTF8.GetString(responseBytes);
                Console.WriteLine(response);
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

        ~ClientHandler() => Dispose();
    }
}