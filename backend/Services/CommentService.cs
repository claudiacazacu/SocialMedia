using instagram.DTOs;
using instagram.Mappings;
using instagram.Repositories;

namespace instagram.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _repository;

    public CommentService(ICommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CommentReadDto>> GetCommentsForPostAsync(int postId)
    {
        var comments = await _repository.GetByPostIdAsync(postId);
        return comments.ToDtoList();
    }

    public async Task<CommentReadDto?> GetCommentByIdAsync(int id)
    {
        var comment = await _repository.GetByIdAsync(id);
        return comment?.ToDto();
    }

    public async Task<CommentReadDto?> CreateCommentAsync(CreateCommentDto dto, string userId)
    {
        if (!await _repository.PostExistsAsync(dto.PostId))
        {
            return null;
        }
        var comment = dto.ToEntity(userId);
        await _repository.AddAsync(comment);
        await _repository.SaveChangesAsync();
        var createdComment = await _repository.GetByIdAsync(comment.Id);
        return createdComment?.ToDto();
    }

    public async Task<CommentReadDto?> UpdateCommentAsync(int id, UpdateCommentDto dto, string currentUserId)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null) return null;
        if (comment.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("n ai permisiune");
        }

        comment.Content = dto.Content;
        await _repository.SaveChangesAsync();
        return comment.ToDto();
    }

    public async Task<bool> DeleteCommentAsync(int id, string currentUserId)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null)
        {
            return false;
        }
        if (comment.UserId != currentUserId)
        {
            throw new UnauthorizedAccessException("n ai permisiune");
        }
        await _repository.DeleteAsync(comment);
        await _repository.SaveChangesAsync();
        return true;
    }
}
