using System.Net.Http.Json;

namespace Project498.WebServer.Services;

public class CheckoutService : ICheckoutService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(HttpClient httpClient, ILogger<CheckoutService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CheckoutResult> InitiateCheckoutAsync(int userId, int comicId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/checkouts", new
            {
                UserId = userId,
                ComicId = comicId
            });

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var existingCheckout = await response.Content.ReadFromJsonAsync<CheckoutDto>();
                if (existingCheckout != null)
                {
                    return new CheckoutResult
                    {
                        Success = true,
                        CheckoutId = existingCheckout.CheckoutId,
                        DueDate = existingCheckout.DueDate
                    };
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                return new CheckoutResult
                {
                    Success = false,
                    ErrorMessage = string.IsNullOrWhiteSpace(errorMessage)
                        ? "Failed to checkout comic"
                        : errorMessage
                };
            }

            var checkout = await response.Content.ReadFromJsonAsync<CheckoutDto>();
            if (checkout == null)
            {
                return new CheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Failed to parse checkout response"
                };
            }
            return new CheckoutResult
            {
                Success = true,
                CheckoutId = checkout.CheckoutId,
                DueDate = checkout.DueDate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Checkout initiation failed");
            return new CheckoutResult
            {
                Success = false,
                ErrorMessage = "Unable to process checkout"
            };
        }
    }

    public async Task<CheckoutDto?> GetCheckoutAsync(int checkoutId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/checkouts/{checkoutId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CheckoutDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get checkout failed");
            return null;
        }
    }

    public async Task<List<CheckoutDto>> GetUserCheckoutsAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/checkouts/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var checkouts = await response.Content.ReadFromJsonAsync<List<CheckoutDto>>();
                return checkouts ?? new();
            }
            return new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get user checkouts failed");
            return new();
        }
    }

    public async Task<bool> ReturnCheckoutAsync(int checkoutId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"api/checkouts/{checkoutId}/return", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Return checkout failed");
            return false;
        }
    }
}
