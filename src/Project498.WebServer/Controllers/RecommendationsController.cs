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

    public IActionResult Index()
    {
        ViewBag.Recommended = _comicService.GetRecommended();
        ViewBag.BecauseYouRead = _comicService.GetBecauseYouRead();
        ViewBag.HiddenGems = _comicService.GetHiddenGems();

        return View();
    }
}