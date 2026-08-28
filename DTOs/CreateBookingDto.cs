using System.ComponentModel.DataAnnotations;

namespace TeskTask.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public Guid HallId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Range(1, 24)]
        public int DurationHours { get; set; }

        public List<Guid> ServiceIds { get; set; } = new();
    }
}
