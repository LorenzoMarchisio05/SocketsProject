using System;

namespace Wheater.Commons.Models
{
    public sealed class WeatherStationDataDto
    {
        public int Temperature { get; set; }
        
        public uint Humidity { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public string StationName { get; set; }

        public WeatherStationDataDto()
        {
            
        }

        public WeatherStationData ToWeatherStationData()
        {
            return WeatherStationData.Create(Humidity, Temperature, DateTime, StationName);
        }
    }
}