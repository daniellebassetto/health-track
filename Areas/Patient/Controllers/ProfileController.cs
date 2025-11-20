using HealthTrack.Areas.Patient.ViewModels;
using HealthTrack.Controllers;
using HealthTrack.Core.Interfaces.Services;
using HealthTrack.Core.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Areas.Patient.Controllers;

[Area("Patient")]
[Authorize]
public class ProfileController(IPatientService patientService, UserManager<User> userManager) : BaseController
{
    private readonly IPatientService _patientService = patientService;
    private readonly UserManager<User> _userManager = userManager;

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

    public async Task<IActionResult> Create()
    {
        var userId = GetCurrentUserId();
        var user = await _userManager.FindByIdAsync(userId);
        var model = new CreatePatientViewModel
        {
            FirstName = user?.FirstName,
            LastName = user?.LastName,
            Email = user?.Email
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePatientViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();
            await _patientService.CreateAsync(model, userId);
            TempData["Success"] = "Perfil criado com sucesso!";
            return RedirectToAction("Index", "Dashboard");
        }
        catch (InvalidOperationException)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit()
    {
        try
        {
            var userId = GetCurrentUserId();
            var model = await _patientService.GetForEditAsync(userId);
            var user = await _userManager.FindByIdAsync(userId);
            model.FirstName = user?.FirstName;
            model.LastName = user?.LastName;
            model.Email = user?.Email;
            return View(model);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditPatientViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();
            await _patientService.UpdateAsync(model, userId);
            TempData["Success"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("Index", "Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}