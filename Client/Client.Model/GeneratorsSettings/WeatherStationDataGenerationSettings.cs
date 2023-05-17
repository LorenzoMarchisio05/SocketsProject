using System;
using Client.Model.Interfaces;

namespace Client.Model.GeneratorsSettings
{
    public sealed class WeatherStationDataGenerationSettings
    {
        internal int MinimumTimeBetweenGenerations { get; set; }
        
        internal int MaximumTimeBetweenGenerations { get; set; }

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