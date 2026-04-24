namespace UniversalYogaCustomerApp.Models;

public class BookingRecord
{
    public string BookingId { get; set; } = Guid.NewGuid().ToString("N");
    public int CourseId { get; set; }
    public int InstanceId { get; set; }
    public string ClassType { get; set; } = string.Empty;
    public string Day { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Teacher { get; set; } = string.Empty;
    public double Price { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}