using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models.WeatherForecast;
using RestSharp;

namespace ElectroPrognizer.Services.Implementation;

public class YandexWeatherIntegrationService : IWeatherIntegrationService
{
    public OperationResult<WeatherForecastData> GetWeatherForecast(double latitude, double longitude)
    {
        using var client = new RestClient("https://api.openweathermap.org/data/3.0/onecall");
        
        var request = new RestRequest();
        request.Method = Method.Get;
        request.AddQueryParameter("lat", latitude.ToString().Replace(",", "."));
        request.AddQueryParameter("lon", longitude.ToString().Replace(",", "."));
        request.AddQueryParameter("units", "metric");
        request.AddQueryParameter("appid", "b92304653485427e65cb1d901c95f452");

        var response = client.ExecuteGet<WeatherForecastData>(request);
        
        if (!response.IsSuccessStatusCode)
            return OperationResult<WeatherForecastData>.Fail(response.ErrorMessage);

        return OperationResult<WeatherForecastData>.Success(response.Data);
    }
}
