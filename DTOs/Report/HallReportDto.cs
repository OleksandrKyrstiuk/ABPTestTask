namespace TeskTask.DTOs.Report
{
    public class HallReportDto
    {
        public Guid HallId { get; set; }
        public string? HallName { get; set; }
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
        public int BookedHours { get; set; }
    }
}
