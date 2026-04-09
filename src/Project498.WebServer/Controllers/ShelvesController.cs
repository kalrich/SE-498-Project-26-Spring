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

    public IActionResult Index()
    {
        ViewBag.CurrentlyReading = _comicService.GetCurrentlyReading();
        ViewBag.UpNext = _comicService.GetUpNext();
        ViewBag.Completed = _comicService.GetCompleted();

        return View();
    }
}