using pl_backend.Models;

namespace pl_backend.DTO
{
    public class AddReviewDto
    {
        public int Rating { get; set; }
        public string? Description { get; set; }
        public int ToId { get; set; }
    }
}
