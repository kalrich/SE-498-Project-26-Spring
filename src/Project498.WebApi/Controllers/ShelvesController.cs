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
public class ShelvesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShelvesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{username}/{shelf}")]
    public async Task<ActionResult<List<Comic>>> GetShelf(string username, string shelf)
    {
        var comics = await _context.UserComics
            .Include(uc => uc.Comic)
            .Include(uc => uc.User)
            .Where(uc => uc.User!.Username == username && uc.Shelf == shelf)
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
                ProgressPercent = uc.ProgressPercent,
                CurrentPage = uc.CurrentPage
            })
            .ToListAsync();

        return Ok(comics);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToShelf([FromBody] AddToShelfRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return NotFound("User not found");

        var existing = await _context.UserComics
            .FirstOrDefaultAsync(uc => uc.UserId == user.Id && uc.ComicId == request.ComicId);

        if (existing == null)
        {
            _context.UserComics.Add(new UserComic
            {
                UserId = user.Id,
                ComicId = request.ComicId,
                Shelf = request.Shelf,
                CurrentPage = 1,
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

    [HttpGet("progress/{username}/{comicId:int}")]
    public async Task<ActionResult<ReadingProgressResponse>> GetProgress(string username, int comicId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            return NotFound("User not found");

        var userComic = await _context.UserComics
            .FirstOrDefaultAsync(uc => uc.UserId == user.Id && uc.ComicId == comicId);

        if (userComic == null)
        {
            return Ok(new ReadingProgressResponse
            {
                ComicId = comicId,
                ProgressPercent = 0,
                CurrentPage = 1
            });
        }

        return Ok(new ReadingProgressResponse
        {
            ComicId = comicId,
            ProgressPercent = userComic.ProgressPercent,
            CurrentPage = userComic.CurrentPage
        });
    }

    [HttpPatch("update-progress")]
    public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return NotFound("User not found");

        var userComic = await _context.UserComics
            .FirstOrDefaultAsync(uc => uc.UserId == user.Id && uc.ComicId == request.ComicId);

        if (userComic == null)
        {
            userComic = new UserComic
            {
                UserId = user.Id,
                ComicId = request.ComicId,
                Shelf = request.ProgressPercent >= 100 ? "Completed" : "CurrentlyReading",
                CurrentPage = Math.Max(1, request.CurrentPage ?? 1)
            };

            _context.UserComics.Add(userComic);
        }

        userComic.ProgressPercent = request.ProgressPercent;
        if (request.CurrentPage.HasValue)
        {
            userComic.CurrentPage = Math.Max(1, request.CurrentPage.Value);
        }

        if (request.ProgressPercent >= 100)
            userComic.Shelf = "Completed";
        else if (userComic.Shelf == "UpNext")
            userComic.Shelf = "CurrentlyReading";

        var history = await _context.ReadingHistories
            .FirstOrDefaultAsync(h => h.UserId == user.Id && h.ComicId == request.ComicId);

        if (history == null)
        {
            history = new ReadingHistory
            {
                UserId = user.Id,
                ComicId = request.ComicId
            };

            _context.ReadingHistories.Add(history);
        }

        history.ProgressPercent = userComic.ProgressPercent;
        history.CurrentPage = userComic.CurrentPage;
        history.LastReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
