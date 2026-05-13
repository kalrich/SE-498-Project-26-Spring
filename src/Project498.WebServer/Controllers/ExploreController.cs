using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ExploreController : Controller
{
    private readonly IComicService _comicService;

    public ExploreController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Index(string? query, string? genre, string? status)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        ViewBag.Query = query ?? "";
        ViewBag.SelectedGenre = genre ?? "";
        ViewBag.SelectedStatus = status ?? "";
        ViewBag.Genres = await _comicService.GetGenresAsync();
        ViewBag.Comics = await _comicService.SearchAsync(query, genre, status, userId);

        return View();
    }
}
