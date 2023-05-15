using System;
using Wheater.Commons.Models;

namespace Client.Application
{
    static class StationDataGenerator
    {
        private static Random _random = new Random(420);
        public static WeatherStationData Generate(ClientSettings settings)
        {
            return WeatherStationData.Create(
                humidity: (uint)_random.Next(0, 100),
                temperature: _random.Next(settings.MinimumTemperture, settings.MaximumTemperature),
                dateTime: DateTime.Now,
                stationName: settings.StationName
            );
        }
    }
}