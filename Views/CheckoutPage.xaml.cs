using UniversalYogaCustomerApp.Models;
using UniversalYogaCustomerApp.Services;

namespace UniversalYogaCustomerApp.Views;

public partial class CheckoutPage : ContentPage
{
    private readonly FirestoreBookingService _bookingService = new();
    private readonly AvailableClassItem? _selectedClass;

    public CheckoutPage()
    {
        InitializeComponent();
    }

    public CheckoutPage(AvailableClassItem selectedClass)
    {
        InitializeComponent();
        _selectedClass = selectedClass;
        BindSelectedClass(selectedClass);
    }

    private void BindSelectedClass(AvailableClassItem selectedClass)
    {
        ClassTypeLabel.Text = $"Type: {selectedClass.ClassType}";
        DayLabel.Text = $"Day: {selectedClass.Day}";
        DateLabel.Text = $"Date: {selectedClass.Date}";
        TimeLabel.Text = $"Time: {selectedClass.Time}";
        TeacherLabel.Text = $"Teacher: {selectedClass.Teacher}";
        PriceLabel.Text = $"Price: {selectedClass.Price:C}";
    }

    private async void OnSubmitBookingClicked(object sender, EventArgs e)
    {
        if (_selectedClass is null)
        {
            await DisplayAlert("Error", "No class selected for checkout.", "OK");
            return;
        }

        var customerName = NameEntry.Text?.Trim() ?? string.Empty;
        var customerEmail = EmailEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(customerName))
        {
            await DisplayAlert("Validation", "Name is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            await DisplayAlert("Validation", "Email is required.", "OK");
            return;
        }

        if (!customerEmail.Contains('@'))
        {
            await DisplayAlert("Validation", "Please enter a valid email.", "OK");
            return;
        }

        var booking = new BookingRecord
        {
            BookingId = Guid.NewGuid().ToString("N"),
            CourseId = _selectedClass.CourseId,
            InstanceId = _selectedClass.InstanceId,
            ClassType = _selectedClass.ClassType,
            Day = _selectedClass.Day,
            Date = _selectedClass.Date,
            Time = _selectedClass.Time,
            Teacher = _selectedClass.Teacher,
            Price = _selectedClass.Price,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CreatedAtUtc = DateTime.UtcNow
        };

        try
        {
            await _bookingService.SubmitBookingAsync(booking);
            await DisplayAlert("Success", "Booking submitted successfully.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Submit Failed", ex.Message, "OK");
        }
    }
}