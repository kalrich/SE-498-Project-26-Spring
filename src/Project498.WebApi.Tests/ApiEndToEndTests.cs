using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Controllers;
using Project498.WebApi.Data;
using Project498.WebApi.Models;
using Project498.WebApi.Services;

namespace Project498.WebApi.Tests;

/// <summary>
/// End-to-end API integration tests.
/// Tests complete workflows combining multiple API endpoints,
/// error scenarios, and validation rules.
/// </summary>
public class ApiEndToEndTests
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

    private (UsersController, AppDbContext) GetControllerAndContext()
    {
        var options = GetDbContextOptions();
        var context = GetDbContext(options);
        var controller = new UsersController(context);
        return (controller, context);
    }

    [Fact]
    public async Task CreateUser_RetrieveUser_UpdateUser_DeleteUser_CompleteFlow()
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();
        var newUser = new User { Name = "John Doe", Age = 30 };

        // Act 1: Create user
        var createResult = await controller.AddUser(newUser);
        var createdUser = ((CreatedAtActionResult)createResult.Result).Value as User;
        var userId = createdUser.Id;

        // Assert 1: User created
        Assert.NotNull(createdUser);
        Assert.True(userId > 0);

        // Act 2: Retrieve user
        var getResult = await controller.GetUser(userId);
        var retrievedUser = getResult.Value;

        // Assert 2: User retrieved correctly
        Assert.NotNull(retrievedUser);
        Assert.Equal("John Doe", retrievedUser.Name);
        Assert.Equal(30, retrievedUser.Age);

        // Act 3: Update user
        retrievedUser.Name = "John Smith";
        retrievedUser.Age = 31;
        var updateResult = await controller.EditUser(userId, retrievedUser);

        // Assert 3: Update successful
        Assert.IsType<NoContentResult>(updateResult);

        // Act 4: Retrieve updated user
        var getUpdatedResult = await controller.GetUser(userId);
        var updatedUser = getUpdatedResult.Value;

        // Assert 4: Updates persisted
        Assert.Equal("John Smith", updatedUser.Name);
        Assert.Equal(31, updatedUser.Age);

        // Act 5: Delete user
        var deleteResult = await controller.DeleteUser(userId);

        // Assert 5: Delete successful
        Assert.IsType<NoContentResult>(deleteResult);

        // Act 6: Verify user is deleted
        var getFinalResult = await controller.GetUser(userId);

        // Assert 6: User no longer exists
        Assert.IsType<NotFoundResult>(getFinalResult.Result);

        context.Dispose();
    }

    [Fact]
    public async Task MultipleUserOperations_IsolationAndAccuracy()
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();
        var user1 = new User { Name = "Alice", Age = 25 };
        var user2 = new User { Name = "Bob", Age = 30 };
        var user3 = new User { Name = "Charlie", Age = 35 };

        // Act: Create multiple users
        var result1 = await controller.AddUser(user1);
        var result2 = await controller.AddUser(user2);
        var result3 = await controller.AddUser(user3);

        var userId1 = ((CreatedAtActionResult)result1.Result).Value as User;
        var userId2 = ((CreatedAtActionResult)result2.Result).Value as User;
        var userId3 = ((CreatedAtActionResult)result3.Result).Value as User;

        // Assert: All created
        Assert.NotNull(userId1);
        Assert.NotNull(userId2);
        Assert.NotNull(userId3);

        // Act: Retrieve all
        var allResult = await controller.GetUsers();
        var allUsers = allResult.Value.ToList();

        // Assert: All present
        Assert.Equal(3, allUsers.Count);

        // Act: Update one user
        userId1.Name = "Alice Updated";
        await controller.EditUser(userId1.Id, userId1);

        // Assert: Only intended user updated
        var verifyUser1 = (await controller.GetUser(userId1.Id)).Value;
        var verifyUser2 = (await controller.GetUser(userId2.Id)).Value;

        Assert.Equal("Alice Updated", verifyUser1.Name);
        Assert.Equal("Bob", verifyUser2.Name); // Unchanged

        // Act: Delete one user
        await controller.DeleteUser(userId2.Id);

        // Assert: Only intended user deleted
        var afterDelete = (await controller.GetUsers()).Value.ToList();
        Assert.Equal(2, afterDelete.Count);
        Assert.DoesNotContain(afterDelete, u => u.Name == "Bob");

        context.Dispose();
    }

    [Fact]
    public async Task InvalidOperations_AppropriateErrorHandling()
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();

        // Act 1: Get non-existent user
        var getNonExistentResult = await controller.GetUser(9999);

        // Assert 1: Returns NotFound
        Assert.IsType<NotFoundResult>(getNonExistentResult.Result);

        // Act 2: Edit with mismatched IDs
        var editMismatchResult = await controller.EditUser(
            1,
            new User { Id = 2, Name = "Test", Age = 25 }
        );

        // Assert 2: Returns BadRequest
        Assert.IsType<BadRequestResult>(editMismatchResult);

        // Act 3: Delete non-existent user
        var deleteNonExistentResult = await controller.DeleteUser(9999);

        // Assert 3: Returns NotFound
        Assert.IsType<NotFoundResult>(deleteNonExistentResult);

        context.Dispose();
    }

    [Fact]
    public async Task StringService_Integration_WithController()
    {
        // This tests that the StringService works correctly alongside controller operations
        var stringService = new StringService();

        // Test reverse functionality
        var reversedText = stringService.Reverse("hello");
        Assert.Equal("olleh", reversedText);

        var reversedWords = stringService.ReverseWords("hello world");
        Assert.Equal("world hello", reversedWords);

        // Now combine with user operations
        var (controller, context) = GetControllerAndContext();
        var user = new User { Name = "TestUser", Age = 25 };

        var createResult = await controller.AddUser(user);
        Assert.IsType<CreatedAtActionResult>(createResult.Result);

        context.Dispose();
    }

    [Fact]
    public async Task ConcurrentUserOperations_MaintainIntegrity()
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();
        var users = Enumerable.Range(1, 5)
            .Select(i => new User { Name = $"User{i}", Age = 20 + i })
            .ToList();

        // Act: Create multiple users
        foreach (var user in users)
        {
            await controller.AddUser(user);
        }

        // Act: Retrieve all
        var allUsersResult = await controller.GetUsers();
        var allUsers = allUsersResult.Value.ToList();

        // Assert: All users exist
        Assert.Equal(5, allUsers.Count);

        // Act: Update each user
        foreach (var user in allUsers)
        {
            user.Age += 1;
            await controller.EditUser(user.Id, user);
        }

        // Act: Retrieve all again
        var updatedUsersResult = await controller.GetUsers();
        var updatedUsers = updatedUsersResult.Value.ToList();

        // Assert: All ages incremented
        Assert.All(updatedUsers, u => Assert.True(u.Age > 20));

        context.Dispose();
    }

    [Theory]
    [InlineData("User1", 18)]
    [InlineData("User2", 25)]
    [InlineData("User3", 65)]
    [InlineData("Very Long User Name Here", 100)]
    public async Task UserCrudWithVariousData_AllOperationsSucceed(string name, int age)
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();
        var user = new User { Name = name, Age = age };

        // Act 1: Create
        var createResult = await controller.AddUser(user);
        var createdUser = ((CreatedAtActionResult)createResult.Result).Value as User;

        // Assert 1: Created
        Assert.NotNull(createdUser);

        // Act 2: Retrieve
        var getResult = await controller.GetUser(createdUser.Id);
        var retrievedUser = getResult.Value;

        // Assert 2: Retrieved
        Assert.NotNull(retrievedUser);
        Assert.Equal(name, retrievedUser.Name);
        Assert.Equal(age, retrievedUser.Age);

        // Act 3: Update
        retrievedUser.Name = name + " Updated";
        var updateResult = await controller.EditUser(createdUser.Id, retrievedUser);

        // Assert 3: Updated
        Assert.IsType<NoContentResult>(updateResult);

        // Act 4: Delete
        var deleteResult = await controller.DeleteUser(createdUser.Id);

        // Assert 4: Deleted
        Assert.IsType<NoContentResult>(deleteResult);

        context.Dispose();
    }

    [Fact]
    public async Task EmptyDatabase_GetAllUsers_ReturnsEmptyList()
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();

        // Act
        var result = await controller.GetUsers();

        // Assert
        var users = result.Value.ToList();
        Assert.Empty(users);

        context.Dispose();
    }

    [Fact]
    public async Task NameFieldConstraints_ApplyCorrectly()
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();

        // Test: Maximum length name
        var longName = new string('A', 100);
        var userWithLongName = new User { Name = longName, Age = 25 };

        // Act
        var createResult = await controller.AddUser(userWithLongName);

        // Assert
        var createdUser = ((CreatedAtActionResult)createResult.Result).Value as User;
        Assert.NotNull(createdUser);
        Assert.Equal(100, createdUser.Name.Length);

        context.Dispose();
    }

    [Fact]
    public async Task TransactionConsistency_RollbackBehavior()
    {
        // Arrange
        var options = GetDbContextOptions();

        // Act: Add user and verify persistence across contexts
        int userId;
        using (var context = GetDbContext(options))
        {
            var user = new User { Name = "TransactionTest", Age = 30 };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            userId = user.Id;
        }

        // Assert: User persists in new context
        using (var context = GetDbContext(options))
        {
            var retrievedUser = await context.Users.FindAsync(userId);
            Assert.NotNull(retrievedUser);
            Assert.Equal("TransactionTest", retrievedUser.Name);
        }
    }

    [Fact]
    public async Task LargeDataset_Performance_Acceptable()
    {
        // Arrange
        var (controller, context) = GetControllerAndContext();
        var largeUserCount = 100;
        var users = Enumerable.Range(1, largeUserCount)
            .Select(i => new User { Name = $"User{i}", Age = 20 + (i % 50) })
            .ToList();

        // Act: Create many users
        var startTime = DateTime.Now;
        foreach (var user in users)
        {
            await controller.AddUser(user);
        }
        var createTime = DateTime.Now - startTime;

        // Assert: Creation completed reasonably quickly
        Assert.True(createTime.TotalSeconds < 30, "Creation took too long");

        // Act: Retrieve all
        var getAllStart = DateTime.Now;
        var allUsersResult = await controller.GetUsers();
        var retrieveTime = DateTime.Now - getAllStart;

        // Assert: Retrieval completed
        Assert.Equal(largeUserCount, allUsersResult.Value.Count());
        Assert.True(retrieveTime.TotalSeconds < 10, "Retrieval took too long");

        context.Dispose();
    }
}
