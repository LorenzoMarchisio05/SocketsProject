using System;

namespace Server.Model
{
    public sealed class WeatherStationData
    {
        public uint Temperature { get; }

        private uint _humidity;
        public uint Humidity
        {
            get => _humidity;
            private set
            {
                if (value > 100)
                {
                    throw new InvalidHumidityException();
                }

                _humidity = value;
            }
        }
        
        public DateTime DateTime { get; }
        
        public string StationName { get; }

        private WeatherStationData(uint humidity, uint temperature, DateTime dateTime, string stationName)
        {
            Humidity = humidity;
            Temperature = temperature;
            DateTime = dateTime;
            StationName = stationName;
        }

        public static WeatherStationData Create(uint humidity, uint temperature, DateTime dateTime, string stationName) => 
            new WeatherStationData(humidity, temperature, dateTime, stationName);
    }
}