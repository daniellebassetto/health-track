using HealthTrack.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthTrack.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }

        protected IActionResult HandleError(Exception ex)
        {
            if (ex.Message == ErrorMessages.PatientNotFound)
            {
                return RedirectToAction("Create", "Profile", new { area = "Patient" });
            }
            TempData["Error"] = ex.Message;
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}