using instagram.Models;
namespace instagram.Repositories;

public interface IRepostRepository
{
    Task<IEnumerable<Repost>> GetByPostIdAsync(int postId);
    Task<Repost?> GetByIdAsync(int id);
    Task AddAsync(Repost repost);
    Task DeleteAsync(Repost repost);
    Task<bool> PostExistsAsync(int postId);
    Task SaveChangesAsync();
}
