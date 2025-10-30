using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Repositories;
using Xunit;

namespace TaskManager.UnitTests.Repositories;

public class UserRepositoryTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsyncAndGetByEmailWorks()
    {
        await using var db = CreateDbContext();
        var repo = new UserRepository(db);

        var user = new User { Email = "user@example.com", PasswordHash = "hash" };
        await repo.AddAsync(user);

        var fetched = await repo.GetByEmailAsync("user@example.com");
        fetched.Should().NotBeNull();
        fetched!.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task AnyByEmailAsyncReturnsTrueWhenExists()
    {
        await using var db = CreateDbContext();
        var repo = new UserRepository(db);

        await repo.AddAsync(new User { Email = "exists@example.com", PasswordHash = "x" });

        var any = await repo.AnyByEmailAsync("exists@example.com");
        any.Should().BeTrue();
        (await repo.AnyByEmailAsync("missing@example.com")).Should().BeFalse();
    }
}


