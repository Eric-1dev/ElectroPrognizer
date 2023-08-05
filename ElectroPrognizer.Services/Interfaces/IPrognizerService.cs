using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Models.Prognizer;

namespace ElectroPrognizer.Services.Interfaces;

public interface IPrognizerService
{
    ConsumptionTableData GetTableContent(int sunstationId, DateTime calculationDate);
}
