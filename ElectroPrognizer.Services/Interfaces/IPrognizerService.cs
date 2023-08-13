using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Services.Models.Prognizer;

namespace ElectroPrognizer.Services.Interfaces;

public interface IPrognizerService
{
    ConsumptionTableData GetTableContent(int sunstationId, DateTime calculationDate);

    TotalConsumptionValues CalculateTotalValuesForDay(int substationId, DateTime calculationDate, double? additionalValueConstant = null);
}
