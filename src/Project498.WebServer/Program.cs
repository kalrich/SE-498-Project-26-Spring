using Project498.WebServer.Services;

using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5272/";

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
builder.Services.AddScoped<ICheckoutService, CheckoutService>();

// Configure HttpClient for DC Comics API
var dcApiUrl = builder.Configuration["DcComicsApiUrl"] ?? "http://localhost:8080";
builder.Services.AddHttpClient<CheckoutService>(client =>
{
    client.BaseAddress = new Uri(dcApiUrl);
});

// Add Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

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