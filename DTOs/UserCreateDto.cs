using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs
{
    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; } = null!;
        
        [Required]
        public string Email { get; set; } = null!;
    }
}