using System;
using System.Collections.Generic;
namespace instagram.Models
{
    public class Post
    {
        public int Id {get; set;}
        public string UserId {get; set;}=null!; //FK-ul
        public ApplicationUser User {get; set;} = null!;
        public string Descriere {get; set;} = null!;
        public DateTime DataPublicarii {get; set;}
        public string ImageUrl {get; set;} = null!;
        public ICollection<Tag> Tags {get; set;} = new List<Tag>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes {get; set;} = new List<Like>();
        
        //public User User {get; set;} = null!; 
    }
}