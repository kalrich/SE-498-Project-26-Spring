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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Comic>().ToTable("Comics");
        modelBuilder.Entity<UserComic>().ToTable("UserComics");

        modelBuilder.Entity<UserComic>()
            .HasOne(uc => uc.User)
            .WithMany()
            .HasForeignKey(uc => uc.UserId);

        modelBuilder.Entity<UserComic>()
            .HasOne(uc => uc.Comic)
            .WithMany()
            .HasForeignKey(uc => uc.ComicId);
    }
}