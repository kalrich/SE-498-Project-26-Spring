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

        var userId = HttpContext.Session.GetInt32("UserId");
    
        if (userId != null)
        {
            try
            {
                var checkouts = await _checkoutService.GetUserCheckoutsAsync(userId.Value);
                var activeCheckout = checkouts.FirstOrDefault(c => c.ComicId == id && !c.ReturnDate.HasValue);
                ViewBag.ActiveCheckout = activeCheckout;
                ViewBag.IsCheckedOut = activeCheckout != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not check checkout status for comic {id}", id);
                ViewBag.IsCheckedOut = false;
                ViewBag.ActiveCheckout = null;
            }
        }
        else
        {
            ViewBag.IsCheckedOut = false;
            ViewBag.ActiveCheckout = null;
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
