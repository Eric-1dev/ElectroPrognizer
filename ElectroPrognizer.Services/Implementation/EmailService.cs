using System.Text;
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

    public ReceivedEmailFiles[] ReceiveNewFiles()
    {
        var sender = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailDataSenderEmail);

        using var imapClient = GetImapClient(FolderAccess.ReadOnly);

        var uids = imapClient.Inbox.Search(SearchQuery.FromContains(sender));

        var result = new List<ReceivedEmailFiles>();

        if (!uids.Any())
            return result.ToArray();

        foreach (var uid in uids)
        {
            var message = imapClient.Inbox.GetMessage(uid);

            var files = new List<FileData>();

            // забираем прикрепленные файлы
            foreach (var attachment in message.Attachments)
            {
                var fileAttachment = attachment as TextPart;

                if (fileAttachment == null || !fileAttachment.FileName.StartsWith("80020") || fileAttachment.ContentType.MimeType != "text/xml")
                    continue;

                var bytes = Encoding.GetEncoding(1251).GetBytes(fileAttachment.Text);

                files.Add(new FileData { Name = fileAttachment.FileName, Content = bytes });
            }

            result.Add(new ReceivedEmailFiles
            {
                MailId = uid,
                FileDatas = files.ToArray()
            });
        }

        imapClient.Disconnect(quit: true);

        return result.ToArray();
    }

    public void MoveAndMarkAsSeen(params UniqueId[] mailUids)
    {
        using var imapClient = GetImapClient(FolderAccess.ReadWrite);

        foreach (var uid in mailUids)
        {
            imapClient.Inbox.SetFlags(uid, MessageFlags.Seen, silent: false);

            var folderName = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailFolderForProcessedMails);

            var folderForProcessed = imapClient.GetFolder(folderName);

            imapClient.Inbox.MoveTo(uid, folderForProcessed);
        }

        imapClient.Disconnect(quit: true);
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

    private ImapClient GetImapClient(FolderAccess folderAccess)
    {
        var imapHost = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailImapAddress);
        var imapPort = ApplicationSettingsService.GetIntValue(ApplicationSettingEnum.MailImapPort);
        var imapLogin = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailImapUsername);
        var imapPassword = ApplicationSettingsService.GetStringValue(ApplicationSettingEnum.MailImapPassword);

        var imapClient = new ImapClient();

        imapClient.Connect(imapHost, imapPort, useSsl: true);

        imapClient.Authenticate(imapLogin, imapPassword);

        imapClient.Inbox.Open(folderAccess);

        return imapClient;
    }
}
