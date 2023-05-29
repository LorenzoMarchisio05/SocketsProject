using System;
using System.Net;

namespace Client.Entity.Events
{
    public sealed class ClientConnectionEventArgs : EventArgs
    {
        public IPEndPoint IpEndPoint { get;  }
        
        public string ConnectedAt { get; }

        private ClientConnectionEventArgs(IPEndPoint ipEndPoint, string connectedAt)
        {
            IpEndPoint = ipEndPoint;
            ConnectedAt = connectedAt;
        }

        public static ClientConnectionEventArgs Create(IPEndPoint ipEndPoint, string connectedAt) =>
            new ClientConnectionEventArgs(ipEndPoint, connectedAt);
    }
}