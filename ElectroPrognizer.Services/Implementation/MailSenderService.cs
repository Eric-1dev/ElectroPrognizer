using System.Net;
using System.Net.Mail;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Implementation;

public class MailSenderService : IMailSenderService
{
    public IApplicationSettingsService ApplicationSettingsService { get; set; }

    public OperationResult SendDaylyReport(string recipient, FileData report)
    {
        try
        {
            SendEmail(recipient, "Отчет за день", report);

            return OperationResult.Success("Отчет успешно отправлен");
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.ToString());
        }
    }

    private void SendEmail(string recipient, string subject, params FileData[] attachments)
    {
        var smtpHost = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailSmtpAddress);
        var smtpPort = ApplicationSettingsService.GetIntValue(ApplicationSettingEnum.MailSmtpPort);
        var smtpLogin = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailSmtpUsername);
        var smtpPassword = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailSmtpPassword);

        var sender = new MailAddress(smtpLogin);

        var recepient = new MailAddress(recipient);

        var message = new MailMessage(sender, recepient);

        var listStreams = new List<MemoryStream>();

        foreach (var attachment in attachments)
        {
            var ms = new MemoryStream(attachment.Content);

            listStreams.Add(ms);

            message.Subject = subject;
            message.Attachments.Add(new Attachment(ms, attachment.Name));
        }

        using var mailClient = new SmtpClient(smtpHost, smtpPort);

        mailClient.Credentials = new NetworkCredential(smtpLogin, smtpPassword);
        mailClient.EnableSsl = true;

        mailClient.Send(message);

        // подчищаем
        foreach (var ms in listStreams)
        {
            ms.Dispose();
        }
    }
}
