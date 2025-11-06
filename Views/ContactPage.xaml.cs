using System;

namespace UniversalYogaCustomerApp.Views
{
    public partial class ContactPage : ContentPage
    {
        public ContactPage()
        {
            InitializeComponent();
        }

        private void OnSendMessageClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                string.IsNullOrWhiteSpace(MessageEditor.Text))
            {
                StatusLabel.TextColor = Colors.Red;
                StatusLabel.Text = "Please fill in all fields before sending.";
                return;
            }

            StatusLabel.TextColor = Colors.Green;
            StatusLabel.Text = $"Thank you, {NameEntry.Text}! We'll reply to {EmailEntry.Text} soon.";

            // Clear form after submission
            NameEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            MessageEditor.Text = string.Empty;
        }
    }
}
