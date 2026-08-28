namespace TeskTask.DTOs.Report
{
    public class ServiceReportDto
    {
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int UsageCount { get; set; }
        public decimal Revenue { get; set; }
    }

}
