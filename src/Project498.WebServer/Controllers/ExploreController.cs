using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ExploreController : Controller
{
    private readonly MockComicService _mockComicService;

    public ExploreController(MockComicService mockComicService)
    {
        _mockComicService = mockComicService;
    }

    public IActionResult Index(string? query, string? genre)
    {
        ViewBag.Query = query ?? "";
        ViewBag.SelectedGenre = genre ?? "";
        ViewBag.Genres = _mockComicService.GetGenres();
        ViewBag.Comics = _mockComicService.Search(query, genre);

        return View();
    }
}