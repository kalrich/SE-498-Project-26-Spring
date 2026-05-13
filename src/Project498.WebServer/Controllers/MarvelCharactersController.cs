using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class MarvelCharactersController : Controller
{
    private readonly IMarvelCharacterService _marvelCharacterService;

    public MarvelCharactersController(IMarvelCharacterService marvelCharacterService)
    {
        _marvelCharacterService = marvelCharacterService;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var characters = await _marvelCharacterService.GetCharactersAsync();
        return View(characters);
    }

    public async Task<IActionResult> Details(int id)
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var character = await _marvelCharacterService.GetCharacterByIdAsync(id);

        if (character == null)
        {
            return NotFound();
        }

        return View(character);
    }
}
