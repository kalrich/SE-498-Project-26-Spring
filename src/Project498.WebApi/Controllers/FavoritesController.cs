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
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _context;

    public FavoritesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<List<Comic>>> GetForUser(int userId)
    {
        var comics = await _context.FavoriteComics
            .Include(f => f.Comic)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => f.Comic!)
            .ToListAsync();

        foreach (var comic in comics)
        {
            comic.IsFavorite = true;
            await AddRatingSummary(comic);
        }

        return Ok(comics);
    }

    [HttpGet("user/{userId:int}/comic/{comicId:int}")]
    public async Task<ActionResult<FavoriteStatusResponse>> GetStatus(int userId, int comicId)
    {
        var isFavorite = await _context.FavoriteComics
            .AnyAsync(f => f.UserId == userId && f.ComicId == comicId);

        return Ok(new FavoriteStatusResponse
        {
            UserId = userId,
            ComicId = comicId,
            IsFavorite = isFavorite
        });
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteStatusResponse>> Add([FromBody] FavoriteRequest request)
    {
        if (!await _context.Users.AnyAsync(u => u.Id == request.UserId))
            return NotFound("User not found.");

        if (!await _context.Comics.AnyAsync(c => c.Id == request.ComicId))
            return NotFound("Comic not found.");

        var exists = await _context.FavoriteComics
            .AnyAsync(f => f.UserId == request.UserId && f.ComicId == request.ComicId);

        if (!exists)
        {
            _context.FavoriteComics.Add(new FavoriteComic
            {
                UserId = request.UserId,
                ComicId = request.ComicId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        return Ok(new FavoriteStatusResponse
        {
            UserId = request.UserId,
            ComicId = request.ComicId,
            IsFavorite = true
        });
    }

    [HttpDelete("user/{userId:int}/comic/{comicId:int}")]
    public async Task<ActionResult<FavoriteStatusResponse>> Remove(int userId, int comicId)
    {
        var favorite = await _context.FavoriteComics
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ComicId == comicId);

        if (favorite != null)
        {
            _context.FavoriteComics.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        return Ok(new FavoriteStatusResponse
        {
            UserId = userId,
            ComicId = comicId,
            IsFavorite = false
        });
    }

    private async Task AddRatingSummary(Comic comic)
    {
        var ratings = await _context.ComicReviews
            .Where(r => r.ComicId == comic.Id)
            .Select(r => r.Rating)
            .ToListAsync();

        comic.ReviewCount = ratings.Count;
        comic.AverageRating = ratings.Count == 0 ? 0 : Math.Round(ratings.Average(), 1);
    }
}
