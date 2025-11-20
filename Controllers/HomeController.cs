using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Controllers;

public class HomeController() : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard", new { area = "Patient" });

        return View();
    }
}
