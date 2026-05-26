using instagram.Models;
namespace instagram.Repositories;
public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAllAsync();
    Task<Post?> GetByIdAsync(int id);
    Task AddAsync(Post post);
    Task DeleteAsync(Post post);
    Task SaveChangesAsync();
}