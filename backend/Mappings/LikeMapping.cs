using instagram.DTOs;
using instagram.Models;

namespace instagram.Mappings;

public static class LikeMappings
{
    public static LikeReadDto ToDto(this Like like)
    {
        return new LikeReadDto(
            like.Id,
            like.PostId,
            like.User?.UserName ?? "Necunoscut"
        );
    }

    public static List<LikeReadDto> ToDtoList(this IEnumerable<Like> likes)
    {
        return likes.Select(l => l.ToDto()).ToList();
    }

    public static Like ToEntity(this CreateLikeDto dto, string userId)
    {
        return new Like
        {
            PostId = dto.PostId,
            UserId = userId
        };
    }
}