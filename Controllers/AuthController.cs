using HealthTrack.Core.Services;
using HealthTrack.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Controllers;

public class AuthController(AuthService authService) : Controller
{
    private readonly AuthService _authService = authService;

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.RegisterAsync(model.FirstName, model.LastName, model.Email, model.Password);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        foreach (var error in result.Errors)
        {
            var errorMessage = error.Code switch
            {
                "DuplicateUserName" => "Este email já está em uso.",
                "DuplicateEmail" => "Este email já está em uso.",
                "PasswordTooShort" => "A senha deve ter pelo menos 6 caracteres.",
                "PasswordRequiresDigit" => "A senha deve conter pelo menos um número.",
                _ => error.Description
            };
            ModelState.AddModelError(string.Empty, errorMessage);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}