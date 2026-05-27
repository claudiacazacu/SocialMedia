using instagram.DTOs;
using instagram.Mappings;
using instagram.Repositories;

namespace instagram.Services;

public class LikeService : ILikeService
{
    private readonly ILikeRepository _repository;
    private readonly ILogger<LikeService> _logger;

    public LikeService(ILikeRepository repository, ILogger<LikeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<LikeReadDto>> GetLikesForPostAsync(int postId)
    {
        _logger.LogInformation("Fetching likes for post {PostId}", postId);
        var likes = await _repository.GetByPostIdAsync(postId);
        return likes.ToDtoList();
    }

    public async Task<LikeReadDto?> CreateLikeAsync(CreateLikeDto dto, string userId)
    {
        _logger.LogInformation("User {UserId} liking post {PostId}", userId, dto.PostId);
        if (!await _repository.PostExistsAsync(dto.PostId))
        {
            _logger.LogWarning("Post {PostId} not found when creating like", dto.PostId);
            return null;
        }

        var alreadyLiked = await _repository.GetByUserAndPostAsync(userId, dto.PostId);
        if (alreadyLiked != null)
        {
            _logger.LogWarning("User {UserId} already liked post {PostId}", userId, dto.PostId);
            throw new InvalidOperationException("Ai deja un like pentru această postare.");
        }

        var like = dto.ToEntity(userId);
        await _repository.AddAsync(like);
        await _repository.SaveChangesAsync();

        var createdLike = await _repository.GetByIdAsync(like.Id);
        _logger.LogInformation("Like {LikeId} created by user {UserId} on post {PostId}", like.Id, userId, dto.PostId);
        return createdLike?.ToDto();
    }

    public async Task<bool> DeleteLikeAsync(int id, string currentUserId)
    {
        _logger.LogInformation("User {UserId} removing like {LikeId}", currentUserId, id);
        var like = await _repository.GetByIdAsync(id);
        if (like == null)
        {
            _logger.LogWarning("Like {LikeId} not found for deletion", id);
            return false;
        }

        if (like.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} unauthorized to delete like {LikeId}", currentUserId, id);
            throw new UnauthorizedAccessException("Nu ai permisiunea să ștergi acest like.");
        }

        await _repository.DeleteAsync(like);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Like {LikeId} deleted successfully", id);
        return true;
    }
}
