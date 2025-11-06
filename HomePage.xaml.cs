using System;

namespace UniversalYogaCustomerApp.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnGetStartedClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CoursesPage");
        }
    }
}
