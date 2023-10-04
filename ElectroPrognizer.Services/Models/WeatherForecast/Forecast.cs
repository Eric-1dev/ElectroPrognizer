using System.Text.Json.Serialization;

namespace ElectroPrognizer.Services.Models.WeatherForecast;

public class Forecast
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("parts")]
    public Part[] Parts { get; set; }
}
