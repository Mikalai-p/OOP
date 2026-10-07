using System.Windows;
using AutoServiceShop.Models;

namespace AutoServiceShop.Views
{
    public partial class ProductDetailWindow : Window
    {
        public ProductDetailWindow(Product product)
        {
            InitializeComponent();
            DataContext = product;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}