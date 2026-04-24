using System.Collections.ObjectModel;
using UniversalYogaCustomerApp.Models;
using UniversalYogaCustomerApp.Services;

namespace UniversalYogaCustomerApp.Views
{
    public partial class CoursesPage : ContentPage
    {
        private readonly FirestoreCourseService _firestoreCourseService = new();
        private bool _isLoaded;

        public ObservableCollection<AvailableClassItem> AvailableClasses { get; set; } = new();
        private List<AvailableClassItem> AllAvailableClasses { get; set; } = new();

        public CoursesPage()
        {
            InitializeComponent();
            CoursesCollectionView.ItemsSource = AvailableClasses;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_isLoaded)
            {
                return;
            }

            await LoadAvailableClassesAsync();
        }

        private async Task LoadAvailableClassesAsync()
        {
            try
            {
                SetStatus("Loading classes from cloud...", true);
                CourseSearchBar.Text = string.Empty;

                var loadedClasses = await _firestoreCourseService.GetAvailableClassesAsync();
                AllAvailableClasses = loadedClasses;

                AvailableClasses.Clear();
                foreach (var item in loadedClasses)
                {
                    AvailableClasses.Add(item);
                }

                _isLoaded = true;

                if (AvailableClasses.Count == 0)
                {
                    SetStatus("No available classes found.", true);
                }
                else
                {
                    SetStatus($"Loaded {AvailableClasses.Count} available class instance(s).", true);
                }
            }
            catch (Exception ex)
            {
                SetStatus("Failed to load classes from cloud.", true);
                await DisplayAlert("Load Error", ex.Message, "OK");
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = e.NewTextValue?.Trim().ToLowerInvariant() ?? string.Empty;

            IEnumerable<AvailableClassItem> filtered = AllAvailableClasses;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                filtered = AllAvailableClasses.Where(c =>
                    c.ClassType.ToLowerInvariant().Contains(keyword) ||
                    c.Day.ToLowerInvariant().Contains(keyword) ||
                    c.Time.ToLowerInvariant().Contains(keyword) ||
                    c.Date.ToLowerInvariant().Contains(keyword) ||
                    c.Teacher.ToLowerInvariant().Contains(keyword));
            }

            AvailableClasses.Clear();
            foreach (var item in filtered)
            {
                AvailableClasses.Add(item);
            }

            if (AllAvailableClasses.Count == 0)
            {
                SetStatus("No available classes found.", true);
            }
            else if (AvailableClasses.Count == 0)
            {
                SetStatus("No matches found for your search.", true);
            }
            else
            {
                SetStatus($"Showing {AvailableClasses.Count} class instance(s).", true);
            }
        }

        private async void OnBookClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is AvailableClassItem selectedClass)
            {
                await Navigation.PushAsync(new CheckoutPage(selectedClass));
            }
        }

        private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is AvailableClassItem selected)
            {
                await DisplayAlert(
                    "Class Instance Details",
                    $"Type: {selected.ClassType}\nDay: {selected.Day}\nDate: {selected.Date}\nTime: {selected.Time}\nTeacher: {selected.Teacher}\nPrice: {selected.Price:C}",
                    "Close");

                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private void SetStatus(string message, bool visible)
        {
            var statusLabel = this.FindByName<Label>("StatusLabel");
            if (statusLabel is null)
            {
                return;
            }

            statusLabel.Text = message;
            statusLabel.IsVisible = visible;
        }
    }
}