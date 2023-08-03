using ElectroPrognizer.Services.Models.Prognizer;

namespace ElectroPrognizer.Services.Interfaces;

public interface IPrognizerService
{
    int[] GetAvailableYears();

    ConsumptionTableData GetTableContent(int year, int month);
}
