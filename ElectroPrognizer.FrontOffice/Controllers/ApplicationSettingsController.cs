using ElectroPrognizer.Entities.Enums;
using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class ApplicationSettingsController : BaseController
{
    public IApplicationSettingsService ApplicationSettingsService { get; set; }

    public IActionResult Index()
    {
        var model = ApplicationSettingsService
            .GetAll()
            .Select(x => new ApplicationSettingViewModel
            {
                Id = (int)x.ApplicationSettingType,
                InternalName = x.InternalName,
                Description = x.Description,
                Value = x.Value
            })
            .ToArray();

        return View(model);
    }

    public IActionResult Edit(int id)
    {
        var setting = ApplicationSettingsService.Get((ApplicationSettingEnum)id);

        var model = new ApplicationSettingViewModel
        {
            Id = (int)setting.ApplicationSettingType,
            InternalName = setting.InternalName,
            Description = setting.Description,
            Value = setting.Value
        };

        return View(model);
    }

    [HttpPost]
    public JsonResult Save(int id, string value)
    {
        var result = ApplicationSettingsService.SetValue((ApplicationSettingEnum)id, value);

        return result.IsSuccess
            ? Success("Успешно сохранено")
            : Fail("Ошибка сохранения");
    }
}
