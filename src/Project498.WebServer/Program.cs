using Project498.WebServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5272/";
var dcApiUrl = builder.Configuration["DcComicsApiUrl"] ?? "http://localhost:5100";

// Register API-backed services
builder.Services.AddHttpClient<IAuthService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IComicService, ComicApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register Checkout Service
builder.Services.AddHttpClient<ICheckoutService, CheckoutService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register DC Character Service
builder.Services.AddHttpClient<IDcCharacterService, DcCharacterService>(client =>
{
    client.BaseAddress = new Uri(dcApiUrl);
});

builder.Services.AddHttpClient<ICharacterImageService, CharacterImageService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IMarvelCharacterService, MarvelCharacterService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
