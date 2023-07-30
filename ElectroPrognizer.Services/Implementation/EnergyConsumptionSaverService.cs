using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Implementation;

public class EnergyConsumptionSaverService : IEnergyConsumptionSaverService
{
    public void SaveToDatabase(IEnumerable<EnergyConsumption> energyConsumptions, ref SaverProgressModel progress)
    {
        var dbContext = new ApplicationContext();
        
        progress.TotalCount = energyConsumptions.Count();
        progress.CurrentIndex = 0;


        foreach (var energyConsumption in energyConsumptions)
        {
            // порефачить добавление чилдренов - вынести в дженерик метод

            // Node
            var existingNode = dbContext.Nodes.FirstOrDefault(x => x.Name == energyConsumption.Node.Name);

            if (existingNode != null)
            {
                energyConsumption.Node = existingNode;
            }
            else
            {
                dbContext.Nodes.Add(energyConsumption.Node);
                dbContext.SaveChanges();
            }

            // Channel
            var existingMeasureChannel = dbContext.MeasuringChannels.FirstOrDefault(x => x.Code == energyConsumption.MeasuringChannel.Code);

            if (existingMeasureChannel != null)
            {
                energyConsumption.MeasuringChannel = existingMeasureChannel;
            }
            else
            {
                dbContext.MeasuringChannels.Add(energyConsumption.MeasuringChannel);
                dbContext.SaveChanges();
            }

            // порефачить - сделать метод добавления, в который передается экспрешны с чилдрами (where T : IdentityEntity)
            // EnergyConsumption
            var existingConsumption = dbContext.EnergyConsumptions.FirstOrDefault(x => x.Date == energyConsumption.Date
            && x.NodeId == energyConsumption.Node.Id
            && x.MeasuringChannel.Id == energyConsumption.MeasuringChannel.Id);

            if (existingConsumption != null)
            {
                existingConsumption.Value = energyConsumption.Value;
                dbContext.EnergyConsumptions.Update(existingConsumption);
            }
            else
            {
                dbContext.EnergyConsumptions.Add(energyConsumption);
            }

            dbContext.SaveChanges();

            progress.CurrentIndex++;
        }
    }
}
