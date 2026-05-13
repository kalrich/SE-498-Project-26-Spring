namespace Project498.WebApi.Dtos;

public class ComicReviewResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public string Username { get; set; } = "";
    public string ComicTitle { get; set; } = "";
    public string CoverImagePath { get; set; } = "";
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
