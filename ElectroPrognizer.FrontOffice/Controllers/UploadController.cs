using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using ElectroPrognizer.Utils.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class UploadController : Controller
{
    public IImportFileService ImportFileService { get; set; }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public JsonResult UploadInputFiles(bool overrideExisting)
    {
        var files = Request.Form.Files;

        if (!files.Any())
            return Json(OperationResult.Fail("Не выбраны файлы для импорта"));

        var uploadTask = Task.Run(() =>
        {
            var uploadedFIles = new List<UploadedFile>();

            foreach (var file in files)
            {
                using (var ms = new MemoryStream())
                {
                    file.OpenReadStream().CopyTo(ms);

                    var bytes = ms.ToArray();

                    uploadedFIles.Add(new UploadedFile { Name = file.FileName, Content = bytes });
                }
            }

            var result = ImportFileService.Import(uploadedFIles, overrideExisting);
        });

        UploadTaskHelper.UploadTask = uploadTask;

        return Json(OperationResult.Success());
    }

    [HttpPost]
    public JsonResult GetProgressStatus()
    {
        return Json(UploadTaskHelper.GetProgressStatus());
    }

    [HttpPost]
    public IActionResult CancelUpload()
    {
        UploadTaskHelper.Cancel();

        return Ok();
    }
}