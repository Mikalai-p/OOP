using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoServiceShop.Controls
{
    public partial class RatingControl : UserControl
    {
        // DependencyProperty Rating с валидацией и коррекцией
        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register(
                "Rating",
                typeof(double),
                typeof(RatingControl),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRatingChanged,
                    CoerceRating,
                    false,
                    UpdateSourceTrigger.PropertyChanged),
                ValidateRating);

        private static bool ValidateRating(object value)
        {
            double v = (double)value;
            return v >= 0.0 && v <= 5.0;
        }

        private static object CoerceRating(DependencyObject d, object baseValue)
        {
            double v = (double)baseValue;
            v = Math.Round(v * 2) / 2.0; // округление до 0.5
            if (v < 0) v = 0;
            if (v > 5) v = 5;
            return v;
        }

        private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RatingControl)d;
            control.UpdateStars();
            control.RaiseEvent(new RoutedEventArgs(RatingChangedEvent, control));
        }

        public double Rating
        {
            get { return (double)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        // События с разными стратегиями маршрутизации
        public static readonly RoutedEvent RatingChangedEvent =
            EventManager.RegisterRoutedEvent("RatingChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RatingControl));

        public static readonly RoutedEvent PreviewRatingChangedEvent =
            EventManager.RegisterRoutedEvent("PreviewRatingChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(RatingControl));

        public static readonly RoutedEvent RatingConfirmedEvent =
            EventManager.RegisterRoutedEvent("RatingConfirmed", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(RatingControl));

        public event RoutedEventHandler RatingChanged
        {
            add { AddHandler(RatingChangedEvent, value); }
            remove { RemoveHandler(RatingChangedEvent, value); }
        }

        public event RoutedEventHandler PreviewRatingChanged
        {
            add { AddHandler(PreviewRatingChangedEvent, value); }
            remove { RemoveHandler(PreviewRatingChangedEvent, value); }
        }

        public event RoutedEventHandler RatingConfirmed
        {
            add { AddHandler(RatingConfirmedEvent, value); }
            remove { RemoveHandler(RatingConfirmedEvent, value); }
        }

        public RatingControl()
        {
            InitializeComponent();
            UpdateStars();
        }

        private void Star_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int star = int.Parse(button.Tag.ToString());

            // Туннельное событие перед изменением
            var previewArgs = new RoutedEventArgs(PreviewRatingChangedEvent, this);
            RaiseEvent(previewArgs);
            if (previewArgs.Handled)
                return;

            Rating = star;
        }

        private void UpdateStars()
        {
            Star1.Foreground = Rating >= 1 ? Brushes.Gold : Brushes.Gray;
            Star2.Foreground = Rating >= 2 ? Brushes.Gold : Brushes.Gray;
            Star3.Foreground = Rating >= 3 ? Brushes.Gold : Brushes.Gray;
            Star4.Foreground = Rating >= 4 ? Brushes.Gold : Brushes.Gray;
            Star5.Foreground = Rating >= 5 ? Brushes.Gold : Brushes.Gray;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Прямое событие
            RaiseEvent(new RoutedEventArgs(RatingConfirmedEvent, this));
        }
    }
}