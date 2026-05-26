using instagram.DTOs;
using instagram.Mappings;
using instagram.Repositories;

namespace instagram.Services;

public class LikeService : ILikeService
{
    private readonly ILikeRepository _repository;

    public LikeService(ILikeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LikeReadDto>> GetLikesForPostAsync(int postId)
    {
        var likes = await _repository.GetByPostIdAsync(postId);
        return likes.ToDtoList();
    }

    public async Task<LikeReadDto?> CreateLikeAsync(CreateLikeDto dto, string userId)
    {
        if (!await _repository.PostExistsAsync(dto.PostId))
        {
            return null;
        }

        var alreadyLiked = await _repository.GetByUserAndPostAsync(userId, dto.PostId);
        if (alreadyLiked != null)
        {
            throw new InvalidOperationException("Ai deja un like pentru această postare.");
        }

        var like = dto.ToEntity(userId);
        await _repository.AddAsync(like);
        await _repository.SaveChangesAsync();

        var createdLike = await _repository.GetByIdAsync(like.Id);
        return createdLike?.ToDto();
    }

    public async Task<bool> DeleteLikeAsync(int id, string currentUserId)
    {
        var like = await _repository.GetByIdAsync(id);
        if (like == null)
        {
            return false;
        }

        if (like.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Nu ai permisiunea să ștergi acest like.");
        }

        await _repository.DeleteAsync(like);
        await _repository.SaveChangesAsync();
        return true;
    }
}
