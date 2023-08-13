using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IMailSenderService
{
    OperationResult SendDaylyReport(string recipient, FileData report);
}
