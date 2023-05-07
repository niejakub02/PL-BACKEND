using System.ComponentModel.DataAnnotations;

namespace pl_backend.Models
{
    public class UserLanguage
    {
        [Key]
        public int Id { get; set; } 
        public int UserId { get; set; }
        [Required]
        public User? User { get; set; } 
        public int LanguageId { get; set; }
        [Required]
        public Language? Language { get; set; }
    }
}
