using ElectroPrognizer.DataModel.Entities;

namespace ElectroPrognizer.Services.Interfaces;

public interface IEnergyConsumptionSaverService
{
    void SaveToDatabase(IEnumerable<EnergyConsumption> energyConsumptions, bool overrideExisting);
}
