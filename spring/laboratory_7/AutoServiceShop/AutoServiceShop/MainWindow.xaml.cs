using System;
using System.Windows;
using System.Windows.Input;
using AutoServiceShop.Controls;
using AutoServiceShop.Models;
using AutoServiceShop.Resources;
using AutoServiceShop.ViewModels;
using AutoServiceShop.Views;

namespace AutoServiceShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Глобальная подписка на события от RatingControl и QuantitySelectorControl (включая тестовые)
            this.AddHandler(RatingControl.PreviewRatingChangedEvent, new RoutedEventHandler(OnPreviewRatingChanged));
            this.AddHandler(RatingControl.RatingChangedEvent, new RoutedEventHandler(OnRatingChanged));
            this.AddHandler(QuantitySelectorControl.PreviewQuantityChangedEvent, new RoutedEventHandler(OnPreviewQuantityChanged));
            this.AddHandler(QuantitySelectorControl.QuantityChangedEvent, new RoutedEventHandler(OnQuantityChanged));
        }

        private void OnPreviewRatingChanged(object sender, RoutedEventArgs e)
        {
            AddLog($"PreviewRatingChanged (Tunnel) from {sender.GetType().Name}");
        }

        private void OnRatingChanged(object sender, RoutedEventArgs e)
        {
            AddLog($"RatingChanged (Bubble) from {sender.GetType().Name}");
        }

        private void OnPreviewQuantityChanged(object sender, RoutedEventArgs e)
        {
            AddLog($"PreviewQuantityChanged (Tunnel) from {sender.GetType().Name}");
        }

        private void OnQuantityChanged(object sender, RoutedEventArgs e)
        {
            AddLog($"QuantityChanged (Bubble) from {sender.GetType().Name}");
        }

        private void AddLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                EventLogListBox.Items.Add($"{DateTime.Now:T}: {message}");
                if (EventLogListBox.Items.Count > 20)
                    EventLogListBox.Items.RemoveAt(0);
                EventLogListBox.ScrollIntoView(EventLogListBox.Items[EventLogListBox.Items.Count - 1]);
            });
        }

        private void ProductCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Product product)
            {
                var vm = DataContext as MainViewModel;
                vm.SelectedProduct = product;
            }
        }

        private void ProductCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (sender is FrameworkElement element && element.DataContext is Product product)
                {
                    var vm = DataContext as MainViewModel;
                    vm.SelectedProduct = product;
                    if (vm.ShowDetailsCommand.CanExecute(null))
                        vm.ShowDetailsCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void SetRussian_Click(object sender, RoutedEventArgs e) => LocalizationManager.ChangeLanguage("ru");

        private void SetEnglish_Click(object sender, RoutedEventArgs e) => LocalizationManager.ChangeLanguage("en");

        private void ShowContactInfo_Click(object sender, RoutedEventArgs e)
        {
            var contactWindow = new Views.ContactWindow();
            contactWindow.Owner = this;
            contactWindow.ShowDialog();
        }

        private void OpenProfile_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Owner = this;
            profileWindow.ShowDialog();
        }

        // Обработчики команды ResetRating
        private void ResetRating_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (DataContext as MainViewModel)?.SelectedProduct != null;
        }

        private void ResetRating_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm?.ResetSelectedProductRating();
            AddLog("Команда ResetRating: рейтинг сброшен");
        }
    }
}