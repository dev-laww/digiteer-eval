using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Repositories;
using Xunit;

namespace TaskManager.UnitTests.Repositories;

public class BaseRepositoryTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAllAsyncWithPredicateFilters()
    {
        await using var db = CreateDbContext();
        var repo = new TaskRepository(db);

        await repo.AddAsync(new TaskItem { Title = "alpha", UserId = 1, IsDone = false });
        await repo.AddAsync(new TaskItem { Title = "beta", UserId = 1, IsDone = true });

        var all = await repo.GetAllAsync();
        var onlyDone = await repo.GetAllAsync(t => t.IsDone);

        all.Should().HaveCount(2);
        onlyDone.Should().HaveCount(1);
        onlyDone[0].Title.Should().Be("beta");
    }

    [Fact]
    public async Task FirstOrDefaultAsyncAndGetByIdAsyncWork()
    {
        await using var db = CreateDbContext();
        var repo = new TaskRepository(db);

        var created = await repo.AddAsync(new TaskItem { Title = "one", UserId = 2, IsDone = false });
        var first = await repo.FirstOrDefaultAsync(t => t.Title == "one");
        var byId = await repo.GetByIdAsync(created.Id);

        first!.Id.Should().Be(created.Id);
        byId!.Title.Should().Be("one");
    }

    [Fact]
    public async Task UpdateWhereAsyncAppliesMutationsAndDeleteWhereAsyncRemoves()
    {
        await using var db = CreateDbContext();
        var repo = new TaskRepository(db);

        for (var i = 0; i < 4; i++)
        {
            await repo.AddAsync(new TaskItem { Title = $"t{i}", UserId = 3, IsDone = false });
        }

        var updatedCount = await repo.UpdateWhereAsync(t => t.UserId == 3, t => t.IsDone = true);
        updatedCount.Should().BeGreaterThan(0);

        var all = await repo.GetAllAsync(t => t.UserId == 3);
        all.Should().OnlyContain(t => t.IsDone);

        var deleted = await repo.DeleteWhereAsync(t => t.Title.StartsWith("t"));
        deleted.Should().BeGreaterThan(0);

        (await repo.GetAllAsync()).Should().BeEmpty();
    }
}


