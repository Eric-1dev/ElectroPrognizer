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

        //var W1_9 = 80.86 - 65.88 - 11.664;
        //var W1_10 = 0;
        //var W1_11 = 86.4 - 48 - 41.832;
        //var W1_12 = -5.04;

        double calc(double W1_9, double W1_10, double W1_11, double W1_12)
        {
            return 0.0068 +
            0.1758 +
            9 +
            (Math.Pow((W1_9 + 0.0034 + 0.0879), 2) + Math.Pow(W1_10, 2)) * 6.640625 * Math.Pow(10, -7) +
            (Math.Pow((W1_9 + 0.0034 + 0.0879 + 9 + (Math.Pow((W1_9 + 0.0034 + 0.0879), 2) + Math.Pow(W1_10, 2)) * 6.640625 * Math.Pow(10, -7)), 2) + Math.Pow(W1_10, 2)) * 4.733554 * Math.Pow(10, -9) +
            0.01726 +
            0.004798 +
            9 +
            (Math.Pow((W1_11 + 0.0034 + 0.0879), 2) + Math.Pow(W1_12, 2)) * 6.640625 * Math.Pow(10, -7) +
            (Math.Pow((W1_11 + 0.0034 + 0.0879 + 9 + (Math.Pow((W1_11 + 0.0034 + 0.0879), 2) + Math.Pow(W1_12, 2)) * 6.640625 * Math.Pow(10, -7)), 2) + Math.Pow(W1_12, 2)) * 4.733554 * Math.Pow(10, -9) +
            0.01726 +
            0.004798;
        }

        double W1_9 = 0;
        double W1_10 = 0;
        double W1_11 = 0;
        double W1_12 = 0;

        double qqq = 0;

        // ПС 110 кВ ГПП Металлист, ЗРУ-6 кВ, 1 СШ 6 кВ, яч. 10, ввод 6 кВ Т-1
        W1_9 = 0;
        W1_10 = 0;
        W1_11 = 0;
        W1_12 = 0;

        qqq += calc(W1_9, W1_10, W1_11, W1_12);

        // ПС 110 кВ ГПП Металлист, ЗРУ-6 кВ, 2 СШ 6 кВ, яч. 21, ввод 6 кВ Т-2
        W1_9 = 76.8;
        W1_10 = 85.68;
        W1_11 = 0;
        W1_12 = 0;

        qqq += calc(W1_9, W1_10, W1_11, W1_12);

        // ПС 110 кВ ГПП Металлист, ЗРУ-6 кВ, 1 СШ 6 кВ, яч. 2, КЛ-6 кВ
        W1_9 = 65.52;
        W1_10 = 47.4;
        W1_11 = 0;
        W1_12 = 0;

        //qqq -= calc(W1_9, W1_10, W1_11, W1_12);

        // ПС 110 кВ ГПП Металлист, ЗРУ-6 кВ, 2 СШ 6 кВ, яч. 27, КЛ-6 кВ
        W1_9 = 0;
        W1_10 = 5.04;
        W1_11 = 0;
        W1_12 = 0;

        //qqq -= calc(W1_9, W1_10, W1_11, W1_12);

        // ПС 110 кВ ГПП Металлист, ЗРУ-6 кВ, 2 СШ 6 кВ, яч. 19, КЛ-6 кВ
        W1_9 = 0;
        W1_10 = 0;
        W1_11 = 0;
        W1_12 = 0;

        //qqq -= calc(W1_9, W1_10, W1_11, W1_12);

        // ПС 110 кВ ГПП Металлист, ЗРУ-6 кВ, 1 СШ 6 кВ, яч. 12, КЛ-6 кВ
        W1_9 = 0;
        W1_10 = 0;
        W1_11 = 11.304;
        W1_12 = 41.544;

        //qqq -= calc(W1_9, W1_10, W1_11, W1_12);

        return new SubstationDto
        {
            Id = id,
            Inn = substation.Inn,
            Name = substation.Name,
            Description = substation.Description,
            AdditionalValueConstant = substation.AdditionalValueConstant,
            Latitude = substation.Latitude,
            Longitude = substation.Longitude,
            //AdditionalValueConstant = qqq
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
