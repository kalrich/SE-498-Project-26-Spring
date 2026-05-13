using Project498.WebApi.Models;

namespace Project498.WebApi.Dtos;

public class AuthResponse
{
    public User User { get; set; } = new();
    public string Token { get; set; } = "";
}
