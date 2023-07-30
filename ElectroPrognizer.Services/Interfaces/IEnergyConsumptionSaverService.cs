using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Models;

namespace ElectroPrognizer.Services.Interfaces
{
    public interface IEnergyConsumptionSaverService
    {
        void SaveToDatabase(IEnumerable<EnergyConsumption> energyConsumptions, ref SaverProgressModel progress);
    }
}
