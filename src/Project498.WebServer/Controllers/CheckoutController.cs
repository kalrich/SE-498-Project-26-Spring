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
        var userId = HttpContext.Session.GetInt32("UserId");
    
        if (string.IsNullOrEmpty(username) || userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var result = await _checkoutService.InitiateCheckoutAsync(userId.Value, comicId);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Unable to checkout comic";
                return RedirectToAction("Initiate", new { comicId });
            }

            return RedirectToAction("Confirmed", new { checkoutId = result.CheckoutId });
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
    public async Task<IActionResult> Confirmed(int checkoutId)
    {
        var username = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Auth");
        }

        var checkout = await _checkoutService.GetCheckoutAsync(checkoutId);
        if (checkout == null)
        {
            return NotFound();
        }

        return View(checkout);
    }

    // GET /Checkout/Active
    // Shows the user's active (unreturned) checkouts
    public async Task<IActionResult> Active()
    {
        var username = HttpContext.Session.GetString("Username");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (string.IsNullOrEmpty(username) || userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var checkouts = await _checkoutService.GetUserCheckoutsAsync(userId.Value);

        return View(checkouts);
    }

    // POST /Checkout/{checkoutId}/Return
// Returns a checked-out comic
    [HttpPost]
    public async Task<IActionResult> Return(int checkoutId)
    {
        var username = HttpContext.Session.GetString("Username");
        var userId = HttpContext.Session.GetInt32("UserId");
    
        if (string.IsNullOrEmpty(username) || userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var returned = await _checkoutService.ReturnCheckoutAsync(checkoutId);
            if (returned)
            {
                TempData["Success"] = "Comic returned successfully";
            }
            else
            {
                TempData["Error"] = "Unable to return the comic";
            }

            return RedirectToAction("Active");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to return checkout {checkoutId}", checkoutId);
            TempData["Error"] = "An error occurred while returning the comic";
            return RedirectToAction("Active");
        }
    }
}
