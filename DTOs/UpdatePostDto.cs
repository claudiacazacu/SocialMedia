using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs;

public record UpdatePostDto(
    [Required] string Descriere,
    [Required] string ImageUrl
);
