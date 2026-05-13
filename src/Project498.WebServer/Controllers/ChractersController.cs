using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class CharactersController : Controller
{
    private readonly IDcCharacterService _dcCharacterService;

    public CharactersController(IDcCharacterService dcCharacterService)
    {
        _dcCharacterService = dcCharacterService;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var characters = await _dcCharacterService.GetCharactersAsync();

        return View(characters);
    }

    public async Task<IActionResult> Details(int id)
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var character = await _dcCharacterService.GetCharacterByIdAsync(id);

        if (character == null)
        {
            return NotFound();
        }

        return View(character);
    }
}