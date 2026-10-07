using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoServiceShop.Controls
{
    public partial class QuantitySelectorControl : UserControl
    {
        // DependencyProperty Quantity
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register(
                "Quantity",
                typeof(int),
                typeof(QuantitySelectorControl),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnQuantityChanged,
                    CoerceQuantity,
                    false,
                    UpdateSourceTrigger.PropertyChanged),
                ValidateQuantity);

        // DependencyProperty MinQuantity
        public static readonly DependencyProperty MinQuantityProperty =
            DependencyProperty.Register(
                "MinQuantity",
                typeof(int),
                typeof(QuantitySelectorControl),
                new PropertyMetadata(0, OnMinMaxChanged, CoerceMinQuantity),
                ValidateMinQuantity);

        // DependencyProperty MaxQuantity
        public static readonly DependencyProperty MaxQuantityProperty =
            DependencyProperty.Register(
                "MaxQuantity",
                typeof(int),
                typeof(QuantitySelectorControl),
                new PropertyMetadata(100, OnMinMaxChanged, CoerceMaxQuantity),
                ValidateMaxQuantity);

        // Валидация Quantity
        private static bool ValidateQuantity(object value)
        {
            return (int)value >= 0;
        }

        // Коррекция Quantity с учётом Min и Max
        private static object CoerceQuantity(DependencyObject d, object baseValue)
        {
            var control = (QuantitySelectorControl)d;
            int v = (int)baseValue;
            if (v < control.MinQuantity) v = control.MinQuantity;
            if (v > control.MaxQuantity) v = control.MaxQuantity;
            return v;
        }

        // Валидация MinQuantity
        private static bool ValidateMinQuantity(object value)
        {
            return (int)value >= 0;
        }

        // Коррекция MinQuantity (не может быть больше Max)
        private static object CoerceMinQuantity(DependencyObject d, object baseValue)
        {
            var control = (QuantitySelectorControl)d;
            int v = (int)baseValue;
            if (v > control.MaxQuantity) v = control.MaxQuantity;
            return v;
        }

        // Валидация MaxQuantity
        private static bool ValidateMaxQuantity(object value)
        {
            return (int)value >= 0;
        }

        // Коррекция MaxQuantity (не может быть меньше Min)
        private static object CoerceMaxQuantity(DependencyObject d, object baseValue)
        {
            var control = (QuantitySelectorControl)d;
            int v = (int)baseValue;
            if (v < control.MinQuantity) v = control.MinQuantity;
            return v;
        }

        private static void OnMinMaxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // При изменении Min или Max пересчитываем Quantity
            ((QuantitySelectorControl)d).CoerceValue(QuantityProperty);
        }

        private static void OnQuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (QuantitySelectorControl)d;
            control.RaiseEvent(new RoutedEventArgs(QuantityChangedEvent, control));
        }

        public int Quantity
        {
            get { return (int)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }

        public int MinQuantity
        {
            get { return (int)GetValue(MinQuantityProperty); }
            set { SetValue(MinQuantityProperty, value); }
        }

        public int MaxQuantity
        {
            get { return (int)GetValue(MaxQuantityProperty); }
            set { SetValue(MaxQuantityProperty, value); }
        }

        // События
        public static readonly RoutedEvent QuantityChangedEvent =
            EventManager.RegisterRoutedEvent("QuantityChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(QuantitySelectorControl));

        public static readonly RoutedEvent PreviewQuantityChangedEvent =
            EventManager.RegisterRoutedEvent("PreviewQuantityChanged", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(QuantitySelectorControl));

        public static readonly RoutedEvent QuantityConfirmedEvent =
            EventManager.RegisterRoutedEvent("QuantityConfirmed", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(QuantitySelectorControl));

        public event RoutedEventHandler QuantityChanged
        {
            add { AddHandler(QuantityChangedEvent, value); }
            remove { RemoveHandler(QuantityChangedEvent, value); }
        }

        public event RoutedEventHandler PreviewQuantityChanged
        {
            add { AddHandler(PreviewQuantityChangedEvent, value); }
            remove { RemoveHandler(PreviewQuantityChangedEvent, value); }
        }

        public event RoutedEventHandler QuantityConfirmed
        {
            add { AddHandler(QuantityConfirmedEvent, value); }
            remove { RemoveHandler(QuantityConfirmedEvent, value); }
        }

        public QuantitySelectorControl()
        {
            InitializeComponent();
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            var previewArgs = new RoutedEventArgs(PreviewQuantityChangedEvent, this);
            RaiseEvent(previewArgs);
            if (previewArgs.Handled)
                return;
            Quantity++;
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            var previewArgs = new RoutedEventArgs(PreviewQuantityChangedEvent, this);
            RaiseEvent(previewArgs);
            if (previewArgs.Handled)
                return;
            Quantity--;
        }

        private void QuantityTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(QuantityConfirmedEvent, this));
        }
    }
}