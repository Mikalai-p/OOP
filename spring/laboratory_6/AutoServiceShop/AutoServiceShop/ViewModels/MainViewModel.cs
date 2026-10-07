using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AutoServiceShop.Commands;
using AutoServiceShop.Data;
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

        public MainViewModel()
        {
            _allProducts = new ObservableCollection<Product>(DataService.LoadProducts());
            _filteredProducts = new ObservableCollection<Product>(_allProducts);

            LocalizationManager.LanguageChanged += OnLanguageChanged;
            UpdateCategories();

            AddProductCommand = new RelayCommand(AddProduct, param => IsAdmin);
            EditProductCommand = new RelayCommand(EditProduct, CanEditOrDelete);
            DeleteProductCommand = new RelayCommand(DeleteProduct, CanEditOrDelete);
            ShowDetailsCommand = new RelayCommand(ShowDetails, param => SelectedProduct != null);
            SortCommand = new RelayCommand(Sort);
            SaveCommand = new RelayCommand(Save);
            UndoCommand = new RelayCommand(Undo, _ => _undoRedoManager.CanUndo);
            RedoCommand = new RelayCommand(Redo, _ => _undoRedoManager.CanRedo);
            ApplyFilterCommand = new RelayCommand(ApplyFilter);

            _minPrice = 0;
            _maxPrice = 10000;

            // Инициализируем менеджер текущим состоянием, чтобы можно было отменить первое действие
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

        private bool CanEditOrDelete(object param) => SelectedProduct != null && IsAdmin;

        private void ValidatePriceRange()
        {
            if (MinPrice > MaxPrice && MaxPrice > 0)
            {
                FilterError = "Цена 'от' не может быть больше цены 'до'";
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
                MessageBox.Show("Цена 'от' не может быть больше цены 'до'", "Ошибка фильтра",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            FilterProducts();
        }

        private void FilterProducts()
        {
            var query = _allProducts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(p =>
                    (p.ShortName?.Contains(SearchText) ?? false) ||
                    (p.Description?.Contains(SearchText) ?? false));

            if (!string.IsNullOrWhiteSpace(SelectedCategory) && SelectedCategory != _allCategoriesPlaceholder)
                query = query.Where(p => p.Category == SelectedCategory);

            if (MinPrice > 0)
                query = query.Where(p => p.Price >= MinPrice);
            if (MaxPrice > 0)
                query = query.Where(p => p.Price <= MaxPrice);

            IEnumerable<Product> sortedQuery = CurrentSortOrder switch
            {
                SortOrder.PriceAscending => query.OrderBy(p => p.Price),
                SortOrder.PriceDescending => query.OrderByDescending(p => p.Price),
                _ => query
            };

            FilteredProducts.Clear();
            foreach (var p in sortedQuery)
                FilteredProducts.Add(p);
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
            DataService.SaveProducts(_allProducts.ToList());
            MessageBox.Show(Application.Current.FindResource("DataSaved").ToString(), "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
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
                DataService.SaveProducts(_allProducts.ToList());
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
                DataService.SaveProducts(_allProducts.ToList());
                UpdateCategories();
                SelectedProduct = null;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void AddProduct(object param)
        {
            // Сохраняем состояние ДО добавления
            PushStateForUndo();

            var viewModel = new AddEditProductViewModel();
            var window = new AddEditProductWindow(viewModel);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                _allProducts.Add(viewModel.Product);
                FilterProducts();
                DataService.SaveProducts(_allProducts.ToList());
                UpdateCategories();
            }
        }

        private void EditProduct(object param)
        {
            if (SelectedProduct == null) return;

            // Сохраняем состояние ДО редактирования (оригинальное состояние продукта)
            PushStateForUndo();

            var viewModel = new AddEditProductViewModel(SelectedProduct);
            var window = new AddEditProductWindow(viewModel);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                // После закрытия окна с сохранением, SelectedProduct уже обновлён,
                // но нужно обновить коллекцию, если изменился ключ (например, Id не меняется, но можно перепривязать)
                // Просто вызываем FilterProducts для обновления отображения
                FilterProducts();
                DataService.SaveProducts(_allProducts.ToList());
                UpdateCategories();
            }
        }

        private void DeleteProduct(object param)
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show(
                string.Format(Application.Current.FindResource("ConfirmDeleteMessage").ToString(), SelectedProduct.ShortName),
                Application.Current.FindResource("ConfirmDelete").ToString(),
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Сохраняем состояние ДО удаления
                PushStateForUndo();
                _allProducts.Remove(SelectedProduct);
                FilterProducts();
                DataService.SaveProducts(_allProducts.ToList());
                SelectedProduct = null;
                UpdateCategories();
            }
        }

        private void ShowDetails(object param)
        {
            if (SelectedProduct == null) return;
            var detailWindow = new ProductDetailWindow(SelectedProduct);
            detailWindow.Owner = Application.Current.MainWindow;
            detailWindow.ShowDialog();
        }
    }
}