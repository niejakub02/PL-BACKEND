using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pl_backend.Models
{
    public class UserLanguage
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        public User? User { get; set; }
        [ForeignKey("Language")]
        public int LanguageId { get; set; }
        [Required]
        public Language? Language { get; set; }
    }
}
