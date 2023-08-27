using ElectroPrognizer.Entities.Models;
using ElectroPrognizer.Services.Interfaces;
using ElectroPrognizer.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public class UploadController : BaseController
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

        var uploadedFiles = new List<FileData>();

        foreach (var file in files)
        {
            using var ms = new MemoryStream();
            using var fileStream = file.OpenReadStream();

            fileStream.CopyTo(ms);

            var bytes = ms.ToArray();

            uploadedFiles.Add(new FileData(file.FileName, bytes));
        }

        var uploadTask = Task.Run(() =>
        {
            ImportFileService.Import(uploadedFiles, overrideExisting);
        });

        return Json(OperationResult.Success());
    }
}
