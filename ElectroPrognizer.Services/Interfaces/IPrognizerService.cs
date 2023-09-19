using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Services.Models.Prognizer;

namespace ElectroPrognizer.Services.Interfaces;

public interface IPrognizerService
{
    ConsumptionTableData GetTableContent(int sunstationId, DateTime calculationDate, double additionalPercent);

    TotalConsumptionValues CalculateTotalValuesForDay(int substationId, DateTime calculationDate, double? additionalValueConstant = null);

    OperationResult SavePrognozeToDatabase(int substationId, DateTime prognozeDate, List<HourData> data);
}
