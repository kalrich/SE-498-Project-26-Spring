namespace Project498.WebApi.Dtos;

public class UpdateProgressRequest
{
    public string Username { get; set; } = "";
    public int ComicId { get; set; }
    public int ProgressPercent { get; set; }
}