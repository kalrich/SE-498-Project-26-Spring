using Project498.WebServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Register API-backed services
builder.Services.AddHttpClient<IAuthService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5272/");
});

builder.Services.AddHttpClient<IComicService, ComicApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5272/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();