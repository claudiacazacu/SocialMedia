using System;
namespace instagram.Models
{
    public class Post
    {
        public int Id {get; set;} 
        public int UserId {get; set;} //FK-ul
        public string Descriere {get; set;} = null!;
        public ICollection<Tag> Tags {get; set;} = new List<Tag>();
        public DateTime DataPublicarii {get; set;}
        public string ImageUrl {get; set;} = null!;
        public int NrLikeuri {get; set;}
        public int NrComentarii {get; set;}
        public int NrDistribuiri {get; set;}
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public User User { get; set; } = null!;
    }
}