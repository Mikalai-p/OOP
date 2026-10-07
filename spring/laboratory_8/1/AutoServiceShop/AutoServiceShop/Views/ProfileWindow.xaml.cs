using System.Windows;
using AutoServiceShop.ViewModels;

namespace AutoServiceShop.Views
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            DataContext = new ProfileViewModel();
        }
    }
}