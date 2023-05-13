﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.Model;
using Server.Model.Events;

namespace Server.Application
{
    public class Server : IDisposable
    {
        public event ServerClientConnectionEvent ClientConnected;
        public event ServerClientConnectionEvent ClientDisconnected;
        
        
        private Socket _server;

        private byte[] _buffer;
        
        private readonly IPAddress _ip;

        private readonly int _port;

        public Server(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
            Setup();
        }

        public void Start()
        {
            Accept();
        }
        
        private void Setup()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            _buffer = new byte[1024];

            var endpoint = new IPEndPoint(_ip, _port);
            
            _server.Bind(endpoint);
            
            _server.Listen(5);

            Console.WriteLine($"Server listening on port: {_port}...");
        }
        
        private void Accept()
        {
            while (true)
            {
                var socket = _server.Accept();

                var ipEndpoint = socket.RemoteEndPoint as IPEndPoint;
                Debug.Assert(ipEndpoint != null, nameof(ipEndpoint) + " != null");
                
                var connectedAt = DateTime.Now.ToLongTimeString();
                
                Console.WriteLine($"Client {ipEndpoint.Address} connected at {connectedAt}");

                ClientConnected?.Invoke(ServerClientConnectionEventArgs.Create(ipEndpoint, connectedAt));
                
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
        
                    Console.WriteLine($"received {receivedBytes} bytes: {Encoding.UTF8.GetString(dataReceived)}");

                    var responseText = "<ACK> -> Server response";

                    var responseBytes = Encoding.UTF8.GetBytes(responseText);

                    socket.Send(responseBytes);
                }
            }
            catch (SocketException)
            {
                if (!socket.Connected)
                {
                    var ipEndpoint = socket.RemoteEndPoint as IPEndPoint;
                    Debug.Assert(ipEndpoint != null, nameof(ipEndpoint) + " != null");
                
                    var disconnectedAt = DateTime.Now.ToLongTimeString();
                    
                    Console.WriteLine("client disconnected");   
                    ClientDisconnected?.Invoke(ServerClientConnectionEventArgs
                        .Create(ipEndpoint, disconnectedAt)
                    );
                }
            }
            
        }

        public void Dispose() => _server.Close();

        ~Server() => Dispose();
    }
}