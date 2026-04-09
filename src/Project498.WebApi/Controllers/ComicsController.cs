using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComicsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ComicsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Comic>>> GetAll([FromQuery] string? query, [FromQuery] string? genre)
    {
        var comics = _context.Comics.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLower();
            comics = comics.Where(c =>
                c.Title.ToLower().Contains(q) ||
                c.Author.ToLower().Contains(q));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            comics = comics.Where(c =>
                c.Genre == genre || c.SecondaryGenre == genre);
        }

        return Ok(await comics.ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Comic>> GetById(int id)
    {
        var comic = await _context.Comics.FirstOrDefaultAsync(c => c.Id == id);

        if (comic == null)
        {
            return NotFound();
        }

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
}