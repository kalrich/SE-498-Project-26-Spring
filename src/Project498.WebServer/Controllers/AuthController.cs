using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class AuthController : Controller
{
    private readonly MockAuthService _mockAuthService;

    public AuthController(MockAuthService mockAuthService)
    {
        _mockAuthService = mockAuthService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _mockAuthService.Login(model.Email, model.Password);

        if (user == null)
        {
            ViewBag.Error = "Invalid email or password.";
            return View(model);
        }

        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Email", user.Email);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Signup()
    {
        return View(new SignupViewModel());
    }

    [HttpPost]
    public IActionResult Signup(SignupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_mockAuthService.EmailExists(model.Email))
        {
            ViewBag.Error = "An account with that email already exists.";
            return View(model);
        }

        var user = _mockAuthService.Signup(model.Username, model.Email, model.Password);

        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Email", user.Email);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}