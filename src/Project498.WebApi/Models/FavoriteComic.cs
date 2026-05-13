namespace Project498.WebApi.Models;

public class FavoriteComic
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Comic? Comic { get; set; }
}
