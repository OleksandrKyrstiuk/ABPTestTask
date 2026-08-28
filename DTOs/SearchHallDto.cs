
namespace TeskTask.DTOs
{
    public class SearchHallDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? Capacity { get; set; }
        public string? Name { get; set; }
        public List<Guid> ServiceIds { get; set; } = new();
    }
}
