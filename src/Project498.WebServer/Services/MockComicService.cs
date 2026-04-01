using Microsoft.EntityFrameworkCore;
using Project498.WebServer.Data;
using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public class MockComicService
{
    private readonly AppDbContext _context;

    // Temporary hardcoded current user
    private const int CurrentUserId = 1;

    public MockComicService(AppDbContext context)
    {
        _context = context;
    }

    public List<Comic> GetAll()
    {
        var comics = _context.Comics.ToList();
        ApplyUserShelfData(comics);
        return comics;
    }

    public List<Comic> Search(string? query, string? genre)
    {
        var comics = _context.Comics.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowered = query.ToLower();

            comics = comics.Where(c =>
                c.Title.ToLower().Contains(lowered) ||
                c.Author.ToLower().Contains(lowered) ||
                c.Description.ToLower().Contains(lowered));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            comics = comics.Where(c =>
                c.Genre.ToLower() == genre.ToLower() ||
                c.SecondaryGenre.ToLower() == genre.ToLower());
        }

        var results = comics.ToList();
        ApplyUserShelfData(results);
        return results;
    }

    public List<string> GetGenres()
    {
        return _context.Comics
            .AsEnumerable()
            .SelectMany(c => new[] { c.Genre, c.SecondaryGenre })
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    public List<Comic> GetFeaturedToday()
    {
        var comics = _context.Comics.ToList();
        ApplyUserShelfData(comics);
        return comics;
    }

    public List<Comic> GetTrendingThisWeek()
    {
        var comicIds = _context.UserComics
            .Where(uc => uc.UserId == CurrentUserId && uc.Shelf == "Trending")
            .Select(uc => uc.ComicId)
            .ToList();

        var comics = _context.Comics
            .Where(c => comicIds.Contains(c.Id) || c.Id <= 3)
            .Take(3)
            .ToList();

        ApplyUserShelfData(comics);
        return comics;
    }

    public List<Comic> GetCurrentlyReading()
    {
        return GetByShelf("CurrentlyReading");
    }

    public List<Comic> GetUpNext()
    {
        return GetByShelf("UpNext");
    }

    public List<Comic> GetCompleted()
    {
        return GetByShelf("Completed");
    }

    public List<Comic> GetRecommended()
    {
        var comics = _context.Comics
            .Where(c => c.Genre == "Mystery" || c.SecondaryGenre == "Mystery")
            .Take(4)
            .ToList();

        ApplyUserShelfData(comics);
        return comics;
    }

    public List<Comic> GetBecauseYouRead()
    {
        var comicIds = _context.UserComics
            .Where(uc => uc.UserId == CurrentUserId &&
                         (uc.Shelf == "Completed" || uc.Shelf == "CurrentlyReading"))
            .Select(uc => uc.ComicId)
            .Take(3)
            .ToList();

        var comics = _context.Comics
            .Where(c => comicIds.Contains(c.Id))
            .ToList();

        ApplyUserShelfData(comics);
        return comics;
    }

    public List<Comic> GetHiddenGems()
    {
        var comics = _context.Comics
            .Where(c => c.Genre == "Classic" || c.SecondaryGenre == "Classic")
            .Take(3)
            .ToList();

        ApplyUserShelfData(comics);
        return comics;
    }

    public Comic? GetById(int id)
    {
        var comic = _context.Comics.FirstOrDefault(c => c.Id == id);

        if (comic == null) return null;

        ApplyUserShelfData(new List<Comic> { comic });
        return comic;
    }

    public void UpdateShelf(int id, string shelf)
    {
        var userComic = _context.UserComics
            .FirstOrDefault(uc => uc.UserId == CurrentUserId && uc.ComicId == id);

        if (userComic == null)
        {
            userComic = new UserComic
            {
                UserId = CurrentUserId,
                ComicId = id,
                Shelf = shelf,
                ProgressPercent = shelf == "Completed" ? 100 :
                                  shelf == "CurrentlyReading" ? 10 : 0
            };

            _context.UserComics.Add(userComic);
        }
        else
        {
            userComic.Shelf = shelf;

            if (shelf == "Completed")
            {
                userComic.ProgressPercent = 100;
            }
            else if (shelf == "UpNext")
            {
                userComic.ProgressPercent = 0;
            }
            else if (shelf == "CurrentlyReading" && userComic.ProgressPercent == 0)
            {
                userComic.ProgressPercent = 10;
            }
        }

        _context.SaveChanges();
    }

    private List<Comic> GetByShelf(string shelf)
    {
        var userComics = _context.UserComics
            .Where(uc => uc.UserId == CurrentUserId && uc.Shelf == shelf)
            .ToList();

        var comicIds = userComics.Select(uc => uc.ComicId).ToList();

        var comics = _context.Comics
            .Where(c => comicIds.Contains(c.Id))
            .ToList();

        ApplyUserShelfData(comics);
        return comics;
    }

    private void ApplyUserShelfData(List<Comic> comics)
    {
        var comicIds = comics.Select(c => c.Id).ToList();

        var userData = _context.UserComics
            .Where(uc => uc.UserId == CurrentUserId && comicIds.Contains(uc.ComicId))
            .ToList();

        foreach (var comic in comics)
        {
            var match = userData.FirstOrDefault(uc => uc.ComicId == comic.Id);
            if (match != null)
            {
                comic.Shelf = match.Shelf;
                comic.ProgressPercent = match.ProgressPercent;
            }
            else
            {
                comic.Shelf = "";
                comic.ProgressPercent = 0;
            }
        }
    }
}