namespace TeskTask.Models
{
    public class Hall
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public List<Guid> ServiceIds { get; set; } = new();
        public decimal BasePricePerHour { get; set; }

    }
}
