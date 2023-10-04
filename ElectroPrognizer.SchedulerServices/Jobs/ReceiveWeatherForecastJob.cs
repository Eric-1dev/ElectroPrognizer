using ElectroPrognizer.Services.Interfaces;
using Quartz;

namespace ElectroPrognizer.SchedulerServices.Jobs;

public class ReceiveWeatherForecastJob : IJob
{
    public IWeatherIntegrationService YandexWeatherIntegrationService { get; set; }

    public Task Execute(IJobExecutionContext context)
    {
        YandexWeatherIntegrationService.GetWeatherForecast(51.5406, 46.0086);

        return Task.CompletedTask;
    }
}
