using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using System.Linq.Expressions;

namespace TaskManager.Repositories;

public abstract class BaseRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly DbSet<TEntity> Set;

    protected BaseRepository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
        Set = DbContext.Set<TEntity>();
    }

    protected Task<int> SaveAsync()
    {
        return DbContext.SaveChangesAsync();
    }

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Set.FirstOrDefaultAsync(predicate);
    }

    public Task<TEntity?> GetByIdAsync(int id)
    {
        return Set.FindAsync(id).AsTask();
    }

    public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        return (predicate == null ? Set : Set.Where(predicate)).ToListAsync();
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        Set.Add(entity);
        await SaveAsync();
        return entity;
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        Set.Update(entity);
        return SaveAsync();
    }

    public virtual Task RemoveAsync(TEntity entity)
    {
        Set.Remove(entity);
        return SaveAsync();
    }

    public async Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var items = await Set.Where(predicate).ToListAsync();
        if (items.Count == 0) return 0;
        Set.RemoveRange(items);
        return await SaveAsync();
    }

    public async Task<int> UpdateWhereAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> updateAction)
    {
        var items = await Set.Where(predicate).ToListAsync();
        if (items.Count == 0) return 0;
        foreach (var entity in items)
        {
            updateAction(entity);
        }
        return await SaveAsync();
    }
}


