using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class SearchController : Controller
{
    private readonly IComicService _comicService;
    private readonly IDcCharacterService _dcCharacterService;
    private readonly ICharacterImageService _characterImageService;
    private readonly IMarvelCharacterService _marvelCharacterService;

    public SearchController(
        IComicService comicService,
        IDcCharacterService dcCharacterService,
        ICharacterImageService characterImageService,
        IMarvelCharacterService marvelCharacterService)
    {
        _comicService = comicService;
        _dcCharacterService = dcCharacterService;
        _characterImageService = characterImageService;
        _marvelCharacterService = marvelCharacterService;
    }

    public async Task<IActionResult> Index(string? query)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        query = query?.Trim() ?? "";
        var userId = HttpContext.Session.GetInt32("UserId");
        var comics = await _comicService.SearchAsync(query, null, null, userId);
        var dcCharacters = await _dcCharacterService.GetCharactersAsync();
        var marvelCharacters = await _marvelCharacterService.GetCharactersAsync();

        await _characterImageService.EnrichWithImagePathsAsync(dcCharacters);

        if (!string.IsNullOrWhiteSpace(query))
        {
            dcCharacters = dcCharacters
                .Where(c => Matches(c.Name, query) || Matches(c.Alias, query) || Matches(c.Description, query))
                .ToList();

            marvelCharacters = marvelCharacters
                .Where(c => Matches(c.Name, query) || Matches(c.Alias, query) || Matches(c.Description, query))
                .ToList();
        }

        var model = new SearchViewModel
        {
            Query = query,
            Comics = comics,
            DcCharacters = dcCharacters,
            MarvelCharacters = marvelCharacters
        };

        return View(model);
    }

    private static bool Matches(string value, string query)
    {
        return value.Contains(query, StringComparison.OrdinalIgnoreCase);
    }
}
