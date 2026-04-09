using Microsoft.EntityFrameworkCore;
using Project498.WebServer.Models;

namespace Project498.WebServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Comic> Comics => Set<Comic>();
    public DbSet<UserComic> UserComics => Set<UserComic>();
}