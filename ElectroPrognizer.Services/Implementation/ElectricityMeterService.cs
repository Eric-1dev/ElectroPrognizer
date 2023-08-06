using ElectroPrognizer.DataLayer;
using ElectroPrognizer.Services.Dto;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class ElectricityMeterService : IElectricityMeterService
{
    private const string NotFoundErrorMessage = "Не найден счетчик с указанным ID";

    public ElectricityMeterDto GetById(int id)
    {
        var dbContext = new ApplicationContext();

        var electricityMeter = dbContext.ElectricityMeters.SingleOrDefault(x => x.Id == id);

        if (electricityMeter == null)
            throw new Exception(NotFoundErrorMessage);

        return new ElectricityMeterDto
        {
            Id = id,
            SubstationId = electricityMeter.SubstationId,
            Name = electricityMeter.Name,
            Description = electricityMeter.Description,
            IsPositiveCounter = electricityMeter.IsPositiveCounter
        };
    }

    public ElectricityMeterDto[] GetBySubstantionId(int substantionId)
    {
        var dbContext = new ApplicationContext();

        var electricityMeterList = dbContext.ElectricityMeters.Where(x => x.SubstationId == substantionId).ToArray();

        return electricityMeterList.Select(x => new ElectricityMeterDto
        {
            Id = x.Id,
            SubstationId = x.SubstationId,
            Name = x.Name,
            Description = x.Description,
            IsPositiveCounter = x.IsPositiveCounter
        }).ToArray();
    }
}
