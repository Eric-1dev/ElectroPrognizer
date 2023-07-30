using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.DataModel.Models.XmlModels;

namespace ElectroPrognizer.Utils.Helpers;

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
            foreach (var measuringPoint in area.MeasuringPoint)
            {
                var node = new Node
                {
                    Name = measuringPoint.Name
                };

                foreach (var measuringChannel in measuringPoint.MeasuringChannel)
                {

                    foreach (var period in measuringPoint.MeasuringChannel.First(x => x.Code == "01").Period)
                    {
                        var startHour = int.Parse(period.Start.Substring(0, 2));
                        var startMinutes = int.Parse(period.Start.Substring(2, 2));
                        var date = reportDate.AddHours(startHour).AddMinutes(startMinutes);

                        var energyConsumption = new EnergyConsumption
                        {
                            Node = node,
                            MeasuringChannel = new MeasuringChannel { Code = measuringChannel.Code, Desc = measuringChannel.Desc },
                            Value = double.Parse(period.Value),
                            Date = date
                        };

                        list.Add(energyConsumption);
                    }
                }
            }
        }

        return list;
    }
}
