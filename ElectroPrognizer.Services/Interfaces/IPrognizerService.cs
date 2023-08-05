using ElectroPrognizer.Services.Models.Prognizer;

namespace ElectroPrognizer.Services.Interfaces;

public interface IPrognizerService
{
    ConsumptionTableData GetTableContent(DateTime calculationDate);
}
