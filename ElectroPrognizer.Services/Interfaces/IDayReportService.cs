using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces;

public interface IDayReportService
{
    FileData GenerateDayReport(int substationId, DateTime calculationDate);
}
