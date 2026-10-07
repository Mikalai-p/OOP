using System.Windows;

namespace AutoServiceShop.Views
{
    public partial class InputDialog : Window
    {
        public string Answer { get; private set; }

        public InputDialog(string prompt, string title, string defaultValue = "")
        {
            InitializeComponent();
            PromptText.Text = prompt;
            Title = title;
            InputTextBox.Text = defaultValue;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Answer = InputTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}