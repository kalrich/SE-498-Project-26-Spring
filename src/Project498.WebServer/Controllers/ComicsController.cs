using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ComicsController : Controller
{
    private readonly MockComicService _mockComicService;

    public ComicsController(MockComicService mockComicService)
    {
        _mockComicService = mockComicService;
    }

    public IActionResult Details(int id)
    {
        var comic = _mockComicService.GetById(id);

        if (comic == null)
        {
            return RedirectToAction("Index", "Explore");
        }

        ViewBag.RelatedComics = _mockComicService
            .GetAll()
            .Where(c => c.Id != comic.Id &&
                        (c.Genre == comic.Genre || c.SecondaryGenre == comic.SecondaryGenre))
            .Take(3)
            .ToList();

        return View(comic);
    }

    [HttpPost]
    public IActionResult AddToShelf(int id, string shelf)
    {
        _mockComicService.UpdateShelf(id, shelf);
        return RedirectToAction("Details", new { id });
    }
}