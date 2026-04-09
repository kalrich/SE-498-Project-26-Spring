using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ProfileController : Controller
{
    private readonly IAuthService _authService;
    private readonly IComicService _comicService;

    public ProfileController(IAuthService authService, IComicService comicService)
    {
        _authService = authService;
        _comicService = comicService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var sessionEmail = HttpContext.Session.GetString("Email");

        if (userId == null || string.IsNullOrWhiteSpace(sessionEmail))
        {
            return RedirectToAction("Login", "Auth");
        }

        var user = await _authService.GetByEmailAsync(sessionEmail);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var completed = await _comicService.GetShelfAsync(userId.Value, "Completed");

        var model = new ProfileViewModel
        {
            Username = user.Username,
            Email = user.Email,
            Password = "",
            TotalBooksRead = completed.Count
        };

        ViewBag.Success = TempData["Success"];
        ViewBag.Error = TempData["Error"];

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ProfileViewModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var sessionEmail = HttpContext.Session.GetString("Email");

        if (userId == null || string.IsNullOrWhiteSpace(sessionEmail))
        {
            return RedirectToAction("Login", "Auth");
        }

        var completed = await _comicService.GetShelfAsync(userId.Value, "Completed");
        model.TotalBooksRead = completed.Count;

        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Please correct the form fields.";
            return View(model);
        }

        var updated = await _authService.UpdateProfileAsync(
            userId.Value,
            model.Username,
            model.Email,
            model.Password
        );

        if (!updated)
        {
            ViewBag.Error = "Unable to update profile. That email may already be in use.";
            return View(model);
        }

        HttpContext.Session.SetString("Username", model.Username);
        HttpContext.Session.SetString("Email", model.Email);

        TempData["Success"] = "Profile updated successfully.";
        return RedirectToAction("Index");
    }
}