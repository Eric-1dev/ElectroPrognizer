using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Interfaces;
using System.Diagnostics;
using System.Globalization;

namespace ElectroPrognizer.Services.Implementation;

public class ExcelConsumptionReader : IExcelConsumptionReader
{
    public void LoadFileContent(string[] fileNames)
    {
        foreach (var file in fileNames)
        {
            ProcessSpreadsheet(file);
        }
    }

    private void ProcessSpreadsheet(string fileName)
    {
        var consumptionData = new List<EnergyConsumption>();

        try
        {
            using var doc = SpreadsheetDocument.Open(fileName, isEditable: false);

            var workbookPart = doc.WorkbookPart;
            var sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
            var sst = sstpart.SharedStringTable;

            var worksheetPart = workbookPart.WorksheetParts.First();

            var sheet = worksheetPart.Worksheet;

            var rows = sheet.Descendants<Row>();

            foreach (var row in rows)
            {
                if (row.RowIndex < 9)
                    continue;
                
                if (row.RowIndex > 100)
                    continue;

                var cells = row.Elements<Cell>();

                var date = GetCellValue(sst, cells.ElementAt(0));
                var time = GetCellValue(sst, cells.ElementAt(1), isTime: true);
                var value = GetCellValue(sst, cells.ElementAt(2));

                var energyConsumption = new EnergyConsumption
                {
                    StartDate = DateTime.Parse($"{date} {time}"),
                    Value = double.Parse(value, CultureInfo.InvariantCulture)
                };

                consumptionData.Add(energyConsumption);
            }

            var dbContext = new ApplicationContext();

            dbContext.EnergyConsumptions.AddRange(consumptionData);

            dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private string GetCellValue(SharedStringTable sst, Cell cell, bool isTime = false)
    {
        string value = null;

        if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
        {
            int ssid = int.Parse(cell.CellValue.Text);
            value = sst.ChildElements[ssid].InnerText;
        }
        else if (cell.CellValue != null)
        {
            value = cell.CellValue.Text;
        }

        if (isTime && value != null)
        {
            var doubleValue = double.Parse(value, CultureInfo.InvariantCulture);
            var timeSpan = TimeSpan.FromDays(doubleValue);

            value = timeSpan.ToString();
        }

        return value ?? string.Empty;
    }
}
