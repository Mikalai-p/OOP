using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoServiceShop.Controls;

namespace AutoServiceShop.Views
{
    public partial class RoutingDemoWindow : Window
    {
        public RoutingDemoWindow()
        {
            InitializeComponent();

            
            // 1. ПОДПИСКА НА ТУННЕЛЬНЫЕ СОБЫТИЯ (Preview*) - идут сверху вниз
            // Внешний красный Border
            OuterBorder.AddHandler(
                RatingControl.PreviewRatingChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНЕШНИЙ Border (Красный): PreviewRatingChanged (TUNNEL)")));

            OuterBorder.AddHandler(
                QuantitySelectorControl.PreviewQuantityChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНЕШНИЙ Border (Красный): PreviewQuantityChanged (TUNNEL)")));

            // Средний синий Border
            MiddleBorder.AddHandler(
                RatingControl.PreviewRatingChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" СРЕДНИЙ Border (Синий): PreviewRatingChanged (TUNNEL)")));

            MiddleBorder.AddHandler(
                QuantitySelectorControl.PreviewQuantityChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" СРЕДНИЙ Border (Синий): PreviewQuantityChanged (TUNNEL)")));

            // Внутренний зеленый Border
            InnerBorder.AddHandler(
                RatingControl.PreviewRatingChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНУТРЕННИЙ Border (Зеленый): PreviewRatingChanged (TUNNEL)")));

            InnerBorder.AddHandler(
                QuantitySelectorControl.PreviewQuantityChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНУТРЕННИЙ Border (Зеленый): PreviewQuantityChanged (TUNNEL)")));

            // 2. ПОДПИСКА НА ВСПЛЫВАЮЩИЕ СОБЫТИЯ - идут снизу вверх
            // Внутренний зеленый Border
            InnerBorder.AddHandler(
                RatingControl.RatingChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНУТРЕННИЙ Border (Зеленый): RatingChanged (BUBBLE)")));

            InnerBorder.AddHandler(
                QuantitySelectorControl.QuantityChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНУТРЕННИЙ Border (Зеленый): QuantityChanged (BUBBLE)")));

            // Средний синий Border
            MiddleBorder.AddHandler(
                RatingControl.RatingChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" СРЕДНИЙ Border (Синий): RatingChanged (BUBBLE)")));

            MiddleBorder.AddHandler(
                QuantitySelectorControl.QuantityChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" СРЕДНИЙ Border (Синий): QuantityChanged (BUBBLE)")));

            // Внешний красный Border
            OuterBorder.AddHandler(
                RatingControl.RatingChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНЕШНИЙ Border (Красный): RatingChanged (BUBBLE)")));

            OuterBorder.AddHandler(
                QuantitySelectorControl.QuantityChangedEvent,
                new RoutedEventHandler((s, e) => AddLog(" ВНЕШНИЙ Border (Красный): QuantityChanged (BUBBLE)")));

            // 3. ПОДПИСКА НА ПРЯМЫЕ СОБЫТИЯ (Direct) - только на самом элементе
            DemoRatingControl.RatingConfirmed += (s, e) =>
                AddLog(" RatingControl: RatingConfirmed (DIRECT - только на контроле)");

            DemoQuantityControl.QuantityConfirmed += (s, e) =>
                AddLog(" QuantitySelectorControl: QuantityConfirmed (DIRECT - только на контроле)");

            // 4. ПОДПИСКА НА САМИ КОНТРОЛЫ (для сравнения)
            DemoRatingControl.PreviewRatingChanged += (s, e) =>
                AddLog(" RatingControl: PreviewRatingChanged (TUNNEL - на самом контроле)");

            DemoRatingControl.RatingChanged += (s, e) =>
                AddLog(" RatingControl: RatingChanged (BUBBLE - на самом контроле)");

            DemoQuantityControl.PreviewQuantityChanged += (s, e) =>
                AddLog(" QuantitySelectorControl: PreviewQuantityChanged (TUNNEL - на самом контроле)");

            DemoQuantityControl.QuantityChanged += (s, e) =>
                AddLog(" QuantitySelectorControl: QuantityChanged (BUBBLE - на самом контроле)");

            AddLog("ДЕМОНСТРАЦИЯ ЗАПУЩЕНА");
            AddLog("Порядок событий:");
            AddLog("1. TUNNEL (Preview*) - идут от корня к источнику");
            AddLog("2. BUBBLE (без Preview) - идут от источника к корню");
            AddLog("3. DIRECT - только на элементе, не маршрутизируются");
        }

        private void AddLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                EventLogListBox.Items.Add($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
                EventLogListBox.ScrollIntoView(EventLogListBox.Items[EventLogListBox.Items.Count - 1]);
            });
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            EventLogListBox.Items.Clear();
        }

        private void Explain_Click(object sender, RoutedEventArgs e)
        {
            string explanation =
                "ОБЪЯСНЕНИЕ ТИПОВ МАРШРУТИЗАЦИИ\n\n" +
                "1️. TUNNELING (Preview* события):\n" +
                "   • Движутся СВЕРХУ ВНИЗ (от корня к источнику)\n" +
                "   • Пример: Внешний Border → Средний Border → Внутренний Border → Контрол\n" +
                "   • Используются для предварительной обработки/перехвата\n\n" +
                "2️. BUBBLING (обычные события):\n" +
                "   • Движутся СНИЗУ ВВЕРХ (от источника к корню)\n" +
                "   • Пример: Контрол → Внутренний Border → Средний Border → Внешний Border\n" +
                "   • Используются для уведомления родительских элементов\n\n" +
                "3️. DIRECT события:\n" +
                "   • Обрабатываются ТОЛЬКО на элементе-источнике\n" +
                "   • Не поднимаются и не опускаются по дереву\n" +
                "   • Пример: RatingConfirmed, QuantityConfirmed\n\n" +
                "🔄 ПОСЛЕДОВАТЕЛЬНОСТЬ при клике на RatingControl:\n" +
                "1. TUNNEL: Внешний Border (Preview) → Средний Border (Preview) → Контрол (Preview)\n" +
                "2. BUBBLE: Контрол (Changed) → Внутренний Border (Changed) → Средний Border (Changed) → Внешний Border (Changed)\n" +
                "3. DIRECT (при двойном клике): Только на контроле (Confirmed)";

            MessageBox.Show(explanation, "Объяснение маршрутизации",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}