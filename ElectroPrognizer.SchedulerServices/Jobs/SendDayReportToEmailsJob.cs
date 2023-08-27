using System.Globalization;
using ElectroPrognizer.DataLayer;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using Quartz;

namespace ElectroPrognizer.SchedulerServices.Jobs;

public class SendDayReportToEmailsJob : IJob
{
    public IEmailService EmailService { get; set; }
    public IDownloadLogService DownloadLogService { get; set; }
    public IApplicationSettingsService ApplicationSettingsService { get; set; }
    public IDayReportService DayReportService { get; set; }

    public Task Execute(IJobExecutionContext context)
    {
        var emails = ApplicationSettingsService.GetStringArrayValue(ApplicationSettingEnum.MailsForDayReport, ";");

        if (emails.Length == 0 || emails.All(x => string.IsNullOrEmpty(x)))
        {
            DownloadLogService.LogError($"Отправка ежедневного отчета не выполнена: в настройке {ApplicationSettingEnum.MailsForDayReport} не указаны получатели");
            return Task.CompletedTask;
        }

        var reportDate = DateTime.Now.AddDays(-1).Date;

        int[] substationIds;

        using (var dbContext = new ApplicationContext())
        {
            substationIds = dbContext.Substations.Select(x => x.Id).ToArray();
        }

        foreach (var substationId in substationIds)
        {
            var reportFileName = $"Приложение №3 за {reportDate.ToString("m", new CultureInfo("ru"))} {reportDate.ToString("yyyy")}.xlsx";
            var reportContent = DayReportService.GenerateDayReport(substationId, reportDate);

            var reportData = new FileData
            {
                Name = reportFileName,
                Content = reportContent
            };

            var sendResult = EmailService.SendDaylyReport(emails, reportData);

            if (sendResult.IsSuccess)
                DownloadLogService.LogInfo($"Выполнена отправка ежедневного отчета за {reportDate.ToString("dd.MM.yyyy")} по подстанции Id = {substationId}");
            else
                DownloadLogService.LogError($"Отправка ежедневного отчета не выполнена: {sendResult.Message}");
        }

        return Task.CompletedTask;
    }
}
