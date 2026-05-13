using System.Net.Http.Json;

namespace Project498.WebServer.Services;

public interface ICharacterImageService
{
    Task<string> GetImagePathAsync(string alias);
    Task EnrichWithImagePathsAsync(IEnumerable<DcCharacterDto> characters);
}

public class CharacterImageService : ICharacterImageService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CharacterImageService> _logger;

    public CharacterImageService(HttpClient httpClient, ILogger<CharacterImageService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetImagePathAsync(string alias)
    {
        var imagePaths = await GetImagePathMapAsync();
        return imagePaths.GetValueOrDefault(NormalizeAlias(alias), "");
    }

    public async Task EnrichWithImagePathsAsync(IEnumerable<DcCharacterDto> characters)
    {
        var imagePaths = await GetImagePathMapAsync();

        foreach (var character in characters)
        {
            character.ImagePath = imagePaths.GetValueOrDefault(NormalizeAlias(character.Alias), "");
        }
    }

    private async Task<Dictionary<string, string>> GetImagePathMapAsync()
    {
        try
        {
            var characters = await _httpClient.GetFromJsonAsync<List<CharacterImageDto>>("api/character-images");

            return characters?
                .Where(c => !string.IsNullOrWhiteSpace(c.Alias))
                .GroupBy(c => NormalizeAlias(c.Alias))
                .ToDictionary(g => g.Key, g => g.First().ImagePath)
                ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch character image paths");
            return new Dictionary<string, string>();
        }
    }

    private static string NormalizeAlias(string alias)
    {
        return new string(alias
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());
    }
}

public class CharacterImageDto
{
    public string Alias { get; set; } = "";
    public string ImagePath { get; set; } = "";
}
