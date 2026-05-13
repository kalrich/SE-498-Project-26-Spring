namespace Project498.WebApi.Models;

public class ComicReview
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? User { get; set; }
    public Comic? Comic { get; set; }
}
