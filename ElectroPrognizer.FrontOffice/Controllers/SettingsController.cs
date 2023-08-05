using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class SettingsController : BaseController
{
    public ISubstationService SubstationService { get; set; }

    public IActionResult Index()
    {
        return View();
    }
}
