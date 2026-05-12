using Microsoft.AspNetCore.Mvc;
using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebServer.Controllers;

public class CheckoutController : Controller  
{
    private readonly ICheckoutService _checkoutService;
    private readonly IComicService _comicService;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(ICheckoutService checkoutService, IComicService comicService, ILogger<CheckoutController> logger)
    {
        _checkoutService = checkoutService;
        _comicService = comicService;
        _logger = logger;
    }

    // GET /Checkout/Initiate
    public async Task<IActionResult> Initiate(int comicId)
    {
        // Check if user is logged in via session
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var comic = await _comicService.GetByIdAsync(comicId);
        if (comic == null)
            return NotFound();

        return View(comic);
    }

    // POST /Checkout/Process
    // Processes the checkout form submission
    [HttpPost]
    public async Task<IActionResult> Process(int comicId)
    {
        var username = HttpContext.Session.GetString("Username");
    
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            // Phase 1: Mock checkout response (no DC API call yet)
            var mockCheckout = new CheckoutDto
            {
                CheckoutId = comicId * 1000 + new Random().Next(100, 999),
                ComicId = comicId,
                CheckoutDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                ReturnDate = null,
                Status = "Active"
            };

            return RedirectToAction("Confirmed", new { checkoutId = mockCheckout.CheckoutId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Checkout process failed for user {username}, comic {comicId}", username, comicId);
            TempData["Error"] = "An unexpected error occurred during checkout";
            return RedirectToAction("Initiate", new { comicId });
        }
    }
    
    // GET /Checkout/Confirmed?checkoutId=X
    // Shows the checkout confirmation page with due date
    public IActionResult Confirmed(int checkoutId)
    {
        // Phase 1: Mock checkout data (no API call)
        var checkout = new CheckoutDto
        {
            CheckoutId = checkoutId,
            ComicId = checkoutId / 1000,
            CheckoutDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(14),
            ReturnDate = null,
            Status = "Active"
        };

        return View(checkout);
    }

    // GET /Checkout/Active
    // Shows the user's active (unreturned) checkouts
    public IActionResult Active()
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        // Phase 1: Mock checkouts list (no API call)
        var mockCheckouts = new List<CheckoutDto>
        {
            new CheckoutDto
            {
                CheckoutId = 1001,
                ComicId = 1,
                CheckoutDate = DateTime.Now.AddDays(-3),
                DueDate = DateTime.Now.AddDays(11),
                ReturnDate = null,
                Status = "Active"
            }
        };

        return View(mockCheckouts);
    }

    // POST /Checkout/{checkoutId}/Return
// Returns a checked-out comic
    [HttpPost]
    public IActionResult Return(int checkoutId)
    {
        var username = HttpContext.Session.GetString("Username");
    
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            // Phase 1: Mock return (no API call)
            TempData["Success"] = "Comic returned successfully";
            return RedirectToAction("Index", "Explore");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to return checkout {checkoutId}", checkoutId);
            TempData["Error"] = "An error occurred while returning the comic";
            return RedirectToAction("Index", "Explore");
        }
    }
}