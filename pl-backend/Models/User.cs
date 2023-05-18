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
        public byte[] PasswordHash { get; set; } = null!;
        [Required]
        public byte[] PasswordSalt { get; set; } = null!;
        public string FirstName { get; set; } = string.Empty;
        public string Description = string.Empty;
        public int Age { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public int? MarkerId { get; set; }
        public Marker Marker { get; set; } = null!;

        public ICollection<Chat> Chats { get; set; } = null!;
    }
}