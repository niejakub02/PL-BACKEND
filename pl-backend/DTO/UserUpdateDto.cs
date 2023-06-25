namespace pl_backend.DTO
{
    public class UserUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public int[]? Language { get; set; }

        public string? Description { get; set; }
    }
}
