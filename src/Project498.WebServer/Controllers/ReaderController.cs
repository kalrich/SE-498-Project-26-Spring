using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ReaderController : Controller
{
    private readonly MockComicService _mockComicService;

    public ReaderController(MockComicService mockComicService)
    {
        _mockComicService = mockComicService;
    }

    public IActionResult Index(int id)
    {
        var comic = _mockComicService.GetById(id);

        if (comic == null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(comic);
    }
}