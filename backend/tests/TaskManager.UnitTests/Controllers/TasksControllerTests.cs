using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API;
using TaskManager.Dto;
using TaskManager.Models;
using TaskManager.Repositories;
using Xunit;

namespace TaskManager.UnitTests.Controllers;

public class TasksControllerTests
{
    private static TasksController CreateControllerWithUser(ITaskRepository repo, int userId)
    {
        var controller = new TasksController(repo);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "TestAuth"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        return controller;
    }

    [Fact]
    public async Task Get_ReturnsMappedTasks_ForCurrentUser()
    {
        var repo = new Mock<ITaskRepository>();
        repo.Setup(r => r.GetAllForUserAsync(7)).ReturnsAsync([
            new TaskItem { Id = 1, Title = "A", IsDone = false, UserId = 7 },
            new TaskItem { Id = 2, Title = "B", IsDone = true, UserId = 7 }
        ]);

        var controller = CreateControllerWithUser(repo.Object, 7);

        var result = await controller.Get();
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<List<TasksController.TaskResponse>>>(ok.Value);
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
        response.Data![0].Title.Should().Be("A");
    }

    [Fact]
    public async Task Create_Returns201_AndPersists()
    {
        var repo = new Mock<ITaskRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync((TaskItem t) => { t.Id = 99; return t; });

        var controller = CreateControllerWithUser(repo.Object, 3);
        var result = await controller.Create(new TasksController.CreateTaskRequest { Title = "New" });

        var created = Assert.IsType<ObjectResult>(result);
        created.StatusCode.Should().Be(201);
        var response = Assert.IsType<ApiResponse<TasksController.TaskResponse>>(created.Value);
        response.Data!.Id.Should().Be(99);
        repo.Verify(r => r.AddAsync(It.Is<TaskItem>(t => t.Title == "New" && t.UserId == 3)), Times.Once);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        var repo = new Mock<ITaskRepository>();
        repo.Setup(r => r.GetByIdForUserAsync(1, 3)).ReturnsAsync((TaskItem?)null);

        var controller = CreateControllerWithUser(repo.Object, 3);
        var result = await controller.Update(1, new TasksController.UpdateTaskRequest { Title = "X" });
        var obj = Assert.IsType<ObjectResult>(result);
        obj.StatusCode.Should().Be(404);
    }
}


