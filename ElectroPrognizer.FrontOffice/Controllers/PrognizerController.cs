using ElectroPrognizer.FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class PrognizerController : Controller
{
    private readonly IDictionary<int, string> _months;

    public PrognizerController()
    {
        _months = new Dictionary<int, string> 
        {
            { 1, "Январь" },
            { 2, "Февраль" },
            { 3, "Март" },
            { 4, "Апрель" },
            { 5, "Май" },
            { 6, "Июнь" },
            { 7, "Июль" },
            { 8, "Август" },
            { 9, "Сентябрь" },
            { 10, "Октябрь" },
            { 11, "Ноябрь" },
            { 12, "Декабрь" }
        };
    }

    public IActionResult Index()
    {
        var model = new PrognizerViewModel
        {
            Months = _months,
            AvailableYears = new[] { 2020 }
        };

        return View(model);
    }
}