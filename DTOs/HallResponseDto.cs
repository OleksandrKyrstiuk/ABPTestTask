using TeskTask.Models;
namespace TeskTask.DTOs
{
    public class HallResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public List<Service> Services { get; set; } = new();
        public decimal BasePricePerHour { get; set; }
    }
}
