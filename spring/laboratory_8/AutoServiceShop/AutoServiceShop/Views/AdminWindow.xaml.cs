using AutoServiceShop.DataAccess;
using AutoServiceShop.Models;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace AutoServiceShop.Views
{
    public partial class AdminWindow : Window
    {
        private readonly SQLiteDataService _dataService;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Order> _orders;

        private Stack<object> _undoStack = new Stack<object>();
        private Stack<object> _redoStack = new Stack<object>();
        private object _currentState;

        // Для отслеживания изменений в заказах
        private Dictionary<int, int> _originalOrderQuantities = new Dictionary<int, int>();

        public AdminWindow()
        {
            InitializeComponent();

            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["AutoServiceDb"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Строка подключения не найдена в App.config.");
                }

                _dataService = new SQLiteDataService(connectionString);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}\n\nОкно администрирования будет закрыто.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        // Обработчик автогенерации столбцов – скрываем свойства, которые не должны редактироваться в таблице
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ImagePaths" || e.PropertyName == "RelatedProductIds")
                e.Cancel = true; // Не создаём столбец для списков
        }

        private void LoadProducts()
        {
            try
            {
                _products = _dataService.GetAllProducts();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = _products;
                _currentState = CloneCurrentData();
                UpdateUndoRedoButtons();
                UpdateSaveButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                dataGrid.ItemsSource = null;
            }
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _dataService.GetAllCategories();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = _categories;
                _currentState = CloneCurrentData();
                UpdateUndoRedoButtons();
                UpdateSaveButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                dataGrid.ItemsSource = null;
            }
        }

        private void LoadOrders()
        {
            try
            {
                _orders = _dataService.GetAllOrders();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = _orders;

                // Сохраняем оригинальные количества для отслеживания изменений
                _originalOrderQuantities.Clear();
                foreach (var order in _orders)
                {
                    _originalOrderQuantities[order.Id] = order.Quantity;
                }

                _currentState = CloneCurrentData();
                UpdateUndoRedoButtons();
                UpdateSaveButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                dataGrid.ItemsSource = null;
            }
        }

        private object CloneCurrentData()
        {
            if (rbProducts.IsChecked == true && _products != null)
            {
                var clone = new ObservableCollection<Product>();
                foreach (var p in _products)
                    clone.Add(p.Clone());
                return clone;
            }
            else if (rbCategories.IsChecked == true && _categories != null)
            {
                var clone = new ObservableCollection<Category>();
                foreach (var c in _categories)
                    clone.Add(new Category { Id = c.Id, Name = c.Name, Description = c.Description });
                return clone;
            }
            else if (rbOrders.IsChecked == true && _orders != null)
            {
                var clone = new ObservableCollection<Order>();
                foreach (var o in _orders)
                    clone.Add(new Order { Id = o.Id, ProductId = o.ProductId, Quantity = o.Quantity, OrderDate = o.OrderDate, CustomerName = o.CustomerName });
                return clone;
            }
            return null;
        }

        private void PushState()
        {
            var clone = CloneCurrentData();
            if (clone != null)
            {
                _undoStack.Push(_currentState);
                _currentState = clone;
                _redoStack.Clear();
                UpdateUndoRedoButtons();
                UpdateSaveButtonState();
            }
        }

        private void Undo()
        {
            if (_undoStack.Count == 0) return;
            _redoStack.Push(_currentState);
            _currentState = _undoStack.Pop();
            RestoreState(_currentState);
            UpdateUndoRedoButtons();
            UpdateSaveButtonState();
        }

        private void Redo()
        {
            if (_redoStack.Count == 0) return;
            _undoStack.Push(_currentState);
            _currentState = _redoStack.Pop();
            RestoreState(_currentState);
            UpdateUndoRedoButtons();
            UpdateSaveButtonState();
        }

        private void RestoreState(object state)
        {
            if (rbProducts.IsChecked == true && state is ObservableCollection<Product> products)
            {
                _products = products;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = _products;
            }
            else if (rbCategories.IsChecked == true && state is ObservableCollection<Category> categories)
            {
                _categories = categories;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = _categories;
            }
            else if (rbOrders.IsChecked == true && state is ObservableCollection<Order> orders)
            {
                _orders = orders;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = _orders;
            }
        }

        private void UpdateUndoRedoButtons()
        {
            btnUndo.IsEnabled = _undoStack.Count > 0;
            btnRedo.IsEnabled = _redoStack.Count > 0;
        }

        private void UpdateSaveButtonState()
        {
            btnSave.IsEnabled = true;
        }

        private void TableSelector_Checked(object sender, RoutedEventArgs e)
        {
            if (_dataService == null) return;
            _undoStack.Clear();
            _redoStack.Clear();

            if (rbProducts.IsChecked == true)
                LoadProducts();
            else if (rbCategories.IsChecked == true)
                LoadCategories();
            else if (rbOrders.IsChecked == true)
                LoadOrders();
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            PushState();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Здесь можно добавить логику, если нужно, но пока оставляем пустым
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {
            Undo();
        }

        private void BtnRedo_Click(object sender, RoutedEventArgs e)
        {
            Redo();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_dataService == null) return;

            if (rbProducts.IsChecked == true)
            {
                var viewModel = new ViewModels.AddEditProductViewModel();
                var window = new AddEditProductWindow(viewModel);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    PushState();
                    LoadProducts();  // Перезагружаем таблицу товаров
                }
            }
            else if (rbCategories.IsChecked == true)
            {
                var dialog = new InputDialog(Application.Current.FindResource("EnterCategoryName").ToString(),
                                             Application.Current.FindResource("NewCategory").ToString());
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Answer))
                {
                    using var connection = new SQLiteConnection(_dataService.ConnectionString);
                    connection.Open();
                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        var cmd = new SQLiteCommand("INSERT INTO Categories (Name) VALUES (@name)", connection, transaction);
                        cmd.Parameters.AddWithValue("@name", dialog.Answer);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        LoadCategories();  // Перезагружаем таблицу категорий
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при добавлении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (rbOrders.IsChecked == true)
            {
                var orderWindow = new AddOrderWindow(_dataService);
                orderWindow.Owner = this;
                if (orderWindow.ShowDialog() == true)
                {
                    LoadOrders();  // Перезагружаем таблицу заказов
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_dataService == null) return;
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show(Application.Current.FindResource("SelectRecord").ToString());
                return;
            }

            if (rbProducts.IsChecked == true && dataGrid.SelectedItem is Product selectedProduct)
            {
                var viewModel = new ViewModels.AddEditProductViewModel(selectedProduct);
                var window = new AddEditProductWindow(viewModel);
                window.Owner = this;
                if (window.ShowDialog() == true)
                {
                    LoadProducts();  // Перезагружаем таблицу товаров
                }
            }
            else if (rbCategories.IsChecked == true && dataGrid.SelectedItem is Category selectedCategory)
            {
                var dialog = new InputDialog("Измените название категории:", "Редактирование", selectedCategory.Name);
                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Answer))
                {
                    using var connection = new SQLiteConnection(_dataService.ConnectionString);
                    connection.Open();
                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        var cmd = new SQLiteCommand("UPDATE Categories SET Name = @name WHERE Id = @id", connection, transaction);
                        cmd.Parameters.AddWithValue("@name", dialog.Answer);
                        cmd.Parameters.AddWithValue("@id", selectedCategory.Id);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        LoadCategories();  // Перезагружаем таблицу категорий
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при обновлении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (rbOrders.IsChecked == true && dataGrid.SelectedItem is Order selectedOrder)
            {
                // Редактирование заказа можно выполнять прямо в DataGrid, поэтому здесь ничего не делаем
                MessageBox.Show("Редактирование заказов осуществляется через DataGrid. Выделите ячейку и измените значение.");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_dataService == null) return;
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            if (MessageBox.Show(Application.Current.FindResource("ConfirmDeleteMessage").ToString(),
                                Application.Current.FindResource("ConfirmDelete").ToString(),
                                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            if (rbProducts.IsChecked == true && dataGrid.SelectedItem is Product product)
            {
                try
                {
                    _dataService.DeleteProduct(product.Id);
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (rbCategories.IsChecked == true && dataGrid.SelectedItem is Category category)
            {
                using var connection = new SQLiteConnection(_dataService.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var cmd = new SQLiteCommand("DELETE FROM Categories WHERE Id = @id", connection, transaction);
                    cmd.Parameters.AddWithValue("@id", category.Id);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    LoadCategories();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка при удалении категории: {ex.Message}\nВозможно, есть связанные товары.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (rbOrders.IsChecked == true && dataGrid.SelectedItem is Order order)
            {
                try
                {
                    _dataService.DeleteOrder(order.Id);
                    LoadOrders();

                    if (_orders.Count == 0)
                    {
                        _dataService.ResetOrdersIdCounter();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataService == null) return;

            try
            {
                if (rbProducts.IsChecked == true && dataGrid.ItemsSource is ObservableCollection<Product> products)
                {
                    foreach (var product in products)
                    {
                        _dataService.UpdateProduct(product);
                    }
                    MessageBox.Show("Изменения в товарах сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (rbCategories.IsChecked == true && dataGrid.ItemsSource is ObservableCollection<Category> categories)
                {
                    using var connection = new SQLiteConnection(_dataService.ConnectionString);
                    connection.Open();
                    foreach (var category in categories)
                    {
                        using var cmd = new SQLiteCommand("UPDATE Categories SET Name=@name, Description=@desc WHERE Id=@id", connection);
                        cmd.Parameters.AddWithValue("@name", category.Name);
                        cmd.Parameters.AddWithValue("@desc", (object)category.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", category.Id);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Изменения в категориях сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (rbOrders.IsChecked == true && dataGrid.ItemsSource is ObservableCollection<Order> orders)
                {
                    using var connection = new SQLiteConnection(_dataService.ConnectionString);
                    connection.Open();

                    foreach (var order in orders)
                    {
                        // Проверяем, изменилось ли количество
                        if (_originalOrderQuantities.TryGetValue(order.Id, out int originalQuantity) && originalQuantity != order.Quantity)
                        {
                            // Получаем текущий заказ из БД для проверки
                            var selectQuery = "SELECT ProductId, Quantity FROM Orders WHERE Id = @OrderId";
                            using var selectCmd = new SQLiteCommand(selectQuery, connection);
                            selectCmd.Parameters.AddWithValue("@OrderId", order.Id);
                            using var reader = selectCmd.ExecuteReader();
                            if (reader.Read())
                            {
                                int productId = reader.GetInt32(0);
                                int dbQuantity = reader.GetInt32(1);
                                reader.Close();

                                // Вычисляем разницу и обновляем товар
                                int quantityDiff = dbQuantity - order.Quantity;

                                var updateProductQuery = "UPDATE Products SET Quantity = Quantity + @diff WHERE Id = @ProductId";
                                using var updateProductCmd = new SQLiteCommand(updateProductQuery, connection);
                                updateProductCmd.Parameters.AddWithValue("@diff", quantityDiff);
                                updateProductCmd.Parameters.AddWithValue("@ProductId", productId);
                                updateProductCmd.ExecuteNonQuery();
                            }
                        }

                        // Обновляем заказ
                        var updateOrderQuery = @"
                            UPDATE Orders SET 
                                Quantity = @Quantity,
                                CustomerName = @CustomerName
                            WHERE Id = @Id";
                        using var updateOrderCmd = new SQLiteCommand(updateOrderQuery, connection);
                        updateOrderCmd.Parameters.AddWithValue("@Id", order.Id);
                        updateOrderCmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                        updateOrderCmd.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                        updateOrderCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Изменения в заказах сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем оригинальные количества
                    _originalOrderQuantities.Clear();
                    foreach (var order in orders)
                    {
                        _originalOrderQuantities[order.Id] = order.Quantity;
                    }
                }

                _currentState = CloneCurrentData();
                _undoStack.Clear();
                _redoStack.Clear();
                UpdateUndoRedoButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}