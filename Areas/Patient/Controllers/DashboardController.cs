using HealthTrack.Controllers;
using HealthTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Areas.Patient.Controllers;

[Area("Patient")]
[Authorize]
public class DashboardController(IPatientService patientService, IExamService examService) : BaseController
{
    private readonly IPatientService _patientService = patientService;
    private readonly IExamService _examService = examService;

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = GetCurrentUserId();
            var dashboardData = await _patientService.GetDashboardDataAsync(userId);
            return View(dashboardData);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }
}