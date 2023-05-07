using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pl_backend.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public bool Status { get; set; }
        public DateTime Met { get; set; }

        [ForeignKey("InvitingUser")]
        public int InvitingUserId { get; set; }
        [Required]
        public User? InvitingUser { get; set; }

        [ForeignKey("InvitedUser")]
        public int InvitedUserId { get; set; }
        [Required]
        public User? InvitedUser { get; set; }
    }
}
