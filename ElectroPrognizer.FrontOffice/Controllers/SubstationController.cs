using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Dto;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class SubstationController : BaseController
{
    public ISubstationService SubstationService { get; set; }
    public IElectricityMeterService ElectricityMeterService { get; set; }

    public IActionResult Index()
    {
        var substations = SubstationService.GetAll();

        var model = substations.Select(x => new SubstationViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Inn = x.Inn,
        }).ToArray();

        return View(model);
    }

    public IActionResult Edit(int id)
    {
        var substation = SubstationService.GetById(id);

        var meters = ElectricityMeterService.GetBySubstantionId(id);

        var metersViewModel = meters.Select(x => new ElectricityMeterViewModel
        {
            Id = x.Id,
            SubstationId = x.SubstationId,
            Name = x.Name,
            Description = x.Description,
            IsPositiveCounter = x.IsPositiveCounter
        }).ToArray();

        var model = new SubstationViewModel
        {
            Id = substation.Id,
            Inn = substation.Inn,
            Description = substation.Description,
            Name = substation.Name,
            ElectricityMeters = metersViewModel
        };

        return View(model);
    }

    [HttpPost]
    public JsonResult Save(SubstationViewModel substation)
    {
        if (!ModelState.IsValid)
            return Fail("Некорректные данные");

        var dto = new SubstationDto
        {
            Id = substation.Id,
            Name = substation.Name,
            Description = substation.Description,
        };

        try
        {
            SubstationService.Save(dto);
            return Success("Успешно сохранено");
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
