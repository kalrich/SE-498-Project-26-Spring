namespace Project498.WebApi.Dtos;

public class CheckoutResponse
{
    public int CheckoutId { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = "";
    public string ComicTitle { get; set; } = "";
    public string CoverImagePath { get; set; } = "";
    public bool IsOverdue { get; set; }
}
