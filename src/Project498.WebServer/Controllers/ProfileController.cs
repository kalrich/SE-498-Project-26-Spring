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
        var username = HttpContext.Session.GetString("Username");
        var sessionEmail = HttpContext.Session.GetString("Email");

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(sessionEmail))
        {
            return RedirectToAction("Login", "Auth");
        }

        var user = await _authService.GetByEmailAsync(sessionEmail);

        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var completed = await _comicService.GetShelfAsync(username, "Completed");
        var userId = HttpContext.Session.GetInt32("UserId");
        var favorites = userId.HasValue
            ? await _comicService.GetFavoritesAsync(userId.Value)
            : new List<Comic>();
        var readingHistory = userId.HasValue
            ? await _comicService.GetReadingHistoryAsync(userId.Value)
            : new List<ReadingHistoryItem>();
        var reviews = userId.HasValue
            ? await _comicService.GetUserReviewsAsync(userId.Value)
            : new List<ComicReviewDto>();

        var model = new ProfileViewModel
        {
            Username = user.Username,
            Email = user.Email,
            Password = "",
            TotalBooksRead = completed.Count,
            Favorites = favorites,
            ReadingHistory = readingHistory,
            Reviews = reviews
        };

        ViewBag.Success = TempData["Success"];
        ViewBag.Error = TempData["Error"];

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ProfileViewModel model)
    {
        var username = HttpContext.Session.GetString("Username");
        var sessionEmail = HttpContext.Session.GetString("Email");

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(sessionEmail))
        {
            return RedirectToAction("Login", "Auth");
        }

        var completed = await _comicService.GetShelfAsync(username, "Completed");
        model.TotalBooksRead = completed.Count;
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId.HasValue)
        {
            model.Favorites = await _comicService.GetFavoritesAsync(userId.Value);
            model.ReadingHistory = await _comicService.GetReadingHistoryAsync(userId.Value);
            model.Reviews = await _comicService.GetUserReviewsAsync(userId.Value);
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Please correct the form fields.";
            return View(model);
        }

        var updated = await _authService.UpdateProfileAsync(
            username,
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
