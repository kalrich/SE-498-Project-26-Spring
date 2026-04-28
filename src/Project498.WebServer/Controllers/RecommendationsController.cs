using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class RecommendationsController : Controller
{
    private readonly IComicService _comicService;

    public RecommendationsController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Recommended = await _comicService.GetRecommendedAsync();
        ViewBag.BecauseYouRead = await _comicService.GetBecauseYouReadAsync();
        ViewBag.HiddenGems = await _comicService.GetHiddenGemsAsync();

        return View();
    }
}