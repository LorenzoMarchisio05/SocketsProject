using Server.Entity.Interfaces;
using Wheater.Commons.DB;

namespace Server.Infrastructure
{
    public class ServerSettings
    {
        internal ILogger<ServerController> Logger { get; set; }
        internal int TimeBetweenLogs { get; set; }
        internal AdoNetController AdoNetDBController { get; set; }

        public void AddLogger(ILogger<ServerController> logger)
        {
            this.Logger = logger;
        }

        public void AddSecondsBetweenLogs(double secondsBetweenLogs)
        {
            this.TimeBetweenLogs = (int)(secondsBetweenLogs * 1000);
        }

        public void AddDBConnection(AdoNetController adoNetController)
        {
            this.AdoNetDBController = adoNetController;
        }
    }


}