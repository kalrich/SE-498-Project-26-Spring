namespace Project498.WebServer.Models;

public class UserComic
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int ComicId { get; set; }
    public Comic? Comic { get; set;}
    public string Shelf { get; set; } = "";
    public int ProgressPercent { get; set; }
}