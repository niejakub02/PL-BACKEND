using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace pl_backend.Models
{
    public class User
    {
        public User()
        {
            Chats = new HashSet<Chat>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PasswordHash { get; set; }
        [Required]
        public string? PasswordSalt { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string Description = string.Empty;
        public int Age { get; set; }
        public string Avatar { get; set; } = null!;

        public ICollection<Marker> Markers { get; set; } = null!;
        public ICollection<Chat> Chats { get; set; } = null!;
    }
}