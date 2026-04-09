namespace Project498.WebApi.Models;

public class UserComic
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public string Shelf { get; set; } = "";
    public int ProgressPercent { get; set; }

    public User? User { get; set; }
    public Comic? Comic { get; set; }
}