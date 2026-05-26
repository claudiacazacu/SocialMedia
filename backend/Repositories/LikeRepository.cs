using instagram.Data;
using instagram.Models;
using Microsoft.EntityFrameworkCore;

namespace instagram.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly AppDbContext _context;

    public LikeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Like>> GetByPostIdAsync(int postId)
    {
        return await _context.Likes
            .Include(l => l.User)
            .Where(l => l.PostId == postId)
            .ToListAsync();
    }

    public async Task<Like?> GetByIdAsync(int id)
    {
        return await _context.Likes
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Like?> GetByUserAndPostAsync(string userId, int postId)
    {
        return await _context.Likes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
    }

    public async Task AddAsync(Like like)
    {
        await _context.Likes.AddAsync(like);
    }

    public async Task DeleteAsync(Like like)
    {
        _context.Likes.Remove(like);
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
