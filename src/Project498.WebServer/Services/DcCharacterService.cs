using System.Net.Http.Json;

namespace Project498.WebServer.Services;

public interface IDcCharacterService
{
    Task<List<DcCharacterDto>> GetCharactersAsync();
    Task<DcCharacterDto?> GetCharacterByIdAsync(int id);
}

public class DcCharacterService : IDcCharacterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DcCharacterService> _logger;

    public DcCharacterService(HttpClient httpClient, ILogger<DcCharacterService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<DcCharacterDto>> GetCharactersAsync()
    {
        try
        {
            var characters = await _httpClient.GetFromJsonAsync<List<DcCharacterDto>>("/api/characters");

            return characters ?? new List<DcCharacterDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch DC characters");
            return new List<DcCharacterDto>();
        }
    }

    public async Task<DcCharacterDto?> GetCharacterByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DcCharacterDto>($"/api/characters/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch DC character {CharacterId}", id);
            return null;
        }
    }
}

public class DcCharacterDto
{
    public int CharacterId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}