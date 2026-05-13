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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Comic>().ToTable("Comics");
        modelBuilder.Entity<UserComic>().ToTable("UserComics");
        modelBuilder.Entity<Checkout>().ToTable("Checkouts");
        modelBuilder.Entity<CharacterImage>().ToTable("CharacterImages");
        modelBuilder.Entity<MarvelCharacter>().ToTable("MarvelCharacters");

        modelBuilder.Entity<UserComic>()
            .Property(uc => uc.CurrentPage)
            .HasDefaultValue(1);

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
    }
}
