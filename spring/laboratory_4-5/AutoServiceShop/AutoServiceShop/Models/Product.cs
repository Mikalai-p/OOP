using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoServiceShop.Models
{
    public class Product : INotifyPropertyChanged
    {
        private int _quantity;
        private List<string> _imagePaths;

        public int Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }

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

        public string Category { get; set; }
        public double Rating { get; set; }
        public decimal Price { get; set; }

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

        public string Country { get; set; }
        public double Discount { get; set; }
        public bool InStock { get; set; } = true;
        public List<int> RelatedProductIds { get; set; } = new List<int>();
        public int SoldCount { get; set; }
        public string Manufacturer { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }

        public bool IsAvailable => Quantity > 0;
        public string FirstImagePath => ImagePaths != null && ImagePaths.Count > 0 ? ImagePaths[0] : null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}