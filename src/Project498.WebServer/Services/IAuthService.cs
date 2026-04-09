using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public interface IAuthService
{
    User? Login(string email, string password);
    bool EmailExists(string email);
    User Signup(string username, string email, string password);
    User? GetByEmail(string email);
    bool UpdateProfile(string currentEmail, string newUsername, string newEmail, string? newPassword);
}