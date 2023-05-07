using System.ComponentModel.DataAnnotations;

namespace pl_backend.Models
{
    public class Language
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? LanguageName { get; set; }
    }
}
