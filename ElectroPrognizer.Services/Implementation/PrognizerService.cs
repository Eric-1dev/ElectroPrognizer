using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Services.Models.Prognizer;
using Microsoft.EntityFrameworkCore;

namespace ElectroPrognizer.Services.Implementation;

public class PrognizerService : IPrognizerService
{
    public IApplicationSettingsService ApplicationSettingsService { get; set; }

    public ConsumptionTableData GetTableContent(int substationId, DateTime calculationDate, double additionalPercent)
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
                    Value = value + (value * additionalPercent) / 100
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

    public OperationResult SavePrognozeToDatabase(int substationId, DateTime prognozeDate, List<HourData> data)
    {
        try
        {
            using var dbContext = new ApplicationContext();

            var existingValues = dbContext.PrognozedDatas
                .Where(x => x.SubstationId == substationId
                    && x.PrognozeDate >= prognozeDate.Date
                    && x.PrognozeDate < prognozeDate.Date.AddDays(1))
                .ToArray();

            dbContext.PrognozedDatas.RemoveRange(existingValues);

            var newData = data.Select(x => new PrognozedData
            {
                SubstationId = substationId,
                PrognozeDate = prognozeDate.Date.AddHours(x.Hour),
                Value = x.Value.Value
            }).ToArray();

            dbContext.PrognozedDatas.AddRange(newData);

            dbContext.SaveChanges();

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Fail(ex.Message);
        }
    }

    public int[] GetSavedPrognozesYearsArray()
    {
        using var dbContext = new ApplicationContext();

        var years = dbContext.PrognozedDatas
            .Select(x => x.PrognozeDate.Year)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        return years;
    }

    public PrevPrognozeTableData GetPrevPrognozeTableContent(int substationId, int year, int month)
    {
        using var dbContext = new ApplicationContext();

        var dateStart = new DateTime(year, month, 1);
        var dateEnd = dateStart.AddMonths(1);

        var additionalValueConstant = dbContext.Substations.Where(x => x.Id == substationId).Select(x => x.AdditionalValueConstant).Single() * 1000;

        var savedDatas = dbContext.PrognozedDatas
            .Where(x => x.SubstationId == substationId && x.PrognozeDate >= dateStart && x.PrognozeDate < dateEnd)
            .OrderBy(x => x.PrognozeDate)
            .ToArray();

        var energyConsuptions = dbContext.EnergyConsumptions
            .Where(x => x.ElectricityMeter.SubstationId == substationId
                && x.MeasuringChannel.MeasuringChannelType == MeasuringChannelTypeEnum.ActiveInput
                && x.StartDate >= dateStart
                && x.StartDate < dateEnd)
            .Include(x => x.ElectricityMeter)
            .ToArray();

        var result = new PrevPrognozeTableData();

        foreach (var savedData in savedDatas)
        {
            var allRealData = energyConsuptions.Where(x => x.StartDate == savedData.PrognozeDate).ToArray();
            var hourData = allRealData.Aggregate(0.0, ValueAggregateFunction) + additionalValueConstant;

            var day = result.Days.FirstOrDefault(x => x.Date == savedData.PrognozeDate.Date);
            if (day == null)
            {
                day = new PrevPrognozeDayData(savedData.PrognozeDate.Date);
                result.Days.Add(day);
            }

            day.Hours.Add(new PrevPrognozeHourData
            {
                Hour = savedData.PrognozeDate.Hour,
                PrognozedValue = savedData.Value * 1000,
                RealValue = hourData
            });
        }

        return result;
    }
}
