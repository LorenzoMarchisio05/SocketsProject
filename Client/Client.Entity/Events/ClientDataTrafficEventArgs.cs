using Client.Entity.Enums;
using System;
using System.Net;
using System.Text;

namespace Client.Entity.Events
{
    public class ClientDataTrafficEventArgs : EventArgs
    {
        public byte[] Bytes { get; }

        public ClientDataDirection DataDirection { get; }

        public string Data { get; }

        public IPEndPoint IPEndPoint { get; }

        private ClientDataTrafficEventArgs(byte[] bytes, ClientDataDirection dataDirection, IPEndPoint iPEndPoint)
        {
            Bytes = bytes;
            DataDirection = dataDirection;

            IPEndPoint = iPEndPoint;

            Data = Encoding.UTF8.GetString(bytes);
        }

        public static ClientDataTrafficEventArgs Create(byte[] bytes, ClientDataDirection dataDirection, IPEndPoint iPEndPoint) 
            => new ClientDataTrafficEventArgs(bytes, dataDirection, iPEndPoint);

    }
}