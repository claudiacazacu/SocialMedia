using instagram.DTOs;
using instagram.Models;
namespace instagram.Mappings;

public static class RepostMappings
{
    public static Repost ToEntity(this CreateRepostDto dto, string userId)
    {
        return new Repost
        {
            UserId = userId,
            PostId = dto.PostId,
            Date = DateTime.UtcNow
        };
    }
    public static RepostReadDto ToDto(this Repost repost)
    {
        return new RepostReadDto(
            repost.Id,
            repost.PostId,
            repost.User?.UserName ?? "Unknown",
            repost.Date
        );
    }
    public static List<RepostReadDto> ToDtoList(this IEnumerable<Repost> reposts)
    {
        return reposts.Select(r => r.ToDto()).ToList();
    }
}