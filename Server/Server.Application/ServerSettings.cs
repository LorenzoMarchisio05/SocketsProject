using Server.Model.Interfaces;

namespace Server.Application
{
    public class ServerSettings
    {
        internal ILogger<Server> Logger;
        internal int TimeBetweenLogs;
        
        public void AddLogger(ILogger<Server> logger)
        {
            this.Logger = logger;
        }

        public void AddSecondsBetweenLogs(double secondsBetweenLogs)
        {
            this.TimeBetweenLogs = (int)(secondsBetweenLogs * 1000);
        }
    }


}