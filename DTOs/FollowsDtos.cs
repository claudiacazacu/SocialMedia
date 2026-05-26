using System.ComponentModel.DataAnnotations;
namespace instagram.DTOs;
public record CreateFollowDto(
    [Required] string FollowingId
);
public record FollowReadDto(
    int Id,
    string FollowerUsername,
    string FollowingUsername,
    DateTime Date
);