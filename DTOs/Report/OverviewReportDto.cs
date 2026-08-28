namespace TeskTask.DTOs.Report
{
    public class OverviewReportDto
    {
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageBookingPrice { get; set; }
        public double AverageBookingDuration { get; set; }
    }
}
