using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class SubstationController : BaseController
{
    public ISubstationService SubstationService { get; set; }

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

        var model = new SubstationViewModel
        {
            Id = substation.Id,
            Inn = substation.Inn,
            Description = substation.Description,
            Name = substation.Name
        };

        return View(model);
    }
}
