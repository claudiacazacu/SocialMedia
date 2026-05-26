using System.ComponentModel.DataAnnotations;
namespace instagram.DTOs;

public record CreateRepostDto(
    [Required] int PostId
);
public record RepostReadDto(
    int Id,
    int PostId,
    string Username,
    DateTime Date
);