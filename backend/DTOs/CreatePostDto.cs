using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs;

public record CreatePostDto(
    [Required] string Descriere,
    [Required] string ImageUrl
);