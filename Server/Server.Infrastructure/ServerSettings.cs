using Server.Model.Interfaces;

namespace Server.Infrastructure
{
    public class ServerSettings
    {
        internal ILogger<ServerController> Logger;
        internal int TimeBetweenLogs;
        
        public void AddLogger(ILogger<ServerController> logger)
        {
            this.Logger = logger;
        }

        public void AddSecondsBetweenLogs(double secondsBetweenLogs)
        {
            this.TimeBetweenLogs = (int)(secondsBetweenLogs * 1000);
        }
    }


}