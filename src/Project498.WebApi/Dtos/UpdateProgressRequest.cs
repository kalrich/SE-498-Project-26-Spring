namespace Project498.WebApi.Dtos;

public class UpdateProgressRequest
{
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public int ProgressPercent { get; set; }
}