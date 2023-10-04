using System.Text.Json.Serialization;

namespace ElectroPrognizer.Services.Models.WeatherForecast;

public class WeatherForecastData
{
    [JsonPropertyName("info")]
    public Info Info { get; set; }

    [JsonPropertyName("forecast")]
    public Forecast Forecast { get; set; }
}
