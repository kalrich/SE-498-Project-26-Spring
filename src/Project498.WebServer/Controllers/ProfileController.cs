using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ProfileController : Controller
{
    private readonly IAuthService _authService;
    private readonly MockComicService _mockComicService;

    public ProfileController(IAuthService authService, MockComicService mockComicService)
    {
        _authService = authService;
        _mockComicService = mockComicService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var sessionEmail = HttpContext.Session.GetString("Email");

        if (string.IsNullOrWhiteSpace(sessionEmail))
        {
            return RedirectToAction("Login", "Auth");
        }

        var user = _authService.GetByEmail(sessionEmail);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = new ProfileViewModel
        {
            Username = user.Username,
            Email = user.Email,
            Password = "",
            TotalBooksRead = _mockComicService.GetCompleted().Count
        };

        ViewBag.Success = TempData["Success"];
        ViewBag.Error = TempData["Error"];

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(ProfileViewModel model)
    {
        var sessionEmail = HttpContext.Session.GetString("Email");

        if (string.IsNullOrWhiteSpace(sessionEmail))
        {
            return RedirectToAction("Login", "Auth");
        }

        model.TotalBooksRead = _mockComicService.GetCompleted().Count;

        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Please correct the form fields.";
            return View(model);
        }

        var updated = _authService.UpdateProfile(
            sessionEmail,
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