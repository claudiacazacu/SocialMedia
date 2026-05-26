using instagram.Data;
using instagram.Models;
using Microsoft.EntityFrameworkCore;

namespace instagram.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(ApplicationUser user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task DeleteAsync(ApplicationUser user)
    {
        _context.Users.Remove(user);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
