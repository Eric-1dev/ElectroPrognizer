using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Models.WeatherForecast;

namespace ElectroPrognizer.Services.Interfaces;

public interface IWeatherIntegrationService
{
    OperationResult<WeatherForecastData> GetWeatherForecast(double latitude, double longitude);
}
