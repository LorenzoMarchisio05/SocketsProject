namespace Server.Entity.Interfaces
{
    public interface ILogger<T>
    {
        void Log(string info);
    }
}