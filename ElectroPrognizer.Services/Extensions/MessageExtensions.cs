using System.Globalization;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.Services.Models.XmlModels;
using ElectroPrognizer.Utils.Extensions;

namespace ElectroPrognizer.Services.Extensions;

public static class MessageExtensions
{
    public static List<EnergyConsumption> MapToEnergyComsumption(this Message message)
    {
        var list = new List<EnergyConsumption>();

        var year = int.Parse(message.Datetime.Day.Substring(0, 4));
        var mounth = int.Parse(message.Datetime.Day.Substring(4, 2));
        var day = int.Parse(message.Datetime.Day.Substring(6, 2));

        var reportDate = new DateTime(year, mounth, day);

        foreach (var area in message.Area)
        {
            var substation = new Substation
            {
                Inn = area.Inn,
                Name = area.Name
            };

            foreach (var measuringPoint in area.MeasuringPoint)
            {
                var electricityMeter = new ElectricityMeter
                {
                    Substation = substation,
                    Name = measuringPoint.Name,
                    IsPositiveCounter = true
                };

                foreach (var measuringChannel in measuringPoint.MeasuringChannel)
                {
                    // собираем данные по одному каналу для объединения временных интервалов
                    var measuringChannelData = new List<EnergyConsumption>();

                    foreach (var period in measuringPoint.MeasuringChannel.SelectMany(x => x.Period))
                    {
                        var startDate = ParseDate(period.Start, reportDate);
                        var endDate = ParseDate(period.End, reportDate);

                        var measuringChannelType = Enum.Parse<MeasuringChannelTypeEnum>(measuringChannel.Code);
                        var measuringChannelDescription = measuringChannelType.GetDescription();

                        var energyConsumption = new EnergyConsumption
                        {
                            ElectricityMeter = electricityMeter,
                            MeasuringChannel = new MeasuringChannel { MeasuringChannelType = measuringChannelType, Description = measuringChannelDescription },
                            Value = double.Parse(period.Value, CultureInfo.InvariantCulture),
                            StartDate = startDate,
                            EndDate = endDate,
                        };

                        measuringChannelData.Add(energyConsumption);
                    }

                    var mergedData = MergeHours(measuringChannelData);

                    list.AddRange(mergedData);
                }
            }
        }

        return list;
    }

    private static List<EnergyConsumption> MergeHours(List<EnergyConsumption> measuringChannelData)
    {
        var channelMergedData = new List<EnergyConsumption>();

        foreach (var firstHalfHourData in measuringChannelData.Where(x => x.EndDate.Minute == 30))
        {
            var secondHalfHourData = measuringChannelData.First(x => x.StartDate == firstHalfHourData.EndDate);

            secondHalfHourData.StartDate = firstHalfHourData.StartDate;
            secondHalfHourData.Value += firstHalfHourData.Value;

            channelMergedData.Add(secondHalfHourData);
        }

        return channelMergedData;
    }

    private static DateTime ParseDate(string stringDate, DateTime reportDate)
    {
        var hour = int.Parse(stringDate.Substring(0, 2));
        var minutes = int.Parse(stringDate.Substring(2, 2));
        var resultDate = reportDate.AddHours(hour).AddMinutes(minutes);

        return resultDate;
    }
}
