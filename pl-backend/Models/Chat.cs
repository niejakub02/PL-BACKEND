using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pl_backend.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public int InvitingUserId { get; set; }
        [Required]
        public User? InvitingUser { get; set; }

        public int InvitedUserId { get; set; }
        [Required]
        public User? InvitedUser { get; set; }

        public ICollection<Message> Messages { get; set; } = null!;
    }
}
