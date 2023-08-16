using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Models;
using MailKit;

namespace ElectroPrognizer.Services.Interfaces;

public interface IEmailService
{
    OperationResult SendDaylyReport(string recipient, FileData report);

    ReceivedEmailFiles[] ReceiveNewFiles();

    void MakeMailsAsSeen(params UniqueId[] mailUids);
}
