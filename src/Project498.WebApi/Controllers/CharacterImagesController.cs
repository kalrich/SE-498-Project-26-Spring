using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/character-images")]
public class CharacterImagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CharacterImagesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<CharacterImage>>> GetAll()
    {
        return Ok(await _context.CharacterImages.OrderBy(c => c.Alias).ToListAsync());
    }
}
