using instagram.DTOs;
namespace instagram.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentReadDto>> GetCommentsForPostAsync(int postId);
    Task<CommentReadDto?> CreateCommentAsync(CreateCommentDto dto, string userId);
    Task<bool> DeleteCommentAsync(int id, string currentUserId);
}
