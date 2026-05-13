using System.ComponentModel.DataAnnotations;
using Project498.WebServer.Services;

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
    public List<Comic> Favorites { get; set; } = new();
    public List<ReadingHistoryItem> ReadingHistory { get; set; } = new();
    public List<ComicReviewDto> Reviews { get; set; } = new();
}
