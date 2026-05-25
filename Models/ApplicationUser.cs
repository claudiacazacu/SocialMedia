using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace instagram.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "numele e obligatoriu")]
        public string Nume {get; set;}=null!;
        [Required(ErrorMessage = "prenumele e obligatoriu")]
        public string Prenume {get; set;}=null!;
        public DateTime DataNasterii {get; set;}

        /*public int Id { get; set;} //PK-ul
        public string Nume {get; set;} = null!;
        public string Prenume {get; set;} = null!;
        [Required(ErrorMessage = "Email obligatoriu")]
        public string Email {get; set;} = null!;
        [Required(ErrorMessage = "Username obligatoriu")]
        public string Username{get; set;} = null!;
        [Required(ErrorMessage = "Parola obligatorie")]
        public string Parola {get; set;} = null!;
        public DateTime DataNasterii {get; set;}
        public int RoleId {get; set;}
        public Role Role {get; set;} = null!;
        */
        public ICollection<Post> Posts { get; set; }=new List<Post>();
        public ICollection<Comment> Comments { get; set; }=new List<Comment>();
        public ICollection<Like> Likes {get; set;}=new List<Like>();    
    }
}