using ElectroPrognizer.FrontOffice.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class DownloadLogController : BaseController
{
    public IDownloadLogService DownloadLogService { get; set; }

    public IActionResult Index()
    {
        var model = new DownloadLogViewModel();

        return View(model);
    }

    public JsonResult GetLogs(DownloadLogViewModel inputFilter)
    {
        inputFilter.PageNumber ??= 1;

        if (!ModelState.IsValid)
            return Fail("Некорректные значения фильтра");

        var filter = new DownloadLogFilter
        {
            PageNumber = inputFilter.PageNumber.Value,
            DateFrom = inputFilter.DateFrom,
            DateTo = inputFilter.DateTo,
        };

        var result = DownloadLogService.GetLogs(filter);

        return Success(result);
    }
}
