using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        };

        return View(model);
    }

    [HttpPost]
    public JsonResult GetTableContent(int? substationId, DateTime calculationDate)
    {
        if (substationId == null)
            return Fail("Не выбрана подстанция");

        var data = PrognizerService.GetTableContent(substationId.Value, calculationDate);

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
}
