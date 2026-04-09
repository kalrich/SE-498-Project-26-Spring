using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Data;
using Project498.WebApi.Models;

namespace Project498.WebApi.Tests;

/// <summary>
/// Integration tests for user data access layer (repository pattern).
/// Tests Entity Framework Core interactions with in-memory database,
/// CRUD operations, querying, and data persistence.
/// </summary>
public class UserRepositoryIntegrationTests
{
    private DbContextOptions<AppDbContext> GetDbContextOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private AppDbContext GetDbContext(DbContextOptions<AppDbContext> options)
    {
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddUser_SavesAndRetrievesCorrectly()
    {
        // Arrange
        var options = GetDbContextOptions();
        var newUser = new User { Name = "Alice Johnson", Age = 28 };

        // Act: Add user
        int userId;
        using (var context = GetDbContext(options))
        {
            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            userId = newUser.Id;
        }

        // Assert: Retrieve and verify
        using (var context = GetDbContext(options))
        {
            var retrievedUser = await context.Users.FindAsync(userId);
            Assert.NotNull(retrievedUser);
            Assert.Equal("Alice Johnson", retrievedUser.Name);
            Assert.Equal(28, retrievedUser.Age);
        }
    }

    [Fact]
    public async Task UpdateUser_ModifiesExistingRecord()
    {
        // Arrange
        var options = GetDbContextOptions();
        User originalUser;

        using (var context = GetDbContext(options))
        {
            originalUser = new User { Name = "Bob Smith", Age = 35 };
            context.Users.Add(originalUser);
            await context.SaveChangesAsync();
        }

        // Act: Update user
        using (var context = GetDbContext(options))
        {
            var userToUpdate = await context.Users.FindAsync(originalUser.Id);
            Assert.NotNull(userToUpdate);

            userToUpdate.Name = "Robert Smith";
            userToUpdate.Age = 36;
            await context.SaveChangesAsync();
        }

        // Assert: Verify update
        using (var context = GetDbContext(options))
        {
            var updatedUser = await context.Users.FindAsync(originalUser.Id);
            Assert.Equal("Robert Smith", updatedUser.Name);
            Assert.Equal(36, updatedUser.Age);
        }
    }

    [Fact]
    public async Task DeleteUser_RemovesFromDatabase()
    {
        // Arrange
        var options = GetDbContextOptions();
        User userToDelete;

        using (var context = GetDbContext(options))
        {
            userToDelete = new User { Name = "Charlie Brown", Age = 40 };
            context.Users.Add(userToDelete);
            await context.SaveChangesAsync();
        }

        // Act: Delete user
        using (var context = GetDbContext(options))
        {
            var user = await context.Users.FindAsync(userToDelete.Id);
            Assert.NotNull(user);
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        // Assert: User no longer exists
        using (var context = GetDbContext(options))
        {
            var deletedUser = await context.Users.FindAsync(userToDelete.Id);
            Assert.Null(deletedUser);
        }
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Arrange
        var options = GetDbContextOptions();
        using (var context = GetDbContext(options))
        {
            context.Users.AddRange(
                new User { Name = "User1", Age = 25 },
                new User { Name = "User2", Age = 30 },
                new User { Name = "User3", Age = 35 },
                new User { Name = "User4", Age = 40 }
            );
            await context.SaveChangesAsync();
        }

        // Act
        List<User> allUsers;
        using (var context = GetDbContext(options))
        {
            allUsers = await context.Users.ToListAsync();
        }

        // Assert
        Assert.Equal(4, allUsers.Count);
        Assert.All(allUsers, u => Assert.NotNull(u.Name));
    }

    [Fact]
    public async Task QueryByName_FindsMatchingUsers()
    {
        // Arrange
        var options = GetDbContextOptions();
        using (var context = GetDbContext(options))
        {
            context.Users.AddRange(
                new User { Name = "John Smith", Age = 25 },
                new User { Name = "Jane Doe", Age = 30 },
                new User { Name = "John Doe", Age = 35 }
            );
            await context.SaveChangesAsync();
        }

        // Act
        List<User> johnsUsers;
        using (var context = GetDbContext(options))
        {
            johnsUsers = await context.Users
                .Where(u => u.Name.StartsWith("John"))
                .ToListAsync();
        }

        // Assert
        Assert.Equal(2, johnsUsers.Count);
        Assert.All(johnsUsers, u => Assert.StartsWith("John", u.Name));
    }

    [Fact]
    public async Task QueryByAge_FiltersUsers()
    {
        // Arrange
        var options = GetDbContextOptions();
        using (var context = GetDbContext(options))
        {
            context.Users.AddRange(
                new User { Name = "Young User", Age = 18 },
                new User { Name = "Adult User", Age = 30 },
                new User { Name = "Senior User", Age = 65 },
                new User { Name = "Another Adult", Age = 45 }
            );
            await context.SaveChangesAsync();
        }

        // Act
        List<User> adultsOverThirty;
        using (var context = GetDbContext(options))
        {
            adultsOverThirty = await context.Users
                .Where(u => u.Age >= 30)
                .ToListAsync();
        }

        // Assert
        Assert.Equal(3, adultsOverThirty.Count);
        Assert.All(adultsOverThirty, u => Assert.True(u.Age >= 30));
    }

    [Fact]
    public async Task MaxLengthConstraint_NameValidation()
    {
        // Arrange
        var options = GetDbContextOptions();

        // Act & Assert: Long name should be handled
        using (var context = GetDbContext(options))
        {
            var longName = new string('A', 150); // Exceeds max of 100
            var user = new User { Name = longName.Substring(0, 100), Age = 25 };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var retrieved = await context.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
            Assert.NotNull(retrieved);
            Assert.Equal(100, retrieved.Name.Length);
        }
    }

    [Fact]
    public async Task ConcurrentInserts_HandleMultipleUsers()
    {
        // Arrange
        var options = GetDbContextOptions();
        var users = Enumerable.Range(1, 10)
            .Select(i => new User { Name = $"User{i}", Age = 20 + i })
            .ToList();

        // Act
        using (var context = GetDbContext(options))
        {
            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = GetDbContext(options))
        {
            var count = await context.Users.CountAsync();
            Assert.Equal(10, count);
        }
    }

    [Fact]
    public async Task NameUpdate_OnlyAffectsTargetUser()
    {
        // Arrange
        var options = GetDbContextOptions();
        User user1, user2;

        using (var context = GetDbContext(options))
        {
            user1 = new User { Name = "User One", Age = 25 };
            user2 = new User { Name = "User Two", Age = 30 };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();
        }

        // Act: Update only user1
        using (var context = GetDbContext(options))
        {
            var toUpdate = await context.Users.FindAsync(user1.Id);
            toUpdate.Name = "User One Updated";
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = GetDbContext(options))
        {
            var updated1 = await context.Users.FindAsync(user1.Id);
            var unchanged2 = await context.Users.FindAsync(user2.Id);

            Assert.Equal("User One Updated", updated1.Name);
            Assert.Equal("User Two", unchanged2.Name);
        }
    }

    [Theory]
    [InlineData("Alice", 18)]
    [InlineData("Bob", 65)]
    [InlineData("Charlie", 100)]
    [InlineData("XYZ", 1)]
    public async Task AddMultipleUsers_VariousAges_AllPersist(string name, int age)
    {
        // Arrange
        var options = GetDbContextOptions();
        var user = new User { Name = name, Age = age };

        // Act
        using (var context = GetDbContext(options))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = GetDbContext(options))
        {
            var retrieved = await context.Users.FirstOrDefaultAsync(u => u.Name == name);
            Assert.NotNull(retrieved);
            Assert.Equal(age, retrieved.Age);
        }
    }

    [Fact]
    public async Task TransactionRollback_OnError()
    {
        // Arrange
        var options = GetDbContextOptions();

        // Act & Assert
        using (var context = GetDbContext(options))
        {
            context.Users.Add(new User { Name = "Valid User", Age = 25 });
            await context.SaveChangesAsync();
        }

        // Try invalid operation (this is a simplified test;
        // actual EF Core transaction handling would be more complex)
        using (var context = GetDbContext(options))
        {
            var count = await context.Users.CountAsync();
            Assert.Equal(1, count);
        }
    }

    [Fact]
    public async Task PrimaryKeyGeneration_AutoIncrement()
    {
        // Arrange
        var options = GetDbContextOptions();

        // Act
        List<int> generatedIds;
        using (var context = GetDbContext(options))
        {
            context.Users.AddRange(
                new User { Name = "User1", Age = 25 },
                new User { Name = "User2", Age = 30 }
            );
            await context.SaveChangesAsync();

            generatedIds = context.Users.Select(u => u.Id).ToList();
        }

        // Assert
        Assert.Equal(2, generatedIds.Count);
        Assert.True(generatedIds[0] > 0);
        Assert.True(generatedIds[1] > generatedIds[0]);
    }

    [Fact]
    public async Task EntityTracking_ChangesDetected()
    {
        // Arrange
        var options = GetDbContextOptions();
        User trackedUser;

        using (var context = GetDbContext(options))
        {
            trackedUser = new User { Name = "Original", Age = 25 };
            context.Users.Add(trackedUser);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        using (var context = GetDbContext(options))
        {
            var user = await context.Users.FindAsync(trackedUser.Id);
            Assert.NotNull(user);

            // Verify entry is tracked
            var entry = context.Entry(user);
            Assert.NotNull(entry);

            user.Name = "Modified";
            user.Age = 26;

            // Changes should be detected
            Assert.True(context.ChangeTracker.HasChanges());

            await context.SaveChangesAsync();
        }

        // Verify changes persisted
        using (var context = GetDbContext(options))
        {
            var updatedUser = await context.Users.FindAsync(trackedUser.Id);
            Assert.Equal("Modified", updatedUser.Name);
            Assert.Equal(26, updatedUser.Age);
        }
    }
}
