using System.ComponentModel.DataAnnotations;

namespace Project498.WebServer.Models;

public class ProfileViewModel
{
    [Required]
    public string Username { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    public string Password { get; set; } = "";

    public int TotalBooksRead { get; set; }
}