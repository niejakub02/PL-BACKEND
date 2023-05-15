using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace pl_backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string Description = string.Empty;
        public int Age { get; set; }
        public string Avatar { get; set; } = null!;
        public int MarkerId { get; set; }
        public Marker Marker { get; set; } = null!;

        public ICollection<Chat> Chats { get; set; } = null!;
    }
}