using System;
using Newtonsoft.Json;
using Server.Model;

namespace Wheater.Commons.Models
{
    public sealed class WeatherStationData
    {
        public int Temperature { get; }

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
        
        private WeatherStationData(uint humidity, int temperature, DateTime dateTime, string stationName)
        {
            Humidity = humidity;
            Temperature = temperature;
            DateTime = dateTime;
            StationName = stationName;
        }

        public static WeatherStationData Create(uint humidity, int temperature, DateTime dateTime, string stationName) => 
            new WeatherStationData(humidity, temperature, dateTime, stationName);

        public string ToJsonString() => JsonConvert.SerializeObject(this);

        public string ToCsvString() => $"{StationName};{DateTime.Now.ToLongTimeString()};{Temperature};{Humidity}";
    }
}