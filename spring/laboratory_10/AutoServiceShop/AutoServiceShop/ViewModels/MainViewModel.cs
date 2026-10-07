using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using AutoServiceShop.Commands;
using AutoServiceShop.DataAccess;
using AutoServiceShop.DataAccess.EfCore;
using AutoServiceShop.Models;
using AutoServiceShop.Resources;
using AutoServiceShop.Services;
using AutoServiceShop.Views;

namespace AutoServiceShop.ViewModels
{
    public enum SortOrder { None, PriceAscending, PriceDescending }

    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<Product> _filteredProducts;
        private string _searchText;
        private string _selectedCategory;
        private decimal _minPrice;
        private decimal _maxPrice;
        private Product _selectedProduct;
        private bool _isAdmin = true;
        private SortOrder _currentSortOrder = SortOrder.None;
        private string _allCategoriesPlaceholder;
        private string _filterError;
        private readonly UndoRedoManager<ObservableCollection<Product>> _undoRedoManager = new();
        private IAutoServiceDataService? _dataService;
        private CancellationTokenSource? _filterCts;

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public decimal MinPrice
        {
            get => _minPrice;
            set
            {
                _minPrice = value;
                OnPropertyChanged();
                ValidatePriceRange();
            }
        }

        public decimal MaxPrice
        {
            get => _maxPrice;
            set
            {
                _maxPrice = value;
                OnPropertyChanged();
                ValidatePriceRange();
            }
        }

