namespace Project498.WebApi.Models;

public class Checkout
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = "Active";

    public User? User { get; set; }
    public Comic? Comic { get; set; }
}
