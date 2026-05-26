using instagram.Models;
namespace instagram.Repositories;

public interface ILikeRepository
{
    Task<IEnumerable<Like>> GetByPostIdAsync(int postId);
    Task<Like?> GetByIdAsync(int id);
    Task<Like?> GetByUserAndPostAsync(string userId, int postId);
    Task AddAsync(Like like);
    Task DeleteAsync(Like like);
    Task<bool> PostExistsAsync(int postId);
    Task SaveChangesAsync();
}
