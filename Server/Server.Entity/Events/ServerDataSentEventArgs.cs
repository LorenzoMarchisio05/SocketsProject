using System.Text;

namespace Server.Entity.Events
{
    public sealed class ServerDataSentEventArgs
    {
        public byte[] Bytes { get; }

        public string Data { get; }

        private ServerDataSentEventArgs(byte[] bytes)
        {
            Bytes = bytes;

            Data = Encoding.UTF8.GetString(bytes);
        }

        public static ServerDataSentEventArgs Create(byte[] bytes)
            => new ServerDataSentEventArgs(bytes);
    }
}