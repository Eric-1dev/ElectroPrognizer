using ElectroPrognizer.DataLayer;
using ElectroPrognizer.Services.Dto;
using ElectroPrognizer.Services.Interfaces;

namespace ElectroPrognizer.Services.Implementation;

public class SubstationService : ISubstationService
{
    private const string NotFoundErrorMessage = "Не найдена подстанция с указанным ID";

    public SubstationDto[] GetAll()
    {
        var dbContext = new ApplicationContext();

        var substationList = dbContext.Substations.ToArray();

        return substationList.Select(x => new SubstationDto
        {
            Id = x.Id,
            Inn = x.Inn,
            Name = x.Name,
            Description = x.Description,
            AdditionalValueConstant = x.AdditionalValueConstant
        }).ToArray();
    }

    public SubstationDto GetById(int id)
    {
        var dbContext = new ApplicationContext();

        var substation = dbContext.Substations.SingleOrDefault(x => x.Id == id);

        if (substation == null)
            throw new Exception(NotFoundErrorMessage);

        return new SubstationDto
        {
            Id = id,
            Inn = substation.Inn,
            Name = substation.Name,
            Description = substation.Description,
            AdditionalValueConstant = substation.AdditionalValueConstant
        };
    }

    public void Save(SubstationDto substation)
    {
        var dbContext = new ApplicationContext();

        var existingSubstation = dbContext.Substations.FirstOrDefault(x => x.Id == substation.Id);

        if (existingSubstation == null)
            throw new Exception(NotFoundErrorMessage);

        existingSubstation.Name = substation.Name;
        existingSubstation.Description = substation.Description;
        existingSubstation.AdditionalValueConstant = substation.AdditionalValueConstant;

        dbContext.SaveChanges();
    }
}
