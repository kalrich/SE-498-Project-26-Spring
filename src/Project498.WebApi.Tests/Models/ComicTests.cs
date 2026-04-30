using Project498.WebApi.Models;
using Xunit;

namespace Project498.WebApi.Tests.Models;

public class ComicTests
{
    [Fact]
    public void Comic_ShouldInitializeWithDefaultValues()
    {
        var comic = new Comic();

        Assert.Equal("", comic.Title);
        Assert.Equal("", comic.Author);
        Assert.Equal("", comic.Genre);
        Assert.Equal(0, comic.Id);
        Assert.False(comic.IsIReadPick);
    }

    [Fact]
    public void Comic_ShouldSetProperties()
    {
        var comic = new Comic
        {
            Id = 1,
            Title = "Test Comic",
            Author = "Test Author",
            Genre = "Action",
            SecondaryGenre = "Adventure",
            Description = "A test comic",
            CoverImagePath = "/images/cover.jpg",
            PdfPath = "/pdfs/comic.pdf",
            IsIReadPick = true,
            Shelf = "Reading",
            ProgressPercent = 50
        };

        Assert.Equal(1, comic.Id);
        Assert.Equal("Test Comic", comic.Title);
        Assert.Equal("Test Author", comic.Author);
        Assert.Equal("Action", comic.Genre);
        Assert.Equal("Adventure", comic.SecondaryGenre);
        Assert.Equal("A test comic", comic.Description);
        Assert.Equal("/images/cover.jpg", comic.CoverImagePath);
        Assert.Equal("/pdfs/comic.pdf", comic.PdfPath);
        Assert.True(comic.IsIReadPick);
        Assert.Equal("Reading", comic.Shelf);
        Assert.Equal(50, comic.ProgressPercent);
    }
}
