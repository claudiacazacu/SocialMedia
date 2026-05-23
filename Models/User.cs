using System.ComponentModel.DataAnnotations;
namespace instagram.Models
{
    public class User
    {
        public int Id { get; set;} //PK-ul
        public string Nume {get; set;} = null!;
        public string Prenume {get; set;} = null!;
        [Required(ErrorMessage = "Email obligatoriu")]
        public string Email {get; set;} = null!;
        [Required(ErrorMessage = "Username obligatoriu")]
        public string Username{get; set;} = null!;
        [Required(ErrorMessage = "Parola obligatorie")]
        public string Parola {get; set;} = null!;
        public string DataNasterii {get; set;} = null!;
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public int RoleId {get; set;}
        public Role Role {get; set;} = null!;
    }
}