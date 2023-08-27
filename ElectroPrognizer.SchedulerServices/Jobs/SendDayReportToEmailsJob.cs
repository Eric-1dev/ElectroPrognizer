using ElectroPrognizer.DataLayer;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Interfaces;
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
            var reportData = DayReportService.GenerateDayReport(substationId, reportDate);

            var sendResult = EmailService.SendDaylyReport(emails, reportData);

            if (sendResult.IsSuccess)
                DownloadLogService.LogInfo($"Выполнена отправка ежедневного отчета за {reportDate.ToString("dd.MM.yyyy")} по подстанции Id = {substationId}");
            else
                DownloadLogService.LogError($"Отправка ежедневного отчета не выполнена: {sendResult.Message}");
        }

        return Task.CompletedTask;
    }
}
