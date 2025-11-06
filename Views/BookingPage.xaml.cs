namespace UniversalYogaCustomerApp.Views
{
    public partial class BookingPage : ContentPage
    {
        private Course _selectedCourse;

        // ✅ Add this parameterless constructor for MAUI to instantiate via Shell or XAML
        public BookingPage()
        {
            InitializeComponent();
        }

        // Existing constructor for when a specific course is passed in
        public BookingPage(Course selectedCourse)
        {
            InitializeComponent();
            _selectedCourse = selectedCourse;

            CourseLabel.Text = selectedCourse.Type;
            DayLabel.Text = $"Day: {selectedCourse.DayOfWeek}";
            TimeLabel.Text = $"Time: {selectedCourse.Time}";
        }

        private async void OnBookConfirmClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                await DisplayAlert("Missing Info", "Please enter your name and email.", "OK");
                return;
            }

            // Store booking in mock static storage
            BookingStorage.Bookings.Add(new BookingRecord
            {
                CourseName = _selectedCourse?.Type ?? "Unknown Course",
                Day = _selectedCourse?.DayOfWeek ?? "Unknown Day",
                Time = _selectedCourse?.Time ?? "Unknown Time",
                SelectedDate = DatePicker.Date
            });

            StatusLabel.Text = "Booking confirmed!";
            await DisplayAlert("Success", $"You’re booked for {_selectedCourse?.Type} on {_selectedCourse?.DayOfWeek}.", "OK");
        }
    }
}
