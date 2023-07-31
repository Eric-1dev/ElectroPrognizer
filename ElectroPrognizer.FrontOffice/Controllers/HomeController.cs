using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}