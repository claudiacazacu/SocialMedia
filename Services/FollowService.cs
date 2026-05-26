using instagram.Models;
using instagram.Repositories;

namespace instagram.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _repository;

    public FollowService(IFollowRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> FollowUserAsync(string followerId, string followingId)
    {
        if (followerId == followingId)
        {
            throw new InvalidOperationException("Nu poți să te urmărești singur.");
        }

        if (!await _repository.UserExistsAsync(followingId))
        {
            return false;
        }

        if (await _repository.IsFollowingAsync(followerId, followingId))
        {
            throw new InvalidOperationException("Urmărești deja acest utilizator.");
        }

        var follow = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId,
            Date = DateTime.UtcNow
        };

        await _repository.AddAsync(follow);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnfollowUserAsync(string followerId, string followingId)
    {
        var follow = await _repository.GetByFollowerAndFollowingAsync(followerId, followingId);
        if (follow == null)
        {
            return false;
        }

        await _repository.DeleteAsync(follow);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetFollowersAsync(string userId)
    {
        return await _repository.GetFollowersAsync(userId);
    }

    public async Task<IEnumerable<string>> GetFollowingAsync(string userId)
    {
        return await _repository.GetFollowingAsync(userId);
    }
}
