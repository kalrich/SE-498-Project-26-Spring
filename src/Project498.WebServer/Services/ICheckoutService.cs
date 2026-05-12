namespace Project498.WebServer.Services;

public interface ICheckoutService
{
    Task<CheckoutResult> InitiateCheckoutAsync(int dcUserId, int comicId, string dcJwt);
    Task<CheckoutDto?> GetCheckoutAsync(int checkoutId, string dcJwt);
    Task<List<CheckoutDto>> GetUserCheckoutsAsync(int dcUserId, string dcJwt);
    Task<bool> ReturnCheckoutAsync(int checkoutId, string dcJwt);
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
    public int ComicId { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? Status { get; set; }
}