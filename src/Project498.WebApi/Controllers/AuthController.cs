using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Dtos;
using Project498.WebApi.Models;

namespace Project498.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == request.Email.ToLower() &&
            u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

    [HttpPost("signup")]
    public async Task<ActionResult<User>> Signup([FromBody] SignupRequest request)
    {
        var exists = await _context.Users.AnyAsync(u =>
            u.Email.ToLower() == request.Email.ToLower());

        if (exists)
        {
            return Conflict("Email already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }
}