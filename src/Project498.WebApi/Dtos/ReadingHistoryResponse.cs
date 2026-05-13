namespace Project498.WebApi.Dtos;

public class ReadingHistoryResponse
{
    public int ComicId { get; set; }
    public string Title { get; set; } = "";
    public string CoverImagePath { get; set; } = "";
    public int CurrentPage { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime LastReadAt { get; set; }
}
