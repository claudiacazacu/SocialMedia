using instagram.DTOs;
namespace instagram.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentReadDto>> GetCommentsForPostAsync(int postId);
    Task<CommentReadDto?> GetCommentByIdAsync(int id);
    Task<CommentReadDto?> CreateCommentAsync(CreateCommentDto dto, string userId);
    Task<CommentReadDto?> UpdateCommentAsync(int id, UpdateCommentDto dto, string currentUserId);
    Task<bool> DeleteCommentAsync(int id, string currentUserId);
}
