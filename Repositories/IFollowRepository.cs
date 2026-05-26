using instagram.Models;
namespace instagram.Repositories;

public interface IFollowRepository
{
    Task<bool> UserExistsAsync(string userId);
    Task<bool> IsFollowingAsync(string followerId, string followingId);
    Task AddAsync(Follow follow);
    Task<Follow?> GetByFollowerAndFollowingAsync(string followerId, string followingId);
    Task DeleteAsync(Follow follow);
    Task<IEnumerable<string>> GetFollowersAsync(string userId);
    Task<IEnumerable<string>> GetFollowingAsync(string userId);
    Task SaveChangesAsync();
}
