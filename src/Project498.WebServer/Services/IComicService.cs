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
    Task<List<Comic>> GetShelfAsync(int userId, string shelf);
    Task AddToShelfAsync(int userId, int comicId, string shelf);
    Task UpdateProgressAsync(int userId, int comicId, int progress);
    Task<List<Comic>> GetRecommendedAsync();
    Task<List<Comic>> GetBecauseYouReadAsync();
    Task<List<Comic>> GetHiddenGemsAsync();
}