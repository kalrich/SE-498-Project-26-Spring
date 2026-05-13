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
        var userId = HttpContext.Session.GetInt32("UserId");
        var comic = await _comicService.GetByIdAsync(id, userId);
        if (comic == null)
            return NotFound();

        if (userId != null)
        {
            try
            {
                var checkouts = await _checkoutService.GetUserCheckoutsAsync(userId.Value);
                var activeCheckout = checkouts.FirstOrDefault(c => c.ComicId == id && !c.ReturnDate.HasValue);
                ViewBag.ActiveCheckout = activeCheckout;
                ViewBag.IsCheckedOut = activeCheckout != null;
                ViewBag.IsFavorite = await _comicService.GetFavoriteStatusAsync(userId.Value, id);
                ViewBag.UserReview = await _comicService.GetUserReviewAsync(userId.Value, id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not check checkout status for comic {id}", id);
                ViewBag.IsCheckedOut = false;
                ViewBag.ActiveCheckout = null;
                ViewBag.IsFavorite = false;
                ViewBag.UserReview = null;
            }
        }
        else
        {
            ViewBag.IsCheckedOut = false;
            ViewBag.ActiveCheckout = null;
            ViewBag.IsFavorite = false;
            ViewBag.UserReview = null;
        }

        ViewBag.Reviews = await _comicService.GetReviewsAsync(id);

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

    [HttpPost]
    public async Task<IActionResult> ToggleFavorite(int id, bool isFavorite)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await _comicService.SetFavoriteAsync(userId.Value, id, !isFavorite);

        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> SaveReview(int id, int rating, string comment)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await _comicService.SaveReviewAsync(userId.Value, id, Math.Clamp(rating, 1, 5), comment ?? "");

        return RedirectToAction("Details", new { id });
    }
}
