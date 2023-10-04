using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.SchedulerServices.Jobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class JobExecutionController : BaseController
{
    private Dictionary<string, JobInfo> _jodDictionary;

    public IServiceProvider ServiceProvider { get; set; }

    public JobExecutionController()
    {
        _jodDictionary = new Dictionary<string, JobInfo>
        {
            {nameof(SendDayReportToEmailsJob), new JobInfo(typeof(SendDayReportToEmailsJob), "Отправка ежедневнго отчета") },
            {nameof(ReceiveNewFilesFromMailJob), new JobInfo(typeof(ReceiveNewFilesFromMailJob), "Загрузка данных из почты") },
            {nameof(ReceiveWeatherForecastJob), new JobInfo(typeof(ReceiveWeatherForecastJob), "Загрузка данных о погоде") }
        };
    }

    public IActionResult Index()
    {
        return View(_jodDictionary);
    }

    [HttpPost]
    public JsonResult Execute(string jobName)
    {
        var jobType = _jodDictionary[jobName].JobType;

        var jobInstance = (IJob)ServiceProvider.GetService(jobType);

        Task.Run(() => jobInstance.Execute(null));

        return Success("Задача запущена");
    }
}
