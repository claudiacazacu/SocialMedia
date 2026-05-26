using instagram.DTOs;
namespace instagram.Services;

public interface IRepostService
{
    Task<IEnumerable<RepostReadDto>> GetRepostsForPostAsync(int postId);
    Task<RepostReadDto?> CreateRepostAsync(CreateRepostDto dto, string userId);
    Task<bool> DeleteRepostAsync(int id, string currentUserId);
}
