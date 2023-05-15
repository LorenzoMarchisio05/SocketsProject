namespace Server.Application.Interfaces
{
    public interface ILogger<T>
    {
        void Log(string info);
    }
}