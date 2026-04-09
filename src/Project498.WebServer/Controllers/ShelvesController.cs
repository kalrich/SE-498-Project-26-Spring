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
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.CurrentlyReading = await _comicService.GetShelfAsync(userId.Value, "CurrentlyReading");
        ViewBag.UpNext = await _comicService.GetShelfAsync(userId.Value, "UpNext");
        ViewBag.Completed = await _comicService.GetShelfAsync(userId.Value, "Completed");

        return View();
    }
}