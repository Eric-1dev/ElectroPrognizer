using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models.Prognizer;
using Microsoft.AspNetCore.Mvc;
using NUglify.JavaScript.Syntax;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class PrognizerController : BaseController
{
    public ISubstationService SubstationService { get; set; }
    public IPrognizerService PrognizerService { get; set; }
    public IDayReportService DayReportService { get; set; }

    public IActionResult Index()
    {
        var substations = SubstationService.GetAll();

        var model = new PrognizerViewModel
        {
            Substations = substations.Select(s => new SubstationViewModel
            {
                Id = s.Id,
                Name = s.Name
            }).ToArray(),

            StartDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"),
            AdditionalPercent = 0,
        };

        return View(model);
    }

    public IActionResult PreviousPrognozes()
    {
        var now = DateTime.Now;

        var model = new PreviousPrognozesViewModel
        {
            CurrentPeriod = new DateTime(now.Year, now.Month, 1)
        };

        return View(model);
    }

    [HttpPost]
    public JsonResult GetTableContent(int? substationId, DateTime calculationDate, double additionalPercent)
    {
        if (substationId == null)
            return Fail("Не выбрана подстанция");

        var data = PrognizerService.GetTableContent(substationId.Value, calculationDate, additionalPercent);

        if (!data.DayDatas.Any())
            return Fail("Нет данных за выбранный период");

        return Success(data);
    }

    [HttpGet]
    public FileResult GenerateDayReport(int? substationId, DateTime calculationDate)
    {
        if (substationId == null)
            throw new ArgumentException("Не выбрана подстанция", nameof(substationId));

        var data = DayReportService.GenerateDayReport(substationId.Value, calculationDate);

        return File(data.Content, "application/octet-stream", data.Name);
    }

    [HttpPost]
    public JsonResult SavePrognozeToDatabase(int substationId, DateTime prognozeDate, List<HourData> data)
    {
        var result = PrognizerService.SavePrognozeToDatabase(substationId, prognozeDate, data);

        return result.IsSuccess
            ? Success()
            : Fail(result.Message);
    }
}
