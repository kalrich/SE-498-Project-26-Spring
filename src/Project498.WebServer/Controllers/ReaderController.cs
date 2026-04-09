using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ReaderController : Controller
{
    private readonly IComicService _comicService;

    public ReaderController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Index(int id)
    {
        var comic = await _comicService.GetByIdAsync(id);

        if (comic == null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(comic);
    }
}