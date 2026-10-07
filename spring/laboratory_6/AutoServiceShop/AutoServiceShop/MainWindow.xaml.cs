using System.Windows;
using AutoServiceShop.Models;
using AutoServiceShop.Resources;
using AutoServiceShop.ViewModels;
using AutoServiceShop.Views;

namespace AutoServiceShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void ProductCard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Product product)
            {
                var vm = DataContext as MainViewModel;
                vm.SelectedProduct = product;
            }
        }

        private void ProductCard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is FrameworkElement element && element.DataContext is Product product)
                {
                    var vm = DataContext as MainViewModel;
                    vm.SelectedProduct = product;
                    if (vm.ShowDetailsCommand.CanExecute(null))
                        vm.ShowDetailsCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void SetRussian_Click(object sender, RoutedEventArgs e) => LocalizationManager.ChangeLanguage("ru");

        private void SetEnglish_Click(object sender, RoutedEventArgs e) => LocalizationManager.ChangeLanguage("en");

        private void ShowContactInfo_Click(object sender, RoutedEventArgs e)
        {
            var contactWindow = new Views.ContactWindow();
            contactWindow.Owner = this;
            contactWindow.ShowDialog();
        }
        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Owner = this;
            profileWindow.ShowDialog();
        }
    }
}