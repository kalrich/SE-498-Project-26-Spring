using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Dtos;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComicReviewsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ComicReviewsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("comic/{comicId:int}")]
    public async Task<ActionResult<List<ComicReviewResponse>>> GetForComic(int comicId)
    {
        var reviews = await _context.ComicReviews
            .Include(r => r.User)
            .Where(r => r.ComicId == comicId)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync();

        return Ok(reviews.Select(ToResponse).ToList());
    }

    [HttpGet("user/{userId:int}/comic/{comicId:int}")]
    public async Task<ActionResult<ComicReviewResponse?>> GetUserReview(int userId, int comicId)
    {
        var review = await _context.ComicReviews
            .Include(r => r.User)
            .Include(r => r.Comic)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ComicId == comicId);

        return Ok(review == null ? null : ToResponse(review));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<List<ComicReviewResponse>>> GetForUser(int userId)
    {
        var reviews = await _context.ComicReviews
            .Include(r => r.User)
            .Include(r => r.Comic)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync();

        return Ok(reviews.Select(ToResponse).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<ComicReviewResponse>> Save([FromBody] ComicReviewRequest request)
    {
        if (request.Rating is < 1 or > 5)
            return BadRequest("Rating must be between 1 and 5.");

        if (!await _context.Users.AnyAsync(u => u.Id == request.UserId))
            return NotFound("User not found.");

        if (!await _context.Comics.AnyAsync(c => c.Id == request.ComicId))
            return NotFound("Comic not found.");

        var now = DateTime.UtcNow;
        var review = await _context.ComicReviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ComicId == request.ComicId);

        if (review == null)
        {
            review = new ComicReview
            {
                UserId = request.UserId,
                ComicId = request.ComicId,
                CreatedAt = now
            };

            _context.ComicReviews.Add(review);
        }

        review.Rating = request.Rating;
        review.Comment = request.Comment.Trim();
        review.UpdatedAt = now;

        await _context.SaveChangesAsync();

        review.User ??= await _context.Users.FirstOrDefaultAsync(u => u.Id == review.UserId);

        return Ok(ToResponse(review));
    }

    [HttpDelete("user/{userId:int}/comic/{comicId:int}")]
    public async Task<IActionResult> Delete(int userId, int comicId)
    {
        var review = await _context.ComicReviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ComicId == comicId);

        if (review == null)
            return NoContent();

        _context.ComicReviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static ComicReviewResponse ToResponse(ComicReview review)
    {
        return new ComicReviewResponse
        {
            Id = review.Id,
            UserId = review.UserId,
            ComicId = review.ComicId,
            Username = review.User?.Username ?? "",
            ComicTitle = review.Comic?.Title ?? "",
            CoverImagePath = review.Comic?.CoverImagePath ?? "",
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}
