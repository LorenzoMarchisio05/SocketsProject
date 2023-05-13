using System;
using System.Net;

namespace Server.Model.Events
{
    public class ServerClientEventArgs : EventArgs
    {
        public IPEndPoint IpEndPoint { get;  }
        
        public string ConnectedAt { get; }

        public ServerClientEventArgs(IPEndPoint ipEndPoint, string connectedAt)
        {
            IpEndPoint = ipEndPoint;
            ConnectedAt = connectedAt;
        }
    }
}