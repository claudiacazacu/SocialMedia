using instagram.DTOs;
namespace instagram.Services;

public interface IFollowService
{
    Task<bool> FollowUserAsync(string followerId, string followingId);
    Task<bool> UnfollowUserAsync(string followerId, string followingId);
    Task<IEnumerable<string>> GetFollowersAsync(string userId);
    Task<IEnumerable<string>> GetFollowingAsync(string userId);
}
