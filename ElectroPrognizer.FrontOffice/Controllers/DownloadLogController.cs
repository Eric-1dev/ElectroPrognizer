using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Utils.Constants;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class DownloadLogController : BaseController
{
    public IDownloadLogService DownloadLogService { get; set; }

    public IActionResult Index()
    {
        var pageSize = DownloadLogConstants.PageSize;

        var totalCount = DownloadLogService.GetTotalCount();

        var totalPages = (totalCount + pageSize - 1) / pageSize;

        return View(totalPages);
    }

    public JsonResult GetLogs(DownloadLogFilter filter)
    {
        var entities = DownloadLogService.GetLogs(filter);

        return Success(entities);
    }
}
