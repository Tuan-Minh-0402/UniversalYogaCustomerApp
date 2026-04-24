namespace UniversalYogaCustomerApp.Models;

public class AvailableClassItem
{
    public int CourseId { get; set; }
    public int InstanceId { get; set; }
    public string ClassType { get; set; } = string.Empty;
    public string Day { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Teacher { get; set; } = string.Empty;
    public double Price { get; set; }
}