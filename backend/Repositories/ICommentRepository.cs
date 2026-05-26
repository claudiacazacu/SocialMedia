using instagram.Models;
namespace instagram.Repositories;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetByPostIdAsync(int postId);
    Task<Comment?> GetByIdAsync(int id);
    Task AddAsync(Comment comment);
    Task DeleteAsync(Comment comment);
    Task<bool> PostExistsAsync(int postId);
    Task SaveChangesAsync();
}
