using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pl_backend.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Description { get; set; }

        [ForeignKey("FromId")]
        public int FromId { get; set; }
        [Required]
        public User? From { get; set; }

        [ForeignKey("ToId")]
        public int ToId { get; set; }
        [Required]
        public User? To { get; set; }
    }
}
