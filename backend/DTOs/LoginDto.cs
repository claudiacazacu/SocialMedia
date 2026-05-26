using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs;

public record LoginDto(
    [Required] string Username,
    [Required] string Password
);