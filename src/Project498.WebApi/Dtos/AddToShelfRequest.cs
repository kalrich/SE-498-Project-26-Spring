namespace Project498.WebApi.Dtos;

public class AddToShelfRequest
{
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public string Shelf { get; set; } = "";
}