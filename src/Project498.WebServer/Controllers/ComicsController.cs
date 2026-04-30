using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ComicsController : Controller
{
    private readonly IComicService _comicService;

    public ComicsController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Details(int id)
    {
        var comic = await _comicService.GetByIdAsync(id);

        if (comic == null)
        {
            return RedirectToAction("Index", "Explore");
        }

        var allComics = await _comicService.GetAllAsync();

        ViewBag.RelatedComics = allComics
            .Where(c => c.Id != comic.Id &&
                        (c.Genre == comic.Genre || c.SecondaryGenre == comic.SecondaryGenre))
            .Take(3)
            .ToList();

        return View(comic);
    }

    [HttpPost]
    public async Task<IActionResult> AddToShelf(int id, string shelf)
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        await _comicService.AddToShelfAsync(username, id, shelf);

        return RedirectToAction("Details", new { id });
    }
}