namespace pl_backend.DTO
{
    public class GetReviewDto
    {
        public int Rating { get; set; }
        public string? Description { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
