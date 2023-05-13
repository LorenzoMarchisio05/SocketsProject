using System;
using System.Net;

namespace Server.Model.Events
{
    public class ServerClientConnectionEventArgs : EventArgs
    {
        public IPEndPoint IpEndPoint { get;  }
        
        public string ConnectedAt { get; }

        private ServerClientConnectionEventArgs(IPEndPoint ipEndPoint, string connectedAt)
        {
            IpEndPoint = ipEndPoint;
            ConnectedAt = connectedAt;
        }

        public static ServerClientConnectionEventArgs Create(IPEndPoint ipEndPoint, string connectedAt) =>
            new ServerClientConnectionEventArgs(ipEndPoint, connectedAt);
    }
}