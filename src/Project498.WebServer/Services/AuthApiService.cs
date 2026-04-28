using System.Net.Http.Json;
using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public class AuthApiService : IAuthService
{
    private readonly HttpClient _http;

    public AuthApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new
        {
            Email = email,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<bool> SignupAsync(string username, string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/signup", new
        {
            Username = username,
            Email = email,
            Password = password
        });

        return response.IsSuccessStatusCode;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _http.GetFromJsonAsync<User>(
            $"api/users/by-email?email={Uri.EscapeDataString(email)}");
    }

    public async Task<bool> UpdateProfileAsync(int userId, string username, string email, string? password)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{userId}", new
        {
            Username = username,
            Email = email,
            Password = password ?? ""
        });

        return response.IsSuccessStatusCode;
    }
}