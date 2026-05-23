using System;
namespace instagram.Models
{
    public class Post
    {
        public int Id {get; set;} 
        public int UserId {get; set;} //FK-ul
        public string Descriere {get; set;} = null!;
        public string Hashtag {get; set;} = null!;
        public DateTime DataPublicarii {get; set;}
        public string ImageUrl {get; set;} = null!;
        public string Imagine{get;set;} = null!;
        public int NrLikeuri {get; set;}
        public int NrComentarii {get; set;}
        public int NrDistribuiri {get; set;}
    }
}