using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Entities.Exceptions;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class EnergyConsumptionSaverService : IEnergyConsumptionSaverService
{
    public IUploadService UploadService { get; set; }

    public void SaveToDatabase(IEnumerable<EnergyConsumption> energyConsumptions, bool overrideExisting)
    {
        using var dbContext = new ApplicationContext();

        UploadService.SetTotalCount(energyConsumptions.Count());

        using var tran = dbContext.Database.BeginTransaction();

        foreach (var energyConsumption in energyConsumptions)
        {
            if (UploadService.IsCanceled)
            {
                tran.Rollback();
                throw new WorkflowException("Загрузка прервана пользователем");
            }

            // Substation
            var existingSubstation = dbContext.Substations.FirstOrDefault(x => x.Inn == energyConsumption.ElectricityMeter.Substation.Inn);
            if (existingSubstation != null)
            {
                energyConsumption.ElectricityMeter.Substation = existingSubstation;
            }
            else
            {
                dbContext.Substations.Add(energyConsumption.ElectricityMeter.Substation);
                dbContext.SaveChanges();
            }

            // Node
            var existingElectricityMeter = dbContext.ElectricityMeters.FirstOrDefault(x => x.Name == energyConsumption.ElectricityMeter.Name);

            if (existingElectricityMeter != null)
            {
                energyConsumption.ElectricityMeter = existingElectricityMeter;
            }
            else
            {
                dbContext.ElectricityMeters.Add(energyConsumption.ElectricityMeter);
                dbContext.SaveChanges();
            }

            // Channel
            var existingMeasureChannel = dbContext.MeasuringChannels.FirstOrDefault(x => x.MeasuringChannelType == energyConsumption.MeasuringChannel.MeasuringChannelType);

            if (existingMeasureChannel != null)
            {
                energyConsumption.MeasuringChannel = existingMeasureChannel;
            }
            else
            {
                dbContext.MeasuringChannels.Add(energyConsumption.MeasuringChannel);
                dbContext.SaveChanges();
            }

            var existingConsumption = dbContext.EnergyConsumptions.FirstOrDefault(x => x.StartDate == energyConsumption.StartDate
            && x.ElectricityMeterId == energyConsumption.ElectricityMeter.Id
            && x.MeasuringChannel.Id == energyConsumption.MeasuringChannel.Id);

            if (existingConsumption != null)
            {
                if (overrideExisting)
                {
                    existingConsumption.Value = energyConsumption.Value;
                    dbContext.EnergyConsumptions.Update(existingConsumption);
                }
            }
            else
            {
                dbContext.EnergyConsumptions.Add(energyConsumption);
            }

            dbContext.SaveChanges();

            UploadService.IncrementCurrentIndex();
        }

        tran.Commit();
    }
}
