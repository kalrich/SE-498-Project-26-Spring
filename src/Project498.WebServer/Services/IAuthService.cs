using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string email, string password);
    Task<bool> SignupAsync(string username, string email, string password);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> UpdateProfileAsync(int userId, string username, string email, string? password);
}