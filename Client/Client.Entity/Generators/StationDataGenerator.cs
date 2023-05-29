using System;
using Client.Entity.GeneratorsSettings;
using Weather.Commons.Models;

namespace Client.Entity.Generators
{
    public static class StationDataGenerator
    {
        private static Random _random = new Random(420);
        public static WeatherStationData Generate(WeatherStationDataGenerationSettings settings)
        {
            return WeatherStationData.Create(
                humidity: (uint)_random.Next(0, 100),
                temperature: _random.Next(settings.MinimumTemperture, settings.MaximumTemperature),
                dateTime: settings.DateTimeProvider.Now,
                stationName: settings.StationName
            );
        }
    }
}