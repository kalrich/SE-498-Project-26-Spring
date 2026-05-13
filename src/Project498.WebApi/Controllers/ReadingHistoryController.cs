using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Dtos;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReadingHistoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReadingHistoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<List<ReadingHistoryResponse>>> GetForUser(int userId)
    {
        var history = await _context.ReadingHistories
            .Include(h => h.Comic)
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.LastReadAt)
            .Select(h => new ReadingHistoryResponse
            {
                ComicId = h.ComicId,
                Title = h.Comic!.Title,
                CoverImagePath = h.Comic.CoverImagePath,
                CurrentPage = h.CurrentPage,
                ProgressPercent = h.ProgressPercent,
                LastReadAt = h.LastReadAt
            })
            .ToListAsync();

        return Ok(history);
    }
}
