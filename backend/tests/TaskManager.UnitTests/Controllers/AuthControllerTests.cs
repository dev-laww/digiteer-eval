using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API;
using TaskManager.Dto;
using TaskManager.Models;
using TaskManager.Repositories;
using TaskManager.Services;
using Xunit;

namespace TaskManager.UnitTests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task RegisterConflictingEmailReturns409()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.AnyByEmailAsync(It.IsAny<string>())).ReturnsAsync(true);

        var tokens = new Mock<ITokenService>();
        var controller = new AuthController(users.Object, tokens.Object);

        var result = await controller.Register(new AuthController.RegisterRequest("dup@example.com", "x"));
        var obj = Assert.IsType<ObjectResult>(result);

        obj.StatusCode.Should().Be(409);
        var response = Assert.IsType<ApiResponse<AuthController.AuthResponse>>(obj.Value);
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task LoginValidCredentialsReturnsToken()
    {
        var user = new User { Id = 1, Email = "user@example.com" };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Passw0rd!");

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByEmailAsync("user@example.com")).ReturnsAsync(user);

        var tokens = new Mock<ITokenService>();
        tokens.Setup(t => t.GenerateToken(user)).Returns("TEST_TOKEN");

        var controller = new AuthController(users.Object, tokens.Object);

        var result = await controller.Login(new AuthController.LoginRequest("user@example.com", "Passw0rd!"));
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AuthController.AuthResponse>>(ok.Value);
        response.Success.Should().BeTrue();
        response.Data!.Token.Should().Be("TEST_TOKEN");
    }
}


