using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Dto;
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

    [HttpPost]
    public JsonResult Save(ElectricityMeterViewModel electricityMeter)
    {
        if (!ModelState.IsValid)
            return Fail("Некорректные данные");

        var dto = new ElectricityMeterDto
        {
            Id = electricityMeter.Id,
            Description = electricityMeter.Description,
            IsPositiveCounter = electricityMeter.IsPositiveCounter
        };

        try
        {
            ElectricityMeterService.Save(dto);
            return Success("Успешно сохранено");
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
