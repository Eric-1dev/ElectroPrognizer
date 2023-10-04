using System.Text.Json.Serialization;

namespace ElectroPrognizer.Services.Models.WeatherForecast;

public class Part
{
    [JsonPropertyName("part_name")]
    public string PartName { get; set; }

    [JsonPropertyName("temp_avg")]
    public int TempAvg { get; set; }
}
