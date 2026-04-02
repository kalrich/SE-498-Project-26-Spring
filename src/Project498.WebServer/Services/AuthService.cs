using Project498.WebServer.Data;
using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

// Production Auth Service

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public User? Login(string email, string password)
    {
        return _context.Users
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower() && u.Password == password);
    }

    public bool EmailExists(string email)
    {
        return _context.Users
            .Any(u => u.Email.ToLower() == email.ToLower());
    }

    public User Signup(string username, string email, string password)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            Password = password
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public User? GetByEmail(string email)
    {
        return _context.Users
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }

    public bool UpdateProfile(string currentEmail, string newUsername, string newEmail, string? newPassword)
    {
        var user = GetByEmail(currentEmail);
        if (user == null) return false;

        var emailTakenByAnotherUser = _context.Users.Any(u =>
            u.Email.ToLower() == newEmail.ToLower() &&
            u.Email.ToLower() != currentEmail.ToLower());

        if (emailTakenByAnotherUser)
        {
            return false;
        }

        user.Username = newUsername;
        user.Email = newEmail;

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            user.Password = newPassword;
        }

        _context.SaveChanges();
        return true;
    }

    public List<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }
}