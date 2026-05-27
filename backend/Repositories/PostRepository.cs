using instagram.Data;
using instagram.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace instagram.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _context.Posts
            .Include(p => p.User)
            .OrderByDescending(p => p.DataPublicarii)
            .ToListAsync();
    }
    public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId)
    {
        return await _context.Posts
            .Where(p => p.UserId == userId)
            .Include(p => p.User)
            .OrderByDescending(p => p.DataPublicarii)
            .ToListAsync();
    }
    public async Task<Post?> GetByIdAsync(int id)
    {
        return await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task AddAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
    }
    public async Task DeleteAsync(Post post)
    {
        _context.Posts.Remove(post);
        await Task.CompletedTask; 
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}