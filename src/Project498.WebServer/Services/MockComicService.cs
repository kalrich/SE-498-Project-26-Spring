using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public class MockComicService
{
    private readonly List<Comic> _comics =
    [
        new Comic
        {
            Id = 1,
            Title = "Marvel Mystery Vol. 1 Issue 12",
            Author = "Marvel Comics",
            Genre = "Mystery",
            SecondaryGenre = "Classic",
            Description = "A classic Golden Age Marvel mystery issue from Volume 1, Issue 12.",
            CoverImagePath = "/images/covers/Marvel_Mystery_Vol1_Iss12_COVER.jpeg",
            PdfPath = "/comics/Marvel_Mystery_Vol1_Iss12.pdf",
            ProgressPercent = 35,
            Shelf = "CurrentlyReading",
            IsIReadPick = true
        },
        new Comic
        {
            Id = 2,
            Title = "Marvel Mystery Vol. 1 Issue 14",
            Author = "Marvel Comics",
            Genre = "Mystery",
            SecondaryGenre = "Adventure",
            Description = "Another early Marvel Mystery issue with classic pulp-era storytelling and action.",
            CoverImagePath = "/images/covers/Marvel_Mystery_Vol1_Iss14_COVER.jpeg",
            PdfPath = "/comics/Marvel_Mystery_Vol1_Iss14.pdf",
            ProgressPercent = 0,
            Shelf = "UpNext",
            IsIReadPick = true
        },
        new Comic
        {
            Id = 3,
            Title = "Marvel Mystery Vol. 1 Issue 15",
            Author = "Marvel Comics",
            Genre = "Mystery",
            SecondaryGenre = "Action",
            Description = "A Golden Age Marvel issue featuring suspense, action, and vintage comic art.",
            CoverImagePath = "/images/covers/Marvel_Mystery_Vol1_Iss15_COVER.jpeg",
            PdfPath = "/comics/Marvel_Mystery_Vol1_Iss15.pdf",
            ProgressPercent = 0,
            Shelf = "Trending",
            IsIReadPick = false
        },
        new Comic
        {
            Id = 4,
            Title = "Mystic Comics Vol. 1 Issue 4",
            Author = "Marvel Comics",
            Genre = "Fantasy",
            SecondaryGenre = "Mystery",
            Description = "A classic Mystic Comics issue featuring supernatural themes and early Marvel storytelling.",
            CoverImagePath = "/images/covers/Mystic_Comics_Vol1_Iss4_COVER.jpeg",
            PdfPath = "/comics/Mystic_Comics_Vol1_Iss4.pdf",
            ProgressPercent = 100,
            Shelf = "Completed",
            IsIReadPick = true
        }
    ];

    public List<Comic> GetFeaturedToday()
    {
        return _comics.Take(4).ToList();
    }

    public List<Comic> GetTrendingThisWeek()
    {
        return _comics
            .Where(c => c.Shelf == "Trending" || c.IsIReadPick)
            .Take(3)
            .ToList();
    }

    public List<Comic> GetCurrentlyReading()
    {
        return _comics
            .Where(c => c.Shelf == "CurrentlyReading")
            .ToList();
    }

    public List<Comic> GetUpNext()
    {
        return _comics
            .Where(c => c.Shelf == "UpNext")
            .ToList();
    }

    public List<Comic> GetCompleted()
    {
        return _comics
            .Where(c => c.Shelf == "Completed")
            .ToList();
    }

    public List<Comic> GetIReadPicks()
    {
        return _comics
            .Where(c => c.IsIReadPick)
            .ToList();
    }

    public Comic? GetById(int id)
    {
        return _comics.FirstOrDefault(c => c.Id == id);
    }
    
    public List<Comic> GetAll()
    {
        return _comics.ToList();
    }

    public List<Comic> Search(string? query, string? genre)
    {
        var comics = _comics.AsQueryable();

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
                c.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase) ||
                c.SecondaryGenre.Equals(genre, StringComparison.OrdinalIgnoreCase));
        }

        return comics.ToList();
    }

    public List<string> GetGenres()
    {
        return _comics
            .SelectMany(c => new[] { c.Genre, c.SecondaryGenre })
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }
    
    public List<Comic> GetRecommended()
    {
        return _comics
            .Where(c => c.Genre == "Mystery" || c.SecondaryGenre == "Mystery" || c.Shelf == "Trending")
            .Take(4)
            .ToList();
    }

    public List<Comic> GetBecauseYouRead()
    {
        return _comics
            .Where(c => c.Shelf == "Completed" || c.Shelf == "CurrentlyReading")
            .Take(3)
            .ToList();
    }

    public List<Comic> GetHiddenGems()
    {
        return _comics
            .Where(c => c.Genre == "Classic" || c.SecondaryGenre == "Classic")
            .Take(3)
            .ToList();
    }
}