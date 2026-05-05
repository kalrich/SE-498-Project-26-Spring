using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ShelvesController : Controller
{
    private readonly IComicService _comicService;

    public ShelvesController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.CurrentlyReading = await _comicService.GetShelfAsync(username, "CurrentlyReading");
        ViewBag.UpNext = await _comicService.GetShelfAsync(username, "UpNext");
        ViewBag.Completed = await _comicService.GetShelfAsync(username, "Completed");

        return View();
    }
}