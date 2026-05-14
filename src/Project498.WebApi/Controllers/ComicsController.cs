using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComicsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ComicsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Comic>>> GetAll(
        [FromQuery] string? query,
        [FromQuery] string? genre,
        [FromQuery] string? status,
        [FromQuery] int? userId)
    {
        var comics = _context.Comics.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            comics = comics.Where(c =>
                c.Title.ToLower().Contains(q) ||
                c.Author.ToLower().Contains(q) ||
                c.Description.ToLower().Contains(q) ||
                c.SeriesName.ToLower().Contains(q));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            comics = comics.Where(c =>
                c.Genre == genre || c.SecondaryGenre == genre);
        }

        if (!string.IsNullOrWhiteSpace(status) && userId.HasValue)
        {
            var activeComicIds = _context.Checkouts
                .Where(c => c.UserId == userId.Value && c.ReturnDate == null)
                .Select(c => c.ComicId);

            if (status.Equals("checkedout", StringComparison.OrdinalIgnoreCase))
            {
                comics = comics.Where(c => activeComicIds.Contains(c.Id));
            }
            else if (status.Equals("available", StringComparison.OrdinalIgnoreCase))
            {
                comics = comics.Where(c => !activeComicIds.Contains(c.Id));
            }
        }

        var results = await comics.ToListAsync();
        await AddUserState(results, userId);

        return Ok(results);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Comic>> GetById(int id, [FromQuery] int? userId)
    {
        var comic = await _context.Comics.FirstOrDefaultAsync(c => c.Id == id);

        if (comic == null)
        {
            return NotFound();
        }

        await AddUserState(new List<Comic> { comic }, userId);

        return Ok(comic);
    }

    [HttpGet("genres")]
    public async Task<ActionResult<List<string>>> GetGenres()
    {
        var comics = await _context.Comics.ToListAsync();

        var genres = comics
            .SelectMany(c => new[] { c.Genre, c.SecondaryGenre })
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();

        return Ok(genres);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<List<Comic>>> GetFeatured()
    {
        return Ok(await _context.Comics.Where(c => c.IsIReadPick).ToListAsync());
    }

    [HttpGet("trending")]
    public async Task<ActionResult<List<Comic>>> GetTrending()
    {
        return Ok(await _context.Comics.OrderBy(c => c.Id).Take(6).ToListAsync());
    }

    [HttpGet("recommended")]
    public async Task<ActionResult<List<Comic>>> GetRecommended()
    {
        return Ok(await _context.Comics.OrderByDescending(c => c.Id).Take(6).ToListAsync());
    }

    [HttpGet("because-you-read")]
    public async Task<ActionResult<List<Comic>>> GetBecauseYouRead()
    {
        return Ok(await _context.Comics.OrderBy(c => c.Title).Take(6).ToListAsync());
    }

    [HttpGet("hidden-gems")]
    public async Task<ActionResult<List<Comic>>> GetHiddenGems()
    {
        return Ok(await _context.Comics.OrderBy(c => c.Id).Take(6).ToListAsync());
    }
    
    [HttpGet("series/{seriesName}")]
    public async Task<ActionResult<List<Comic>>> GetBySeries(string seriesName)
    {
        var comics = await _context.Comics
            .Where(c => c.SeriesName.ToLower() == seriesName.ToLower())
            .OrderBy(c => c.VolumeNumber)
            .ThenBy(c => c.IssueNumber)
            .ToListAsync();

        return Ok(comics);
    }
    
    private async Task AddUserState(List<Comic> comics, int? userId)
    {
        if (comics.Count == 0)
            return;

        var comicIds = comics.Select(c => c.Id).ToList();
        var ratingSummaries = await _context.ComicReviews
            .Where(r => comicIds.Contains(r.ComicId))
            .GroupBy(r => r.ComicId)
            .Select(g => new
            {
                ComicId = g.Key,
                AverageRating = g.Average(r => r.Rating),
                ReviewCount = g.Count()
            })
            .ToListAsync();

        var favoriteIds = userId.HasValue
            ? await _context.FavoriteComics
                .Where(f => f.UserId == userId.Value && comicIds.Contains(f.ComicId))
                .Select(f => f.ComicId)
                .ToListAsync()
            : new List<int>();

        var activeCheckoutDueDates = new Dictionary<int, DateTime>();

        if (userId.HasValue)
        {
            var activeCheckoutRows = await _context.Checkouts
                .Where(c =>
                    c.UserId == userId.Value &&
                    comicIds.Contains(c.ComicId) &&
                    c.ReturnDate == null)
                .OrderByDescending(c => c.CheckoutDate)
                .Select(c => new
                {
                    c.ComicId,
                    c.DueDate
                })
                .ToListAsync();

            activeCheckoutDueDates = activeCheckoutRows
                .GroupBy(c => c.ComicId)
                .ToDictionary(g => g.Key, g => g.First().DueDate);
        }

        foreach (var comic in comics)
        {
            var rating = ratingSummaries.FirstOrDefault(r => r.ComicId == comic.Id);
            comic.AverageRating = rating == null ? 0 : Math.Round(rating.AverageRating, 1);
            comic.ReviewCount = rating?.ReviewCount ?? 0;
            comic.IsFavorite = favoriteIds.Contains(comic.Id);
            comic.IsCheckedOut = activeCheckoutDueDates.TryGetValue(comic.Id, out var dueDate);
            comic.ActiveCheckoutDueDate = comic.IsCheckedOut ? dueDate : null;
        }
    }
}
