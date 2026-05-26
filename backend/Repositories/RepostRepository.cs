using instagram.Data;
using instagram.Models;
using Microsoft.EntityFrameworkCore;

namespace instagram.Repositories;

public class RepostRepository : IRepostRepository
{
    private readonly AppDbContext _context;

    public RepostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Repost>> GetByPostIdAsync(int postId)
    {
        return await _context.Reposts
            .Include(r => r.User)
            .Where(r => r.PostId == postId)
            .ToListAsync();
    }

    public async Task<Repost?> GetByIdAsync(int id)
    {
        return await _context.Reposts.FindAsync(id);
    }

    public async Task AddAsync(Repost repost)
    {
        await _context.Reposts.AddAsync(repost);
    }

    public async Task DeleteAsync(Repost repost)
    {
        _context.Reposts.Remove(repost);
        await Task.CompletedTask;
    }

    public async Task<bool> PostExistsAsync(int postId)
    {
        return await _context.Posts.AnyAsync(p => p.Id == postId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
