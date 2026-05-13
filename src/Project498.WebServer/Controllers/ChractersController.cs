using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class CharactersController : Controller
{
    private readonly IDcCharacterService _dcCharacterService;
    private readonly ICharacterImageService _characterImageService;

    public CharactersController(
        IDcCharacterService dcCharacterService,
        ICharacterImageService characterImageService)
    {
        _dcCharacterService = dcCharacterService;
        _characterImageService = characterImageService;
    }

    public async Task<IActionResult> Index()
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var characters = await _dcCharacterService.GetCharactersAsync();
        await _characterImageService.EnrichWithImagePathsAsync(characters);

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

        character.ImagePath = await _characterImageService.GetImagePathAsync(character.Alias);

        return View(character);
    }
}
