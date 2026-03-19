using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;

namespace Project498.WebServer.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var username = HttpContext.Session.GetString("Username");

        if (!string.IsNullOrEmpty(username))
        {
            ViewBag.Username = username;
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