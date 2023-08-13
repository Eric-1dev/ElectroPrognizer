namespace ElectroPrognizer.Services.Interfaces;

public interface IDayReportService
{
    byte[] GenerateDayReport(int substationId, DateTime calculationDate);
}
