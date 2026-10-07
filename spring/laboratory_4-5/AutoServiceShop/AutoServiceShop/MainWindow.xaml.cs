using System.Windows;
using AutoServiceShop.Models;
using AutoServiceShop.Resources;
using AutoServiceShop.ViewModels;

namespace AutoServiceShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        // Выделение товара при клике (одинарный клик)
        private void ProductCard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Product product)
            {
                var vm = DataContext as MainViewModel;
                vm.SelectedProduct = product;
            }
        }

        // Обработка двойного клика через MouseDown
        private void ProductCard_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) // двойной щелчок
            {
                if (sender is FrameworkElement element && element.DataContext is Product product)
                {
                    var vm = DataContext as MainViewModel;
                    vm.SelectedProduct = product;
                    if (vm.ShowDetailsCommand.CanExecute(null))
                        vm.ShowDetailsCommand.Execute(null);
                }
                e.Handled = true; // предотвращаем дальнейшую обработку
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
    }
}