using Project498.WebServer.Data;
using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public class ComicService : IComicService
{
    private readonly AppDbContext _context;

    // TEMP: we’ll replace this with session later
    private int CurrentUserId = 1;

    public ComicService(AppDbContext context)
    {
        _context = context;
    }

    public List<Comic> GetAll()
    {
        return _context.Comics.ToList();
    }

    public List<Comic> Search(string? query, string? genre)
    {
        var comics = _context.Comics.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.ToLower();
            comics = comics.Where(c =>
                c.Title.ToLower().Contains(query) ||
                c.Author.ToLower().Contains(query));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            comics = comics.Where(c =>
                c.Genre == genre || c.SecondaryGenre == genre);
        }

        return comics.ToList();
    }

    public List<string> GetGenres()
    {
        return _context.Comics
            .AsEnumerable() 
            .SelectMany(c => new[] { c.Genre, c.SecondaryGenre })
            .Where(g => !string.IsNullOrEmpty(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    public List<Comic> GetFeaturedToday()
    {
        return ApplyUserShelfData(_context.Comics
            .Where(c => c.IsIReadPick)
            .ToList());
    }

    public List<Comic> GetTrendingThisWeek()
    {
        return ApplyUserShelfData(_context.Comics
            .OrderBy(c => c.Id)
            .Take(6)
            .ToList());
    }

    public List<Comic> GetCurrentlyReading()
    {
        return GetByShelf("Reading");
    }

    public List<Comic> GetUpNext()
    {
        return GetByShelf("To Read");
    }

    public List<Comic> GetCompleted()
    {
        return GetByShelf("Finished");
    }

    public List<Comic> GetRecommended()
    {
        return ApplyUserShelfData(_context.Comics
            .OrderByDescending(c => c.Id)
            .Take(6)
            .ToList());
    }

    public List<Comic> GetBecauseYouRead()
    {
        return ApplyUserShelfData(_context.Comics
            .OrderBy(c => c.Title)
            .Take(6)
            .ToList());
    }

    public List<Comic> GetHiddenGems()
    {
        return ApplyUserShelfData(_context.Comics
            .OrderBy(c => Guid.NewGuid())
            .Take(6)
            .ToList());
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
                ProgressPercent = shelf == "Finished" ? 100 :
                                  shelf == "Reading" ? 10 : 0
            };

            _context.UserComics.Add(userComic);
        }
        else
        {
            userComic.Shelf = shelf;

            if (shelf == "Finished")
                userComic.ProgressPercent = 100;
            else if (shelf == "To Read")
                userComic.ProgressPercent = 0;
        }

        _context.SaveChanges();
    }

    // -----------------------------
    // HELPERS
    // -----------------------------

    private List<Comic> GetByShelf(string shelf)
    {
        var comics = _context.UserComics
            .Where(uc => uc.UserId == CurrentUserId && uc.Shelf == shelf)
            .Select(uc => uc.Comic)
            .ToList();

        return ApplyUserShelfData(comics);
    }

    private List<Comic> ApplyUserShelfData(List<Comic> comics)
    {
        var userData = _context.UserComics
            .Where(uc => uc.UserId == CurrentUserId)
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
                comic.Shelf = "To Read";
                comic.ProgressPercent = 0;
            }
        }

        return comics;
    }
}