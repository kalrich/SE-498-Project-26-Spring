using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class RecommendationsController : Controller
{
    private readonly IComicService _comicService;

    public RecommendationsController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Index(string query = "")
    {
        var recommended = await _comicService.GetRecommendedAsync();
        var becauseYouRead = await _comicService.GetBecauseYouReadAsync();
        var hiddenGems = await _comicService.GetHiddenGemsAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            recommended = recommended
                .Where(c => c.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            c.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            c.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            becauseYouRead = becauseYouRead
                .Where(c => c.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            c.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            c.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            hiddenGems = hiddenGems
                .Where(c => c.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            c.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            c.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        ViewBag.Query = query;
        ViewBag.Recommended = recommended;
        ViewBag.BecauseYouRead = becauseYouRead;
        ViewBag.HiddenGems = hiddenGems;

        return View();
    }
}