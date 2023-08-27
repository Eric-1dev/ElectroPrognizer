using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Models;
using MailKit;

namespace ElectroPrognizer.Services.Interfaces;

public interface IEmailService
{
    OperationResult SendDaylyReport(string[] recipients, FileData report);

    ReceivedEmailFiles[] ReceiveNewFiles();

    void MoveAndMarkAsSeen(params UniqueId[] mailUids);
}
