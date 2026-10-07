using AutoServiceShop.DataAccess;
using AutoServiceShop.Models;
using System;
using System.Windows;

namespace AutoServiceShop.Views
{
    public partial class OrderWindow : Window
    {
        private readonly SQLiteDataService _dataService;
        private readonly int _productId;
        private Product _product;

        public int Quantity { get; set; }
        public string CustomerName { get; set; }
        public Product Product => _product;

        public OrderWindow(int productId, SQLiteDataService dataService)
        {
            InitializeComponent();
            _productId = productId;
            _dataService = dataService;
            Quantity = 1;
            CustomerName = string.Empty;

            LoadProduct();

            DataContext = this;
        }

        private void LoadProduct()
        {
            try
            {
                _product = _dataService.GetProductById(_productId);
                if (_product == null)
                {
                    MessageBox.Show(
                        Application.Current.FindResource("ProductNotFound").ToString(),
                        Application.Current.FindResource("DatabaseError").ToString(),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка загрузки товара: {ex.Message}",
                    Application.Current.FindResource("DatabaseError").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            // Проверка имени
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                MessageBox.Show(
                    Application.Current.FindResource("EnterCustomerName").ToString(),
                    Application.Current.FindResource("DatabaseError").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Обновляем данные товара перед проверкой
            try
            {
                _product = _dataService.GetProductById(_productId);
                if (_product == null)
                {
                    MessageBox.Show(
                        Application.Current.FindResource("ProductNotFound").ToString(),
                        Application.Current.FindResource("DatabaseError").ToString(),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // Проверка наличия (разрешён заказ, даже если Quantity == Product.Quantity - последний товар)
                if (Quantity > _product.Quantity)
                {
                    MessageBox.Show(
                        string.Format(Application.Current.FindResource("InsufficientStock").ToString(), _product.Quantity),
                        Application.Current.FindResource("DatabaseError").ToString(),
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка проверки наличия: {ex.Message}",
                    Application.Current.FindResource("DatabaseError").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создаём заказ
            var order = new Order
            {
                ProductId = _productId,
                Quantity = Quantity,
                OrderDate = DateTime.Now,
                CustomerName = CustomerName.Trim()
            };

            try
            {
                _dataService.AddOrder(order);
                MessageBox.Show(
                    Application.Current.FindResource("OrderSuccess").ToString(),
                    "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (InvalidOperationException ex) // Недостаточно товара
            {
                MessageBox.Show(
                    ex.Message,
                    Application.Current.FindResource("DatabaseError").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при оформлении заказа: {ex.Message}",
                    Application.Current.FindResource("DatabaseError").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}