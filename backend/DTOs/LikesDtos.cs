using System.ComponentModel.DataAnnotations;
namespace instagram.DTOs;
public record CreateLikeDto(
    [Required] int PostId
);
public record LikeReadDto(
    int Id,
    int PostId,
    string Username
);