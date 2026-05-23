using System;
using System.ComponentModel.DataAnnotations;
namespace instagram.Models
{
    public class Comment
    {
        public int Id {get; set;}
        [Required(ErrorMessage = "Comentariul nu poate sa fie gol")]
        public string Content {get; set;}
        public DateTime Date {get; set;}
        [Required(ErrorMessage = "Utilizatorul nu poate sa fie gol")]
        public int UserId {get; set;} //FK-ul
        public int PostId {get; set;} //FK-ul
        public Post Post { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}