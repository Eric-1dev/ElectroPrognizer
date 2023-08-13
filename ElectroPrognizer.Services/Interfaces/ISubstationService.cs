using ElectroPrognizer.Services.Dto;

namespace ElectroPrognizer.Services.Interfaces;

public interface ISubstationService
{
    SubstationDto[] GetAll();

    SubstationDto GetById(int id);

    void Save(SubstationDto substation);
}
