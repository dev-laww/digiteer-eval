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

    // Request DTOs for create and update bodies
    public record class CreateTaskRequest
    {
        [Required]
        public string Title { get; init; } = string.Empty;
    }

    public record class UpdateTaskRequest
    {
        public string? Title { get; init; }
        public bool? IsDone { get; init; }
    }

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
            .ToListAsync();
        return Ok(ApiResponse<List<TaskItem>>.Ok(tasks));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var userId = GetUserId();
        var task = new TaskItem { Title = request.Title, IsDone = false, UserId = userId };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return StatusCode(201, ApiResponse<TaskItem>.Ok(task, "Created", 201));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest updated)
    {
        var userId = GetUserId();
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null) return StatusCode(404, ApiResponse<TaskItem>.Fail("Task not found", 404));

        if (updated.Title != null) task.Title = updated.Title;
        if (updated.IsDone.HasValue) task.IsDone = updated.IsDone.Value;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<TaskItem>.Ok(task, "Updated"));
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