using instagram.DTOs;
using instagram.Models;

namespace instagram.Mappings;

public static class CommentMappings
{
    public static CommentReadDto ToDto(this Comment comment)
    {
        return new CommentReadDto(
            comment.Id,
            comment.Content,
            comment.Date,
            comment.User?.UserName ?? "Necunoscut"
        );
    }

    public static List<CommentReadDto> ToDtoList(this IEnumerable<Comment> comments)
    {
        return comments.Select(c => c.ToDto()).ToList();
    }

    public static Comment ToEntity(this CreateCommentDto dto, string userId)
    {
        return new Comment
        {
            Content = dto.Content,
            PostId = dto.PostId,
            Date = DateTime.UtcNow,
            UserId = userId
        };
    }
}