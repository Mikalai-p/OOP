using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoServiceShop.Models
{
    public class Product : INotifyPropertyChanged
    {
        private int _quantity;
        private List<string> _imagePaths;
        private double _rating;
        private decimal _price;
        private string _shortName;
        private string _fullName;
        private string _description;
        private string _category;
        private string _country;
        private double _discount;
        private bool _inStock;
        private string _manufacturer;
        private string _color;
        private string _size;

        public int Id { get; set; }

        public string ShortName
        {
            get => _shortName;
            set { _shortName = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public List<string> ImagePaths
        {
            get => _imagePaths;
            set
            {
                _imagePaths = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FirstImagePath));
            }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        public double Rating
        {
            get => _rating;
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsAvailable));
                }
            }
        }

        public string Country
        {
            get => _country;
            set { _country = value; OnPropertyChanged(); }
        }

        public double Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(); }
        }

        public bool InStock
        {
            get => _inStock;
            set { _inStock = value; OnPropertyChanged(); }
        }

        public List<int> RelatedProductIds { get; set; } = new List<int>();

        public int SoldCount { get; set; }

        public string Manufacturer
        {
            get => _manufacturer;
            set { _manufacturer = value; OnPropertyChanged(); }
        }

        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        public string Size
        {
            get => _size;
            set { _size = value; OnPropertyChanged(); }
        }

        public bool IsAvailable => Quantity > 0;
        public string FirstImagePath => ImagePaths != null && ImagePaths.Count > 0 ? ImagePaths[0] : null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Product Clone()
        {
            return new Product
            {
                Id = this.Id,
                ShortName = this.ShortName,
                FullName = this.FullName,
                Description = this.Description,
                ImagePaths = this.ImagePaths != null ? new System.Collections.Generic.List<string>(this.ImagePaths) : new System.Collections.Generic.List<string>(),
                Category = this.Category,
                Rating = this.Rating,
                Price = this.Price,
                Quantity = this.Quantity,
                Country = this.Country,
                Discount = this.Discount,
                InStock = this.InStock,
                RelatedProductIds = this.RelatedProductIds != null ? new System.Collections.Generic.List<int>(this.RelatedProductIds) : new System.Collections.Generic.List<int>(),
                SoldCount = this.SoldCount,
                Manufacturer = this.Manufacturer,
                Color = this.Color,
                Size = this.Size
            };
        }
    }
}