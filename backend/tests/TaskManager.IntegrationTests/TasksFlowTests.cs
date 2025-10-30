using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TaskManager.IntegrationTests;

public class TasksFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TasksFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateListUpdateDeleteTaskSucceeds()
    {
        var client = _factory.CreateClient();

        var registerPayload = new { email = $"bob{Guid.NewGuid():N}@example.com", password = "Passw0rd!" };
        var registerResp = await client.PostAsJsonAsync("/auth/register", registerPayload);
        registerResp.EnsureSuccessStatusCode();
        var registerData = await registerResp.Content.ReadFromJsonAsync<AuthResponseEnvelope>();
        var token = registerData!.data.token;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createResp = await client.PostAsJsonAsync("/tasks", new { title = "Task 1" });
        createResp.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var created = await createResp.Content.ReadFromJsonAsync<TaskEnvelope>();
        var id = created!.data.id;

        // List
        var listResp = await client.GetAsync("/tasks");
        listResp.EnsureSuccessStatusCode();
        var list = await listResp.Content.ReadFromJsonAsync<TaskListEnvelope>();
        list!.data.Should().ContainSingle(t => t.id == id && t.title == "Task 1" && t.isDone == false);

        var updateResp = await client.PutAsJsonAsync($"/tasks/{id}", new { isDone = true });
        updateResp.EnsureSuccessStatusCode();

        var deleteResp = await client.DeleteAsync($"/tasks/{id}");
        deleteResp.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    private record AuthResponseEnvelope(bool success, int code, string message, TokenDto data);
    private record TokenDto(string token);
    private record TaskEnvelope(bool success, int code, string message, TaskDto data);
    private record TaskDto(int id, string title, bool isDone);
    private record TaskListEnvelope(bool success, int code, string message, List<TaskDto> data);
}


