namespace UniversalYogaCustomerApp.Views
{
    public partial class BookingSummaryPage : ContentPage
    {
        public BookingSummaryPage()
        {
            InitializeComponent();
            LoadBookings();
        }

        private void LoadBookings()
        {
            BookingCollectionView.ItemsSource = BookingStorage.Bookings;
        }

        private async void OnClearBookingsClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure you want to clear all bookings?", "Yes", "No");
            if (confirm)
            {
                BookingStorage.Bookings.Clear();
                BookingCollectionView.ItemsSource = null;
                BookingCollectionView.ItemsSource = BookingStorage.Bookings;
            }
        }
    }
}
