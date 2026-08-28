namespace TeskTask.Models
{
    public class AppData
    {
        public List<Hall> Halls { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    }
}