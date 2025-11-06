using System.Collections.ObjectModel;

namespace UniversalYogaCustomerApp.Views
{
    public partial class CoursesPage : ContentPage
    {
        public ObservableCollection<Course> Courses { get; set; } = new();
        private ObservableCollection<Course> AllCourses { get; set; } = new();

        public CoursesPage()
        {
            InitializeComponent();
            LoadCourses();
        }

        private void LoadCourses()
        {
            AllCourses = new ObservableCollection<Course>
            {
                new Course { Type = "Flow Yoga", DayOfWeek = "Monday", Time = "10:00 AM", Price = 100 },
                new Course { Type = "Aerial Yoga", DayOfWeek = "Wednesday", Time = "2:00 PM", Price = 120 },
                new Course { Type = "Power Yoga", DayOfWeek = "Friday", Time = "6:00 PM", Price = 110 }
            };

            Courses = new ObservableCollection<Course>(AllCourses);
            CoursesCollectionView.ItemsSource = Courses;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var keyword = e.NewTextValue?.ToLower() ?? "";
            var filtered = AllCourses.Where(c =>
                c.Type.ToLower().Contains(keyword) ||
                c.DayOfWeek.ToLower().Contains(keyword));
            CoursesCollectionView.ItemsSource = filtered.ToList();
        }

        private async void OnBookClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Course course)
            {
                await Navigation.PushAsync(new BookingPage(course));
            }
        }


        private async void OnCourseSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Course selected)
            {
                await DisplayAlert("Course Details",
                    $"{selected.Type}\nDay: {selected.DayOfWeek}\nTime: {selected.Time}\nPrice: ${selected.Price}",
                    "Close");
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }

    public class Course
    {
        public string? Type { get; set; }
        public string? DayOfWeek { get; set; }
        public string? Time { get; set; }
        public double Price { get; set; }
    }
}
