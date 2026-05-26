using instagram.Models;
namespace instagram.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task AddAsync(ApplicationUser user);
    Task DeleteAsync(ApplicationUser user);
    Task<bool> ExistsAsync(string id);
    Task SaveChangesAsync();
}
