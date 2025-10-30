using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Repositories;
using Xunit;

namespace TaskManager.UnitTests.Repositories;

public class TaskRepositoryTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetAllForUserAsync_ReturnsOnlyUserTasks()
    {
        await using var db = CreateDbContext();
        var repo = new TaskRepository(db);

        await repo.AddAsync(new TaskItem { Title = "A", UserId = 1, IsDone = false });
        await repo.AddAsync(new TaskItem { Title = "B", UserId = 2, IsDone = true });
        await repo.AddAsync(new TaskItem { Title = "C", UserId = 1, IsDone = true });

        var tasks = await repo.GetAllForUserAsync(1);
        tasks.Should().HaveCount(2);
        tasks.Select(t => t.Title).Should().BeEquivalentTo("A", "C");
    }

    [Fact]
    public async Task GetByIdForUserAsync_RespectsOwnership()
    {
        await using var db = CreateDbContext();
        var repo = new TaskRepository(db);

        var mine = await repo.AddAsync(new TaskItem { Title = "Mine", UserId = 5, IsDone = false });
        var notMine = await repo.AddAsync(new TaskItem { Title = "NotMine", UserId = 6, IsDone = false });

        (await repo.GetByIdForUserAsync(mine.Id, 5))!.Title.Should().Be("Mine");
        (await repo.GetByIdForUserAsync(notMine.Id, 5)).Should().BeNull();
    }

    [Fact]
    public async Task GetPagedForUserAsync_AppliesFiltersAndPaging()
    {
        await using var db = CreateDbContext();
        var repo = new TaskRepository(db);

        for (var i = 1; i <= 6; i++)
        {
            await repo.AddAsync(new TaskItem { Title = $"t{i}", UserId = 10, IsDone = i % 2 == 0 });
        }

        await repo.AddAsync(new TaskItem { Title = "other", UserId = 11, IsDone = true });

        var (items, total) = await repo.GetPagedForUserAsync(userId: 10, page: 2, pageSize: 2, search: "t", isDone: true);

        total.Should().Be(3);
        items.Should().HaveCount(1);
        items.Should().OnlyContain(t => t.UserId == 10 && t.IsDone);
    }
}


