using Project498.WebServer.Models;
using Project498.WebServer.Services;

namespace Project498.WebApi.Tests;

/// <summary>
/// Integration tests for MockAuthService workflows.
/// NOTE: These tests focus on signup workflows due to MockAuthService's static nature.
/// </summary>
public class AuthenticationIntegrationTests
{
    [Fact]
    public void SignupLoginWorkflow()
    {
        var authService = new MockAuthService();
        var email = $"workflow{Guid.NewGuid()}@marvel.com";

        // Signup
        var signup = authService.Signup("WorkflowUser", email, "password123");
        Assert.NotNull(signup);

        // Login
        var login = authService.Login(email, "password123");
        Assert.NotNull(login);
        Assert.Equal("WorkflowUser", login.Username);
    }

    [Fact]
    public void ProfileUpdateAfterSignup()
    {
        var authService = new MockAuthService();
        var email = $"update{Guid.NewGuid()}@marvel.com";
        var newEmail = $"updated{Guid.NewGuid()}@marvel.com";

        authService.Signup("UpdateUser", email, "pass");
        var updateResult = authService.UpdateProfile(email, "UpdatedUser", newEmail, "newpass");

        Assert.True(updateResult);
        var updated = authService.GetByEmail(newEmail);
        Assert.NotNull(updated);
    }

    [Fact]
    public void MultipleSignupsWork()
    {
        var authService = new MockAuthService();
        var email1 = $"user1{Guid.NewGuid()}@marvel.com";
        var email2 = $"user2{Guid.NewGuid()}@marvel.com";

        var u1 = authService.Signup("User1", email1, "pass1");
        var u2 = authService.Signup("User2", email2, "pass2");

        Assert.NotNull(u1);
        Assert.NotNull(u2);

        var login1 = authService.Login(email1, "pass1");
        var login2 = authService.Login(email2, "pass2");

        Assert.NotNull(login1);
        Assert.NotNull(login2);
    }

    [Fact]
    public void EmailValidation()
    {
        var authService = new MockAuthService();
        var email = $"validate{Guid.NewGuid()}@marvel.com";

        authService.Signup("ValidateUser", email, "pass");
        var exists = authService.EmailExists(email);

        Assert.True(exists);
    }
}
