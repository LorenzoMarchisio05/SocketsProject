using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client.Application
{
    public sealed class Client
    {
        private Socket _client;

        private readonly IPAddress _ip;

        private readonly int _port;

        private readonly IPEndPoint _endpoint;
        
        public Client(IPAddress ip, int port)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            _ip = ip;
            _port = port;
            _endpoint = new IPEndPoint(_ip, _port);
        }

        public void StartConnection()
        {
            var connected = TryConnect();

            if (!connected)
            {
                Console.WriteLine("Connection error: too many attemps of connection");
                return;
            }
            
            Console.WriteLine($"Connceted to port: {_port}");
            
            SendLoop();
        }

        private bool TryConnect()
        {
            var attemps = 0;
            while (!_client.Connected && attemps < 10)
            {
                try
                {
                    attemps++;
                    _client.Connect(_endpoint);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine($"Connnection attemps {attemps}");
                }
                
                Thread.Sleep(1000);
            }

            return attemps < 10;
        }

        private void SendLoop()
        {
            while (true)
            {
                Console.Write("enter a message: ");
                var message = Console.ReadLine();
                var bytes = Encoding.UTF8.GetBytes(message);

                _client.Send(bytes);

                var responseBytes = new byte[1024];
                var receivedBytes = _client.Receive(responseBytes);
                Array.Resize(ref responseBytes, receivedBytes);

                var response = Encoding.UTF8.GetString(responseBytes);
                
                Console.WriteLine(response);
            }
        }
    }
}