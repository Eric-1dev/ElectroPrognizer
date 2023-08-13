using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class DayReportService : IDayReportService
{
    private const string _currentDatePlaceholder = "#{CurrentDate}#";
    private const string _totalForDayPlaceholder = "#{TotalForDay}#";
    private const string _cumulativeTotalPlaceholder = "#{CumulativeTotal}#";

    public IPrognizerService PrognizerService { get; set; }
    public IMailSenderService MailSenderService { get; set; }

    public byte[] GenerateDayReport(int substationId, DateTime calculationDate)
    {
        var templatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates/DayReportTemplate.xlsx");
        var report = SpreadsheetDocument.CreateFromTemplate(templatePath);

        var currentDateCell = report.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First().SharedStringTable.First(x => x.InnerText == _currentDatePlaceholder);
        var totalForDayCell = report.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First().SharedStringTable.First(x => x.InnerText == _totalForDayPlaceholder);
        var cumulativeTotalCell = report.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First().SharedStringTable.First(x => x.InnerText == _cumulativeTotalPlaceholder);

        var totalConsumptionValues = PrognizerService.CalculateTotalValuesForDay(substationId, calculationDate);

        var totalForDayStringValue = ValueToMegawatt(totalConsumptionValues.TotalForDay);

        var cumulativeTotalStringValue = ValueToMegawatt(totalConsumptionValues.CumulativeTotalForMonth);

        currentDateCell.InnerXml = currentDateCell.InnerXml.Replace(_currentDatePlaceholder, calculationDate.ToString("dd.MM.yyyy"));
        totalForDayCell.InnerXml = totalForDayCell.InnerXml.Replace(_totalForDayPlaceholder, totalForDayStringValue);
        cumulativeTotalCell.InnerXml = cumulativeTotalCell.InnerXml.Replace(_cumulativeTotalPlaceholder, cumulativeTotalStringValue);

        using var ms = new MemoryStream();

        report.Clone(ms);

        var bytes = ms.ToArray();

        return bytes;
    }

    private static string ValueToMegawatt(double? value)
    {
        return value.HasValue
            ? Math.Round(value.Value / 1000, 2, MidpointRounding.AwayFromZero).ToString()
            : "-";
    }
}
