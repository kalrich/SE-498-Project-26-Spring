namespace Project498.WebServer.Services;

public interface ICheckoutService
{
    Task<CheckoutResult> InitiateCheckoutAsync(int userId, int comicId);
    Task<CheckoutDto?> GetCheckoutAsync(int checkoutId);
    Task<List<CheckoutDto>> GetUserCheckoutsAsync(int userId);
    Task<bool> ReturnCheckoutAsync(int checkoutId);
}

public class CheckoutResult
{
    public bool Success { get; set; }
    public int CheckoutId { get; set; }
    public DateTime DueDate { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CheckoutDto
{
    public int CheckoutId { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? Status { get; set; }
}
