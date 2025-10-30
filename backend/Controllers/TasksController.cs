using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManager.Dto;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.API;

[Authorize]
[Route("tasks")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public record CreateTaskRequest
    {
        [Required] public string Title { get; init; } = string.Empty;
    }

    public record UpdateTaskRequest
    {
        public string? Title { get; init; }
        public bool? IsDone { get; init; }
    }

    public record TaskResponse(int Id, string Title, bool IsDone);

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();
        var tasks = await _context.Tasks
            .Where(t => t.UserId == userId)
            .Select(t => new TaskResponse(t.Id, t.Title, t.IsDone))
            .ToListAsync();
        return Ok(ApiResponse<List<TaskResponse>>.Ok(tasks));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var userId = GetUserId();
        var task = new TaskItem { Title = request.Title, IsDone = false, UserId = userId };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        var response = new TaskResponse(task.Id, task.Title, task.IsDone);
        return StatusCode(201, ApiResponse<TaskResponse>.Ok(response, "Created", 201));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest updated)
    {
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return StatusCode(404, ApiResponse<object>.Fail("Task not found", 404));

        if (updated.Title != null) task.Title = updated.Title;
        if (updated.IsDone.HasValue) task.IsDone = updated.IsDone.Value;
        await _context.SaveChangesAsync();

        var response = new TaskResponse(task.Id, task.Title, task.IsDone);
        return Ok(ApiResponse<TaskResponse>.Ok(response, "Updated"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return StatusCode(404, ApiResponse<object>.Fail("Task not found", 404));

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return StatusCode(204, ApiResponse<object>.Ok(null, "Deleted", 204));
    }

    private int GetUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst(ClaimTypes.Name)?.Value
                      ?? User.FindFirst("sub")?.Value;
        if (idClaim == null) throw new UnauthorizedAccessException("Missing user id claim");
        return int.Parse(idClaim);
    }
}