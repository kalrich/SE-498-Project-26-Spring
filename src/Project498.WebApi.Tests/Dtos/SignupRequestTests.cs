using Project498.WebApi.Dtos;
using Xunit;

namespace Project498.WebApi.Tests.Dtos;

public class SignupRequestTests
{
    [Fact]
    public void SignupRequest_ShouldInitializeWithDefaultValues()
    {
        var request = new SignupRequest();

        Assert.Equal("", request.Username);
        Assert.Equal("", request.Email);
        Assert.Equal("", request.Password);
    }

    [Fact]
    public void SignupRequest_ShouldSetProperties()
    {
        var request = new SignupRequest
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "securepass"
        };

        Assert.Equal("newuser", request.Username);
        Assert.Equal("newuser@example.com", request.Email);
        Assert.Equal("securepass", request.Password);
    }
}
