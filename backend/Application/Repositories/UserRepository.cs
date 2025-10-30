using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> AnyByEmailAsync(string email);
    Task<User> AddAsync(User user);
}

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext db) : base(db) { }

    public Task<User?> GetByEmailAsync(string email)
    {
        return Set.FirstOrDefaultAsync(u => u.Email == email)!;
    }

    public Task<bool> AnyByEmailAsync(string email)
    {
        return Set.AnyAsync(u => u.Email == email);
    }

    // AddAsync inherited from BaseRepository
}


