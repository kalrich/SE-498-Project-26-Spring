using Microsoft.AspNetCore.Mvc;

namespace Project498.WebServer.Controllers;

public class ProfileController : Controller
{
    public IActionResult Index()
    {
        var username = HttpContext.Session.GetString("Username") ?? "Guest User";
        var email = HttpContext.Session.GetString("Email") ?? "guest@example.com";

        ViewBag.Username = username;
        ViewBag.Email = email;

        return View();
    }
}