using System.ComponentModel.DataAnnotations;

namespace Project498.WebServer.Models;

public class SignupViewModel
{
    [Required]
    public string Username { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";
}