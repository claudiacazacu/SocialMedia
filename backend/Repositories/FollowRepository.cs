using instagram.Data;
using instagram.Models;
using Microsoft.EntityFrameworkCore;

namespace instagram.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly AppDbContext _context;

    public FollowRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followingId)
    {
        return await _context.Follows.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    public async Task AddAsync(Follow follow)
    {
        await _context.Follows.AddAsync(follow);
    }

    public async Task<Follow?> GetByFollowerAndFollowingAsync(string followerId, string followingId)
    {
        return await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    public async Task DeleteAsync(Follow follow)
    {
        _context.Follows.Remove(follow);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<string>> GetFollowersAsync(string userId)
    {
        return await _context.Follows
            .Include(f => f.Follower)
            .Where(f => f.FollowingId == userId)
            .Select(f => f.Follower.UserName!)  
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetFollowingAsync(string userId)
    {
        return await _context.Follows
            .Include(f => f.Following)
            .Where(f => f.FollowerId == userId)
            .Select(f => f.Following.UserName!)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
