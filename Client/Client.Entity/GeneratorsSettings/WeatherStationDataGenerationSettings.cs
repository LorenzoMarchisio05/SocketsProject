using System;
using Client.Entity.Interfaces;

namespace Client.Entity.GeneratorsSettings
{
    public sealed class WeatherStationDataGenerationSettings
    {
        public int MinimumTimeBetweenGenerations { get; set; }
        
        public int MaximumTimeBetweenGenerations { get; set; }

        internal int MinimumTemperture { get; set; }
        
        internal int MaximumTemperature { get; set; }

        internal string StationName { get; set; }
        internal IDateTimeProvider DateTimeProvider { get; set; }
    }

    public static class WeatherStationDataGenerationSettingsExention
    {
        public static WeatherStationDataGenerationSettings AddTimeBetweenGenerations(
            this WeatherStationDataGenerationSettings settings, 
            int minimumTime, 
            int maximumTime)
        {
            settings.MinimumTimeBetweenGenerations = minimumTime;
            settings.MaximumTimeBetweenGenerations = maximumTime;
            return settings;
        }

        public static WeatherStationDataGenerationSettings AddTemperatureRange(
            this WeatherStationDataGenerationSettings settings, 
            int minimumTemp, 
            int maximumTemp)
        {
            settings.MinimumTemperture = minimumTemp;
            settings.MaximumTemperature = maximumTemp;
            return settings;
        }

        public static WeatherStationDataGenerationSettings AddStationName(
            this WeatherStationDataGenerationSettings settings, 
            string stationName)
        {
            settings.StationName = stationName;
            return settings;
        }

        public static WeatherStationDataGenerationSettings AddDateTimeProvider(
            this WeatherStationDataGenerationSettings settings, 
            IDateTimeProvider dateTimeProvider)
        {
            settings.DateTimeProvider = dateTimeProvider;
            return settings;
        }
    }
}