namespace instagram.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string UserId {get; set;}=null!;
        public ApplicationUser User {get; set;} = null!;
        public int PostId {get; set;}
        //public User User {get; set;} = null!;
        public Post Post {get; set;} = null!;
    }
}