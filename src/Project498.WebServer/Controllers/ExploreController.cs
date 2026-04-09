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

    public async Task<IActionResult> Index(string? query, string? genre)
    {
        ViewBag.Query = query ?? "";
        ViewBag.SelectedGenre = genre ?? "";
        ViewBag.Genres = await _comicService.GetGenresAsync();
        ViewBag.Comics = await _comicService.SearchAsync(query, genre);

        return View();
    }
}