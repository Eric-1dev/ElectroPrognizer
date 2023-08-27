using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Services.Models.Prognizer;
using Microsoft.EntityFrameworkCore;

namespace ElectroPrognizer.Services.Implementation;

public class PrognizerService : IPrognizerService
{
    public IApplicationSettingsService ApplicationSettingsService { get; set; }

    public ConsumptionTableData GetTableContent(int substationId, DateTime calculationDate)
    {
        var analizingDayCount = ApplicationSettingsService.GetIntValue(ApplicationSettingEnum.AnalizingDaysCount);
        var prognozeDayCount = ApplicationSettingsService.GetIntValue(ApplicationSettingEnum.PrognizingDaysCount);

        using var dbContext = new ApplicationContext();

        var consumptionTableData = new ConsumptionTableData
        {
            SubstationId = substationId
        };

        var startAnalizingDate = calculationDate.AddDays(-analizingDayCount - prognozeDayCount).Date;

        // берем analizingDayCount дней от даты рассчета
        var energyConsuptions = dbContext.EnergyConsumptions
            .Where(x => x.ElectricityMeter.SubstationId == substationId
                && x.MeasuringChannel.MeasuringChannelType == MeasuringChannelTypeEnum.ActiveInput
                && x.StartDate.Date >= startAnalizingDate
                && x.StartDate.Date < calculationDate.Date.AddDays(1 - prognozeDayCount))
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

        for (int dayCounter = 0; dayCounter < totalExistingDays; dayCounter++)
        {
            var currentDate = minExistingDate.AddDays(dayCounter).Date;

            dayDatas[dayCounter] = new DayData
            {
                Date = currentDate,
                HourDatas = new HourData[24],
                IsRealData = true,
            };

            for (int hourCounter = 0; hourCounter < 24; hourCounter++)
            {
                var consumptionOnHour = energyConsuptions.Where(x => x.StartDate == currentDate.AddHours(hourCounter)).ToArray();

                double? value = consumptionOnHour.Any()
                    ? consumptionOnHour.Aggregate(0.0, ValueAggregateFunction) + additionalValueConstant
                    : null;

                dayDatas[dayCounter].HourDatas[hourCounter] = new HourData
                {
                    Hour = hourCounter,
                    Value = value
                };
            }

            var totals = CalculateTotalValuesForDay(substationId, currentDate, additionalValueConstant);

            dayDatas[dayCounter].Total = totals.TotalForDay;
            dayDatas[dayCounter].CumulativeTotal = totals.CumulativeTotalForMonth;
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
                Total = null,
                CumulativeTotal = null
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

    public TotalConsumptionValues CalculateTotalValuesForDay(int substationId, DateTime calculationDate, double? additionalValueConstant = null)
    {
        using var dbContext = new ApplicationContext();

        additionalValueConstant ??= dbContext.Substations.Where(x => x.Id == substationId).Select(x => x.AdditionalValueConstant).Single() * 1000;

        var startMonthDate = new DateTime(calculationDate.Year, calculationDate.Month, 1).Date;

        var energyConsuptions = dbContext.EnergyConsumptions
            .Where(x => x.ElectricityMeter.SubstationId == substationId
                && x.MeasuringChannel.MeasuringChannelType == MeasuringChannelTypeEnum.ActiveInput
                && x.StartDate.Date >= startMonthDate
                && x.StartDate.Date <= calculationDate.Date)
            .Include(x => x.ElectricityMeter)
            .ToArray();

        var dayRange = Enumerable.Range(startMonthDate.Day, calculationDate.Date.Day);

        double? cumulativeTotal = 0;
        double? total = null;

        for (var dayCounter = 0; dayCounter < calculationDate.Date.Day; dayCounter++)
        {
            var currentDayStartDate = startMonthDate.AddDays(dayCounter);

            total = 0;

            var valuesForDay = energyConsuptions.Where(x => x.StartDate.Date == currentDayStartDate);

            if (!valuesForDay.Any())
                cumulativeTotal = null;

            var dateGroups = valuesForDay.GroupBy(x => x.StartDate);

            foreach (var group in dateGroups)
            {
                var totalForHour = group.Aggregate(0.0, ValueAggregateFunction) + additionalValueConstant;
                total += totalForHour;
            }

            if (cumulativeTotal != null)
                cumulativeTotal += total;
        }

        return new TotalConsumptionValues(total, cumulativeTotal);
    }

    private static double ValueAggregateFunction(double x, EnergyConsumption energyConsumption)
    {
        var signedValue = energyConsumption.ElectricityMeter.IsPositiveCounter ? energyConsumption.Value : -energyConsumption.Value;

        return x + signedValue;
    }

    private static double? PrognozeValue(double?[] prevData)
    {
        var notNullValues = prevData.Where(x => x.HasValue).Select(x => x.Value).ToArray();

        if (!notNullValues.Any())
            return null;

        //var hourForecast = TimeSeries.AdaptiveRateSmoothing(notNullValues, 1, 0.5, 1);
        var hourForecast = TimeSeries.ExponentialSmoothing(notNullValues, 1, 0.2);

        var value = (double)hourForecast.Rows[notNullValues.Length]["Forecast"];
        //var value = prevData.Sum(x => x.Value) / notNullValues.Length;

        return value;
    }
}
