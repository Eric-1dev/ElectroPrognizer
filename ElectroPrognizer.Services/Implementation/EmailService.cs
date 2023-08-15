using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;

namespace ElectroPrognizer.Services.Implementation;

public class EmailService : IEmailService
{
    public IApplicationSettingsService ApplicationSettingsService { get; set; }

    public OperationResult SendDaylyReport(string recipient, FileData report)
    {
        try
        {
            SendEmail(recipient, "Отчет за день", "Ежедневный отчет", report);

            return OperationResult.Success("Отчет успешно отправлен");
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.ToString());
        }
    }

    public List<FileData> ReceiveNewFiles()
    {
        var imapHost = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailImapAddress);
        var imapPort = ApplicationSettingsService.GetIntValue(ApplicationSettingEnum.MailImapPort);
        var imapLogin = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailImapUsername);
        var imapPassword = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailImapPassword);
        var sender = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailDataSenderEmail);

        using var imapClient = new ImapClient();

        imapClient.Connect(imapHost, imapPort, useSsl: true);

        imapClient.Authenticate(imapLogin, imapPassword);

        imapClient.Inbox.Open(FolderAccess.ReadWrite);

        var uids = imapClient.Inbox.Search(SearchQuery.And(SearchQuery.NotSeen, SearchQuery.FromContains(sender)));

        var result = new List<FileData>();

        if (!uids.Any())
            return result;

        foreach (var uid in uids)
        {
            var message = imapClient.Inbox.GetMessage(uid);

            // забираем прикрепленные файлы
            foreach (var attachment in message.Attachments)
            {
                var fileAttachment = attachment as TextPart;

                if (fileAttachment == null || !fileAttachment.FileName.StartsWith("80020") || fileAttachment.ContentType.MimeType != "text/xml")
                    continue;

                using var ms = new MemoryStream();

                attachment.WriteTo(ms);

                var bytes = ms.ToArray();

                result.Add(new FileData { Name = fileAttachment.FileName, Content = bytes });
            }

            imapClient.Inbox.SetFlags(uid, MessageFlags.Seen, silent: false);
        }

        imapClient.Disconnect(quit: true);

        return result;
    }

    private void SendEmail(string recipient, string subject, string body, params FileData[] attachments)
    {
        var smtpHost = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailSmtpAddress);
        var smtpPort = ApplicationSettingsService.GetIntValue(ApplicationSettingEnum.MailSmtpPort);
        var smtpLogin = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailSmtpUsername);
        var smtpPassword = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailSmtpPassword);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("ElectroPrognizer", smtpLogin));
        message.To.Add(new MailboxAddress("", recipient));
        message.Subject = subject;

        var messageBody = new BodyBuilder();
        messageBody.HtmlBody = body;

        foreach (var attachment in attachments)
        {
            if (attachment.Content.Length == 0)
                continue;

            messageBody.Attachments.Add(attachment.Name, attachment.Content);
        }

        message.Body = messageBody.ToMessageBody();

        using var smtpClient = new SmtpClient();

        smtpClient.Connect(smtpHost, smtpPort, useSsl: true);

        smtpClient.Authenticate(smtpLogin, smtpPassword);

        smtpClient.Send(message);

        smtpClient.Disconnect(quit: true);
    }
}
