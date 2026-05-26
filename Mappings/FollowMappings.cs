using instagram.DTOs;
using instagram.Models;
namespace instagram.Mappings;

public static class FollowMappings
{
    public static Follow ToEntity(this CreateFollowDto dto, string followerId)
    {
        return new Follow
        {
            FollowerId = followerId,
            FollowingId = dto.FollowingId,
            Date = DateTime.UtcNow
        };
    }
    public static FollowReadDto ToDto(this Follow follow)
    {
        return new FollowReadDto(
            follow.Id,
            follow.Follower?.UserName ?? "Unknown",
            follow.Following?.UserName ?? "Unknown",
            follow.Date
        );
    }
    public static List<FollowReadDto> ToDtoList(this IEnumerable<Follow> follows)
    {
        return follows.Select(f => f.ToDto()).ToList();
    }
}