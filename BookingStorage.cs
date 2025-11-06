using System.Collections.ObjectModel;

namespace UniversalYogaCustomerApp
{
    public static class BookingStorage
    {
        public static ObservableCollection<BookingRecord> Bookings { get; } = new();
    }

    public class BookingRecord
    {
        public string CourseName { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public DateTime SelectedDate { get; set; }
    }
}
    