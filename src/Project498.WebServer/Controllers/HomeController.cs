using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class HomeController : Controller
{
    private readonly MockComicService _mockComicService;

    public HomeController(MockComicService mockComicService)
    {
        _mockComicService = mockComicService;
    }

    public IActionResult Index()
    {
        var username = HttpContext.Session.GetString("Username");

        if (!string.IsNullOrEmpty(username))
        {
            ViewBag.Username = username;
            ViewBag.FeaturedToday = _mockComicService.GetFeaturedToday();
            ViewBag.TrendingThisWeek = _mockComicService.GetTrendingThisWeek();
            ViewBag.CurrentlyReading = _mockComicService.GetCurrentlyReading();
            ViewBag.UpNext = _mockComicService.GetUpNext();
            ViewBag.Completed = _mockComicService.GetCompleted();
            ViewBag.IReadPicks = _mockComicService.GetIReadPicks();

            return View("Dashboard");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}