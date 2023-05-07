using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pl_backend.Models
{
    public class Marker
    {
        [Key]
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool OffersHelp { get; set; }
        [Required]
        public string? City { get; set; } 

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
