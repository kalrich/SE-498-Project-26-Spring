using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebApi.Tests;

/// <summary>
/// Unit tests for MockAuthService.
/// NOTE: These tests are intentionally minimal due to MockAuthService's static initialization.
/// For comprehensive auth testing, consider creating a testable auth interface.
/// </summary>
public class AuthenticationServiceTests
{
    [Fact]
    public void Signup_CreatesNewUser()
    {
        var authService = new MockAuthService();
        var email = $"test{Guid.NewGuid()}@marvel.com";

        var result = authService.Signup("TestUser", email, "password");

        Assert.NotNull(result);
        Assert.Equal("TestUser", result.Username);
    }

    [Fact]
    public void Signup_CanBeRetrieved()
    {
        var authService = new MockAuthService();
        var email = $"retrieve{Guid.NewGuid()}@marvel.com";

        authService.Signup("RetrieveUser", email, "pass");
        var user = authService.GetByEmail(email);

        Assert.NotNull(user);
        Assert.Equal("RetrieveUser", user.Username);
    }

    [Fact]
    public void Signup_CanLogin()
    {
        var authService = new MockAuthService();
        var email = $"login{Guid.NewGuid()}@marvel.com";

        authService.Signup("LoginUser", email, "mypass");
        var result = authService.Login(email, "mypass");

        Assert.NotNull(result);
    }
}
