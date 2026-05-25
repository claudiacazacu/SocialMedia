using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs;

public record CreateCommentDto(
    [Required] string Content, 
    [Required] int PostId
);

public record CommentReadDto(
    int Id, 
    string Content, 
    DateTime Date, 
    string Username
);