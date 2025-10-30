using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllForUserAsync(int userId);
    Task<TaskItem?> GetByIdForUserAsync(int id, int userId);
    Task<TaskItem> AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task RemoveAsync(TaskItem task);
    Task<(List<TaskItem> Items, int TotalCount)> GetPagedForUserAsync(
        int userId,
        int page,
        int pageSize,
        string? search,
        bool? isDone);
}

public class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext db) : base(db) { }

    public Task<List<TaskItem>> GetAllForUserAsync(int userId)
    {
        return Set.Where(t => t.UserId == userId).ToListAsync();
    }

    public Task<TaskItem?> GetByIdForUserAsync(int id, int userId)
    {
        return Set.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    // Add/Update/Remove inherited from BaseRepository

    public async Task<(List<TaskItem> Items, int TotalCount)> GetPagedForUserAsync(
        int userId,
        int page,
        int pageSize,
        string? search,
        bool? isDone)
    {
        var query = Set.AsQueryable().Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Title.ToLower().Contains(search.ToLower()));
        }
        if (isDone.HasValue)
        {
            query = query.Where(t => t.IsDone == isDone.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}


