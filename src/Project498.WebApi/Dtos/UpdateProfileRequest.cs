namespace Project498.WebApi.Dtos;

public class UpdateProfileRequest
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Password { get; set; }
}