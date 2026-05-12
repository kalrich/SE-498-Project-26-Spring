using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class ComicsController : Controller
{
    private readonly IComicService _comicService;
    private readonly ICheckoutService _checkoutService;
    private readonly ILogger<ComicsController> _logger;

    public ComicsController(
        IComicService comicService,
        ICheckoutService checkoutService,
        ILogger<ComicsController> logger)
    {
        _comicService = comicService;
        _checkoutService = checkoutService;
        _logger = logger;
    }

    public async Task<IActionResult> Details(int id)
    {
        var comic = await _comicService.GetByIdAsync(id);
        if (comic == null)
            return NotFound();

        // Phase 1: Check if user has checked out this comic
        var dcUserIdStr = HttpContext.Session.GetString("DcUserId");
        var dcJwt = HttpContext.Session.GetString("DcJwt");
    
        if (!string.IsNullOrEmpty(dcUserIdStr) && !string.IsNullOrEmpty(dcJwt) && int.TryParse(dcUserIdStr, out var dcUserId))
        {
            try
            {
                var checkouts = await _checkoutService.GetUserCheckoutsAsync(dcUserId, dcJwt);
                ViewBag.IsCheckedOut = checkouts?.Any(c => c.ComicId == id && !c.ReturnDate.HasValue) ?? false;
            }
            catch (Exception ex)
            {
                // Phase 1: API not available, default to showing checkout button
                _logger.LogWarning(ex, "Could not check checkout status for comic {id}", id);
                ViewBag.IsCheckedOut = false;
            }
        }
        else
        {
            ViewBag.IsCheckedOut = false;
        }

        return View(comic);
    }

    [HttpPost]
    public async Task<IActionResult> AddToShelf(int id, string shelf)
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        await _comicService.AddToShelfAsync(username, id, shelf);

        return RedirectToAction("Details", new { id });
    }
}