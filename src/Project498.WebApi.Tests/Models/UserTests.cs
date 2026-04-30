using Project498.WebApi.Models;
using Xunit;

namespace Project498.WebApi.Tests.Models;

public class UserTests
{
    [Fact]
    public void User_ShouldInitializeWithDefaultValues()
    {
        var user = new User();

        Assert.Equal("", user.Username);
        Assert.Equal("", user.Email);
        Assert.Equal("", user.Password);
        Assert.Equal(0, user.Id);
    }

    [Fact]
    public void User_ShouldSetProperties()
    {
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        Assert.Equal(1, user.Id);
        Assert.Equal("testuser", user.Username);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("password123", user.Password);
    }
}
