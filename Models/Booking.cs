namespace TeskTask.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid HallId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationHours { get; set; }
        public List<Guid> ServiceIds { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
