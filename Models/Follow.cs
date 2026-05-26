using System;
using System.ComponentModel.DataAnnotations;
namespace instagram.Models
{
    public class Follow
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Follower nu poate fi gol")]
        public string FollowerId { get; set; } = null!; 
        public ApplicationUser Follower { get; set; } = null!;
        [Required(ErrorMessage = "Following nu poate fi gol")]
        public string FollowingId { get; set; } = null!; 
        public ApplicationUser Following { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}