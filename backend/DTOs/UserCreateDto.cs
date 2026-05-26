using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs;
public record UserCreateDto
(
    [Required] string Username,
    [Required, EmailAddress] string Email,
    [Required] string Nume,
    [Required] string Prenume
);
