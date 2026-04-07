using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ShelvesController : Controller
{
    private readonly MockComicService _mockComicService;

    public ShelvesController(MockComicService mockComicService)
    {
        _mockComicService = mockComicService;
    }

    public IActionResult Index()
    {
        ViewBag.CurrentlyReading = _mockComicService.GetCurrentlyReading();
        ViewBag.UpNext = _mockComicService.GetUpNext();
        ViewBag.Completed = _mockComicService.GetCompleted();

        return View();
    }
}