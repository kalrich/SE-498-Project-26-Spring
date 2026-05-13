namespace Project498.WebApi.Dtos;

public class FavoriteStatusResponse
{
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public bool IsFavorite { get; set; }
}
