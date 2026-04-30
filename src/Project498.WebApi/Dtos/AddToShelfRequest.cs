namespace Project498.WebApi.Dtos;

public class AddToShelfRequest
{
    public string Username { get; set; } = "";
    public int ComicId { get; set; }
    public string Shelf { get; set; } = "";
}