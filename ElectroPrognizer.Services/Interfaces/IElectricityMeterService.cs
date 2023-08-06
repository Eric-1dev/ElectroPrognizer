using ElectroPrognizer.Services.Dto;

namespace ElectroPrognizer.Services.Interfaces;

public interface IElectricityMeterService
{
    ElectricityMeterDto GetById(int id);

    ElectricityMeterDto[] GetBySubstantionId(int substantionId);

    void Save(ElectricityMeterDto electricityMeter);
}
