using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace pl_backend.Models
{
    [Index(nameof(Email), IsUnique = true)]
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
        public string LastName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public int? MarkerId { get; set; }
        public Marker Marker { get; set; } = null!;
        public ICollection<Contact> Contacts { get; set; } = null!;
        public ICollection<UserLanguage> Languages { get; set; } = null!;
        public ICollection<Chat> Chats { get; set; } = null!;
    }
}