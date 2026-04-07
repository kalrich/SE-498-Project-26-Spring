using Project498.WebServer.Models;

namespace Project498.WebServer.Services;

public class MockAuthService
{
    private static readonly List<MockUser> Users =
    [
        new MockUser
        {
            Username = "Peter Parker",
            Email = "peter@marvel.com",
            Password = "spiderman123"
        },
        new MockUser
        {
            Username = "Tony Stark",
            Email = "tony@marvel.com",
            Password = "ironman123"
        }
    ];

    public MockUser? Login(string email, string password)
    {
        return Users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }

    public bool EmailExists(string email)
    {
        return Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public MockUser Signup(string username, string email, string password)
    {
        var user = new MockUser
        {
            Username = username,
            Email = email,
            Password = password
        };

        Users.Add(user);
        return user;
    }

    public MockUser? GetByEmail(string email)
    {
        return Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public bool UpdateProfile(string currentEmail, string newUsername, string newEmail, string? newPassword)
    {
        var user = GetByEmail(currentEmail);
        if (user == null) return false;

        var emailTakenByAnotherUser = Users.Any(u =>
            u.Email.Equals(newEmail, StringComparison.OrdinalIgnoreCase) &&
            !u.Email.Equals(currentEmail, StringComparison.OrdinalIgnoreCase));

        if (emailTakenByAnotherUser)
        {
            return false;
        }

        user.Username = newUsername;
        user.Email = newEmail;

        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            user.Password = newPassword;
        }

        return true;
    }

    public List<MockUser> GetAllUsers()
    {
        return Users;
    }
}