using instagram.DTOs;
using instagram.Mappings;
using instagram.Models;
using instagram.Repositories;

namespace instagram.Services;

public class RepostService : IRepostService
{
    private readonly IRepostRepository _repository;

    public RepostService(IRepostRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RepostReadDto>> GetRepostsForPostAsync(int postId)
    {
        var reposts = await _repository.GetByPostIdAsync(postId);
        return reposts.ToDtoList();
    }

    public async Task<RepostReadDto?> CreateRepostAsync(CreateRepostDto dto, string userId)
    {
        if (!await _repository.PostExistsAsync(dto.PostId))
        {
            return null;
        }

        var repost = dto.ToEntity(userId);
        await _repository.AddAsync(repost);
        await _repository.SaveChangesAsync();

        var createdRepost = await _repository.GetByIdAsync(repost.Id);
        return createdRepost?.ToDto();
    }

    public async Task<bool> DeleteRepostAsync(int id, string currentUserId)
    {
        var repost = await _repository.GetByIdAsync(id);
        if (repost == null)
        {
            return false;
        }

        if (repost.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("Nu ai permisiunea să ștergi acest repost.");
        }

        await _repository.DeleteAsync(repost);
        await _repository.SaveChangesAsync();
        return true;
    }
}
