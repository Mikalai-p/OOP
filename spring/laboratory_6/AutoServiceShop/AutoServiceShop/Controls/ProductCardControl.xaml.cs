using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoServiceShop.Models;
using AutoServiceShop.ViewModels;

namespace AutoServiceShop.Controls
{
    public partial class ProductCardControl : UserControl
    {
        public ProductCardControl()
        {
            InitializeComponent();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is Product product)
            {
                var mainVM = FindParent<MainWindow>()?.DataContext as MainViewModel;
                if (mainVM != null)
                {
                    mainVM.SelectedProduct = product;
                }
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && DataContext is Product product)
            {
                var mainVM = FindParent<MainWindow>()?.DataContext as MainViewModel;
                if (mainVM?.ShowDetailsCommand.CanExecute(null) == true)
                    mainVM.ShowDetailsCommand.Execute(null);
                e.Handled = true;
            }
        }

        private T FindParent<T>() where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is T))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as T;
        }
    }
}