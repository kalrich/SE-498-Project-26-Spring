using Project498.WebApi.Dtos;
using Xunit;

namespace Project498.WebApi.Tests.Dtos;

public class LoginRequestTests
{
    [Fact]
    public void LoginRequest_ShouldInitializeWithDefaultValues()
    {
        var request = new LoginRequest();

        Assert.Equal("", request.Email);
        Assert.Equal("", request.Password);
    }

    [Fact]
    public void LoginRequest_ShouldSetProperties()
    {
        var request = new LoginRequest
        {
            Email = "user@example.com",
            Password = "mypassword"
        };

        Assert.Equal("user@example.com", request.Email);
        Assert.Equal("mypassword", request.Password);
    }
}
