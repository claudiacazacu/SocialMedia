using System;
namespace instagram.Models
{
    public class User
    {
        public int Id { get; set;} //PK-ul
        public string Nume {get; set;} = null!;
        public string Prenume {get; set;} = null!;
        public string Email {get; set;} = null!;
        public string Username{get; set;} = null!;
        public string Parola {get; set;} = null!;
        public string DataNasterii {get; set;} = null!;
    }
}