using ElectroPrognizer.DataLayer;
using ElectroPrognizer.DataModel.Entities;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class SubstationService : ISubstationService
{
    public Substation[] GetAll()
    {
        var dbContext = new ApplicationContext();

        return dbContext.Substations.ToArray();
    }

    public Substation GetById(int id)
    {
        var dbContext = new ApplicationContext();

        return dbContext.Substations.FirstOrDefault(s => s.Id == id);
    }
}
