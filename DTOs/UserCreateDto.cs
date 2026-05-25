using System.ComponentModel.DataAnnotations;

namespace instagram.DTOs
{
    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        public string Nume {get;set;}=null!;
        public string Prenume {get;set;}=null!;
    }
}