        public string FilterError
        {
            get => _filterError;
            set { _filterError = value; OnPropertyChanged(); }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsClient));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsClient
        {
            get => !_isAdmin;
            set => IsAdmin = !value;
        }

        public SortOrder CurrentSortOrder
        {
            get => _currentSortOrder;
            set
            {
                if (_currentSortOrder != value)
                {
                    _currentSortOrder = value;
                    OnPropertyChanged();
                    FilterProducts();
                }
            }
        }

        private ObservableCollection<string> _categories;
        public ObservableCollection<string> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand ShowDetailsCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand PlaceOrderCommand { get; }

        public MainViewModel()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["AutoServiceDb"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Строка подключения не найдена в App.config.");
                }
                _dataService = new EfAutoServiceDataService(connectionString);
                _allProducts = _dataService.GetAllProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации БД: {ex.Message}\n\nПриложение будет работать без базы данных.",
                                "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _dataService = null;
                _allProducts = new ObservableCollection<Product>();
            }

            _filteredProducts = new ObservableCollection<Product>(_allProducts);

            LocalizationManager.LanguageChanged += OnLanguageChanged;
            UpdateCategories();

            AddProductCommand = new RelayCommand(AddProduct, param => IsAdmin && _dataService != null);
            EditProductCommand = new RelayCommand(EditProduct, param => SelectedProduct != null && IsAdmin && _dataService != null);
            DeleteProductCommand = new RelayCommand(DeleteProduct, param => SelectedProduct != null && IsAdmin && _dataService != null);
            ShowDetailsCommand = new RelayCommand(ShowDetails, param => SelectedProduct != null);
            SortCommand = new RelayCommand(Sort);
            SaveCommand = new RelayCommand(Save);
            UndoCommand = new RelayCommand(Undo, _ => _undoRedoManager.CanUndo);
            RedoCommand = new RelayCommand(Redo, _ => _undoRedoManager.CanRedo);
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            PlaceOrderCommand = new RelayCommand(PlaceOrder, param => _dataService != null);

            _minPrice = 0;
            _maxPrice = 10000;

            PushStateForUndo();
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            UpdateCategories();
            if (SelectedCategory == _allCategoriesPlaceholder || SelectedCategory == "")
                ApplyFilter(null);
        }

        private void UpdateCategories()
        {
            _allCategoriesPlaceholder = Application.Current.FindResource("AllCategories") as string ?? "Все категории";
            var cats = _allProducts.Select(p => p.Category).Distinct().ToList();
            cats.Insert(0, _allCategoriesPlaceholder);
            Categories = new ObservableCollection<string>(cats);
            SelectedCategory = _allCategoriesPlaceholder;
        }

        private bool CanEditOrDelete(object param) => SelectedProduct != null && IsAdmin && _dataService != null;

        private void ValidatePriceRange()
        {
            if (MinPrice > MaxPrice && MaxPrice > 0)
            {
                FilterError = Application.Current.FindResource("PriceRangeError").ToString();
            }
            else
            {
                FilterError = string.Empty;
            }
        }

        private void ApplyFilter(object param)
        {
            if (MinPrice > MaxPrice && MaxPrice > 0)
            {
                MessageBox.Show(
                    Application.Current.FindResource("PriceRangeError").ToString(),
                    Application.Current.FindResource("FilterErrorTitle").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            FilterProducts();
        }

        private void FilterProducts()
        {
            if (_dataService == null) return;

            _filterCts?.Cancel();
            _filterCts = new CancellationTokenSource();
            var ct = _filterCts.Token;

            var dataService = _dataService;
            _ = FilterProductsInternalAsync(dataService, ct);
        }

        private async Task FilterProductsInternalAsync(IAutoServiceDataService dataService, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return;

            string? categoryFilter = null;
            if (!string.IsNullOrWhiteSpace(SelectedCategory) && SelectedCategory != _allCategoriesPlaceholder)
                categoryFilter = SelectedCategory;

            // Демонстрация поиска: "short:" - только по ShortName, иначе поиск по ShortName+Description
            bool searchOnlyShortName = false;
            string? search = SearchText;
            if (!string.IsNullOrWhiteSpace(search) &&
                search.StartsWith("short:", StringComparison.OrdinalIgnoreCase))
            {
                searchOnlyShortName = true;
                search = search.Substring("short:".Length).Trim();
            }

            var sortOrder = CurrentSortOrder switch
            {
                SortOrder.PriceAscending => ProductSortOrder.PriceAscending,
                SortOrder.PriceDescending => ProductSortOrder.PriceDescending,
                _ => ProductSortOrder.None
            };

            ObservableCollection<Product> result = searchOnlyShortName
                ? await dataService.QueryProductsByShortNameAsync(
                    search,
                    categoryFilter,
                    MinPrice,
                    MaxPrice,
                    sortOrder,
                    ct)
                : await dataService.QueryProductsAsync(
                    search,
                    categoryFilter,
                    MinPrice,
                    MaxPrice,
                    sortOrder,
                    ct);

            if (ct.IsCancellationRequested) return;
            FilteredProducts = result;
        }

        private void Sort(object param)
        {
            CurrentSortOrder = CurrentSortOrder switch
            {
                SortOrder.None => SortOrder.PriceAscending,
                SortOrder.PriceAscending => SortOrder.PriceDescending,
                _ => SortOrder.None
            };
        }

        private void Save(object param)
        {
            MessageBox.Show("Данные автоматически сохраняются в базе данных.");
        }

        private ObservableCollection<Product> CloneCurrentCollection()
        {
            return new ObservableCollection<Product>(_allProducts.Select(p => p.Clone()));
        }

        private void PushStateForUndo()
        {
            var clonedList = CloneCurrentCollection();
            _undoRedoManager.PushState(clonedList);
        }

        private void Undo(object param)
        {
            var previousState = _undoRedoManager.Undo();
            if (previousState != null)
            {
                _allProducts = new ObservableCollection<Product>(previousState.Select(p => p.Clone()));
                FilterProducts();
                UpdateCategories();
                SelectedProduct = null;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void Redo(object param)
        {
            var nextState = _undoRedoManager.Redo();
            if (nextState != null)
            {
                _allProducts = new ObservableCollection<Product>(nextState.Select(p => p.Clone()));
                FilterProducts();
                UpdateCategories();
                SelectedProduct = null;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public void ResetSelectedProductRating()
        {
            if (SelectedProduct == null || _dataService == null) return;
            PushStateForUndo();
            SelectedProduct.Rating = 0;
            _dataService.UpdateProduct(SelectedProduct);
        }

        private void AddProduct(object param)
        {
            if (_dataService == null) return;

            var viewModel = new AddEditProductViewModel();
            var window = new AddEditProductWindow(viewModel);
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() == true)
            {
                try
                {
                    var newProduct = viewModel.Product;
                    int newId = _dataService.AddProduct(newProduct);
                    newProduct.Id = newId;
                    _allProducts.Add(newProduct);
                    FilterProducts();
                    UpdateCategories();

                    MessageBox.Show(
                        "Товар успешно добавлен в базу данных!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при добавлении товара: {ex.Message}",
                        "Ошибка базы данных",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void EditProduct(object param)
        {
            if (SelectedProduct == null || _dataService == null) return;

            var productCopy = SelectedProduct.Clone();
            var viewModel = new AddEditProductViewModel(productCopy);
            var window = new AddEditProductWindow(viewModel);
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() == true)
            {
                try
                {
                    // Копируем все поля из productCopy в SelectedProduct
                    SelectedProduct.ShortName = productCopy.ShortName;
                    SelectedProduct.FullName = productCopy.FullName;
                    SelectedProduct.Description = productCopy.Description;
                    SelectedProduct.Category = productCopy.Category;
                    SelectedProduct.Price = productCopy.Price;
                    SelectedProduct.Quantity = productCopy.Quantity;
                    SelectedProduct.Rating = productCopy.Rating;
                    SelectedProduct.Country = productCopy.Country;
                    SelectedProduct.Discount = productCopy.Discount;
                    SelectedProduct.InStock = productCopy.InStock;
                    SelectedProduct.Manufacturer = productCopy.Manufacturer;
                    SelectedProduct.Color = productCopy.Color;
                    SelectedProduct.Size = productCopy.Size;
                    SelectedProduct.ImagePaths = new System.Collections.Generic.List<string>(productCopy.ImagePaths);

                    _dataService.UpdateProduct(SelectedProduct);

                    FilterProducts();
                    UpdateCategories();

                    MessageBox.Show(
                        "Товар успешно обновлен в базе данных!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при обновлении товара: {ex.Message}",
                        "Ошибка базы данных",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void DeleteProduct(object param)
        {
            if (SelectedProduct == null || _dataService == null) return;

            var result = MessageBox.Show(
                string.Format(Application.Current.FindResource("ConfirmDeleteProduct").ToString(), SelectedProduct.ShortName),
                Application.Current.FindResource("ConfirmDelete").ToString(),
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    PushStateForUndo();
                    _dataService.DeleteProduct(SelectedProduct.Id);
                    _allProducts.Remove(SelectedProduct);
                    FilterProducts();
                    SelectedProduct = null;
                    UpdateCategories();

                    MessageBox.Show(
                        "Товар успешно удален из базы данных!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при удалении товара: {ex.Message}",
                        "Ошибка базы данных",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void ShowDetails(object param)
        {
            if (SelectedProduct == null) return;
            var detailWindow = new ProductDetailWindow(SelectedProduct);
            detailWindow.Owner = Application.Current.MainWindow;
            detailWindow.ShowDialog();
        }

        private void PlaceOrder(object param)
        {
            if (param is Product product && _dataService != null)
            {
                var orderWindow = new OrderWindow(product.Id, _dataService);
                orderWindow.Owner = Application.Current.MainWindow;
                if (orderWindow.ShowDialog() == true)
                {
                    RefreshData();
                }
            }
        }

        public void RefreshData()
        {
            if (_dataService == null) return;
            try
            {
                _allProducts = _dataService.GetAllProducts();
                UpdateCategories();
                FilterProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}