using System.ComponentModel.DataAnnotations;

namespace pl_backend.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Content { get; set; }
        public DateTime At { get; set; }

        public int UserId { get; set; }
        [Required]
        public User? User { get; set; }

        public int ChatId { get; set; }
        [Required]
        public Chat? Chat { get; set; }
    }
}
