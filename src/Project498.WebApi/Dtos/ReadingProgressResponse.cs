namespace Project498.WebApi.Dtos;

public class ReadingProgressResponse
{
    public int ComicId { get; set; }
    public int ProgressPercent { get; set; }
    public int CurrentPage { get; set; } = 1;
}
