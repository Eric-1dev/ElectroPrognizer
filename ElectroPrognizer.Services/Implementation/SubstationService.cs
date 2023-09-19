using ElectroPrognizer.DataLayer;
using ElectroPrognizer.Services.Dto;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class SubstationService : ISubstationService
{
    private const string NotFoundErrorMessage = "Не найдена подстанция с указанным ID";

    public SubstationDto[] GetAll()
    {
        using var dbContext = new ApplicationContext();

        var substationList = dbContext.Substations.ToArray();

        return substationList.Select(x => new SubstationDto
        {
            Id = x.Id,
            Inn = x.Inn,
            Name = x.Name,
            Description = x.Description,
            AdditionalValueConstant = x.AdditionalValueConstant,
            Latitude = x.Latitude,
            Longitude = x.Longitude
        }).ToArray();
    }

    public SubstationDto GetById(int id)
    {
        using var dbContext = new ApplicationContext();

        var substation = dbContext.Substations.SingleOrDefault(x => x.Id == id);

        if (substation == null)
            throw new Exception(NotFoundErrorMessage);

        return new SubstationDto
        {
            Id = id,
            Inn = substation.Inn,
            Name = substation.Name,
            Description = substation.Description,
            AdditionalValueConstant = substation.AdditionalValueConstant,
            Latitude = substation.Latitude,
            Longitude = substation.Longitude,
        };
    }

    public void Save(SubstationDto substation)
    {
        using var dbContext = new ApplicationContext();

        var existingSubstation = dbContext.Substations.FirstOrDefault(x => x.Id == substation.Id);

        if (existingSubstation == null)
            throw new Exception(NotFoundErrorMessage);

        existingSubstation.Name = substation.Name;
        existingSubstation.Description = substation.Description;
        existingSubstation.AdditionalValueConstant = substation.AdditionalValueConstant;
        existingSubstation.Latitude = substation.Latitude;
        existingSubstation.Longitude = substation.Longitude;

        dbContext.SaveChanges();
    }
}
