using System.Net.Http.Json;
using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public class ComicApiService : IComicService
{
    private readonly HttpClient _http;

    public ComicApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Comic>> GetShelfAsync(string username, string shelf)
    {
        return await _http.GetFromJsonAsync<List<Comic>>(
                   $"api/shelves/{Uri.EscapeDataString(username)}/{shelf}")
               ?? new List<Comic>();
    }

    public async Task<Comic?> GetByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<Comic>($"api/comics/{id}");
    }
    
    public async Task<List<Comic>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<Comic>>("api/comics")
               ?? new List<Comic>();
    }

    public async Task<List<Comic>> SearchAsync(string? query, string? genre)
    {
        var url = $"api/comics?query={Uri.EscapeDataString(query ?? "")}&genre={Uri.EscapeDataString(genre ?? "")}";
        return await _http.GetFromJsonAsync<List<Comic>>(url)
               ?? new List<Comic>();
    }

    public async Task<List<string>> GetGenresAsync()
    {
        return await _http.GetFromJsonAsync<List<string>>("api/comics/genres")
               ?? new List<string>();
    }

    public async Task<List<Comic>> GetShelfAsync(int userId, string shelf)
    {
        return await _http.GetFromJsonAsync<List<Comic>>($"api/shelves/{userId}/{shelf}")
               ?? new List<Comic>();
    }

    public async Task AddToShelfAsync(string username, int comicId, string shelf)
    {
        var body = new
        {
            Username = username,
            ComicId = comicId,
            Shelf = shelf
        };

        await _http.PostAsJsonAsync("api/shelves/add", body);
    }


    public async Task UpdateProgressAsync(string username, int comicId, int progress)
    {
        var body = new
        {
            Username = username,
            ComicId = comicId,
            ProgressPercent = progress
        };

        var request = new HttpRequestMessage(HttpMethod.Patch, "api/shelves/update-progress")
        {
            Content = JsonContent.Create(body)
        };

        await _http.SendAsync(request);
    }

    public async Task<ReadingProgressDto> GetReadingProgressAsync(string username, int comicId)
    {
        return await _http.GetFromJsonAsync<ReadingProgressDto>(
                   $"api/shelves/progress/{Uri.EscapeDataString(username)}/{comicId}")
               ?? new ReadingProgressDto
               {
                   ComicId = comicId,
                   ProgressPercent = 0,
                   CurrentPage = 1
               };
    }

    public async Task UpdateReadingProgressAsync(string username, int comicId, int progress, int currentPage)
    {
        var body = new
        {
            Username = username,
            ComicId = comicId,
            ProgressPercent = progress,
            CurrentPage = currentPage
        };

        var request = new HttpRequestMessage(HttpMethod.Patch, "api/shelves/update-progress")
        {
            Content = JsonContent.Create(body)
        };

        await _http.SendAsync(request);
    }
    
    public async Task<List<Comic>> GetFeaturedTodayAsync()
    {
        return await _http.GetFromJsonAsync<List<Comic>>("api/comics/featured")
               ?? new List<Comic>();
    }

    public async Task<List<Comic>> GetTrendingThisWeekAsync()
    {
        return await _http.GetFromJsonAsync<List<Comic>>("api/comics/trending")
               ?? new List<Comic>();
    }
    
    public async Task<List<Comic>> GetRecommendedAsync()
    {
        return await _http.GetFromJsonAsync<List<Comic>>("api/comics/recommended")
               ?? new List<Comic>();
    }

    public async Task<List<Comic>> GetBecauseYouReadAsync()
    {
        return await _http.GetFromJsonAsync<List<Comic>>("api/comics/because-you-read")
               ?? new List<Comic>();
    }

    public async Task<List<Comic>> GetHiddenGemsAsync()
    {
        return await _http.GetFromJsonAsync<List<Comic>>("api/comics/hidden-gems")
               ?? new List<Comic>();
    }
    
    public async Task<List<Comic>> GetSeriesAsync(string seriesName)
    {
        return await _http.GetFromJsonAsync<List<Comic>>(
                   $"api/comics/series/{Uri.EscapeDataString(seriesName)}")
               ?? new List<Comic>();
    }
}
