using ElectroPrognizer.DataLayer;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models.Prognizer;

namespace ElectroPrognizer.Services.Implementation;

public class PrognizerService : IPrognizerService
{
    private const int prognozeDayCount = 31;

    public ConsumptionTableData GetTableContent(int sunstationId, DateTime calculationDate)
    {
        var dbContext = new ApplicationContext();

        var consumptionTableData = new ConsumptionTableData();

        var energyConsuptions = dbContext.EnergyConsumptions
            .Where(x => x.ElectricityMeter.SubstationId == sunstationId
                && x.ElectricityMeterId == 1 //todo брать все, сумму рассчитывать исходя из настроек счетчика
                && x.MeasuringChannel.MeasuringChannelType == MeasuringChannelTypeEnum.ActiveInput
                && x.StartDate.Date > calculationDate.AddDays(-prognozeDayCount - 2).Date
                && x.StartDate.Date <= calculationDate.Date)
            .ToArray();

        if (!energyConsuptions.Any())
            return consumptionTableData;

        var dayDatas = new DayData[prognozeDayCount + 2];

        for (int dayCounter = 0; dayCounter < dayDatas.Length; dayCounter++)
        {
            var currentDate = calculationDate.AddDays(-prognozeDayCount - 1 + dayCounter).Date;

            var isRealData = true;

            dayDatas[dayCounter] = new DayData
            {
                Date = currentDate,
                HourDatas = new HourData[24],

                Total = energyConsuptions
                    .Where(x => x.StartDate.Date == currentDate)
                    .Sum(x => x.Value),

                CumulativeTotal = energyConsuptions
                    .Where(x => x.StartDate.Date <= currentDate)
                    .Sum(x => x.Value)
            };

            for (int hourCounter = 0; hourCounter < 24; hourCounter++)
            {
                var value = energyConsuptions.FirstOrDefault(x => x.StartDate == currentDate.AddHours(hourCounter))?.Value;

                if (value == null)
                    isRealData = false;

                dayDatas[dayCounter].HourDatas[hourCounter] = new HourData
                {
                    Hour = hourCounter,
                    Value = value
                };
            }

            dayDatas[dayCounter].IsRealData = isRealData;
        }

        consumptionTableData.DayDatas = dayDatas;

        return consumptionTableData;
    }
}
