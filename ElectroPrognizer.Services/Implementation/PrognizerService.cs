using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models.Prognizer;
using ElectroPrognizer.Utils.Helpers;

namespace ElectroPrognizer.Services.Implementation;

public class PrognizerService : IPrognizerService
{
    public int[] GetAvailableYears()
    {
        var dbContext = new ApplicationContext();

        var minYear = dbContext.EnergyConsumptions.Select(x => x.StartDate).DefaultIfEmpty().Min().Year;
        var maxYear = dbContext.EnergyConsumptions.Select(x => x.StartDate).DefaultIfEmpty().Max().Year;

        var years = Enumerable.Range(minYear, maxYear - minYear).Append(DateTime.Now.Year);

        return years.Distinct().ToArray();
    }

    public ConsumptionTableData GetTableContent(int year, int month)
    {
        var dbContext = new ApplicationContext();

        var energyConsuptions = dbContext.EnergyConsumptions.Where(x => x.StartDate.Month == month && x.StartDate.Year == year).ToArray();

        // пока заглушка

        return new ConsumptionTableData();
    }
}
