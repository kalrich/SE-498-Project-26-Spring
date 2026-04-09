using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public interface IComicService
{
    List<Comic> GetAll();
    List<Comic> Search(string? query, string? genre);
    List<string> GetGenres();

    List<Comic> GetFeaturedToday();
    List<Comic> GetTrendingThisWeek();
    List<Comic> GetCurrentlyReading();
    List<Comic> GetUpNext();
    List<Comic> GetCompleted();

    List<Comic> GetRecommended();
    List<Comic> GetBecauseYouRead();
    List<Comic> GetHiddenGems();

    Comic? GetById(int id);
    void UpdateShelf(int id, string shelf);
}