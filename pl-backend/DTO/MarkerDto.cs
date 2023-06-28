using pl_backend.Models;

namespace pl_backend.DTO
{
    public class MarkerDto
    {
        public Marker marker { get; set; }
        public User user { get; set; }

        public MarkerDto(Marker marker, User user)
        {
            this.marker = marker;
            this.user = user;
        }
    }
}
