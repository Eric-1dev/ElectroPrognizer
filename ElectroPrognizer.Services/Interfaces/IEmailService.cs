using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IEmailService
{
    OperationResult SendDaylyReport(string recipient, FileData report);

    List<FileData> ReceiveNewFiles();
}
