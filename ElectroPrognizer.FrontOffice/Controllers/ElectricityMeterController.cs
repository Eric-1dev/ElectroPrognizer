using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class ElectricityMeterController : BaseController
{
    public IElectricityMeterService ElectricityMeterService { get; set; }

    public IActionResult Edit(int id)
    {
        var electricityMeter = ElectricityMeterService.GetById(id);

        var model = new ElectricityMeterViewModel
        {
            Id = id,
            SubstationId = electricityMeter.SubstationId,
            Name = electricityMeter.Name,
            Description = electricityMeter.Description,
            IsPositiveCounter = electricityMeter.IsPositiveCounter
        };

        return View(model);
    }
}
