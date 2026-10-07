using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AutoServiceShop.Commands;
using AutoServiceShop.Data;
using AutoServiceShop.Models;
using AutoServiceShop.Resources;
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

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; FilterProducts(); OnPropertyChanged(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                FilterProducts();
                OnPropertyChanged();
            }
        }

        public decimal MinPrice
        {
            get => _minPrice;
            set { _minPrice = value; FilterProducts(); OnPropertyChanged(); }
        }

        public decimal MaxPrice
        {
            get => _maxPrice;
            set { _maxPrice = value; FilterProducts(); OnPropertyChanged(); }
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
            set { _currentSortOrder = value; FilterProducts(); OnPropertyChanged(); }
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

        public MainViewModel()
        {
            _allProducts = new ObservableCollection<Product>(DataService.LoadProducts());
            FilteredProducts = new ObservableCollection<Product>(_allProducts);

            LocalizationManager.LanguageChanged += OnLanguageChanged;
            UpdateCategories();

            AddProductCommand = new RelayCommand(AddProduct, param => IsAdmin);
            EditProductCommand = new RelayCommand(EditProduct, CanEditOrDelete);
            DeleteProductCommand = new RelayCommand(DeleteProduct, CanEditOrDelete);
            ShowDetailsCommand = new RelayCommand(ShowDetails, param => SelectedProduct != null);
            SortCommand = new RelayCommand(Sort);
            SaveCommand = new RelayCommand(Save);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            UpdateCategories();
            if (SelectedCategory == _allCategoriesPlaceholder || SelectedCategory == "")
                FilterProducts();
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

        private void FilterProducts()
        {
            var query = _allProducts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(p => p.ShortName.Contains(SearchText) || p.Description.Contains(SearchText));

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
            MessageBox.Show(Application.Current.FindResource("DataSaved").ToString(), "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddProduct(object param)
        {
            var viewModel = new AddEditProductViewModel();
            var window = new AddEditProductWindow(viewModel);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                _allProducts.Add(viewModel.Product);
                FilterProducts();
                DataService.SaveProducts(_allProducts.ToList());
                UpdateCategories(); // обновить список категорий, если появилась новая
            }
        }

        private void EditProduct(object param)
        {
            if (SelectedProduct == null) return;
            var viewModel = new AddEditProductViewModel(SelectedProduct);
            var window = new AddEditProductWindow(viewModel);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                FilterProducts();
                DataService.SaveProducts(_allProducts.ToList());
                // Категория могла измениться, обновим список
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