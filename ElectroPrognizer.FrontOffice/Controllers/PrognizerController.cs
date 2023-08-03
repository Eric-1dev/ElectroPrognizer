using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class PrognizerController : Controller
{
    private readonly IDictionary<int, string> _months;

    public IPrognizerService PrognizerService { get; set; }

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
        var availableYears = PrognizerService.GetAvailableYears();

        var model = new PrognizerViewModel
        {
            Months = _months,
            Years = availableYears
        };

        return View(model);
    }

    [HttpPost]
    public JsonResult GetTableContent(int year, int month)
    {
        var data = PrognizerService.GetTableContent(year, month);

        if (!data.DayDatas.Any())
            return Fail("Нет данных за выбранный период");

        return Success(data);
    }

    private JsonResult Fail(string message)
    {
        return Json(OperationResult.Fail(message));
    }

    private JsonResult Success<T>(T entity)
    {
        return Json(OperationResult<T>.Success(entity));
    }
}
