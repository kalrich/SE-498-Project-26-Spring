using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/marvel-characters")]
[Authorize]
public class MarvelCharactersController : ControllerBase
{
    private readonly AppDbContext _context;

    public MarvelCharactersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<MarvelCharacter>>> GetAll()
    {
        return Ok(await _context.MarvelCharacters.OrderBy(c => c.Alias).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MarvelCharacter>> GetById(int id)
    {
        var character = await _context.MarvelCharacters.FirstOrDefaultAsync(c => c.Id == id);

        if (character == null)
        {
            return NotFound();
        }

        return Ok(character);
    }
}
