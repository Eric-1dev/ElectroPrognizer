using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models.Prognizer;
using Microsoft.EntityFrameworkCore;

namespace ElectroPrognizer.Services.Implementation;

public class PrognizerService : IPrognizerService
{
    private const int analizingDayCount = 31;
    private const int prognozeDayCount = 2;

    public ConsumptionTableData GetTableContent(int substationId, DateTime calculationDate)
    {
        var dbContext = new ApplicationContext();

        var consumptionTableData = new ConsumptionTableData();

        // берем analizingDayCount дней от даты рассчета
        var energyConsuptions = dbContext.EnergyConsumptions
            .Where(x => x.ElectricityMeter.SubstationId == substationId
                && x.MeasuringChannel.MeasuringChannelType == MeasuringChannelTypeEnum.ActiveInput
                && x.StartDate.Date > calculationDate.AddDays(-analizingDayCount - prognozeDayCount).Date
                && x.StartDate.Date <= calculationDate.Date)
            .Include(x => x.ElectricityMeter)
            .ToArray();

        if (!energyConsuptions.Any())
            return consumptionTableData;

        var additionalValueConstant = dbContext.Substations.Where(x => x.Id == substationId).Select(x => x.AdditionalValueConstant).Single() * 1000;

        // считаем максимальную минимальную из имеющухся в БД дат из выборки
        var maxExistingDate = energyConsuptions.Max(x => x.StartDate).Date;
        var minExistingDate = energyConsuptions.Min(x => x.StartDate).Date;

        var totalExistingDays = (maxExistingDate - minExistingDate).Days + 1;

        // создаем массив из имеющихся дней + prognozeDayCount дней на прогнозирование
        var dayDatas = new DayData[totalExistingDays + prognozeDayCount];

        var cumulativeTotal = 0.0;

        for (int dayCounter = 0; dayCounter < totalExistingDays; dayCounter++)
        {
            var total = 0.0;

            var currentDate = calculationDate.AddDays(-analizingDayCount - 1 + dayCounter).Date;

            dayDatas[dayCounter] = new DayData
            {
                Date = currentDate,
                HourDatas = new HourData[24],
                IsRealData = true,
            };

            for (int hourCounter = 0; hourCounter < 24; hourCounter++)
            {
                var consumptionOnHour = energyConsuptions.Where(x => x.StartDate == currentDate.AddHours(hourCounter)).ToArray();

                double? value;

                if (!consumptionOnHour.Any())
                {
                    value = null;
                }
                else
                {
                    value = consumptionOnHour.Aggregate(0.0, ValueAggregateFunction) + additionalValueConstant;
                    total += value.Value;
                }

                dayDatas[dayCounter].HourDatas[hourCounter] = new HourData
                {
                    Hour = hourCounter,
                    Value = value
                };
            }

            if (currentDate.Day == 1)
                cumulativeTotal = total;
            else
                cumulativeTotal += total;

            dayDatas[dayCounter].Total = total;
            dayDatas[dayCounter].CumulativeTotal = cumulativeTotal;
        }

        for (int dayCounter = 0; dayCounter < prognozeDayCount; dayCounter++)
        {
            var currentDayNumber = totalExistingDays + dayCounter;

            var previousDayData = dayDatas[currentDayNumber - 1];

            dayDatas[currentDayNumber] = new DayData
            {
                IsRealData = false,
                Date = previousDayData.Date.AddDays(1),
                HourDatas = new HourData[24],
                Total = 0,
                CumulativeTotal = 0
            };

            for (int hourCounter = 0; hourCounter < 24; hourCounter++)
            {
                dayDatas[currentDayNumber].HourDatas[hourCounter] = new HourData
                {
                    Hour = hourCounter,
                    Value = PrognozeValue(dayDatas.Where(x => x != null && x.Date <= previousDayData.Date)
                        .SelectMany(x => x.HourDatas)
                        .Where(x => x.Hour == hourCounter)
                        .Select(x => x.Value)
                        .ToArray())
                };
            }
        }

        consumptionTableData.DayDatas = dayDatas;

        return consumptionTableData;
    }

    private static double ValueAggregateFunction(double x, EnergyConsumption energyConsumption)
    {
        var signedValue = energyConsumption.ElectricityMeter.IsPositiveCounter ? energyConsumption.Value : -energyConsumption.Value;

        return x + signedValue;
    }

    private static double PrognozeValue(double?[] prevData)
    {
        var value = prevData.Sum(x => x ?? 0) / prevData.Length;
        return value;
    }
}
