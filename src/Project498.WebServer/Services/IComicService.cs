using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public interface IComicService
{
    Task<List<Comic>> GetAllAsync();
    Task<Comic?> GetByIdAsync(int id, int? userId = null);
    Task<List<Comic>> SearchAsync(string? query, string? genre, string? status = null, int? userId = null);
    Task<List<string>> GetGenresAsync();
    Task<List<Comic>> GetFeaturedTodayAsync();
    Task<List<Comic>> GetTrendingThisWeekAsync();
    Task<List<Comic>> GetShelfAsync(string username, string shelf);
    Task AddToShelfAsync(string username, int comicId, string shelf);
    Task UpdateProgressAsync(string username, int comicId, int progress);
    Task<ReadingProgressDto> GetReadingProgressAsync(string username, int comicId);
    Task UpdateReadingProgressAsync(string username, int comicId, int progress, int currentPage);
    Task<List<Comic>> GetRecommendedAsync();
    Task<List<Comic>> GetBecauseYouReadAsync();
    Task<List<Comic>> GetHiddenGemsAsync();
    Task<List<Comic>> GetSeriesAsync(string seriesName);
    Task<List<Comic>> GetFavoritesAsync(int userId);
    Task<bool> GetFavoriteStatusAsync(int userId, int comicId);
    Task SetFavoriteAsync(int userId, int comicId, bool isFavorite);
    Task<List<ReadingHistoryItem>> GetReadingHistoryAsync(int userId);
    Task<List<ComicReviewDto>> GetReviewsAsync(int comicId);
    Task<ComicReviewDto?> GetUserReviewAsync(int userId, int comicId);
    Task<List<ComicReviewDto>> GetUserReviewsAsync(int userId);
    Task SaveReviewAsync(int userId, int comicId, int rating, string comment);
}

public class ReadingProgressDto
{
    public int ComicId { get; set; }
    public int ProgressPercent { get; set; }
    public int CurrentPage { get; set; } = 1;
}

public class ReadingHistoryItem
{
    public int ComicId { get; set; }
    public string Title { get; set; } = "";
    public string CoverImagePath { get; set; } = "";
    public int CurrentPage { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime LastReadAt { get; set; }
}

public class ComicReviewDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ComicId { get; set; }
    public string Username { get; set; } = "";
    public string ComicTitle { get; set; } = "";
    public string CoverImagePath { get; set; } = "";
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
