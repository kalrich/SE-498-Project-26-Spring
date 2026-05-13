namespace Project498.WebApi.Dtos;

public class ComicReviewRequest
{
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
}
