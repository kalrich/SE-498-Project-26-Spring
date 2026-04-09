using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Dtos;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("by-email")]
    public async Task<ActionResult<User>> GetByEmail([FromQuery] string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == email.ToLower());

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        var emailTaken = await _context.Users.AnyAsync(u =>
            u.Id != id && u.Email.ToLower() == request.Email.ToLower());

        if (emailTaken)
        {
            return Conflict("Email already in use.");
        }

        user.Username = request.Username;
        user.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.Password = request.Password;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
}