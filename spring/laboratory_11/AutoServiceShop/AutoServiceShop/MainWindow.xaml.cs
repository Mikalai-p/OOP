using System.Windows;
using System.Windows.Input;
using AutoServiceShop.Controls;
using AutoServiceShop.ViewModels;

namespace AutoServiceShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            this.AddHandler(RatingControl.PreviewRatingChangedEvent, new RoutedEventHandler(OnPreviewRatingChanged));
            this.AddHandler(RatingControl.RatingChangedEvent, new RoutedEventHandler(OnRatingChanged));
            this.AddHandler(QuantitySelectorControl.PreviewQuantityChangedEvent, new RoutedEventHandler(OnPreviewQuantityChanged));
            this.AddHandler(QuantitySelectorControl.QuantityChangedEvent, new RoutedEventHandler(OnQuantityChanged));
        }

        private void OnPreviewRatingChanged(object sender, RoutedEventArgs e) =>
            (DataContext as MainViewModel)?.AddEventLog($"PreviewRatingChanged (Tunnel) from {sender.GetType().Name}");

        private void OnRatingChanged(object sender, RoutedEventArgs e) =>
            (DataContext as MainViewModel)?.AddEventLog($"RatingChanged (Bubble) from {sender.GetType().Name}");

        private void OnPreviewQuantityChanged(object sender, RoutedEventArgs e) =>
            (DataContext as MainViewModel)?.AddEventLog($"PreviewQuantityChanged (Tunnel) from {sender.GetType().Name}");

        private void OnQuantityChanged(object sender, RoutedEventArgs e) =>
            (DataContext as MainViewModel)?.AddEventLog($"QuantityChanged (Bubble) from {sender.GetType().Name}");

        private void ResetRating_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (DataContext as MainViewModel)?.SelectedProduct != null;
        }

        private void ResetRating_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm?.ResetSelectedProductRating();
            vm?.AddEventLog("Команда ResetRating: рейтинг сброшен");
        }
    }
}