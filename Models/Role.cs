namespace instagram.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Relație One-to-Many: Un rol (ex: User) poate fi deținut de mai mulți utilizatori
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}