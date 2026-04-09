using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class HomeController : Controller
{
    private readonly IComicService _comicService;

    public HomeController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.Session.GetString("Username");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!string.IsNullOrEmpty(username) && userId.HasValue)
        {
            ViewBag.Username = username;
            ViewBag.FeaturedToday = await _comicService.GetFeaturedTodayAsync();
            ViewBag.TrendingThisWeek = await _comicService.GetTrendingThisWeekAsync();
            ViewBag.CurrentlyReading = await _comicService.GetShelfAsync(userId.Value, "CurrentlyReading");
            ViewBag.UpNext = await _comicService.GetShelfAsync(userId.Value, "UpNext");
            ViewBag.Completed = await _comicService.GetShelfAsync(userId.Value, "Completed");

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