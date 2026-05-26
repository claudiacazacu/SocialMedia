using System.ComponentModel.DataAnnotations;
namespace instagram.Models
{
    public class Repost
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int PostId { get; set; } 
        public Post Post { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}