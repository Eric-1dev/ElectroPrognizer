using ElectroPrognizer.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPrognizer.FrontOffice.Controllers;

public abstract class BaseController : Controller
{
    protected JsonResult Fail(string message)
    {
        return Json(OperationResult.Fail(message));
    }

    protected JsonResult Success<T>(T entity)
    {
        return Json(OperationResult<T>.Success(entity));
    }
}
