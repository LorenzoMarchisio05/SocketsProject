namespace Client.Application
{
    public sealed class ClientSettings
    {
        internal int MinimumTimeBetweenGenerations;
        
        internal int MaximumTimeBetweenGenerations;

        internal int MinimumTemperture;
        
        internal int MaximumTemperature;

        internal string StationName;

        public void AddTimeBetweenGenerations(int minimumTime, int maximumTime)
        {
            MinimumTimeBetweenGenerations = minimumTime;
            MaximumTimeBetweenGenerations = maximumTime;
        }

        public void AddTemperatureRange(int minimumTemp, int maximumTemp)
        {
            MinimumTemperture = minimumTemp;
            MaximumTemperature = maximumTemp;
        }

        public void AddStationName(string stationName)
        {
            StationName = stationName;
        }
    }
}