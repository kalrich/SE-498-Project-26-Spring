using System.Net.Http.Json;

namespace Project498.WebServer.Services;

public interface IMarvelCharacterService
{
    Task<List<MarvelCharacterDto>> GetCharactersAsync();
    Task<MarvelCharacterDto?> GetCharacterByIdAsync(int id);
}

public class MarvelCharacterService : IMarvelCharacterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MarvelCharacterService> _logger;

    public MarvelCharacterService(HttpClient httpClient, ILogger<MarvelCharacterService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<MarvelCharacterDto>> GetCharactersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<MarvelCharacterDto>>("api/marvel-characters")
                   ?? new List<MarvelCharacterDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch Marvel characters");
            return new List<MarvelCharacterDto>();
        }
    }

    public async Task<MarvelCharacterDto?> GetCharacterByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MarvelCharacterDto>($"api/marvel-characters/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch Marvel character {CharacterId}", id);
            return null;
        }
    }
}

public class MarvelCharacterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Alias { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImagePath { get; set; } = "";
}
