using ElectroPrognizer.DataLayer;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models.Prognizer;

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
        var consumptionTableData = new ConsumptionTableData
        {
            Month = month,
            Year = year,
            MaxDay = DateTime.DaysInMonth(year, month)
        };

        var dbContext = new ApplicationContext();

        var energyConsuptions = dbContext.EnergyConsumptions
            .Where(x => x.MeasuringChannel.MeasuringChannelType == Entities.Enums.MeasuringChannelTypeEnum.ActiveInput
                && x.StartDate.Month == month
                && x.StartDate.Year == year)
            .ToArray();

        if (!energyConsuptions.Any())
            return consumptionTableData;

        var minDate = energyConsuptions.Min(x => x.StartDate).Day;
        var maxDate = energyConsuptions.Max(x => x.StartDate).Day;

        var dayDatas = new DayData[maxDate];

        for (int dayCounter = 0; dayCounter < dayDatas.Length; dayCounter++)
        {
            var currentDate = new DateTime(year, month, dayCounter + 1);

            dayDatas[dayCounter] = new DayData
            {
                DayNumber = dayCounter + 1,
                HourDatas = new HourData[24],

                Total = energyConsuptions
                    .Where(x => x.StartDate.Date == currentDate)
                    .Sum(x => x.Value),

                CumulativeTotal = energyConsuptions
                    .Where(x => x.StartDate.Date <= currentDate)
                    .Sum(x => x.Value)
            };

            var date = new DateTime(year, month, dayCounter + 1);

            for (int hourCounter = 0; hourCounter < 24; hourCounter++)
            {
                var value = energyConsuptions.FirstOrDefault(x => x.StartDate == date.AddHours(hourCounter))?.Value;

                dayDatas[dayCounter].HourDatas[hourCounter] = new HourData
                {
                    Hour = hourCounter,
                    Value = value
                };
            }
        }

        consumptionTableData.DayDatas = dayDatas;

        return consumptionTableData;
    }
}
