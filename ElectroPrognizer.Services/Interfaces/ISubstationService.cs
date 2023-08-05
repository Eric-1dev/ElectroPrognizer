using ElectroPrognizer.DataModel.Entities;

namespace ElectroPrognizer.Services.Interfaces;

public interface ISubstationService
{
    Substation[] GetAll();

    Substation GetById(int id);
}
