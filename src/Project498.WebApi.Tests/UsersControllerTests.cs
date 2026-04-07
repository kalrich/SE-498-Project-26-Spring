using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project498.WebApi.Controllers;
using Project498.WebApi.Data;
using Project498.WebApi.Models;

namespace Project498.WebApi.Tests;

public class UsersControllerTests
{
    private DbContextOptions<AppDbContext> GetDbContextOptions()
    {
        // Use an in-memory database for testing
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private AppDbContext GetDbContext(DbContextOptions<AppDbContext> options)
    {
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetUsers_ReturnsAllUsers()
    {
        // Arrange
        var options = GetDbContextOptions();
        using (var context = GetDbContext(options))
        {
            context.Users.AddRange(
                new User { Name = "Alice", Age = 25 },
                new User { Name = "Bob", Age = 30 },
                new User { Name = "Charlie", Age = 28 }
            );
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);
            var result = await controller.GetUsers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var returnValue = Assert.IsType<List<User>>(actionResult.Value);
            Assert.Equal(3, returnValue.Count);
            Assert.Single(returnValue.Where(u => u.Name == "Alice"));
        }
    }

    [Fact]
    public async Task GetUser_WithValidId_ReturnsUser()
    {
        // Arrange
        var options = GetDbContextOptions();
        User testUser;

        using (var context = GetDbContext(options))
        {
            testUser = new User { Name = "TestUser", Age = 25 };
            context.Users.Add(testUser);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);
            var result = await controller.GetUser(testUser.Id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var returnValue = Assert.IsType<User>(actionResult.Value);
            Assert.Equal(testUser.Id, returnValue.Id);
            Assert.Equal("TestUser", returnValue.Name);
            Assert.Equal(25, returnValue.Age);
        }
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var options = GetDbContextOptions();
        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);

            // Act
            var result = await controller.GetUser(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task AddUser_WithValidUser_ReturnsCreatedAtAction()
    {
        // Arrange
        var options = GetDbContextOptions();
        var newUser = new User { Name = "NewUser", Age = 30 };

        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);

            // Act
            var result = await controller.AddUser(newUser);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(UsersController.GetUser), createdResult.ActionName);
            var returnedUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal("NewUser", returnedUser.Name);
            Assert.Equal(30, returnedUser.Age);
        }
    }

    [Fact]
    public async Task AddUser_PersistsToDatabase()
    {
        // Arrange
        var options = GetDbContextOptions();
        var newUser = new User { Name = "PersistenceTest", Age = 35 };

        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);
            await controller.AddUser(newUser);
        }

        // Act & Assert
        using (var context = GetDbContext(options))
        {
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Name == "PersistenceTest");
            Assert.NotNull(savedUser);
            Assert.Equal(35, savedUser.Age);
        }
    }

    [Fact]
    public async Task EditUser_WithValidData_UpdatesUser()
    {
        // Arrange
        var options = GetDbContextOptions();
        User testUser;

        using (var context = GetDbContext(options))
        {
            testUser = new User { Name = "OriginalName", Age = 25 };
            context.Users.Add(testUser);
            await context.SaveChangesAsync();
        }

        var updatedUser = new User { Id = testUser.Id, Name = "UpdatedName", Age = 26 };

        // Act
        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);
            var result = await controller.EditUser(testUser.Id, updatedUser);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Verify update in database
        using (var context = GetDbContext(options))
        {
            var verifyUser = await context.Users.FindAsync(testUser.Id);
            Assert.NotNull(verifyUser);
            Assert.Equal("UpdatedName", verifyUser.Name);
            Assert.Equal(26, verifyUser.Age);
        }
    }

    [Fact]
    public async Task EditUser_WithMismatchedId_ReturnsBadRequest()
    {
        // Arrange
        var options = GetDbContextOptions();
        var user = new User { Name = "Test", Age = 25 };

        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);

            // Act
            var result = await controller.EditUser(1, new User { Id = 2, Name = "Test", Age = 25 });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }

    [Fact]
    public async Task DeleteUser_WithValidId_RemovesUser()
    {
        // Arrange
        var options = GetDbContextOptions();
        User testUser;

        using (var context = GetDbContext(options))
        {
            testUser = new User { Name = "DeleteMe", Age = 25 };
            context.Users.Add(testUser);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);
            var result = await controller.DeleteUser(testUser.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Verify deletion
        using (var context = GetDbContext(options))
        {
            var deletedUser = await context.Users.FindAsync(testUser.Id);
            Assert.Null(deletedUser);
        }
    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var options = GetDbContextOptions();
        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);

            // Act
            var result = await controller.DeleteUser(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Theory]
    [InlineData("ValidName", 18)]
    [InlineData("Another User", 65)]
    [InlineData("X", 100)]
    public async Task AddUser_WithVariousValidData_SuccessfullyCreates(string name, int age)
    {
        // Arrange
        var options = GetDbContextOptions();
        var user = new User { Name = name, Age = age };

        using (var context = GetDbContext(options))
        {
            var controller = new UsersController(context);

            // Act
            var result = await controller.AddUser(user);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(name, returnedUser.Name);
            Assert.Equal(age, returnedUser.Age);
        }
    }
}
