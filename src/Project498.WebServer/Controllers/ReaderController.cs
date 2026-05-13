using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ReaderController : Controller
{
    private readonly IComicService _comicService;

    public ReaderController(IComicService comicService)
    {
        _comicService = comicService;
    }

    public async Task<IActionResult> Index(int id)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var comic = await _comicService.GetByIdAsync(id);

        if (comic == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var progress = await _comicService.GetReadingProgressAsync(username, id);
        ViewBag.CurrentPage = Math.Max(1, progress.CurrentPage);

        return View(comic);
    }

    [HttpPost]
    public async Task<IActionResult> SaveProgress([FromBody] SaveReaderProgressRequest request)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }

        if (request.ComicId <= 0)
        {
            return BadRequest();
        }

        await _comicService.UpdateReadingProgressAsync(
            username,
            request.ComicId,
            Math.Clamp(request.ProgressPercent, 0, 100),
            Math.Max(1, request.CurrentPage));

        return NoContent();
    }
}

public class SaveReaderProgressRequest
{
    public int ComicId { get; set; }
    public int ProgressPercent { get; set; }
    public int CurrentPage { get; set; } = 1;
}
