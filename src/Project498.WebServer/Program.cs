using Project498.WebServer.Services;


var builder = WebApplication.CreateBuilder(args);

// Comments temporarily until DB is connected
// Builder for the DB 
// builder.Services.AddDbContext<AppDbContext>(options =>
// options.UseSqlite("Data Source=Database/project498.db")

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MockAuthService>();
builder.Services.AddSingleton<MockComicService>();
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