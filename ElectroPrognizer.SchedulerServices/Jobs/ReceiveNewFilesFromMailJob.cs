using ElectroPrognizer.SchedulerServices.Helpers;
using ElectroPrognizer.Services.Interfaces;
using Quartz;

namespace ElectroPrognizer.SchedulerServices.Jobs;

public class ReceiveNewFilesFromMailJob : IJob
{
    public IEmailService EmailService { get; set; }
    public IImportFileService ImportFileService { get; set; }
    public IDownloadLogService DbLogService { get; set; }

    public Task Execute(IJobExecutionContext context)
    {
        var semaphore = ReceiveEmailJobSemaphoreHelper.GetSemaphore();

        if (semaphore.CurrentCount == 0)
            return Task.CompletedTask;

        try
        {
            semaphore.Wait();

            var mailFiles = EmailService.ReceiveNewFiles();

            foreach (var mailFileData in mailFiles)
            {
                try
                {
                    if (!mailFileData.FileDatas.Any())
                        continue;

                    DbLogService.LogInfo("Найдены новые файлы в почте");

                    ImportFileService.Import(mailFileData.FileDatas, overrideExisting: true);

                    EmailService.MoveAndMarkAsSeen(mailFileData.MailId);

                    DbLogService.LogInfo($"Успешно обработано файлов: {mailFileData.FileDatas.Length}");
                }
                catch (Exception ex)
                {
                    DbLogService.LogError($"При обработке файлов из почты произошла ошибка", ex);
                }
            }

            return Task.CompletedTask;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
