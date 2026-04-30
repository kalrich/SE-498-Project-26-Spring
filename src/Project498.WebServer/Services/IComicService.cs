using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public interface IComicService
{
    Task<List<Comic>> GetAllAsync();
    Task<Comic?> GetByIdAsync(int id);
    Task<List<Comic>> SearchAsync(string? query, string? genre);
    Task<List<string>> GetGenresAsync();
    Task<List<Comic>> GetFeaturedTodayAsync();
    Task<List<Comic>> GetTrendingThisWeekAsync();
    Task<List<Comic>> GetShelfAsync(string username, string shelf);
    Task AddToShelfAsync(string username, int comicId, string shelf);
    Task UpdateProgressAsync(string username, int comicId, int progress);
    Task<List<Comic>> GetRecommendedAsync();
    Task<List<Comic>> GetBecauseYouReadAsync();
    Task<List<Comic>> GetHiddenGemsAsync();
    Task<List<Comic>> GetSeriesAsync(string seriesName);
}