using Project498.WebServer.Services;
using Microsoft.EntityFrameworkCore;
using Project498.WebServer.Data;

var builder = WebApplication.CreateBuilder(args);


// Builder for the DB 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Database/project498.db"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MockAuthService>();
builder.Services.AddScoped<MockComicService>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enables files from wwwroot like css, images, PDFs, etc.
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();