using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs;

public record RegisterDto(
    [Required] string Nume,
    [Required] string Prenume,
    [Required] string Username,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password
);