using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class PrognizerController : Controller
{
    public IPrognizerService PrognizerService { get; set; }

    public IActionResult Index()
    {
        var model = new PrognizerViewModel
        {
            StartDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"),
        };

        return View(model);
    }

    [HttpPost]
    public JsonResult GetTableContent(DateTime calculationDate)
    {
        var data = PrognizerService.GetTableContent(calculationDate);

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
