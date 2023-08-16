using System.Globalization;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class DayReportService : IDayReportService
{
    public IPrognizerService PrognizerService { get; set; }

    public byte[] GenerateDayReport(int substationId, DateTime calculationDate)
    {
        var totalConsumptionValues = PrognizerService.CalculateTotalValuesForDay(substationId, calculationDate);

        var placeholderCalculators = new Dictionary<string, Func<string>>
        {
            { "#{CurrentDayMonth}#", () => calculationDate.ToString("m", new CultureInfo("ru_RU")) },
            { "#{CurrentYear}#", () => calculationDate.ToString("yyyy") },
            { "#{TotalForDay}#", () => ValueToMegawatt(totalConsumptionValues.TotalForDay) },
            { "#{CumulativeTotal}#", () => ValueToMegawatt(totalConsumptionValues.CumulativeTotalForMonth) },
        };

        var templatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates/DayReportTemplate.xlsx");
        var report = SpreadsheetDocument.CreateFromTemplate(templatePath);

        foreach ( var item in placeholderCalculators )
        {
            var cell = report.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault()?.SharedStringTable.FirstOrDefault(x => x.InnerText == item.Key);

            if (cell == null)
                throw new Exception($"Не найден плейсхолдер для {item.Key}");

            cell.InnerXml = cell.InnerXml.Replace(item.Key, item.Value());
        }

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
