using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class RecommendationsController : Controller
{
    private readonly MockComicService _mockComicService;

    public RecommendationsController(MockComicService mockComicService)
    {
        _mockComicService = mockComicService;
    }

    public IActionResult Index()
    {
        ViewBag.Recommended = _mockComicService.GetRecommended();
        ViewBag.BecauseYouRead = _mockComicService.GetBecauseYouRead();
        ViewBag.HiddenGems = _mockComicService.GetHiddenGems();

        return View();
    }
}