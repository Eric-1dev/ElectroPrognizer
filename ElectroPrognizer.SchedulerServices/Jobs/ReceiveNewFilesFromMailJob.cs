using ElectroPrognizer.SchedulerServices.Helpers;
using ElectroPrognizer.Services.Interfaces;
using Quartz;

namespace ElectroPrognizer.SchedulerServices.Jobs;

public class ReceiveNewFilesFromMailJob : IJob
{
    public IEmailService EmailService { get; set; }
    public IImportFileService ImportFileService { get; set; }

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
                    // добавить лог запуска, если есть новые письма

                    ImportFileService.Import(mailFileData.FileDatas, overrideExisting: false);

                    EmailService.MakeMailsAsSeen(mailFileData.MailId);

                    // добавить лог с количеством файлов
                }
                catch (Exception ex)
                {
                    // добавить логи
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
