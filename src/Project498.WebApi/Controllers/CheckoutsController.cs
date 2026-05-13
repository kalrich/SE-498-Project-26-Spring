using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Dtos;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckoutsController : ControllerBase
{
    private const int MaxActiveCheckouts = 3;
    private readonly AppDbContext _context;

    public CheckoutsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<CheckoutResponse>> Create([FromBody] CreateCheckoutRequest request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            return NotFound("User not found.");
        }

        var comicExists = await _context.Comics.AnyAsync(c => c.Id == request.ComicId);
        if (!comicExists)
        {
            return NotFound("Comic not found.");
        }

        var existingActiveCheckout = await _context.Checkouts
            .FirstOrDefaultAsync(c =>
                c.UserId == request.UserId &&
                c.ComicId == request.ComicId &&
                c.ReturnDate == null);

        if (existingActiveCheckout != null)
        {
            return Conflict(ToResponse(existingActiveCheckout));
        }

        var activeCheckoutCount = await _context.Checkouts
            .CountAsync(c => c.UserId == request.UserId && c.ReturnDate == null);

        if (activeCheckoutCount >= MaxActiveCheckouts)
        {
            return BadRequest($"Checkout limit reached. You can have up to {MaxActiveCheckouts} active checkouts at once.");
        }

        var checkoutDate = DateTime.UtcNow;
        var checkout = new Checkout
        {
            UserId = request.UserId,
            ComicId = request.ComicId,
            CheckoutDate = checkoutDate,
            DueDate = checkoutDate.AddDays(14),
            Status = "Active"
        };

        _context.Checkouts.Add(checkout);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = checkout.Id }, ToResponse(checkout));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CheckoutResponse>> GetById(int id)
    {
        var checkout = await _context.Checkouts
            .Include(c => c.Comic)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checkout == null)
        {
            return NotFound();
        }

        return Ok(ToResponse(checkout));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<List<CheckoutResponse>>> GetForUser(int userId, [FromQuery] bool activeOnly = true)
    {
        var checkouts = _context.Checkouts
            .Where(c => c.UserId == userId);

        if (activeOnly)
        {
            checkouts = checkouts.Where(c => c.ReturnDate == null);
        }

        var checkoutList = await checkouts
            .Include(c => c.Comic)
            .OrderByDescending(c => c.CheckoutDate)
            .ToListAsync();

        return Ok(checkoutList.Select(ToResponse).ToList());
    }

    [HttpPut("{id:int}/return")]
    public async Task<IActionResult> Return(int id)
    {
        var checkout = await _context.Checkouts.FirstOrDefaultAsync(c => c.Id == id);

        if (checkout == null)
        {
            return NotFound();
        }

        if (checkout.ReturnDate == null)
        {
            checkout.ReturnDate = DateTime.UtcNow;
            checkout.Status = "Returned";
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    private static CheckoutResponse ToResponse(Checkout checkout)
    {
        var isOverdue = checkout.ReturnDate == null && checkout.DueDate < DateTime.UtcNow;

        return new CheckoutResponse
        {
            CheckoutId = checkout.Id,
            UserId = checkout.UserId,
            ComicId = checkout.ComicId,
            CheckoutDate = checkout.CheckoutDate,
            DueDate = checkout.DueDate,
            ReturnDate = checkout.ReturnDate,
            Status = isOverdue ? "Overdue" : checkout.Status,
            ComicTitle = checkout.Comic?.Title ?? "",
            CoverImagePath = checkout.Comic?.CoverImagePath ?? "",
            IsOverdue = isOverdue
        };
    }
}
