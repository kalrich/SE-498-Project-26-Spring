using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Dtos;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShelvesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShelvesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId:int}/{shelf}")]
    public async Task<ActionResult<List<Comic>>> GetShelf(int userId, string shelf)
    {
        var comics = await _context.UserComics
            .Include(uc => uc.Comic)
            .Where(uc => uc.UserId == userId && uc.Shelf == shelf)
            .Select(uc => new Comic
            {
                Id = uc.Comic!.Id,
                Title = uc.Comic.Title,
                Author = uc.Comic.Author,
                Genre = uc.Comic.Genre,
                SecondaryGenre = uc.Comic.SecondaryGenre,
                Description = uc.Comic.Description,
                CoverImagePath = uc.Comic.CoverImagePath,
                PdfPath = uc.Comic.PdfPath,
                IsIReadPick = uc.Comic.IsIReadPick,
                Shelf = uc.Shelf,
                ProgressPercent = uc.ProgressPercent
            })
            .ToListAsync();

        return Ok(comics);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToShelf([FromBody] AddToShelfRequest request)
    {
        var existing = await _context.UserComics
            .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.ComicId == request.ComicId);

        if (existing == null)
        {
            _context.UserComics.Add(new UserComic
            {
                UserId = request.UserId,
                ComicId = request.ComicId,
                Shelf = request.Shelf,
                ProgressPercent = request.Shelf == "Completed" ? 100 :
                                  request.Shelf == "CurrentlyReading" ? 10 : 0
            });
        }
        else
        {
            existing.Shelf = request.Shelf;

            if (request.Shelf == "Completed")
                existing.ProgressPercent = 100;
            else if (request.Shelf == "UpNext")
                existing.ProgressPercent = 0;
            else if (request.Shelf == "CurrentlyReading" && existing.ProgressPercent == 0)
                existing.ProgressPercent = 10;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("update-progress")]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
    {
        var userComic = await _context.UserComics
            .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.ComicId == request.ComicId);

        if (userComic == null)
        {
            return NotFound();
        }

        userComic.ProgressPercent = request.ProgressPercent;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}