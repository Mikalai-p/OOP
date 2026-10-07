using AutoServiceShop.DataAccess;
using AutoServiceShop.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace AutoServiceShop.Views
{
    public partial class AddOrderWindow : Window
    {
        private readonly IAutoServiceDataService _dataService;
        public ObservableCollection<Product> Products { get; set; }

        public AddOrderWindow(IAutoServiceDataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            Products = _dataService.GetAllProducts();
            ProductComboBox.ItemsSource = Products;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ProductComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                MessageBox.Show("Введите имя клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProduct = (Product)ProductComboBox.SelectedItem;
            int quantity = QuantitySelector.Quantity;

            if (quantity > selectedProduct.Quantity)
            {
                MessageBox.Show($"Недостаточно товара на складе. Доступно: {selectedProduct.Quantity}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var order = new Order
            {
                ProductId = selectedProduct.Id,
                Quantity = quantity,
                OrderDate = DateTime.Now,
                CustomerName = CustomerNameTextBox.Text
            };

            try
            {
                _dataService.AddOrder(order);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}