using instagram.DTOs;
namespace instagram.Services;

public interface ILikeService
{
    Task<IEnumerable<LikeReadDto>> GetLikesForPostAsync(int postId);
    Task<LikeReadDto?> CreateLikeAsync(CreateLikeDto dto, string userId);
    Task<bool> DeleteLikeAsync(int id, string currentUserId);
}
