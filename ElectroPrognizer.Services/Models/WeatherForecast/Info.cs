using System.Text.Json.Serialization;

namespace ElectroPrognizer.Services.Models.WeatherForecast;

public class Info
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    public double Longitude { get; set; }
}
