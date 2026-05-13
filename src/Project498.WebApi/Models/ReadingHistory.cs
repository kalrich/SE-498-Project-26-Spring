namespace Project498.WebApi.Models;

public class ReadingHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int ProgressPercent { get; set; }
    public DateTime LastReadAt { get; set; }

    public User? User { get; set; }
    public Comic? Comic { get; set; }
}
