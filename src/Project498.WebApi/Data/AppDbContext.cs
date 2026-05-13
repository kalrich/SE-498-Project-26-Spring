using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Models;

namespace Project498.WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Comic> Comics => Set<Comic>();
    public DbSet<UserComic> UserComics => Set<UserComic>();
    public DbSet<Checkout> Checkouts => Set<Checkout>();
    public DbSet<CharacterImage> CharacterImages => Set<CharacterImage>();
    public DbSet<MarvelCharacter> MarvelCharacters => Set<MarvelCharacter>();
    public DbSet<FavoriteComic> FavoriteComics => Set<FavoriteComic>();
    public DbSet<ReadingHistory> ReadingHistories => Set<ReadingHistory>();
    public DbSet<ComicReview> ComicReviews => Set<ComicReview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Comic>().ToTable("Comics");
        modelBuilder.Entity<UserComic>().ToTable("UserComics");
        modelBuilder.Entity<Checkout>().ToTable("Checkouts");
        modelBuilder.Entity<CharacterImage>().ToTable("CharacterImages");
        modelBuilder.Entity<MarvelCharacter>().ToTable("MarvelCharacters");
        modelBuilder.Entity<FavoriteComic>().ToTable("FavoriteComics");
        modelBuilder.Entity<ReadingHistory>().ToTable("ReadingHistories");
        modelBuilder.Entity<ComicReview>().ToTable("ComicReviews");

        modelBuilder.Entity<UserComic>()
            .Property(uc => uc.CurrentPage)
            .HasDefaultValue(1);

        modelBuilder.Entity<ReadingHistory>()
            .Property(h => h.CurrentPage)
            .HasDefaultValue(1);

        modelBuilder.Entity<ReadingHistory>()
            .Property(h => h.ProgressPercent)
            .HasDefaultValue(0);

        modelBuilder.Entity<UserComic>()
            .HasOne(uc => uc.User)
            .WithMany()
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserComic>()
            .HasOne(uc => uc.Comic)
            .WithMany()
            .HasForeignKey(uc => uc.ComicId);

        modelBuilder.Entity<Checkout>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<Checkout>()
            .HasOne(c => c.Comic)
            .WithMany()
            .HasForeignKey(c => c.ComicId);

        modelBuilder.Entity<FavoriteComic>()
            .HasIndex(f => new { f.UserId, f.ComicId })
            .IsUnique();

        modelBuilder.Entity<FavoriteComic>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId);

        modelBuilder.Entity<FavoriteComic>()
            .HasOne(f => f.Comic)
            .WithMany()
            .HasForeignKey(f => f.ComicId);

        modelBuilder.Entity<ReadingHistory>()
            .HasIndex(h => new { h.UserId, h.ComicId })
            .IsUnique();

        modelBuilder.Entity<ReadingHistory>()
            .HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId);

        modelBuilder.Entity<ReadingHistory>()
            .HasOne(h => h.Comic)
            .WithMany()
            .HasForeignKey(h => h.ComicId);

        modelBuilder.Entity<ComicReview>()
            .HasIndex(r => new { r.UserId, r.ComicId })
            .IsUnique();

        modelBuilder.Entity<ComicReview>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<ComicReview>()
            .HasOne(r => r.Comic)
            .WithMany()
            .HasForeignKey(r => r.ComicId);
    }
}
