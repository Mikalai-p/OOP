using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using AutoServiceShop.Commands;
using AutoServiceShop.Models;

namespace AutoServiceShop.ViewModels
{
    public class AddEditProductViewModel : BaseViewModel, IDataErrorInfo
    {
        private Product _product;
        private bool _isEditMode;
        private ObservableCollection<string> _editableImagePaths;

        public Product Product
        {
            get => _product;
            set { _product = value; OnPropertyChanged(); }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set { _isEditMode = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> EditableImagePaths
        {
            get => _editableImagePaths;
            set { _editableImagePaths = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddImageCommand { get; }
        public ICommand RemoveImageCommand { get; }

        public event EventHandler<Product> SaveRequested;
        public event EventHandler CancelRequested;

        public AddEditProductViewModel(Product product = null)
        {
            if (product != null)
            {
                Product = product.Clone();
                IsEditMode = true;
                EditableImagePaths = new ObservableCollection<string>(Product.ImagePaths ?? new System.Collections.Generic.List<string>());
            }
            else
            {
                Product = new Product
                {
                    ImagePaths = new System.Collections.Generic.List<string>(),
                    Quantity = 0,
                    Price = 0,
                    Rating = 0,
                    Discount = 0,
                    InStock = true
                };
                IsEditMode = false;
                EditableImagePaths = new ObservableCollection<string>();
            }

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            AddImageCommand = new RelayCommand(AddImage);
            RemoveImageCommand = new RelayCommand(RemoveImage);
        }

        private bool CanSave(object param)
        {
            return string.IsNullOrEmpty(Error) &&
                   Product.Quantity >= 0 &&
                   Product.Price >= 0 &&
                   Product.Rating >= 0 &&
                   Product.Rating <= 5;
        }

        private void Save(object param)
        {
            try
            {
                Product.ImagePaths = EditableImagePaths.ToList();
                SaveRequested?.Invoke(this, Product);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Ошибка при сохранении: {ex.Message}",
                    "Ошибка",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void Cancel(object param)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        private void AddImage(object param)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            if (dlg.ShowDialog() == true)
            {
                EditableImagePaths.Add(dlg.FileName);
            }
        }

        public void AddImagePath(string path)
        {
            EditableImagePaths.Add(path);
        }

        private void RemoveImage(object param)
        {
            if (param is string path)
            {
                EditableImagePaths.Remove(path);
            }
        }

        #region IDataErrorInfo

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;
                switch (columnName)
                {
                    case nameof(Product.ShortName):
                        if (string.IsNullOrWhiteSpace(Product.ShortName))
                            error = "Краткое название обязательно";
                        break;
                    case nameof(Product.Category):
                        if (string.IsNullOrWhiteSpace(Product.Category))
                            error = "Категория обязательна";
                        break;
                    case nameof(Product.Price):
                        if (Product.Price <= 0)
                            error = "Цена должна быть больше 0";
                        break;
                    case nameof(Product.Quantity):
                        if (Product.Quantity < 0)
                            error = "Количество не может быть отрицательным";
                        break;
                    case nameof(Product.Rating):
                        if (Product.Rating < 0 || Product.Rating > 5)
                            error = "Рейтинг должен быть от 0 до 5";
                        break;
                }
                return error;
            }
        }

        #endregion
    }
}