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

    public async Task<CheckoutResult> InitiateCheckoutAsync(int dcUserId, int comicId, string dcJwt)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8080/api/checkouts");
            request.Headers.Authorization = new("Bearer", dcJwt);
            request.Content = JsonContent.Create(new { comicId });

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new CheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Failed to checkout comic"
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

    public async Task<CheckoutDto?> GetCheckoutAsync(int checkoutId, string dcJwt)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:8080/api/checkouts/{checkoutId}");
            request.Headers.Authorization = new("Bearer", dcJwt);

            var response = await _httpClient.SendAsync(request);
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

    public async Task<List<CheckoutDto>> GetUserCheckoutsAsync(int dcUserId, string dcJwt)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:8080/api/checkouts/user/{dcUserId}");
            request.Headers.Authorization = new("Bearer", dcJwt);

            var response = await _httpClient.SendAsync(request);
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

    public async Task<bool> ReturnCheckoutAsync(int checkoutId, string dcJwt)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, $"http://localhost:8080/api/checkouts/{checkoutId}/return");
            request.Headers.Authorization = new("Bearer", dcJwt);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Return checkout failed");
            return false;
        }
    }
}