using instagram.DTOs;
using instagram.Mappings;
using instagram.Repositories;

namespace instagram.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _repository;
    private readonly ILogger<CommentService> _logger;

    public CommentService(ICommentRepository repository, ILogger<CommentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<CommentReadDto>> GetCommentsForPostAsync(int postId)
    {
        _logger.LogInformation("Fetching comments for post {PostId}", postId);
        var comments = await _repository.GetByPostIdAsync(postId);
        return comments.ToDtoList();
    }

    public async Task<CommentReadDto?> GetCommentByIdAsync(int id)
    {
        _logger.LogInformation("Fetching comment {CommentId}", id);
        var comment = await _repository.GetByIdAsync(id);
        return comment?.ToDto();
    }

    public async Task<CommentReadDto?> CreateCommentAsync(CreateCommentDto dto, string userId)
    {
        _logger.LogInformation("User {UserId} creating comment on post {PostId}", userId, dto.PostId);
        if (!await _repository.PostExistsAsync(dto.PostId))
        {
            _logger.LogWarning("Post {PostId} not found when creating comment", dto.PostId);
            return null;
        }
        var comment = dto.ToEntity(userId);
        await _repository.AddAsync(comment);
        await _repository.SaveChangesAsync();
        var createdComment = await _repository.GetByIdAsync(comment.Id);
        _logger.LogInformation("Comment {CommentId} created on post {PostId}", comment.Id, dto.PostId);
        return createdComment?.ToDto();
    }

    public async Task<CommentReadDto?> UpdateCommentAsync(int id, UpdateCommentDto dto, string currentUserId)
    {
        _logger.LogInformation("User {UserId} updating comment {CommentId}", currentUserId, id);
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null)
        {
            _logger.LogWarning("Comment {CommentId} not found for update", id);
            return null;
        }
        if (comment.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} unauthorized to update comment {CommentId}", currentUserId, id);
            throw new UnauthorizedAccessException("n ai permisiune");
        }

        comment.Content = dto.Content;
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Comment {CommentId} updated successfully", id);
        return comment.ToDto();
    }

    public async Task<bool> DeleteCommentAsync(int id, string currentUserId)
    {
        _logger.LogInformation("User {UserId} deleting comment {CommentId}", currentUserId, id);
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null)
        {
            _logger.LogWarning("Comment {CommentId} not found for deletion", id);
            return false;
        }
        if (comment.UserId != currentUserId)
        {
            _logger.LogWarning("User {UserId} unauthorized to delete comment {CommentId}", currentUserId, id);
            throw new UnauthorizedAccessException("n ai permisiune");
        }
        await _repository.DeleteAsync(comment);
        await _repository.SaveChangesAsync();
        _logger.LogInformation("Comment {CommentId} deleted successfully", id);
        return true;
    }
}
